namespace TradingBot.Core.Models;

public sealed record ReportSummary(
    decimal Pnl,
    decimal WinRate,
    int Trades,
    decimal Equity,
    decimal MaxDrawdown,
    int ActiveSignals,
    DateTimeOffset GeneratedAt);
