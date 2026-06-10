using TradingBot.Core.Models;

namespace TradingBot.Engine.Services;

public class SignalService
{
    public RecommendationDto Synthesize(string symbol)
    {
        return new RecommendationDto(
            symbol,
            "hold",
            0.62m,
            DateTimeOffset.UtcNow);
    }
}
