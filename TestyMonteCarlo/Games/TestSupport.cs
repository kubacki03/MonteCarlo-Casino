using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MonteCarlo.NET.Data;
using MonteCarlo.NET.Models;

namespace TestyMonteCarlo.Games
{
    public sealed class TestDatabase : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<CasinoContext> _options;

        public TestDatabase()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            _options = new DbContextOptionsBuilder<CasinoContext>().UseSqlite(_connection).Options;

            using var context = new CasinoContext(_options);
            context.Database.EnsureCreated();
        }

        public CasinoContext CreateContext() => new CasinoContext(_options);

        public void Dispose() => _connection.Dispose();
    }

    public sealed class ScriptedRandom : Random
    {
        private readonly Queue<int> _values;

        public ScriptedRandom(params int[] values)
        {
            _values = new Queue<int>(values);
        }

        public override int Next(int maxValue) => _values.Dequeue();

        public override int Next(int minValue, int maxValue) => _values.Dequeue();
    }

    public static class TestData
    {
        public static UserAccount AddUser(CasinoContext context, double balance, int level = 0)
        {
            var user = new UserAccount
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "player",
                FirstName = "Test",
                LastName = "Player",
                Balance = balance,
                Level = level
            };
            context.UserAccounts.Add(user);
            context.SaveChanges();
            return user;
        }

        public static Match AddFutureMatch(CasinoContext context)
        {
            var match = new Match
            {
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
                HomeTeamId = 1,
                AwayTeamId = 2,
                HomeTeamName = "Olimpia .NeT",
                AwayTeamName = "Java FC",
                HomeTeamOdds = 3,
                AwayTeamOdds = 2
            };
            context.Matches.Add(match);
            context.SaveChanges();
            return match;
        }

        public static double BalanceOf(TestDatabase database, string userId)
        {
            using var context = database.CreateContext();
            return context.UserAccounts.Single(u => u.Id == userId).Balance ?? 0;
        }
    }
}
