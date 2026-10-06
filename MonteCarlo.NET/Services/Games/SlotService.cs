using MonteCarlo.NET.Models;

namespace MonteCarlo.NET.Services.Games
{
    public sealed record SlotSpin(string[] Symbols, bool Won, double Prize);

    public interface ISlotService
    {
        Task<PlayResult<SlotSpin>> SpinAsync(UserAccount user, double stake);
    }

    public class SlotService : ISlotService
    {
        public const int PrizeMultiplier = 70;

        public static readonly string[] Symbols =
        {
            "7️⃣", "❌", "🍓", "🍋", "🍉", "🍒", "💵", "🍊", "🍎"
        };

        private const int ReelCount = 3;

        private readonly IGameService _games;
        private readonly Random _random;

        public SlotService(IGameService games, Random random)
        {
            _games = games;
            _random = random;
        }

        public Task<PlayResult<SlotSpin>> SpinAsync(UserAccount user, double stake)
        {
            return _games.PlayAsync(user, GameNames.Slots, stake, _ =>
            {
                var reels = new string[ReelCount];
                for (var i = 0; i < ReelCount; i++)
                {
                    reels[i] = Symbols[_random.Next(Symbols.Length)];
                }

                var won = reels.All(symbol => symbol == reels[0]);
                var prize = won ? PrizeMultiplier * stake : 0;
                return (prize, new SlotSpin(reels, won, prize));
            });
        }
    }
}
