namespace TradingBot.MarketWorker.Workers;

public class KafkaConsumerWorker : BackgroundService
{
    private readonly ILogger<KafkaConsumerWorker> _logger;

    public KafkaConsumerWorker(ILogger<KafkaConsumerWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Waiting for Kafka messages on trading.signals.");
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
