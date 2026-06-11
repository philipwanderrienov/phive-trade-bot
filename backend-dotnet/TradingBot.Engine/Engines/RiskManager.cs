using TradingBot.Core.Models;

namespace TradingBot.Engine.Engines;

public class RiskManager
{
    public RiskAssessment Assess(string action, decimal entryPrice, IndicatorSnapshot indicators)
    {
        if (entryPrice <= 0m)
        {
            return new RiskAssessment(0m, 0m, 0m, 0m, false, "Invalid entry price.");
        }

        var volatilityBuffer = Math.Clamp(indicators.Volatility / 100m, 0.015m, 0.08m);
        var rewardBuffer = volatilityBuffer * 2.4m;

        var stopLoss = action == "Sell"
            ? entryPrice * (1m + volatilityBuffer)
            : entryPrice * (1m - volatilityBuffer);

        var targetPrice = action == "Sell"
            ? entryPrice * (1m - rewardBuffer)
            : entryPrice * (1m + rewardBuffer);

        var risk = Math.Abs(entryPrice - stopLoss);
        var reward = Math.Abs(targetPrice - entryPrice);
        var riskReward = risk == 0m ? 0m : Math.Round(reward / risk, 2);
        var positionSize = Math.Round(Math.Clamp(2m / Math.Max(volatilityBuffer * 100m, 1m), 0.25m, 2m), 2);
        var allowed = action != "Hold" && riskReward >= 1.8m;

        return new RiskAssessment(
            riskReward,
            Math.Round(stopLoss, 2),
            Math.Round(targetPrice, 2),
            positionSize,
            allowed,
            allowed ? "Risk/reward meets strategy threshold." : "Signal retained for review.");
    }
}
