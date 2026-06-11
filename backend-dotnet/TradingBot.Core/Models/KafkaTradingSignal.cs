using System.Text.Json.Serialization;

namespace TradingBot.Core.Models;

public sealed record KafkaTradingSignal(
    [property: JsonPropertyName("symbol")] string Symbol,
    [property: JsonPropertyName("market")] string? Market,
    [property: JsonPropertyName("recommendation")] string Recommendation,
    [property: JsonPropertyName("confidence")] decimal Confidence,
    [property: JsonPropertyName("model_score")] decimal ModelScore,
    [property: JsonPropertyName("macro_events")] int MacroEvents,
    [property: JsonPropertyName("entry_price")] decimal EntryPrice,
    [property: JsonPropertyName("last_close")] decimal LastClose,
    [property: JsonPropertyName("momentum")] decimal Momentum,
    [property: JsonPropertyName("source")] string? Source,
    [property: JsonPropertyName("rationale")] string? Rationale,
    [property: JsonPropertyName("generated_at")] DateTimeOffset GeneratedAt);
