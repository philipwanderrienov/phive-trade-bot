namespace TradingBot.Core.Models;

public sealed record BacktestResult(
    string Symbol,
    string StrategyName,
    DateTimeOffset From,
    DateTimeOffset To,
    decimal StartingEquity,
    decimal EndingEquity,
    decimal Pnl,
    decimal WinRate,
    int Trades,
    decimal MaxDrawdown,
    DateTimeOffset GeneratedAt);
