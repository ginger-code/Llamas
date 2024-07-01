using System.Text.Json.Serialization;

namespace Llamas.Models;

/// <summary>
/// Details about a model
/// </summary>
public sealed record ModelInfoDetails
{
    /// The format of the model
    [JsonPropertyName("format"), JsonRequired]
    public required string Format { get; init; }

    /// Family to which the model belongs
    [JsonPropertyName("family"), JsonRequired]
    public required string Family { get; init; }

    /// Collection of families to which the model belongs
    [JsonPropertyName("families"), JsonRequired]
    public required string[] Families { get; init; }

    /// Size of model parameters
    [JsonPropertyName("parameter_size"), JsonRequired]
    public required string ParameterSize { get; init; }

    /// Level of quantization applied to model
    [JsonPropertyName("quantization_level"), JsonRequired]
    public required string QuantizationLevel { get; init; }
}
