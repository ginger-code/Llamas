using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Llamas.Primitives;

namespace Llamas.Requests;

/// <summary>
/// Request to generate embeddings
/// </summary>
public sealed record GenerateEmbeddingsRequest
{
    /// <summary>
    /// Request to generate embeddings
    /// </summary>
    public GenerateEmbeddingsRequest() { }

    /// <summary>
    /// Request to generate embeddings
    /// </summary>
    [method: SetsRequiredMembers]
    public GenerateEmbeddingsRequest(
        string model,
        string prompt,
        KeepAliveTimeSpan? keepAlive = null,
        Dictionary<string, object>? options = null
    )
    {
        Model = model;
        Prompt = prompt;
        KeepAlive = keepAlive;
        Options = options;
    }

    /// Model is the model name; it should be a name familiar to Ollama from the library at https://ollama.com/library
    [JsonPropertyName("model"), JsonRequired]
    public required string Model { get; init; }

    /// Prompt is the textual prompt to embed
    [JsonPropertyName("prompt"), JsonRequired]
    public required string Prompt { get; init; }

    /// KeepAlive controls how long the model will stay loaded in memory following this request.
    [JsonPropertyName("keep_alive"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public KeepAliveTimeSpan? KeepAlive { get; init; }

    /// Options lists model-specific options. For example, temperature can be set through this field, if the model supports it.
    [JsonPropertyName("options"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? Options { get; init; }
}
