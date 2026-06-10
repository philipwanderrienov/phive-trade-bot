namespace TradingBot.Engine.Engines;

public class RiskManager
{
    public bool IsAllowed(decimal exposure) => exposure >= 0m;
}
