namespace TradingBot.Core.Models;

public sealed record RecommendationDto(
    string Symbol,
    string Recommendation,
    decimal Confidence,
    DateTimeOffset GeneratedAt);
