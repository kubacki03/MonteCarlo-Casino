using MonteCarlo.NET.Services.Games;

namespace TestyMonteCarlo.Games
{
    public class RouletteServiceTests
    {
        [Theory]
        [InlineData(17, 17, 10, 350)]   
        [InlineData(17, 18, 10, 0)]
        [InlineData(0, 0, 1, 35)]
        [InlineData(7, 38, 10, 20)]    
        [InlineData(8, 38, 10, 0)]
        [InlineData(8, 37, 10, 20)]    
        [InlineData(0, 37, 10, 0)]     
        [InlineData(0, 38, 10, 0)]
        [InlineData(36, 39, 10, 30)]
        [InlineData(35, 40, 10, 30)]
        [InlineData(34, 41, 10, 30)] 
        [InlineData(34, 39, 10, 0)]
        [InlineData(1, 42, 10, 30)]
        [InlineData(12, 42, 10, 30)]
        [InlineData(13, 43, 10, 30)]
        [InlineData(24, 43, 10, 30)]
        [InlineData(25, 44, 10, 30)]
        [InlineData(36, 44, 10, 30)]
        [InlineData(12, 43, 10, 0)]
        [InlineData(18, 45, 10, 20)]
        [InlineData(19, 45, 10, 0)]
        [InlineData(19, 46, 10, 20)]
        [InlineData(2, 47, 10, 20)]
        [InlineData(3, 47, 10, 0)]
        [InlineData(3, 48, 10, 20)] 
        public void Payout_FollowsRouletteRules(int number, int position, int amount, long expected)
        {
            Assert.Equal(expected, RouletteService.Payout(number, position, amount));
        }

        [Fact]
        public async Task SpinAsync_PaysWinningBetsAndChargesTotalStake()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var service = new RouletteService(new GameService(context), new ScriptedRandom(17));

            var result = await service.SpinAsync(user, new[] { new RouletteBet(17, 2), new RouletteBet(37, 10) });

            Assert.True(result.Succeeded);
            Assert.Equal(17, result.Outcome!.Number);
            Assert.Equal(12, result.Outcome.Stake);
            Assert.Equal(70 + 20, result.Outcome.Winnings); 
            Assert.Equal(100 - 12 + 90, TestData.BalanceOf(database, user.Id));
        }

        [Fact]
        public async Task SpinAsync_RejectsInvalidBets()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var service = new RouletteService(new GameService(context), new ScriptedRandom());

            foreach (var bets in new[]
            {
                Array.Empty<RouletteBet>(),
                new[] { new RouletteBet(5, 0) },
                new[] { new RouletteBet(5, -10) },
                new[] { new RouletteBet(49, 1) },
                new[] { new RouletteBet(5, 1), new RouletteBet(5, 1) },
            })
            {
                var result = await service.SpinAsync(user, bets);
                Assert.False(result.Succeeded);
            }

            Assert.Equal(100, TestData.BalanceOf(database, user.Id));
        }

        [Fact]
        public async Task SpinAsync_RejectsStakeAboveBalance()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 10);
            var service = new RouletteService(new GameService(context), new ScriptedRandom());

            var result = await service.SpinAsync(user, new[] { new RouletteBet(5, 50) });

            Assert.False(result.Succeeded);
            Assert.Equal(10, TestData.BalanceOf(database, user.Id));
        }
    }
}
