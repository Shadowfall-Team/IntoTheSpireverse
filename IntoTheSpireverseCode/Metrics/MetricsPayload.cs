using System.Text.Json;
using System.Text.Json.Serialization;
using MegaCrit.Sts2.Core.Runs.Metrics;
using MegaCrit.Sts2.GameInfo;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Metrics;

internal sealed class MetricsPayload
{
    [JsonPropertyName("schema_version")] public required int SchemaVersion { get; init; }

    [JsonPropertyName("environment")] public required string Environment { get; init; }

    [JsonPropertyName("mod_version")] public required string ModVersion { get; init; }

    [JsonPropertyName("data")] public required JsonElement Data { get; init; }
}

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    IncludeFields = true,
    UseStringEnumConverter = true,
    Converters = [typeof(ModelIdMetricsConverter)])]
[JsonSerializable(typeof(MetricsPayload))]
[JsonSerializable(typeof(RunMetrics))]
internal partial class MetricsPayloadSerializerContext : JsonSerializerContext;