using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Llamas.Requests;

/// <summary>
/// Request to copy a model
/// </summary>
public sealed record CopyModelRequest
{
    /// <summary>
    /// Request to copy a model
    /// </summary>
    public CopyModelRequest() { }

    /// <summary>
    /// Request to copy a model
    /// </summary>
    [method: SetsRequiredMembers]
    public CopyModelRequest(string source, string destination)
    {
        Source = source;
        Destination = destination;
    }

    /// Name of the model to copy
    [JsonPropertyName("source"), JsonRequired]
    public required string Source { get; init; }

    /// Name of copied model
    [JsonPropertyName("destination"), JsonRequired]
    public required string Destination { get; init; }
}
