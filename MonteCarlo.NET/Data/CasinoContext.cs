using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MonteCarlo.NET.Models;

namespace MonteCarlo.NET.Data
{
    public class CasinoContext : IdentityDbContext<UserAccount>
    {
        public CasinoContext(DbContextOptions<CasinoContext> options) : base(options) { }

        public DbSet<Ban> Bans { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameAccount> GameAccounts { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Limit> Limits { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Bet> Bets { get; set; }
        public DbSet<Level> Levels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ban>().ToTable("Ban");
            modelBuilder.Entity<Game>().ToTable("Gra");
            modelBuilder.Entity<GameAccount>().ToTable("GraKonto");
            modelBuilder.Entity<Limit>().ToTable("Limit");
            modelBuilder.Entity<Transaction>().ToTable("Transakcja");
            modelBuilder.Entity<Report>().ToTable("Zgloszenie");
            modelBuilder.Entity<Team>().ToTable("Druzyna");
            modelBuilder.Entity<Match>().ToTable("Mecz");
            modelBuilder.Entity<Bet>().ToTable("Zaklady");
            modelBuilder.Entity<Level>().ToTable("Levele");

            modelBuilder.Entity<Match>()
                .HasKey(m => m.MatchId);

            modelBuilder.Entity<Team>()
                .HasKey(t => t.TeamId);

            modelBuilder.Entity<Level>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<GameAccount>()
                .HasKey(gk => gk.GameAccountId);

            modelBuilder.Entity<GameAccount>()
                .HasOne(gk => gk.Game)
                .WithMany(g => g.GameAccounts)
                .HasForeignKey(gk => gk.GameId);

            modelBuilder.Entity<GameAccount>()
                .HasOne(gk => gk.UserAccount)
                .WithMany(ku => ku.GameAccounts)
                .HasForeignKey(gk => gk.UserAccountId);

            modelBuilder.Entity<Ban>()
                .HasKey(b => b.BanId);

            modelBuilder.Entity<Game>()
                .HasKey(g => g.GameId);

            modelBuilder.Entity<Limit>()
                .HasKey(l => l.LimitId);

            modelBuilder.Entity<Transaction>()
                .HasKey(t => t.TransactionId);

            modelBuilder.Entity<Report>()
                .HasKey(z => z.ReportId);

            modelBuilder.Entity<Bet>()
           .HasKey(z => z.BetId);


            modelBuilder.Entity<Match>()
                .HasOne(m => m.HomeTeam)
                .WithMany()
                .HasForeignKey(m => m.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayTeam)
                .WithMany()
                .HasForeignKey(m => m.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            string adminRoleId = Guid.NewGuid().ToString();
            string adminUserId = Guid.NewGuid().ToString();

            modelBuilder.Entity<Game>().HasData(
                new Game { GameId = 1, Name = "Slotsy", MinStake = 1 },
                new Game { GameId = 2, Name = "Zdrapka koniczynka", MinStake = 1 },
                new Game { GameId = 3, Name = "Zdrapka Prosta", MinStake = 1 },
                new Game { GameId = 4, Name = "Wyscigi Konne", MinStake = 1 },
                new Game { GameId = 5, Name = "Obstawianie", MinStake = 1 },
                new Game { GameId = 6, Name = "Ruletka", MinStake = 1 },
                new Game { GameId = 7, Name = "Kosci", MinStake = 1 }
            );

            modelBuilder.Entity<Level>().HasData(
                new Level { Id = 1, NumberOfLevel = 0, MinimumPlayedGames = 0 },
                new Level { Id = 2, NumberOfLevel = 1, MinimumPlayedGames = 5 },
                new Level { Id = 3, NumberOfLevel = 2, MinimumPlayedGames = 15 },
                new Level { Id = 4, NumberOfLevel = 3, MinimumPlayedGames = 50 },
                new Level { Id = 5, NumberOfLevel = 4, MinimumPlayedGames = 100 },
                new Level { Id = 6, NumberOfLevel = 5, MinimumPlayedGames = 500 },
                new Level { Id = 7, NumberOfLevel = 6, MinimumPlayedGames = 1000 }
            );

            var adminUser = new UserAccount
            {
                Id = adminUserId,
                FirstName = "Admin",
                LastName = "Admin",
                UserName = "admin@gmail.com",
                NormalizedUserName = "ADMIN@GMAIL.COM",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                EmailConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                Level = 0,
                Balance = 0,
                LockoutEnabled = true
            };

            var hasher = new PasswordHasher<UserAccount>();
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Qwerty12#");

            modelBuilder.Entity<UserAccount>().HasData(adminUser);
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = adminRoleId, Name = "Administrator", NormalizedName = "ADMINISTRATOR", ConcurrencyStamp = "asd1" });

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = adminUserId, RoleId = adminRoleId });

            modelBuilder.Entity<Team>().HasData(
                new Team { TeamId = 1, Name = "Olimpia .NeT", League = "Premier League" },
                new Team { TeamId = 2, Name = "Java FC", League = "Premier League" }
            );

            modelBuilder.Entity<Match>().HasData(
                new Match { MatchId = 1, AwayTeamGoals = 2, AwayTeamId = 1, AwayTeamName = "Java FC", AwayTeamOdds = 2, Date = new DateOnly(2025, 2, 2), HomeTeamGoals = 2, HomeTeamId = 2, HomeTeamName = "Olimpia .NeT", HomeTeamOdds = 3 },
                new Match { MatchId = 2, AwayTeamGoals = 2, AwayTeamId = 1, AwayTeamName = "Java FC", AwayTeamOdds = 2, Date = new DateOnly(2025, 2, 3), HomeTeamGoals = 2, HomeTeamId = 2, HomeTeamName = "Olimpia .NeT", HomeTeamOdds = 3 }
            );
        }       
    }

}
