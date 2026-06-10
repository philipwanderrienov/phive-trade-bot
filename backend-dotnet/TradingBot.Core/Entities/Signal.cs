namespace TradingBot.Core.Entities;

public class Signal
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Symbol { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
    public decimal Confidence { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
