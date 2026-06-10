using TradingBot.Core.Models;

namespace TradingBot.Engine.Services;

public class BacktestingEngine
{
    public object Run(BacktestRequest request)
    {
        return new
        {
            request.Symbol,
            request.StrategyName,
            status = "queued",
            requestedAt = DateTimeOffset.UtcNow
        };
    }
}
