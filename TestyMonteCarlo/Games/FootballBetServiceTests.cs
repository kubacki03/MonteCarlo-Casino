using MonteCarlo.NET.Data;
using MonteCarlo.NET.Models;
using MonteCarlo.NET.Services.Games;

namespace TestyMonteCarlo.Games
{
    public class FootballBetServiceTests
    {
        private static FootballBetService CreateService(CasinoContext context, Random random)
        {
            return new FootballBetService(context, new GameService(context), random);
        }

        [Fact]
        public async Task PlaceBetAsync_StoresBetAndChargesStake()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var match = TestData.AddFutureMatch(context);
            var service = CreateService(context, new ScriptedRandom());

            var result = await service.PlaceBetAsync(user, match.MatchId, match.HomeTeamId, 30);

            Assert.True(result.Succeeded);
            Assert.Equal(70, TestData.BalanceOf(database, user.Id));
            using var verify = database.CreateContext();
            var bet = Assert.Single(verify.Bets);
            Assert.Equal(match.HomeTeamId, bet.WinnerTeamId);
            Assert.Equal(30, bet.StakedAmount);
            Assert.False(bet.IsRewardGranted);
            Assert.Single(verify.GameAccounts);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public async Task PlaceBetAsync_RejectsNonPositiveStake(long stake)
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var match = TestData.AddFutureMatch(context);
            var service = CreateService(context, new ScriptedRandom());

            var result = await service.PlaceBetAsync(user, match.MatchId, match.HomeTeamId, stake);

            Assert.False(result.Succeeded);
            Assert.Equal(100, TestData.BalanceOf(database, user.Id));
            using var verify = database.CreateContext();
            Assert.Empty(verify.Bets);
        }

        [Fact]
        public async Task PlaceBetAsync_RejectsStakeAboveBalance()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 20);
            var match = TestData.AddFutureMatch(context);
            var service = CreateService(context, new ScriptedRandom());

            var result = await service.PlaceBetAsync(user, match.MatchId, match.HomeTeamId, 50);

            Assert.False(result.Succeeded);
            Assert.Equal(20, TestData.BalanceOf(database, user.Id));
            using var verify = database.CreateContext();
            Assert.Empty(verify.Bets);
        }

        [Fact]
        public async Task PlaceBetAsync_RejectsTeamThatDoesNotPlayInTheMatch()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var match = TestData.AddFutureMatch(context);
            var service = CreateService(context, new ScriptedRandom());

            var result = await service.PlaceBetAsync(user, match.MatchId, 999, 10);

            Assert.False(result.Succeeded);
            Assert.Equal(100, TestData.BalanceOf(database, user.Id));
        }

        [Fact]
        public async Task PlaceBetAsync_RejectsMatchThatAlreadyStarted()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var service = CreateService(context, new ScriptedRandom());

            var result = await service.PlaceBetAsync(user, matchId: 1, teamId: 1, stake: 10);

            Assert.False(result.Succeeded);
            Assert.Equal(100, TestData.BalanceOf(database, user.Id));
        }

        [Fact]
        public async Task GetOverviewAsync_PaysWinningBetOnlyOnce()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var pastMatch = context.Matches.Single(m => m.MatchId == 1);
            context.Bets.Add(new Bet
            {
                PlayerId = user.Id,
                MatchId = pastMatch.MatchId,
                WinnerTeamId = pastMatch.AwayTeamId,
                StakedAmount = 10,
                IsRewardGranted = false
            });
            context.SaveChanges();
            var random = new ScriptedRandom(3, 1);
            var service = CreateService(context, random);

            await service.GetOverviewAsync(user);
            var secondOverview = await service.GetOverviewAsync(user);

            Assert.Equal(100 + pastMatch.AwayTeamOdds * 10, TestData.BalanceOf(database, user.Id), 3);
            var bet = Assert.Single(secondOverview.Bets);
            Assert.True(bet.HasWon);
            Assert.True(bet.IsRewardGranted);
        }

        [Fact]
        public async Task GetOverviewAsync_LosingBetPaysNothing()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var pastMatch = context.Matches.Single(m => m.MatchId == 1);
            context.Bets.Add(new Bet
            {
                PlayerId = user.Id,
                MatchId = pastMatch.MatchId,
                WinnerTeamId = pastMatch.AwayTeamId,
                StakedAmount = 10,
                IsRewardGranted = false
            });
            context.SaveChanges();
            var service = CreateService(context, new ScriptedRandom(0, 2));

            var overview = await service.GetOverviewAsync(user);

            Assert.Equal(100, TestData.BalanceOf(database, user.Id));
            var bet = Assert.Single(overview.Bets);
            Assert.False(bet.HasWon);
            Assert.True(bet.IsRewardGranted);
        }
    }
}
