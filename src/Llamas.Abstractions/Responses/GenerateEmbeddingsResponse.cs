using System.Text.Json.Serialization;

namespace Llamas.Responses;

/// <summary>
/// A response containing generated embedding weights
/// </summary>
public record GenerateEmbeddingsResponse
{
    /// Generated embedding weights
    [JsonPropertyName("embedding"), JsonRequired]
    public required double[] Embedding { get; init; }
}
