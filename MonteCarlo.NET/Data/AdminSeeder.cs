using Microsoft.AspNetCore.Identity;
using MonteCarlo.NET.Models;

namespace MonteCarlo.NET.Data
{
    public static class AdminSeeder
    {
        public const string AdminRole = "Administrator";

        public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
        {
            using var scope = services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(AdminSeeder));
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserAccount>>();

            if (!await roleManager.RoleExistsAsync(AdminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(AdminRole));
            }

            var email = configuration["Admin:Email"];
            var password = configuration["Admin:Password"];
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                logger.LogWarning("Admin:Email / Admin:Password not configured - skipping admin account seeding.");
                return;
            }

            var admin = await userManager.FindByEmailAsync(email);
            if (admin == null)
            {
                admin = new UserAccount
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "Admin",
                    Level = 0,
                    Balance = 0,
                    LockoutEnabled = true
                };

                var created = await userManager.CreateAsync(admin, password);
                if (!created.Succeeded)
                {
                    logger.LogError("Could not create admin account: {Errors}", string.Join("; ", created.Errors.Select(e => e.Description)));
                    return;
                }
            }

            if (!await userManager.IsInRoleAsync(admin, AdminRole))
            {
                await userManager.AddToRoleAsync(admin, AdminRole);
            }
        }
    }
}
