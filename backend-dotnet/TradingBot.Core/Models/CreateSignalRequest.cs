namespace TradingBot.Core.Models;

public sealed record CreateSignalRequest(
    string Symbol,
    string Market,
    string Recommendation,
    decimal Confidence,
    decimal EntryPrice,
    decimal StopLoss,
    decimal TargetPrice,
    decimal RiskRewardRatio,
    string Rationale,
    string? Source);
