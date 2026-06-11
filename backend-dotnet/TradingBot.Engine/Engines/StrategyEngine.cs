using TradingBot.Core.Models;

namespace TradingBot.Engine.Engines;

public class StrategyEngine
{
    public string Evaluate(IndicatorSnapshot indicators, decimal macroScore)
    {
        var score = indicators.Momentum + indicators.Macd + macroScore;

        if (indicators.Rsi > 72m || score < -2.5m)
        {
            return "Sell";
        }

        if (indicators.Rsi < 36m || score > 2.5m)
        {
            return "Buy";
        }

        return "Hold";
    }

    public decimal CalculateConfidence(IndicatorSnapshot indicators, decimal macroScore)
    {
        var signalStrength = Math.Abs(indicators.Momentum) + Math.Abs(indicators.Macd) + Math.Abs(macroScore);
        var riskPenalty = Math.Min(indicators.Volatility * 1.25m, 22m);
        return Math.Round(Math.Clamp(52m + signalStrength * 4m - riskPenalty, 35m, 94m), 2);
    }
}
