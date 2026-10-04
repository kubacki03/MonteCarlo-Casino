using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MonteCarlo.NET.Models;

namespace MonteCarlo.NET.Data
{
    public class KasynoContext : IdentityDbContext<KontoUzytkownika>
    {
        public KasynoContext(DbContextOptions<KasynoContext> options) : base(options) { }

        public DbSet<Ban> Ban { get; set; }
        public DbSet<Gra> Gra { get; set; }
        public DbSet<GraKonto> GraKonto { get; set; }
        public DbSet<KontoUzytkownika> KontoUzytkownika { get; set; }
        public DbSet<Limit> Limit { get; set; }
        public DbSet<Transakcja> Transakcja { get; set; }
        public DbSet<Zgloszenie> Zgloszenie { get; set; }
        public DbSet<Druzyna> Druzyna { get; set; }
        public DbSet<Mecz> Mecz { get; set; }

        // Dodane DbSet dla Zaklad
        public DbSet<Zaklad> Zaklady { get; set; }

        public DbSet<Level> Levele { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Level>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<GraKonto>()
                .HasKey(gk => gk.IdGraKonto);

            modelBuilder.Entity<GraKonto>()
                .HasOne(gk => gk.Gra)
                .WithMany(g => g.GryKonta)
                .HasForeignKey(gk => gk.IdGry);

            modelBuilder.Entity<GraKonto>()
                .HasOne(gk => gk.KontoUzytkownika)
                .WithMany(ku => ku.GryKonta)
                .HasForeignKey(gk => gk.KontoUzytkownikaId);

            modelBuilder.Entity<Ban>()
                .HasKey(b => b.IdBana);

            modelBuilder.Entity<Gra>()
                .HasKey(g => g.IdGry);

            modelBuilder.Entity<Limit>()
                .HasKey(l => l.IdLimitu);

            modelBuilder.Entity<Transakcja>()
                .HasKey(t => t.IdTransakcji);

            modelBuilder.Entity<Zgloszenie>()
                .HasKey(z => z.IdZgloszenia);

            modelBuilder.Entity<Zaklad>()
           .HasKey(z => z.IdZakladu);


            modelBuilder.Entity<Mecz>()
                .HasOne(m => m.DruzynaHome)
                .WithMany()
                .HasForeignKey(m => m.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mecz>()
                .HasOne(m => m.DruzynaAway)
                .WithMany()
                .HasForeignKey(m => m.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            string adminRoleId = Guid.NewGuid().ToString();
            string adminUserId = Guid.NewGuid().ToString();

            modelBuilder.Entity<Gra>().HasData(
                new Gra { IdGry = 1, Nazwa = "Slotsy", MinStawka = 1 },
                new Gra { IdGry = 2, Nazwa = "Zdrapka koniczynka", MinStawka = 1 },
                new Gra { IdGry = 3, Nazwa = "Zdrapka Prosta", MinStawka = 1 },
                new Gra { IdGry = 4, Nazwa = "Wyscigi Konne", MinStawka = 1 },
                new Gra { IdGry = 5, Nazwa = "Obstawianie", MinStawka = 1 },
                new Gra { IdGry = 6, Nazwa = "Ruletka", MinStawka = 1 },
                new Gra { IdGry = 7, Nazwa = "Kosci", MinStawka = 1 }
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

            var adminUser = new KontoUzytkownika
            {
                Id = adminUserId,
                Imie = "Admin",
                Nazwisko = "Admin",
                UserName = "admin@gmail.com",
                NormalizedUserName = "ADMIN@GMAIL.COM",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                EmailConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                Level = 0,
                Saldo = 0,
                LockoutEnabled = true
            };

            var hasher = new PasswordHasher<KontoUzytkownika>();
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Qwerty12#");

            modelBuilder.Entity<KontoUzytkownika>().HasData(adminUser);
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = adminRoleId, Name = "Administrator", NormalizedName = "ADMINISTRATOR", ConcurrencyStamp = "asd1" });

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = adminUserId, RoleId = adminRoleId });

            modelBuilder.Entity<Druzyna>().HasData(
                new Druzyna { DruzynaId = 1, Name = "Olimpia .NeT", League = "Premier League" },
                new Druzyna { DruzynaId = 2, Name = "Java FC", League = "Premier League" }
            );

            modelBuilder.Entity<Mecz>().HasData(
                new Mecz { MeczId = 1, AwayTeamGoals = 2, AwayTeamId = 1, AwayTeamName = "Java FC", AwayTeamOdds = 2, data = DateOnly.Parse("02.02.2025"), HomeTeamGoals = 2, HomeTeamId = 2, HomeTeamName = "Olimpia .NeT", HomeTeamOdds = 3 },
                new Mecz { MeczId = 2, AwayTeamGoals = 2, AwayTeamId = 1, AwayTeamName = "Java FC", AwayTeamOdds = 2, data = DateOnly.Parse("03.02.2025"), HomeTeamGoals = 2, HomeTeamId = 2, HomeTeamName = "Olimpia .NeT", HomeTeamOdds = 3 }
            );
        }       
    }

}
