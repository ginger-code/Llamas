using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Llamas.Requests;

/// <summary>
/// Request to pull a model for local execution
/// </summary>
public sealed record PullModelRequest
{
    /// <summary>
    /// Request to pull a model for local execution
    /// </summary>
    public PullModelRequest() { }

    /// <summary>
    /// Request to pull a model for local execution
    /// </summary>
    [method: SetsRequiredMembers]
    public PullModelRequest(string name, bool? allowInsecureConnections = null)
    {
        Name = name;
        AllowInsecureConnections = allowInsecureConnections;
    }

    /// Model is the model name; it should be a name familiar to Ollama from the library at https://ollama.com/library
    [JsonPropertyName("name"), JsonRequired]
    public required string Name { get; init; }

    /// Allow insecure connections to the library; only use this if you are pulling from your own library during development
    [JsonPropertyName("insecure"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AllowInsecureConnections { get; init; }
}
