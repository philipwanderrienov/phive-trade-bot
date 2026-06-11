using TradingBot.Core.Models;

namespace TradingBot.Engine.Engines;

public class IndicatorEngine
{
    public IndicatorSnapshot Calculate(string symbol, IReadOnlyList<MarketCandle> candles)
    {
        if (candles.Count < 2)
        {
            return new IndicatorSnapshot(symbol, 50m, 0m, 0m, 0m, DateTimeOffset.UtcNow);
        }

        var closes = candles.Select((c) => c.Close).ToArray();
        var momentum = Math.Round((closes[^1] - closes[0]) / closes[0] * 100m, 2);
        var rsi = CalculateRsi(closes);
        var macd = Math.Round(Average(closes.TakeLast(3)) - Average(closes.TakeLast(6)), 2);
        var volatility = Math.Round(
            candles.Average((c) => c.High == 0m ? 0m : (c.High - c.Low) / c.High) * 100m,
            2);

        return new IndicatorSnapshot(symbol, rsi, macd, momentum, volatility, DateTimeOffset.UtcNow);
    }

    private static decimal CalculateRsi(IReadOnlyList<decimal> closes)
    {
        var gains = new List<decimal>();
        var losses = new List<decimal>();

        for (var index = 1; index < closes.Count; index++)
        {
            var change = closes[index] - closes[index - 1];
            gains.Add(Math.Max(change, 0m));
            losses.Add(Math.Abs(Math.Min(change, 0m)));
        }

        var averageGain = Average(gains);
        var averageLoss = Average(losses);

        if (averageLoss == 0m)
        {
            return 100m;
        }

        var relativeStrength = averageGain / averageLoss;
        return Math.Round(100m - (100m / (1m + relativeStrength)), 2);
    }

    private static decimal Average(IEnumerable<decimal> values)
    {
        var materialized = values.ToArray();
        return materialized.Length == 0 ? 0m : materialized.Average();
    }
}
