using MonteCarlo.NET.Models;

namespace MonteCarlo.NET.Services.Games
{
    public sealed record DiceResult(bool Won, bool Draw);

    public interface IDiceService
    {
        Task<string?> CheckBetAsync(UserAccount user, double stake);

        Task<PlayResult<DiceResult>> SettleAsync(UserAccount user, double stake, double prize);
    }

    public class DiceService : IDiceService
    {
        public const int WinMultiplier = 3;

        private readonly IGameService _games;

        public DiceService(IGameService games)
        {
            _games = games;
        }

        public Task<string?> CheckBetAsync(UserAccount user, double stake)
        {
            return _games.GetStakeRejectionAsync(user, GameNames.Dice, stake);
        }

        public Task<PlayResult<DiceResult>> SettleAsync(UserAccount user, double stake, double prize)
        {
            // The dice are still rolled in the browser, so at least refuse payouts the game can never produce.
            var won = prize == WinMultiplier * stake;
            var draw = prize == stake;
            if (!won && !draw && prize != 0)
            {
                return Task.FromResult(PlayResult<DiceResult>.Failure("Nieprawidłowa wygrana"));
            }

            return _games.PlayAsync(user, GameNames.Dice, stake, _ => (prize, new DiceResult(won, draw)));
        }
    }
}
