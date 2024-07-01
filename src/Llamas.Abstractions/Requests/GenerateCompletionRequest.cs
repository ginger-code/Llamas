using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Llamas.Enums;
using Llamas.Models;
using Llamas.Primitives;

namespace Llamas.Requests;

/// <summary>
/// Request to generate a prompt completion
/// </summary>
public sealed record GenerateCompletionRequest
{
    /// <summary>
    /// Request to generate a prompt completion
    /// </summary>
    public GenerateCompletionRequest() { }

    /// <summary>
    /// Request to generate a prompt completion
    /// </summary>
    [method: SetsRequiredMembers]
    public GenerateCompletionRequest(
        string model,
        string prompt,
        string? system = null,
        string? template = null,
        int[]? context = null,
        ResponseFormat? format = null,
        bool? raw = null,
        KeepAliveTimeSpan? keepAlive = null,
        string[]? images = null,
        ModelOptions? options = null
    )
    {
        Model = model;
        Prompt = prompt;
        System = system;
        Template = template;
        Context = context;
        Format = format;
        Raw = raw;
        KeepAlive = keepAlive;
        Images = images;
        Options = options;
    }

    /// Model is the model name; it should be a name familiar to Ollama from the library at https://ollama.com/library
    [JsonPropertyName("model"), JsonRequired]
    public required string Model { get; init; }

    /// Prompt is the textual prompt to send to the model.
    [JsonPropertyName("prompt"), JsonRequired]
    public required string Prompt { get; init; }

    /// System overrides the model's default system message/prompt.
    [JsonPropertyName("system"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? System { get; init; }

    /// Template overrides the model's default prompt template.
    [JsonPropertyName("template"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Template { get; init; }

    /// Context is the context parameter returned from a previous call to Generate call. It can be used to keep a short conversational memory.
    [JsonPropertyName("context"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int[]? Context { get; init; }

    /// Format specifies the format to return a response in. When format is set to json, the output will always be a well-formed JSON object. It's important to also instruct the model to respond in JSON.
    [JsonPropertyName("format"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ResponseFormat? Format { get; init; }

    /// Raw set to true means that no formatting will be applied to the prompt.
    [JsonPropertyName("raw"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Raw { get; init; }

    /// KeepAlive controls how long the model will stay loaded in memory following this request.
    [JsonPropertyName("keep_alive"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public KeepAliveTimeSpan? KeepAlive { get; init; }

    /// Images is an optional list of base64-encoded images accompanying this request, for multimodal models.
    [JsonPropertyName("images"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? Images { get; init; }

    /// Options lists model-specific options. For example, temperature can be set through this field, if the model supports it.
    [JsonPropertyName("options"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ModelOptions? Options { get; init; }
}
