namespace TradingBot.Core.Models;

public sealed record BacktestRequest(
    string Symbol,
    DateTimeOffset From,
    DateTimeOffset To,
    string StrategyName);
