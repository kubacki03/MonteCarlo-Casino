using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MonteCarlo.NET.Data;
using MonteCarlo.NET.HealthCheck;
using MonteCarlo.NET.Models;
using MonteCarlo.NET.Services;
using MonteCarlo.NET.Services.Games;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MonteCarloDB");

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSignalR();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");

builder.Services.AddSingleton<ExceptionTracker>();
builder.Services.AddSingleton(Random.Shared);
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IScratchCardService, ScratchCardService>();
builder.Services.AddScoped<IFootballBetService, FootballBetService>();
builder.Services.AddScoped<ISlotService, SlotService>();
builder.Services.AddScoped<IRouletteService, RouletteService>();
builder.Services.AddScoped<IDiceService, DiceService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<ILimitService, LimitService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPayoutService, MonteCarlo.NET.Services.PayoutService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddDbContext<CasinoContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<UserAccount>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<CasinoContext>();

builder.Services.AddSingleton<IStripeClient>(new StripeClient(builder.Configuration["Stripe:SecretKey"]));

builder.Services.AddHealthChecks()
    .AddCheck<CpuHealthCheck>("CPU Load")
    .AddCheck<ExceptionHealthCheck>("Exception Check")
    .AddCheck("Application", () => HealthCheckResult.Healthy("Aplikacja dziala poprawnie"))
    .AddSqlServer(
        connectionString: connectionString!,
        name: "Database",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "db", "sql" });

builder.Services.AddHealthChecksUI(options =>
{
    options.SetEvaluationTimeInSeconds(15);
    options.MaximumHistoryEntriesPerEndpoint(50);
    options.AddHealthCheckEndpoint("Serwer MonteCarlo", "/health");
}).AddInMemoryStorage();

var app = builder.Build();

try
{
    await AdminSeeder.SeedAsync(app.Services, app.Configuration);
}
catch (Exception ex)
{
    app.Logger.LogWarning(ex, "Admin seeding failed - has the database been created? Run: dotnet ef database update");
}

app.UseExceptionHandler("/Home/CustomError");
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/chathub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecksUI();

app.Run();
