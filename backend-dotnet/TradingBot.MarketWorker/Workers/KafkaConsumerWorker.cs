using System.Globalization;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using TradingBot.Core.Data;
using TradingBot.Core.Entities;
using TradingBot.Core.Models;

namespace TradingBot.MarketWorker.Workers;

public class KafkaConsumerWorker : BackgroundService
{
    private readonly ILogger<KafkaConsumerWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly string _bootstrapServers;
    private readonly string _topic;
    private readonly string _consumerGroup;
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);

    public KafkaConsumerWorker(
        ILogger<KafkaConsumerWorker> logger,
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _bootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS")
            ?? configuration["Kafka:BootstrapServers"]
            ?? "localhost:9092";
        _topic = Environment.GetEnvironmentVariable("SIGNAL_TOPIC")
            ?? configuration["Kafka:Topic"]
            ?? "trading.signals";
        _consumerGroup = Environment.GetEnvironmentVariable("KAFKA_CONSUMER_GROUP")
            ?? configuration["Kafka:ConsumerGroup"]
            ?? "tradingbot-market-worker";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = _consumerGroup,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        consumer.Subscribe(_topic);
        _logger.LogInformation("Kafka consumer subscribed to {Topic} at {BootstrapServers}.", _topic, _bootstrapServers);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);

                if (result?.Message?.Value is null)
                {
                    continue;
                }

                await PersistSignalAsync(result.Message.Value, stoppingToken);
                consumer.Commit(result);
            }
            catch (ConsumeException exception)
            {
                _logger.LogError(exception, "Kafka consume error on topic {Topic}.", _topic);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to process Kafka signal message.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        consumer.Close();
    }

    private async Task PersistSignalAsync(string message, CancellationToken cancellationToken)
    {
        var signal = JsonSerializer.Deserialize<KafkaTradingSignal>(message, _serializerOptions);

        if (signal is null || string.IsNullOrWhiteSpace(signal.Symbol))
        {
            _logger.LogWarning("Skipping Kafka message with invalid signal payload: {Message}", message);
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var entity = ToEntity(signal);

        var duplicateExists = await dbContext.Signals.AnyAsync((existing) =>
            existing.Symbol == entity.Symbol
            && existing.Source == entity.Source
            && existing.CreatedAt == entity.CreatedAt,
            cancellationToken);

        if (duplicateExists)
        {
            _logger.LogInformation(
                "Skipping duplicate signal for {Symbol} generated at {GeneratedAt}.",
                entity.Symbol,
                entity.CreatedAt);
            return;
        }

        dbContext.Signals.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation(
            "Persisted Kafka signal {Recommendation} {Symbol} with confidence {Confidence}.",
            entity.Recommendation,
            entity.Symbol,
            entity.Confidence);
    }

    private static Signal ToEntity(KafkaTradingSignal signal)
    {
        var recommendation = NormalizeRecommendation(signal.Recommendation);
        var entryPrice = signal.EntryPrice > 0m ? signal.EntryPrice : signal.LastClose;
        var risk = CalculateRisk(recommendation, entryPrice);

        return new Signal
        {
            Symbol = signal.Symbol.Trim().ToUpperInvariant(),
            Market = string.IsNullOrWhiteSpace(signal.Market) ? InferMarket(signal.Symbol) : signal.Market.Trim(),
            Recommendation = recommendation,
            Confidence = Math.Clamp(signal.Confidence, 0m, 99m),
            EntryPrice = Math.Round(entryPrice, 6),
            StopLoss = risk.StopLoss,
            TargetPrice = risk.TargetPrice,
            RiskRewardRatio = risk.RiskRewardRatio,
            Rationale = BuildRationale(signal),
            Source = string.IsNullOrWhiteSpace(signal.Source) ? "python-scheduler" : signal.Source.Trim(),
            CreatedAt = signal.GeneratedAt.UtcDateTime
        };
    }

    private static string NormalizeRecommendation(string recommendation)
    {
        if (string.IsNullOrWhiteSpace(recommendation))
        {
            return "Hold";
        }

        return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(recommendation.Trim().ToLowerInvariant());
    }

    private static string InferMarket(string symbol)
    {
        if (symbol.EndsWith("-USD", StringComparison.OrdinalIgnoreCase))
        {
            return "Crypto";
        }

        if (symbol.EndsWith("=X", StringComparison.OrdinalIgnoreCase))
        {
            return "Forex";
        }

        return "NASDAQ";
    }

    private static string BuildRationale(KafkaTradingSignal signal)
    {
        if (!string.IsNullOrWhiteSpace(signal.Rationale))
        {
            return signal.Rationale.Trim();
        }

        return string.Create(
            CultureInfo.InvariantCulture,
            $"Python Kafka signal. Model score {signal.ModelScore}, momentum {signal.Momentum}, macro events {signal.MacroEvents}.");
    }

    private static (decimal StopLoss, decimal TargetPrice, decimal RiskRewardRatio) CalculateRisk(
        string recommendation,
        decimal entryPrice)
    {
        if (entryPrice <= 0m)
        {
            return (0m, 0m, 0m);
        }

        var riskBuffer = 0.025m;
        var rewardBuffer = 0.055m;

        if (recommendation.Equals("Sell", StringComparison.OrdinalIgnoreCase))
        {
            return (
                Math.Round(entryPrice * (1m + riskBuffer), 2),
                Math.Round(entryPrice * (1m - rewardBuffer), 2),
                Math.Round(rewardBuffer / riskBuffer, 2));
        }

        return (
            Math.Round(entryPrice * (1m - riskBuffer), 2),
            Math.Round(entryPrice * (1m + rewardBuffer), 2),
            Math.Round(rewardBuffer / riskBuffer, 2));
    }
}
