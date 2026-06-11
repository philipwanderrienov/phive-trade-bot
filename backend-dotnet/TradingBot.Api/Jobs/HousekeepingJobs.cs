using Microsoft.EntityFrameworkCore;
using TradingBot.Core.Data;

namespace TradingBot.Api.Jobs;

public class HousekeepingJobs : BackgroundService
{
    private readonly ILogger<HousekeepingJobs> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan _interval = TimeSpan.FromHours(6);

    public HousekeepingJobs(ILogger<HousekeepingJobs> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CleanupAsync(stoppingToken);
            await Task.Delay(_interval, stoppingToken);
        }
    }

    public async Task CleanupAsync(CancellationToken cancellationToken = default)
    {
        var retentionDays = int.TryParse(
            Environment.GetEnvironmentVariable("SIGNAL_RETENTION_DAYS"),
            out var configuredRetentionDays)
            ? configuredRetentionDays
            : 30;
        var cutoff = DateTime.UtcNow.AddDays(-retentionDays);

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var deletedSignals = await dbContext.Signals
            .Where((signal) => signal.CreatedAt < cutoff)
            .ExecuteDeleteAsync(cancellationToken);
        var deletedMarketData = await dbContext.MarketData
            .Where((marketData) => marketData.CreatedAt < cutoff)
            .ExecuteDeleteAsync(cancellationToken);

        if (deletedSignals > 0 || deletedMarketData > 0)
        {
            _logger.LogInformation(
                "Housekeeping deleted {Signals} signals and {MarketData} market data rows older than {Cutoff}.",
                deletedSignals,
                deletedMarketData,
                cutoff);
        }
    }
}
