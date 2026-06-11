using Microsoft.EntityFrameworkCore;
using TradingBot.Api.Jobs;
using TradingBot.Core.Configuration;
using TradingBot.Core.Data;
using TradingBot.Engine.Services;

DotEnvLoader.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddLogging();
builder.Services.AddSignalR();
builder.Services.AddDbContext<ApplicationDbContext>((options) =>
{
    options.UseNpgsql(GetPostgresConnectionString(builder.Configuration));
});
builder.Services.AddCors((options) =>
{
    options.AddDefaultPolicy((policy) =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});
builder.Services.AddScoped<TradingBot.Engine.Engines.IndicatorEngine>();
builder.Services.AddScoped<TradingBot.Engine.Engines.StrategyEngine>();
builder.Services.AddScoped<TradingBot.Engine.Engines.RiskManager>();
builder.Services.AddScoped<TradingBot.Engine.Services.OrderRecommender>();
builder.Services.AddScoped<TradingBot.Engine.Services.BacktestingEngine>();
builder.Services.AddScoped<TradingBot.Engine.Services.ReportingService>();
builder.Services.AddScoped<TradingBot.Engine.Services.SignalService>();
builder.Services.AddHostedService<HousekeepingJobs>();

var app = builder.Build();

app.UseCors();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
}

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    checkedAt = DateTimeOffset.UtcNow
}));

app.MapControllers();
app.MapHub<NotificationHub>("/hub/notifications");

app.Run();

static string GetPostgresConnectionString(IConfiguration configuration)
{
    return Environment.GetEnvironmentVariable("POSTGRES_CONNECTION")
        ?? configuration.GetConnectionString("Postgres")
        ?? throw new InvalidOperationException("Postgres connection string is not configured.");
}
