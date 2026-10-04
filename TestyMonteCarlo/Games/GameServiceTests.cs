using MonteCarlo.NET.Models;
using MonteCarlo.NET.Services.Games;

namespace TestyMonteCarlo.Games
{
    public class GameServiceTests
    {
        [Fact]
        public async Task PlayAsync_DebitsStakeCreditsPrizeAndRecordsPlay()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var service = new GameService(context);

            var result = await service.PlayAsync(user, GameNames.SimpleScratchCard, 10, _ => (25d, "outcome"));

            Assert.True(result.Succeeded);
            Assert.Equal("outcome", result.Outcome);
            Assert.Equal(115, TestData.BalanceOf(database, user.Id));
            using var verify = database.CreateContext();
            var record = Assert.Single(verify.GameAccounts);
            Assert.Equal(10, record.AmountStaked);
            Assert.Equal(25, record.AmountWon);
        }

        [Fact]
        public async Task PlayAsync_RejectsStakeAboveBalance_AndChangesNothing()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 5);
            var service = new GameService(context);

            var result = await service.PlayAsync(user, GameNames.SimpleScratchCard, 10, _ => (100d, "outcome"));

            Assert.False(result.Succeeded);
            Assert.Contains("zbyt niskie", result.ErrorMessage);
            Assert.Equal(5, TestData.BalanceOf(database, user.Id));
            using var verify = database.CreateContext();
            Assert.Empty(verify.GameAccounts);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-50)]
        public async Task PlayAsync_RejectsNonPositiveStake(double stake)
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var service = new GameService(context);

            var result = await service.PlayAsync(user, GameNames.SimpleScratchCard, stake, _ => (0d, "outcome"));

            Assert.False(result.Succeeded);
            Assert.Equal(100, TestData.BalanceOf(database, user.Id));
        }

        [Fact]
        public async Task PlayAsync_RejectsWhenDailyLimitWouldBeExceeded()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var game = context.Games.Single(g => g.Name == GameNames.SimpleScratchCard);
            context.Limits.Add(new Limit { UserAccountId = user.Id, Amount = 10, Date = DateTime.Now });
            context.GameAccounts.Add(new GameAccount
            {
                UserAccountId = user.Id,
                GameId = game.GameId,
                AmountStaked = 8,
                AmountWon = 0,
                PlayedAt = DateTime.Now
            });
            context.SaveChanges();
            var service = new GameService(context);

            var result = await service.PlayAsync(user, GameNames.SimpleScratchCard, 5, _ => (0d, "outcome"));

            Assert.False(result.Succeeded);
            Assert.Equal("Przekroczono limit", result.ErrorMessage);
            Assert.Equal(100, TestData.BalanceOf(database, user.Id));
        }

        [Fact]
        public async Task PlayAsync_UpdatesLevelFromNumberOfPlayedGames()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var game = context.Games.Single(g => g.Name == GameNames.SimpleScratchCard);
            for (var i = 0; i < 5; i++)
            {
                context.GameAccounts.Add(new GameAccount
                {
                    UserAccountId = user.Id,
                    GameId = game.GameId,
                    AmountStaked = 1,
                    AmountWon = 0,
                    PlayedAt = DateTime.Now.AddDays(-2)
                });
            }
            context.SaveChanges();
            var service = new GameService(context);

            await service.PlayAsync(user, GameNames.SimpleScratchCard, 1, _ => (0d, "outcome"));

            using var verify = database.CreateContext();
            Assert.Equal(1, verify.UserAccounts.Single(u => u.Id == user.Id).Level);
        }

        [Fact]
        public async Task GetPlayRejectionAsync_ReturnsNullWhenPlayerMayPlay()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var service = new GameService(context);

            var rejection = await service.GetPlayRejectionAsync(user, GameNames.CloverScratchCard);

            Assert.Null(rejection);
        }
    }
}
