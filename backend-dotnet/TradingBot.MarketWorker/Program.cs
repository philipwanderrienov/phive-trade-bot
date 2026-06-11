using Microsoft.EntityFrameworkCore;
using TradingBot.Core.Data;
using TradingBot.MarketWorker.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>((options) =>
{
    options.UseNpgsql(GetPostgresConnectionString(builder.Configuration));
});
builder.Services.AddHostedService<KafkaConsumerWorker>();

var host = builder.Build();
host.Run();

static string GetPostgresConnectionString(IConfiguration configuration)
{
    return Environment.GetEnvironmentVariable("POSTGRES_CONNECTION")
        ?? configuration.GetConnectionString("Postgres")
        ?? throw new InvalidOperationException("Postgres connection string is not configured.");
}
