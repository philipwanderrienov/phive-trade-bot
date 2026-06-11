using System.Globalization;
using TradingBot.Core.Models;
using TradingBot.Engine.Engines;

namespace TradingBot.Engine.Services;

public class OrderRecommender
{
    private readonly IndicatorEngine _indicatorEngine;
    private readonly StrategyEngine _strategyEngine;
    private readonly RiskManager _riskManager;

    public OrderRecommender(
        IndicatorEngine indicatorEngine,
        StrategyEngine strategyEngine,
        RiskManager riskManager)
    {
        _indicatorEngine = indicatorEngine;
        _strategyEngine = strategyEngine;
        _riskManager = riskManager;
    }

    public OrderRecommendation Recommend(string symbol, string market, IReadOnlyList<MarketCandle> candles, decimal macroScore)
    {
        var indicators = _indicatorEngine.Calculate(symbol, candles);
        var action = _strategyEngine.Evaluate(indicators, macroScore);
        var confidence = _strategyEngine.CalculateConfidence(indicators, macroScore);
        var entryPrice = candles.LastOrDefault()?.Close ?? 0m;
        var risk = _riskManager.Assess(action, entryPrice, indicators);

        if (!risk.IsAllowed && action != "Hold")
        {
            action = "Watch";
            confidence = Math.Min(confidence, 68m);
        }

        return new OrderRecommendation(
            symbol,
            market,
            action,
            confidence,
            Math.Round(entryPrice, 2),
            risk.StopLoss,
            risk.TargetPrice,
            risk.RiskRewardRatio,
            BuildRationale(indicators, risk),
            DateTimeOffset.UtcNow);
    }

    private static string BuildRationale(IndicatorSnapshot indicators, RiskAssessment risk)
    {
        return string.Create(
            CultureInfo.InvariantCulture,
            $"RSI {indicators.Rsi}, MACD {indicators.Macd}, momentum {indicators.Momentum}%. {risk.Reason}");
    }
}
