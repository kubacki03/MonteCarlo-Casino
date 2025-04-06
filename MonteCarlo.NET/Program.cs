using Microsoft.EntityFrameworkCore;
using MonteCarlo.NET.Data;
using MonteCarlo.NET.Models;
using Microsoft.AspNetCore.Identity;
using Stripe;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MonteCarlo.NET.HealthCheck;

var builder = WebApplication.CreateBuilder(args);


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
builder.Services.AddSingleton<ExceptionTracker>();




builder.Services.AddHealthChecks()
    .AddCheck<CpuHealthCheck>("CPU Load")
    .AddCheck<ExceptionHealthCheck>("Exception Check")
.AddSqlServer(connectionString: builder.Configuration.GetConnectionString("MonteCarloDB"));




builder.Services.AddDbContext<KasynoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MonteCarloDB"))
);

builder.Services.AddDefaultIdentity<KontoUzytkownika>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<KasynoContext>();

builder.Services.AddSingleton<IStripeClient>(new StripeClient(builder.Configuration["Stripe:SecretKey"]));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


builder.Services.AddHealthChecks()
    .AddSqlServer(
        connectionString: builder.Configuration.GetConnectionString("MonteCarloDB"),
        name: "Database",
        failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
        tags: new[] { "db", "sql" })
    .AddCheck("Application", () => HealthCheckResult.Healthy("Aplikacja dziala poprawnie"));

builder.Services.AddSingleton<IHealthCheck, CpuHealthCheck>();



builder.Services.AddHealthChecksUI(options =>
{
    options.SetEvaluationTimeInSeconds(15);
    options.MaximumHistoryEntriesPerEndpoint(50);
    options.AddHealthCheckEndpoint("Serwer MonteCarlo", "/health");
}).AddInMemoryStorage();

var app = builder.Build();






app.UseCors("AllowAll");

app.UseExceptionHandler("/Home/CustomError");
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseExceptionHandler("/Home/CustomError");
}

app.UseMiddleware<ExceptionHandlingMiddleware>();


app.UseRouting();

app.UseSession();
app.UseStaticFiles();


app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{

    endpoints.MapHub<ChatHub>("/chathub");


    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");


    endpoints.MapRazorPages();


    endpoints.MapHealthChecks("/health", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });


    endpoints.MapHealthChecksUI();
});

app.Run();