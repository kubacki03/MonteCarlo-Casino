using MonteCarlo.NET.Models;
using MonteCarlo.NET.Services;

namespace TestyMonteCarlo.Games
{
    public class PlayerServiceTests
    {
        [Fact]
        public async Task GetRankingAsync_NamesMostActivePlayerAndGamesToNextLevel()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var game = context.Games.First();
            for (var i = 0; i < 2; i++)
            {
                context.GameAccounts.Add(new GameAccount
                {
                    GameId = game.GameId,
                    UserAccountId = user.Id,
                    AmountStaked = 1,
                    AmountWon = 0,
                    PlayedAt = DateTime.Now
                });
            }
            context.SaveChanges();
            var service = new PlayerService(context);

            var ranking = await service.GetRankingAsync(user);

            Assert.Equal("player", ranking.BestPlayer);
            Assert.True(ranking.GamesToNextLevel >= 0);
            Assert.NotEmpty(ranking.Levels);
        }

        [Fact]
        public async Task GetRankingAsync_WithoutAnyGamesDoesNotThrow()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var service = new PlayerService(context);

            var ranking = await service.GetRankingAsync(user);

            Assert.Null(ranking.BestPlayer);
        }

        [Fact]
        public async Task GetTransactionsAsync_ReturnsOnlyTheUsersTransactions()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            context.Transactions.Add(new Transaction { UserAccountId = user.Id, Amount = 5, Type = "Wpłata", Date = DateTime.Now });
            context.SaveChanges();
            var service = new PlayerService(context);

            Assert.Single(await service.GetTransactionsAsync(user));
        }
    }
}
