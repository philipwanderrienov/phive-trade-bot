namespace TradingBot.Core.Entities;

public class Signal
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Symbol { get; set; } = string.Empty;
    public string Market { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
    public decimal Confidence { get; set; }
    public decimal EntryPrice { get; set; }
    public decimal StopLoss { get; set; }
    public decimal TargetPrice { get; set; }
    public decimal RiskRewardRatio { get; set; }
    public string Rationale { get; set; } = string.Empty;
    public string Source { get; set; } = "engine";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
