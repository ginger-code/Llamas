using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Llamas.Models;

/// <summary>
/// Information about a model
/// </summary>
public sealed record ModelInfo
{
    /// Model file
    [JsonPropertyName("modelfile"), JsonRequired]
    public required string File { get; init; }

    /// Model parameters
    [JsonPropertyName("parameters"), JsonRequired]
    public required string Parameters { get; init; }

    /// Model template
    [JsonPropertyName("template"), JsonRequired]
    public required string Template { get; init; }

    /// Details about the model
    [JsonPropertyName("details"), JsonRequired]
    public required ModelInfoDetails Details { get; init; }

    /// Info about the model
    [JsonPropertyName("model_info"), JsonRequired]
    public required Dictionary<string, object>? Info { get; init; }
}
