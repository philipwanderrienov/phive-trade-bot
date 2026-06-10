namespace TradingBot.Core.Models;

public sealed record ReportSummary(
    decimal Pnl,
    decimal WinRate,
    int Trades,
    DateTimeOffset GeneratedAt);
