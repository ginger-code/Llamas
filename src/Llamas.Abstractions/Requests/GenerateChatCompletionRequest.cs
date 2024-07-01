using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Llamas.Enums;
using Llamas.Models;
using Llamas.Primitives;

namespace Llamas.Requests;

/// <summary>
/// Request to generate a chat completion
/// </summary>
public sealed record GenerateChatCompletionRequest
{
    /// <summary>
    /// Request to generate a chat completion
    /// </summary>
    public GenerateChatCompletionRequest() { }

    /// <summary>
    /// Request to generate a chat completion
    /// </summary>
    [method: SetsRequiredMembers]
    public GenerateChatCompletionRequest(
        string model,
        List<ChatMessage> messages,
        ResponseFormat? format = null,
        KeepAliveTimeSpan? keepAlive = null,
        ModelOptions? options = null
    )
    {
        Model = model;
        Messages = messages;
        Format = format;
        KeepAlive = keepAlive;
        Options = options;
    }

    /// Model is the model name; it should be a name familiar to Ollama from the library at https://ollama.com/library
    [JsonPropertyName("model"), JsonRequired]
    public required string Model { get; init; }

    /// Messages is the messages of the chat - can be used to keep a chat memory.
    [JsonPropertyName("messages"), JsonRequired]
    public required List<ChatMessage> Messages { get; init; }

    /// Format specifies the format to return a response in. When format is set to json, the output will always be a well-formed JSON object. It's important to also instruct the model to respond in JSON.
    [JsonPropertyName("format"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ResponseFormat? Format { get; init; }

    /// KeepAlive controls how long the model will stay loaded in memory following this request.
    [JsonPropertyName("keep_alive"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public KeepAliveTimeSpan? KeepAlive { get; init; }

    /// Options lists model-specific options. For example, temperature can be set through this field, if the model supports it.
    [JsonPropertyName("options"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ModelOptions? Options { get; init; }
}
