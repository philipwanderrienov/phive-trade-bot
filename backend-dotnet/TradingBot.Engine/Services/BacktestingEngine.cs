using TradingBot.Core.Models;

namespace TradingBot.Engine.Services;

public class BacktestingEngine
{
    public BacktestResult Run(BacktestRequest request)
    {
        var candles = SignalService.GetSeedCandles(request.Symbol);
        var trades = Math.Max(candles.Count - 1, 1);
        var wins = 0;
        var equity = 10_000m;
        var peak = equity;
        var maxDrawdown = 0m;

        for (var index = 1; index < candles.Count; index++)
        {
            var previous = candles[index - 1].Close;
            var current = candles[index].Close;
            var returnPercent = previous == 0m ? 0m : (current - previous) / previous;
            var tradePnl = equity * returnPercent * 0.35m;

            if (tradePnl > 0m)
            {
                wins++;
            }

            equity += tradePnl;
            peak = Math.Max(peak, equity);
            maxDrawdown = Math.Max(maxDrawdown, peak == 0m ? 0m : (peak - equity) / peak * 100m);
        }

        return new BacktestResult(
            request.Symbol,
            request.StrategyName,
            request.From,
            request.To,
            10_000m,
            Math.Round(equity, 2),
            Math.Round(equity - 10_000m, 2),
            Math.Round((decimal)wins / trades * 100m, 2),
            trades,
            Math.Round(maxDrawdown, 2),
            DateTimeOffset.UtcNow);
    }
}
