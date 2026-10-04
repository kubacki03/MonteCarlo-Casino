using MonteCarlo.NET.Models;

namespace MonteCarlo.NET.Services.Games
{
    public interface IScratchCardService
    {
        Task<PlayResult<double>> PlaySimpleAsync(UserAccount user);

        Task<PlayResult<CloverScratchCard>> PlayCloverAsync(UserAccount user);
    }

    public class ScratchCardService : IScratchCardService
    {
        private static readonly double[] SimplePrizes = { 1, 5, 10, 0, 0, 0, 0, 0 };

        private readonly IGameService _games;
        private readonly Random _random;

        public ScratchCardService(IGameService games, Random random)
        {
            _games = games;
            _random = random;
        }

        public Task<PlayResult<double>> PlaySimpleAsync(UserAccount user)
        {
            return _games.PlayAtMinimumStakeAsync(user, GameNames.SimpleScratchCard, _ =>
            {
                var prize = SimplePrizes[_random.Next(SimplePrizes.Length)];
                if (prize != 0)
                {
                    prize = SimplePrizes[_random.Next(SimplePrizes.Length)];
                }

                return (prize, prize);
            });
        }

        public Task<PlayResult<CloverScratchCard>> PlayCloverAsync(UserAccount user)
        {
            return _games.PlayAtMinimumStakeAsync(user, GameNames.CloverScratchCard, _ =>
            {
                var card = new CloverScratchCard();
                card.InitGame(_random);
                double prize = card.CheckForWin() ? card.Prize : 0;
                return (prize, card);
            });
        }
    }
}
