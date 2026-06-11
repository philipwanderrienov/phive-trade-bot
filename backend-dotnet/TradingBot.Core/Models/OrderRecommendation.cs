namespace TradingBot.Core.Models;

public sealed record OrderRecommendation(
    string Symbol,
    string Market,
    string Action,
    decimal Confidence,
    decimal EntryPrice,
    decimal StopLoss,
    decimal TargetPrice,
    decimal RiskRewardRatio,
    string Rationale,
    DateTimeOffset GeneratedAt);
