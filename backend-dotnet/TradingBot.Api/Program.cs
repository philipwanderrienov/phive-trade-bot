var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddLogging();
builder.Services.AddScoped<TradingBot.Engine.Services.BacktestingEngine>();
builder.Services.AddScoped<TradingBot.Engine.Services.ReportingService>();
builder.Services.AddScoped<TradingBot.Engine.Services.SignalService>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    checkedAt = DateTimeOffset.UtcNow
}));

app.MapControllers();

app.Run();
