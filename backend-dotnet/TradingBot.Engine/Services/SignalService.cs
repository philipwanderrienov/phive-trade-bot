using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using TradingBot.Core.Data;
using TradingBot.Core.Entities;
using TradingBot.Core.Models;

namespace TradingBot.Engine.Services;

public class SignalService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly OrderRecommender _orderRecommender;
    private readonly IHubContext<NotificationHub> _notificationHub;

    public SignalService(
        ApplicationDbContext dbContext,
        OrderRecommender orderRecommender,
        IHubContext<NotificationHub> notificationHub)
    {
        _dbContext = dbContext;
        _orderRecommender = orderRecommender;
        _notificationHub = notificationHub;
    }

    public async Task<IReadOnlyList<RecommendationDto>> GetRecommendationsAsync(CancellationToken cancellationToken = default)
    {
        var persistedSignals = await _dbContext.Signals
            .AsNoTracking()
            .OrderByDescending((signal) => signal.CreatedAt)
            .Take(25)
            .ToArrayAsync(cancellationToken);

        if (persistedSignals.Length > 0)
        {
            return persistedSignals.Select(ToDto).ToArray();
        }

        var generatedSignals = SeedUniverse()
            .Select((asset) => ToDto(_orderRecommender.Recommend(
                asset.Symbol,
                asset.Market,
                asset.Candles,
                asset.MacroScore)))
            .ToArray();

        _dbContext.Signals.AddRange(generatedSignals.Select((signal) => ToEntity(signal, "seed-engine")));
        await _dbContext.SaveChangesAsync(cancellationToken);

        return generatedSignals;
    }

    public async Task<RecommendationDto> SynthesizeAsync(string symbol, CancellationToken cancellationToken = default)
    {
        var persistedSignal = await _dbContext.Signals
            .AsNoTracking()
            .Where((signal) => signal.Symbol == symbol)
            .OrderByDescending((signal) => signal.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (persistedSignal is not null)
        {
            return ToDto(persistedSignal);
        }

        var asset = SeedUniverse()
            .FirstOrDefault((candidate) => candidate.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase))
            ?? SeedUniverse()[0] with { Symbol = symbol, Market = "Custom" };

        var generatedSignal = ToDto(_orderRecommender.Recommend(asset.Symbol, asset.Market, asset.Candles, asset.MacroScore));
        _dbContext.Signals.Add(ToEntity(generatedSignal, "seed-engine"));
        await _dbContext.SaveChangesAsync(cancellationToken);

        return generatedSignal;
    }

    public async Task<RecommendationDto> CreateAsync(CreateSignalRequest request, CancellationToken cancellationToken = default)
    {
        var signal = new Signal
        {
            Symbol = request.Symbol.Trim().ToUpperInvariant(),
            Market = request.Market.Trim(),
            Recommendation = request.Recommendation.Trim(),
            Confidence = request.Confidence,
            EntryPrice = request.EntryPrice,
            StopLoss = request.StopLoss,
            TargetPrice = request.TargetPrice,
            RiskRewardRatio = request.RiskRewardRatio,
            Rationale = request.Rationale.Trim(),
            Source = string.IsNullOrWhiteSpace(request.Source) ? "api" : request.Source.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Signals.Add(signal);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var createdSignal = ToDto(signal);
        await _notificationHub.Clients.All.SendAsync("SignalCreated", createdSignal, cancellationToken);

        return createdSignal;
    }

    public static IReadOnlyList<MarketCandle> GetSeedCandles(string symbol)
    {
        return SeedUniverse()
            .FirstOrDefault((asset) => asset.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase))
            ?.Candles
            ?? SeedUniverse()[0].Candles;
    }

    private static RecommendationDto ToDto(OrderRecommendation recommendation)
    {
        return new RecommendationDto(
            recommendation.Symbol,
            recommendation.Market,
            recommendation.Action,
            recommendation.Confidence,
            recommendation.EntryPrice,
            recommendation.StopLoss,
            recommendation.TargetPrice,
            recommendation.RiskRewardRatio,
            recommendation.Rationale,
            recommendation.GeneratedAt);
    }

    private static RecommendationDto ToDto(Signal signal)
    {
        return new RecommendationDto(
            signal.Symbol,
            signal.Market,
            signal.Recommendation,
            signal.Confidence,
            signal.EntryPrice,
            signal.StopLoss,
            signal.TargetPrice,
            signal.RiskRewardRatio,
            signal.Rationale,
            new DateTimeOffset(DateTime.SpecifyKind(signal.CreatedAt, DateTimeKind.Utc)));
    }

    private static Signal ToEntity(RecommendationDto recommendation, string source)
    {
        return new Signal
        {
            Symbol = recommendation.Symbol,
            Market = recommendation.Market,
            Recommendation = recommendation.Recommendation,
            Confidence = recommendation.Confidence,
            EntryPrice = recommendation.EntryPrice,
            StopLoss = recommendation.StopLoss,
            TargetPrice = recommendation.TargetPrice,
            RiskRewardRatio = recommendation.RiskRewardRatio,
            Rationale = recommendation.Rationale,
            Source = source,
            CreatedAt = recommendation.GeneratedAt.UtcDateTime
        };
    }

    private static AssetSeed[] SeedUniverse()
    {
        var now = DateTimeOffset.UtcNow.Date;

        return
        [
            new AssetSeed("AAPL", "NASDAQ", 0.6m, BuildCandles("AAPL", now, [188.2m, 189.9m, 191.4m, 190.8m, 194.6m, 196.2m])),
            new AssetSeed("TSLA", "NASDAQ", -0.2m, BuildCandles("TSLA", now, [182.4m, 178.1m, 176.9m, 181.2m, 179.6m, 175.3m])),
            new AssetSeed("BTC-USD", "Crypto", 1.1m, BuildCandles("BTC-USD", now, [67200m, 68150m, 67620m, 69080m, 70440m, 71120m])),
            new AssetSeed("EURUSD=X", "Forex", 0.1m, BuildCandles("EURUSD=X", now, [1.081m, 1.084m, 1.082m, 1.087m, 1.089m, 1.088m]))
        ];
    }

    private static MarketCandle[] BuildCandles(string symbol, DateTimeOffset anchor, decimal[] closes)
    {
        return closes
            .Select((close, index) =>
            {
                var open = index == 0 ? close * 0.996m : closes[index - 1];
                var high = Math.Max(open, close) * 1.008m;
                var low = Math.Min(open, close) * 0.992m;

                return new MarketCandle(
                    symbol,
                    anchor.AddDays(index - closes.Length + 1),
                    Math.Round(open, 4),
                    Math.Round(high, 4),
                    Math.Round(low, 4),
                    close,
                    1_000_000m + index * 125_000m);
            })
            .ToArray();
    }

    private sealed record AssetSeed(
        string Symbol,
        string Market,
        decimal MacroScore,
        IReadOnlyList<MarketCandle> Candles);
}
