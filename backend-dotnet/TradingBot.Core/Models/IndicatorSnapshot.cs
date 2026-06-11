namespace TradingBot.Core.Models;

public sealed record IndicatorSnapshot(
    string Symbol,
    decimal Rsi,
    decimal Macd,
    decimal Momentum,
    decimal Volatility,
    DateTimeOffset GeneratedAt);
