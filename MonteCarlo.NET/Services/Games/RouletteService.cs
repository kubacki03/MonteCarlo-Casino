using MonteCarlo.NET.Models;

namespace MonteCarlo.NET.Services.Games
{
    public sealed record RouletteBet(int Position, int Amount);

    public sealed record RouletteSpin(int Number, double Stake, double Winnings);

    public interface IRouletteService
    {
        Task<PlayResult<RouletteSpin>> SpinAsync(UserAccount user, IReadOnlyCollection<RouletteBet> bets);
    }

    public class RouletteService : IRouletteService
    {
        public static readonly string[] BetLabels =
        {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
            "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
            "21", "22", "23", "24", "25", "26", "27", "28", "29", "30",
            "31", "32", "33", "34", "35", "36",
            "Czarne", "Czerwone",
            "Rząd 1", "Rząd 2", "Rząd 3",
            "1-12", "13-24", "25-36",
            "1-18", "19-36",
            "Parzyste", "Nieparzyste",
        };

        private const int MaxBetsPerSpin = 49;
        private static readonly HashSet<int> RedNumbers = new() { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };

        private readonly IGameService _games;
        private readonly Random _random;

        public RouletteService(IGameService games, Random random)
        {
            _games = games;
            _random = random;
        }

        public Task<PlayResult<RouletteSpin>> SpinAsync(UserAccount user, IReadOnlyCollection<RouletteBet> bets)
        {
            if (bets.Count == 0 || bets.Count > MaxBetsPerSpin
                || bets.Any(b => b.Position < 0 || b.Position >= BetLabels.Length || b.Amount <= 0)
                || bets.Select(b => b.Position).Distinct().Count() != bets.Count)
            {
                return Task.FromResult(PlayResult<RouletteSpin>.Failure("Nieprawidłowe zakłady"));
            }

            double stake;
            try
            {
                stake = bets.Sum(b => (long)b.Amount);
            }
            catch (OverflowException)
            {
                return Task.FromResult(PlayResult<RouletteSpin>.Failure("Nieprawidłowe zakłady"));
            }

            return _games.PlayAsync(user, GameNames.Roulette, stake, _ =>
            {
                var number = _random.Next(37);
                double winnings = bets.Sum(b => (double)Payout(number, b.Position, b.Amount));
                return (winnings, new RouletteSpin(number, stake, winnings));
            });
        }

        /// <summary>Total amount returned (stake included) for a single bet, 0 when it loses.</summary>
        public static long Payout(int number, int position, int amount)
        {
            if (position <= 36)
            {
                return position == number ? 35L * amount : 0;
            }

            // zero loses every outside bet
            if (number == 0)
            {
                return 0;
            }

            return position switch
            {
                37 => !RedNumbers.Contains(number) ? 2L * amount : 0,
                38 => RedNumbers.Contains(number) ? 2L * amount : 0,
                // grid rows: row 1 = 3,6,...,36; row 2 = 2,5,...,35; row 3 = 1,4,...,34
                39 => number % 3 == 0 ? 3L * amount : 0,
                40 => number % 3 == 2 ? 3L * amount : 0,
                41 => number % 3 == 1 ? 3L * amount : 0,
                >= 42 and <= 44 => (number - 1) / 12 == position - 42 ? 3L * amount : 0,
                45 => number <= 18 ? 2L * amount : 0,
                46 => number >= 19 ? 2L * amount : 0,
                47 => number % 2 == 0 ? 2L * amount : 0,
                48 => number % 2 == 1 ? 2L * amount : 0,
                _ => 0
            };
        }
    }
}
