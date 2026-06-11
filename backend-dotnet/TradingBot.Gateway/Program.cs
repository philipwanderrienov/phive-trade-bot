using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new
{
    service = "TradingBot Gateway",
    status = "ready"
}));

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    checkedAt = DateTimeOffset.UtcNow
}));

await app.UseOcelot();

await app.RunAsync();
