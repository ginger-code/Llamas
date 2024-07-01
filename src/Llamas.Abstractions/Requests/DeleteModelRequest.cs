using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Llamas.Requests;

/// <summary>
/// Request to delete a model
/// </summary>
public sealed record DeleteModelRequest
{
    /// <summary>
    /// Request to delete a model
    /// </summary>
    public DeleteModelRequest() { }

    /// <summary>
    /// Request to delete a model
    /// </summary>
    [method: SetsRequiredMembers]
    public DeleteModelRequest(string name)
    {
        Name = name;
    }

    /// Name of model to delete
    [JsonPropertyName("name"), JsonRequired]
    public required string Name { get; init; }
}
