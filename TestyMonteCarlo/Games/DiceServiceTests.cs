using MonteCarlo.NET.Services.Games;

namespace TestyMonteCarlo.Games
{
    public class DiceServiceTests
    {
        [Theory]
        [InlineData(0, false, false, 90)]
        [InlineData(10, false, true, 100)]
        [InlineData(30, true, false, 120)]
        public async Task SettleAsync_AppliesPrizeToBalance(double prize, bool won, bool draw, double expectedBalance)
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var service = new DiceService(new GameService(context));

            var result = await service.SettleAsync(user, stake: 10, prize);

            Assert.True(result.Succeeded);
            Assert.Equal(won, result.Outcome!.Won);
            Assert.Equal(draw, result.Outcome.Draw);
            Assert.Equal(expectedBalance, TestData.BalanceOf(database, user.Id));
        }

        [Fact]
        public async Task SettleAsync_RejectsPrizeTheGameCannotPay()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var service = new DiceService(new GameService(context));

            var result = await service.SettleAsync(user, stake: 10, prize: 1000);

            Assert.False(result.Succeeded);
            Assert.Equal(100, TestData.BalanceOf(database, user.Id));
            using var verify = database.CreateContext();
            Assert.Empty(verify.GameAccounts);
        }

        [Fact]
        public async Task CheckBetAsync_RejectsStakeAboveBalance()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 5);
            var service = new DiceService(new GameService(context));

            Assert.NotNull(await service.CheckBetAsync(user, stake: 10));
            Assert.Null(await service.CheckBetAsync(user, stake: 5));
        }
    }
}
