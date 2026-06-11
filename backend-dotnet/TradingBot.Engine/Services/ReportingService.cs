using TradingBot.Core.Models;

namespace TradingBot.Engine.Services;

public class ReportingService
{
    public ReportSummary GetSummary()
    {
        return new ReportSummary(
            Pnl: 742.35m,
            WinRate: 58.8m,
            Trades: 34,
            Equity: 10_742.35m,
            MaxDrawdown: 4.7m,
            ActiveSignals: 4,
            GeneratedAt: DateTimeOffset.UtcNow);
    }
}
