using System.Data;
using Microsoft.EntityFrameworkCore;
using MonteCarlo.NET.Data;
using MonteCarlo.NET.Models;

namespace MonteCarlo.NET.Services.Games
{
    public class GameService : IGameService
    {
        private const string GameUnavailableMessage = "Ta gra jest obecnie niedostępna";

        private readonly CasinoContext _context;

        public GameService(CasinoContext context)
        {
            _context = context;
        }

        public async Task<string?> GetPlayRejectionAsync(UserAccount user, string gameName)
        {
            var game = await FindGameAsync(gameName);
            if (game == null)
            {
                return GameUnavailableMessage;
            }

            return await GetStakeRejectionAsync(user, game, game.MinStake);
        }

        public async Task<string?> GetStakeRejectionAsync(UserAccount user, string gameName, double stake)
        {
            var game = await FindGameAsync(gameName);
            if (game == null)
            {
                return GameUnavailableMessage;
            }

            return await GetStakeRejectionAsync(user, game, stake);
        }

        public async Task<double?> GetMinStakeAsync(string gameName)
        {
            return (await FindGameAsync(gameName))?.MinStake;
        }

        public Task<PlayResult<TOutcome>> PlayAsync<TOutcome>(
            UserAccount user,
            string gameName,
            double stake,
            Func<Game, (double Prize, TOutcome Outcome)> play)
        {
            return PlayCoreAsync(user, gameName, _ => stake, play);
        }

        public Task<PlayResult<TOutcome>> PlayAtMinimumStakeAsync<TOutcome>(
            UserAccount user,
            string gameName,
            Func<Game, (double Prize, TOutcome Outcome)> play)
        {
            return PlayCoreAsync(user, gameName, game => game.MinStake, play);
        }

        public async Task<T> InTransactionAsync<T>(Func<Task<T>> action)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var result = await action();
            await transaction.CommitAsync();
            return result;
        }

        public Task InTransactionAsync(Func<Task> action)
        {
            return InTransactionAsync(async () =>
            {
                await action();
                return true;
            });
        }

        private Task<PlayResult<TOutcome>> PlayCoreAsync<TOutcome>(
            UserAccount user,
            string gameName,
            Func<Game, double> stakeFor,
            Func<Game, (double Prize, TOutcome Outcome)> play)
        {
            return InTransactionAsync(async () =>
            {
                await _context.Entry(user).ReloadAsync();

                var game = await FindGameAsync(gameName);
                if (game == null)
                {
                    return PlayResult<TOutcome>.Failure(GameUnavailableMessage);
                }

                var stake = stakeFor(game);
                var rejection = await GetStakeRejectionAsync(user, game, stake);
                if (rejection != null)
                {
                    return PlayResult<TOutcome>.Failure(rejection);
                }

                var (prize, outcome) = play(game);
                await RecordPlayAsync(user, game, stake, prize);
                return PlayResult<TOutcome>.Success(outcome);
            });
        }

        private Task<Game?> FindGameAsync(string gameName)
        {
            return _context.Games.FirstOrDefaultAsync(g => g.Name == gameName);
        }

        private async Task<string?> GetStakeRejectionAsync(UserAccount user, Game game, double stake)
        {
            if (!double.IsFinite(stake) || stake <= 0 || stake < game.MinStake)
            {
                return $"Minimalna stawka to {game.MinStake} zł";
            }

            if ((user.Balance ?? 0) < stake)
            {
                return $"Twoje saldo jest zbyt niskie, aby zagrać w tę grę. Musisz mieć co najmniej {stake} zł";
            }

            var limit = await _context.Limits
                .Where(l => l.UserAccountId == user.Id)
                .OrderByDescending(l => l.Date)
                .FirstOrDefaultAsync();

            if (limit != null)
            {
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);
                var spentToday = await _context.GameAccounts
                    .Where(g => g.UserAccountId == user.Id && g.PlayedAt >= today && g.PlayedAt < tomorrow)
                    .SumAsync(g => g.AmountStaked);

                if (spentToday + stake > limit.Amount)
                {
                    return "Przekroczono limit";
                }
            }

            return null;
        }

        private async Task RecordPlayAsync(UserAccount user, Game game, double stake, double prize)
        {
            user.Balance = (user.Balance ?? 0) - stake + prize;

            var playedGames = await _context.GameAccounts.CountAsync(g => g.UserAccountId == user.Id);
            var level = await _context.Levels
                .Where(l => l.MinimumPlayedGames <= playedGames)
                .OrderByDescending(l => l.MinimumPlayedGames)
                .FirstOrDefaultAsync();
            if (level != null)
            {
                user.Level = level.NumberOfLevel;
            }

            _context.GameAccounts.Add(new GameAccount
            {
                GameId = game.GameId,
                UserAccountId = user.Id,
                AmountStaked = stake,
                AmountWon = prize,
                PlayedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }
    }
}
