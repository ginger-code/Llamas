using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Llamas.Requests;

/// <summary>
/// Request to push a local model to a remote source
/// </summary>
public sealed record PushModelRequest
{
    /// <summary>
    /// Request to push a local model to a remote source
    /// </summary>
    public PushModelRequest() { }

    /// <summary>
    /// Request to push a local model to a remote source.
    /// Uses the format "namespace/model:tag"
    /// </summary>
    [method: SetsRequiredMembers]
    public PushModelRequest(string name, bool? allowInsecureConnections = null)
    {
        Name = name;
        AllowInsecureConnections = allowInsecureConnections;
    }

    /// <summary>
    /// Request to push a local model to a remote source
    /// Uses the format "namespace/model:tag"
    /// </summary>
    [method: SetsRequiredMembers]
    public PushModelRequest(
        string nameSpace,
        string model,
        string tag = "latest",
        bool? allowInsecureConnections = null
    )
    {
        Name = $"{nameSpace}/{model}:{tag}";
        AllowInsecureConnections = allowInsecureConnections;
    }

    /// Model is the model name; it should be a name familiar to Ollama from the library at https://ollama.com/library
    [JsonPropertyName("name"), JsonRequired]
    public required string Name { get; init; }

    /// Allow insecure connections to the library; only use this if you are pulling from your own library during development
    [JsonPropertyName("insecure"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AllowInsecureConnections { get; init; }
}
