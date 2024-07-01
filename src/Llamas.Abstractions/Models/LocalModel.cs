using System;
using System.Text.Json.Serialization;

namespace Llamas.Models;

/// <summary>
/// A model available locally
/// </summary>
public record LocalModel
{
    /// Name of the model
    [JsonPropertyName("name"), JsonRequired]
    public required string Name { get; init; }

    /// Date of last modification
    [JsonPropertyName("modified_at")]
    public DateTimeOffset? ModifiedAt { get; init; }

    /// Size of model
    [JsonPropertyName("size"), JsonRequired]
    public required long Size { get; init; }

    /// Model digest
    [JsonPropertyName("digest"), JsonRequired]
    public required string Digest { get; init; }

    /// Details about the model
    [JsonPropertyName("details"), JsonRequired]
    public required ModelInfoDetails Details { get; init; }
}

/// <summary>
///Internal collection type for retrieving local models
/// </summary>
internal record LocalModels(LocalModel[] Models);
