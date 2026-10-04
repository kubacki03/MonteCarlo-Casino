using MonteCarlo.NET.Models;
using MonteCarlo.NET.Services.Games;

namespace TestyMonteCarlo.Games
{
    public class ScratchCardServiceTests
    {
        [Fact]
        public async Task PlaySimpleAsync_WinningFirstRollIsRerolled()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var service = new ScratchCardService(new GameService(context), new ScriptedRandom(0, 2));

            var result = await service.PlaySimpleAsync(user);

            Assert.True(result.Succeeded);
            Assert.Equal(10, result.Outcome);
            Assert.Equal(100 - 1 + 10, TestData.BalanceOf(database, user.Id));
        }

        [Fact]
        public async Task PlaySimpleAsync_LosingRollPaysNothing()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var service = new ScratchCardService(new GameService(context), new ScriptedRandom(3));

            var result = await service.PlaySimpleAsync(user);

            Assert.True(result.Succeeded);
            Assert.Equal(0, result.Outcome);
            Assert.Equal(99, TestData.BalanceOf(database, user.Id));
        }

        [Fact]
        public async Task PlaySimpleAsync_FailsWithoutFunds()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 0);
            var service = new ScratchCardService(new GameService(context), new ScriptedRandom());

            var result = await service.PlaySimpleAsync(user);

            Assert.False(result.Succeeded);
            Assert.Equal(0, TestData.BalanceOf(database, user.Id));
        }

        [Fact]
        public async Task PlayCloverAsync_ThreeCloversPayTheCardPrize()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var random = new ScriptedRandom(0, 0, 0, 5, 5, 5, 5, 5, 5, 7);
            var service = new ScratchCardService(new GameService(context), random);

            var result = await service.PlayCloverAsync(user);

            Assert.True(result.Succeeded);
            Assert.Equal(3, result.Outcome!.WinningCombinationCount);
            Assert.Equal(7, result.Outcome.Prize);
            Assert.Equal(100 - 1 + 7, TestData.BalanceOf(database, user.Id));
        }

        [Fact]
        public async Task PlayCloverAsync_FewerThanThreeCloversPayNothing()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var random = new ScriptedRandom(0, 0, 5, 5, 5, 5, 5, 5, 5, 4);
            var service = new ScratchCardService(new GameService(context), random);

            var result = await service.PlayCloverAsync(user);

            Assert.True(result.Succeeded);
            Assert.Equal(99, TestData.BalanceOf(database, user.Id));
        }
    }
}
