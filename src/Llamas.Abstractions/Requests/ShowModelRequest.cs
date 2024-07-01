using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Llamas.Requests;

/// <summary>
/// Request to retrieve model details
/// </summary>
public sealed record ShowModelRequest
{
    /// <summary>
    /// Request to retrieve model details
    /// </summary>
    public ShowModelRequest() { }

    /// <summary>
    /// Request to retrieve model details
    /// </summary>
    [method: SetsRequiredMembers]
    public ShowModelRequest(string name, string? template, string? system)
    {
        Name = name;
        Template = template;
        System = system;
    }

    /// Name of the model to retrieve details for
    [JsonPropertyName("model"), JsonRequired]
    public required string Name { get; init; }

    /// Optional template used to overwrite default
    [JsonPropertyName("template"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Template { get; init; }

    /// Optional system message used to overwrite default
    [JsonPropertyName("system"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? System { get; init; }
}
