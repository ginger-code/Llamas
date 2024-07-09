using System.Text.Json.Serialization;

namespace Llamas.Models;

/// <summary>
/// Parameters to be passed to the model, overriding values set in Modelfile
/// </summary>
public sealed record ModelOptions
{
    /// <summary>
    /// Create a ModelParameters instance
    /// </summary>
    public ModelOptions() { }

    /// <summary>
    /// Create a ModelParameters instance
    /// </summary>
    public ModelOptions(
        int? mirostat = null,
        float? mirostatEta = null,
        float? mirostatTau = null,
        int? numCtx = null,
        int? repeatLastN = null,
        float? repeatPenalty = null,
        float? temperature = null,
        int? seed = null,
        string[]? stop = null,
        float? tfsZ = null,
        int? numPredict = null,
        int? topK = null,
        float? topP = null
    )
    {
        Mirostat = mirostat;
        MirostatEta = mirostatEta;
        MirostatTau = mirostatTau;
        NumCtx = numCtx;
        RepeatLastN = repeatLastN;
        RepeatPenalty = repeatPenalty;
        Temperature = temperature;
        Seed = seed;
        Stop = stop;
        TfsZ = tfsZ;
        NumPredict = numPredict;
        TopK = topK;
        TopP = topP;
    }

    /// <summary>
    /// Enable Mirostat sampling for controlling perplexity. (default: 0, 0 = disabled, 1 = Mirostat, 2 = Mirostat 2.0)
    /// </summary>
    [JsonPropertyName("mirostat"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Mirostat { get; init; }

    /// <summary>
    /// Influences how quickly the algorithm responds to feedback from the generated text.
    /// A lower learning rate will result in slower adjustments, while a higher learning rate will make the algorithm more responsive. (Default: 0.1)
    /// </summary>
    [JsonPropertyName("mirostat_eta"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? MirostatEta { get; init; }

    /// <summary>
    /// Controls the balance between coherence and diversity of the output.
    /// A lower value will result in more focused and coherent text. (Default: 5.0)
    /// </summary>
    [JsonPropertyName("mirostat_tau"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? MirostatTau { get; init; }

    /// <summary>
    /// Sets the size of the context window used to generate the next token. (Default: 2048)
    /// </summary>
    [JsonPropertyName("num_ctx"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? NumCtx { get; init; }

    /// <summary>
    /// Sets how far back for the model to look back to prevent repetition. (Default: 64, 0 = disabled, -1 = num_ctx)
    /// </summary>
    [
        JsonPropertyName("repeat_last_n"),
        JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)
    ]
    public int? RepeatLastN { get; init; }

    /// <summary>
    /// Sets how strongly to penalize repetitions.
    /// A higher value (e.g., 1.5) will penalize repetitions more strongly, while a lower value (e.g., 0.9) will be more lenient. (Default: 1.1)
    /// </summary>
    [
        JsonPropertyName("repeat_penalty"),
        JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)
    ]
    public float? RepeatPenalty { get; init; }

    /// <summary>
    /// The temperature of the model.
    /// Increasing the temperature will make the model answer more creatively. (Default: 0.8)
    /// </summary>
    [JsonPropertyName("temperature"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? Temperature { get; init; }

    /// <summary>
    /// Sets the random number seed to use for generation.
    /// Setting this to a specific number will make the model generate the same text for the same prompt. (Default: 0)
    /// </summary>
    [JsonPropertyName("seed"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Seed { get; init; }

    /// <summary>
    /// Sets the stop sequences to use.
    /// When this pattern is encountered the LLM will stop generating text and return.
    /// Multiple stop patterns may be set by specifying multiple separate stop parameters in a modelfile.
    /// </summary>
    [JsonPropertyName("stop"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? Stop { get; init; }

    /// <summary>
    /// Tail free sampling is used to reduce the impact of less probable tokens from the output.
    /// A higher value (e.g., 2.0) will reduce the impact more, while a value of 1.0 disables this setting. (default: 1)
    /// </summary>
    [JsonPropertyName("tfs_z"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? TfsZ { get; init; }

    /// <summary>
    /// Maximum number of tokens to predict when generating text. (Default: 128, -1 = infinite generation, -2 = fill context)
    /// </summary>
    [JsonPropertyName("num_predict"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? NumPredict { get; init; }

    /// <summary>
    /// Reduces the probability of generating nonsense.
    /// A higher value (e.g. 100) will give more diverse answers, while a lower value (e.g. 10) will be more conservative. (Default: 40)
    /// </summary>
    [JsonPropertyName("top_k"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? TopK { get; init; }

    /// <summary>
    /// Works together with top-k.
    /// A higher value (e.g., 0.95) will lead to more diverse text, while a lower value (e.g., 0.5) will generate more focused and conservative text. (Default: 0.9)
    /// </summary>
    [JsonPropertyName("top_p"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? TopP { get; init; }
}
