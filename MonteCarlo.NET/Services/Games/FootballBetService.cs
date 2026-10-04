using Microsoft.EntityFrameworkCore;
using MonteCarlo.NET.Data;
using MonteCarlo.NET.Models;

namespace MonteCarlo.NET.Services.Games
{
    public sealed record FootballOverview(IReadOnlyList<Match> Matches, IReadOnlyList<Bet> Bets);

    public interface IFootballBetService
    {
        Task<FootballOverview> GetOverviewAsync(UserAccount user);

        Task<PlayResult<Bet>> PlaceBetAsync(UserAccount user, int matchId, int teamId, long stake);
    }

    public class FootballBetService : IFootballBetService
    {
        private readonly CasinoContext _context;
        private readonly IGameService _games;
        private readonly Random _random;

        public FootballBetService(CasinoContext context, IGameService games, Random random)
        {
            _context = context;
            _games = games;
            _random = random;
        }

        public async Task<FootballOverview> GetOverviewAsync(UserAccount user)
        {
            await _games.InTransactionAsync(() => SettleFinishedBetsAsync(user));

            var matches = await _context.Matches.AsNoTracking().ToListAsync();
            var bets = await _context.Bets.Where(b => b.PlayerId == user.Id).ToListAsync();
            return new FootballOverview(matches, bets);
        }

        public async Task<PlayResult<Bet>> PlaceBetAsync(UserAccount user, int matchId, int teamId, long stake)
        {
            if (stake <= 0)
            {
                return PlayResult<Bet>.Failure("Stawka musi być większa od zera");
            }

            var match = await _context.Matches.FirstOrDefaultAsync(m => m.MatchId == matchId);
            if (match == null || match.Date <= DateOnly.FromDateTime(DateTime.Now))
            {
                return PlayResult<Bet>.Failure("Nie można obstawić tego meczu");
            }

            if (teamId != match.HomeTeamId && teamId != match.AwayTeamId)
            {
                return PlayResult<Bet>.Failure("Wybrana drużyna nie gra w tym meczu");
            }

            return await _games.PlayAsync(user, GameNames.FootballBetting, stake, _ =>
            {
                var bet = new Bet
                {
                    PlayerId = user.Id,
                    MatchId = matchId,
                    WinnerTeamId = teamId,
                    StakedAmount = stake,
                    IsRewardGranted = false
                };
                _context.Bets.Add(bet);
                return (0d, bet);
            });
        }

        private async Task SettleFinishedBetsAsync(UserAccount user)
        {
            await _context.Entry(user).ReloadAsync();

            var today = DateOnly.FromDateTime(DateTime.Now);
            var openBets = await _context.Bets
                .Where(b => b.PlayerId == user.Id && !b.IsRewardGranted)
                .ToListAsync();

            foreach (var bet in openBets)
            {
                var match = await _context.Matches.FirstOrDefaultAsync(m => m.MatchId == bet.MatchId);
                if (match == null || match.Date >= today)
                {
                    continue;
                }

                match.AwayTeamGoals = _random.Next(5);
                match.HomeTeamGoals = _random.Next(5);

                var awayWon = match.AwayTeamId == bet.WinnerTeamId && match.AwayTeamGoals > match.HomeTeamGoals;
                var homeWon = match.HomeTeamId == bet.WinnerTeamId && match.HomeTeamGoals > match.AwayTeamGoals;

                bet.HasWon = awayWon || homeWon;
                bet.IsRewardGranted = true;

                if (awayWon)
                {
                    user.Balance = (user.Balance ?? 0) + (double)match.AwayTeamOdds * bet.StakedAmount;
                }
                else if (homeWon)
                {
                    user.Balance = (user.Balance ?? 0) + (double)match.HomeTeamOdds * bet.StakedAmount;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
