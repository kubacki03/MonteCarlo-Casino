using MonteCarlo.NET.Services.Games;

namespace TestyMonteCarlo.Games
{
    public class SlotServiceTests
    {
        [Fact]
        public async Task SpinAsync_ThreeMatchingSymbolsPayMultiplier()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var service = new SlotService(new GameService(context), new ScriptedRandom(2, 2, 2));

            var result = await service.SpinAsync(user, stake: 2);

            Assert.True(result.Succeeded);
            Assert.True(result.Outcome!.Won);
            Assert.Equal(2 * SlotService.PrizeMultiplier, result.Outcome.Prize);
            Assert.Equal(100 - 2 + 2 * SlotService.PrizeMultiplier, TestData.BalanceOf(database, user.Id));
        }

        [Fact]
        public async Task SpinAsync_MixedSymbolsLoseTheStake()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var service = new SlotService(new GameService(context), new ScriptedRandom(0, 1, 2));

            var result = await service.SpinAsync(user, stake: 5);

            Assert.True(result.Succeeded);
            Assert.False(result.Outcome!.Won);
            Assert.Equal(95, TestData.BalanceOf(database, user.Id));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        public async Task SpinAsync_RejectsInvalidStakes(double stake)
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var service = new SlotService(new GameService(context), new ScriptedRandom());

            var result = await service.SpinAsync(user, stake);

            Assert.False(result.Succeeded);
            Assert.Equal(100, TestData.BalanceOf(database, user.Id));
        }

        [Fact]
        public async Task SpinAsync_RejectsStakeAboveBalance()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 5);
            var service = new SlotService(new GameService(context), new ScriptedRandom());

            var result = await service.SpinAsync(user, stake: 50);

            Assert.False(result.Succeeded);
            Assert.Equal(5, TestData.BalanceOf(database, user.Id));
        }
    }
}
