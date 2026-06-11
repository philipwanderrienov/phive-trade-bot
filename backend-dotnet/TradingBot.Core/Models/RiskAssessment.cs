namespace TradingBot.Core.Models;

public sealed record RiskAssessment(
    decimal RiskRewardRatio,
    decimal StopLoss,
    decimal TargetPrice,
    decimal PositionSizePercent,
    bool IsAllowed,
    string Reason);
