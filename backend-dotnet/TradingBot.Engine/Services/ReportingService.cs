using TradingBot.Core.Models;

namespace TradingBot.Engine.Services;

public class ReportingService
{
    public ReportSummary GetSummary()
    {
        return new ReportSummary(
            Pnl: 0m,
            WinRate: 0m,
            Trades: 0,
            GeneratedAt: DateTimeOffset.UtcNow);
    }
}
