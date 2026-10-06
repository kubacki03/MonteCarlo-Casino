using MonteCarlo.NET.Models;
using MonteCarlo.NET.Services;

namespace TestyMonteCarlo.Games
{
    public class LimitServiceTests
    {
        [Fact]
        public async Task SetLimitAsync_StoresFirstLimit()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var service = new LimitService(context);

            var result = await service.SetLimitAsync(user, 50);

            Assert.True(result.Succeeded);
            Assert.Equal(50, (await service.GetCurrentLimitAsync(user))!.Amount);
        }

        [Fact]
        public async Task SetLimitAsync_RejectsChangeWithin24Hours()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var service = new LimitService(context);
            await service.SetLimitAsync(user, 50);

            var result = await service.SetLimitAsync(user, 80);

            Assert.False(result.Succeeded);
            Assert.Equal(50, result.Limit!.Amount);
            Assert.Equal(50, (await service.GetCurrentLimitAsync(user))!.Amount);
        }

        [Fact]
        public async Task SetLimitAsync_AllowsChangeAfter24Hours()
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            context.Limits.Add(new Limit { UserAccountId = user.Id, Amount = 50, Date = DateTime.Now.AddDays(-2) });
            context.SaveChanges();
            var service = new LimitService(context);

            var result = await service.SetLimitAsync(user, 80);

            Assert.True(result.Succeeded);
            Assert.Equal(80, (await service.GetCurrentLimitAsync(user))!.Amount);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public async Task SetLimitAsync_RejectsNonPositiveAmounts(long amount)
        {
            using var database = new TestDatabase();
            using var context = database.CreateContext();
            var user = TestData.AddUser(context, balance: 100);
            var service = new LimitService(context);

            var result = await service.SetLimitAsync(user, amount);

            Assert.False(result.Succeeded);
            Assert.Null(await service.GetCurrentLimitAsync(user));
        }
    }
}
