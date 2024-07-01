using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Llamas.Responses;

/// <summary>
/// A response generated as a completion
/// </summary>
[JsonPolymorphic]
[JsonDerivedType(typeof(GenerateProgressResponse))]
[JsonDerivedType(typeof(GenerateCompleteResponse))]
public abstract record GenerateCompletionResponse
{
    /// <summary>
    /// A progress response containing response data
    /// </summary>
    public sealed record GenerateProgressResponse : GenerateCompletionResponse
    {
        /// Model is the model name; it should be a name familiar to Ollama from the library at https://ollama.com/library
        [JsonPropertyName("model"), JsonRequired]
        public required string Model { get; set; }

        /// When the completion was generated
        [JsonPropertyName("created_at"), JsonRequired]
        public required DateTimeOffset CreatedAt { get; set; }

        /// The next token in the response stream, empty if the last token in the response stream
        [JsonPropertyName("response"), JsonRequired]
        public required string Response { get; set; }

        /// Should be false if this is not the last response
        [JsonPropertyName("done"), JsonRequired]
        public required bool Done { get; set; }
    }

    /// <summary>
    /// A completion response containing diagnostic and analytical data
    /// </summary>
    public sealed record GenerateCompleteResponse : GenerateCompletionResponse
    {
        /// Model is the model name; it should be a name familiar to Ollama from the library at https://ollama.com/library
        [JsonPropertyName("model"), JsonRequired]
        public required string Model { get; set; }

        /// When the completion was generated
        [JsonPropertyName("created_at"), JsonRequired]
        public required DateTimeOffset CreatedAt { get; set; }

        /// The next token in the response stream, empty if the last token in the response stream
        [JsonPropertyName("response"), JsonRequired]
        public required string Response { get; set; }

        /// Should be true if this is the last response
        [JsonPropertyName("done"), JsonRequired]
        public required bool Done { get; set; }

        /// An encoding of the conversation used in this response, this can be sent in the next request to keep a conversational memory
        [JsonPropertyName("context"), JsonRequired]
        public required int[] Context { get; init; }

        /// Time spent in nanoseconds generating the response
        [JsonPropertyName("total_duration"), JsonRequired]
        public required long TotalDuration { get; init; }

        /// Time spent in nanoseconds loading the model
        [JsonPropertyName("load_duration"), JsonRequired]
        public required long LoadDuration { get; init; }

        /// Number of tokens in the prompt
        [JsonPropertyName("prompt_eval_count"), JsonRequired]
        public required int PromptEvaluationCount { get; init; }

        /// Time spent in nanoseconds evaluating the prompt
        [JsonPropertyName("prompt_eval_duration"), JsonRequired]
        public required long PromptEvaluationDuration { get; init; }

        /// Number of tokens in the response
        [JsonPropertyName("eval_count"), JsonRequired]
        public required int EvaluationCount { get; init; }

        /// Time in nanoseconds spent generating the response
        [JsonPropertyName("eval_duration"), JsonRequired]
        public required long EvaluationDuration { get; init; }

        /// How fast the response is generated in tokens per second (token/s)
        [JsonPropertyName("tokens_per_second")]
        public long TokensPerSecondGenerated => EvaluationCount / EvaluationDuration * 10000000000L;
    }

    /// <summary>
    /// Custom JSON converter for <see cref="GenerateCompletionResponse" />
    /// </summary>
    internal class JsonConverter : JsonConverter<GenerateCompletionResponse>
    {
        /// <inheritdoc/>
        public override GenerateCompletionResponse Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            string? model = null;
            DateTimeOffset? createdAt = null;
            string? response = null;
            bool? done = null;
            int[]? context = null;
            long? totalDuration = null;
            long? loadDuration = null;
            int? promptEvaluationCount = null;
            long? promptEvaluationDuration = null;
            int? evaluationCount = null;
            long? evaluationDuration = null;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    if (
                        model is not null
                        && createdAt.HasValue
                        && response is not null
                        && done.HasValue
                        && context is null
                    )
                    {
                        return new GenerateProgressResponse
                        {
                            Model = model,
                            CreatedAt = createdAt.Value,
                            Response = response,
                            Done = done.Value
                        };
                    }

                    if (
                        model is not null
                        && createdAt.HasValue
                        && response is not null
                        && done.HasValue
                        && context is not null
                        && totalDuration.HasValue
                        && loadDuration.HasValue
                        && promptEvaluationCount.HasValue
                        && promptEvaluationDuration.HasValue
                        && evaluationCount.HasValue
                        && evaluationDuration.HasValue
                    )
                    {
                        return new GenerateCompleteResponse
                        {
                            Model = model,
                            CreatedAt = createdAt.Value,
                            Response = response,
                            Done = done.Value,
                            Context = context,
                            TotalDuration = totalDuration.Value,
                            LoadDuration = loadDuration.Value,
                            PromptEvaluationCount = promptEvaluationCount.Value,
                            PromptEvaluationDuration = promptEvaluationDuration.Value,
                            EvaluationCount = evaluationCount.Value,
                            EvaluationDuration = evaluationDuration.Value,
                        };
                    }

                    throw new JsonException(
                        $"Failed to deserialize {nameof(GenerateCompletionResponse)}"
                    );
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException("Invalid JSON");

                var propertyName = reader.GetString();
                ArgumentNullException.ThrowIfNull(propertyName);
                reader.Read();
                switch (propertyName)
                {
                    case "model":
                        if (reader.TokenType != JsonTokenType.String)
                            throw new JsonException("Model must be a string");
                        model = reader.GetString()!;
                        break;
                    case "created_at":
                        if (reader.TokenType != JsonTokenType.String)
                            throw new JsonException("CreatedAt must be a datetime string");
                        createdAt = DateTimeOffset.Parse(reader.GetString()!);
                        break;
                    case "response":
                        if (reader.TokenType != JsonTokenType.String)
                            throw new JsonException("Response must be a string");
                        response = reader.GetString();
                        break;
                    case "done":
                        if (
                            reader.TokenType != JsonTokenType.True
                            && reader.TokenType != JsonTokenType.False
                        )
                            throw new JsonException("Done must be a boolean");
                        done = reader.GetBoolean();
                        break;
                    case "context":
                        if (reader.TokenType == JsonTokenType.Null)
                            break;
                        if (reader.TokenType != JsonTokenType.StartArray)
                            throw new JsonException(
                                "Context must be an array of integers, but did not start with '['"
                            );

                        var c = new List<int>();
                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonTokenType.EndArray)
                                break;
                            if (reader.TokenType != JsonTokenType.Number)
                                throw new JsonException(
                                    "Invalid JSON, expected number or end of array"
                                );
                            c.Add(reader.GetInt32());
                        }

                        context = c.ToArray();
                        break;
                    case "total_duration":
                        if (reader.TokenType != JsonTokenType.Number)
                            throw new JsonException("TotalDuration must be an Int64");
                        totalDuration = reader.GetInt64();
                        break;
                    case "load_duration":
                        if (reader.TokenType != JsonTokenType.Number)
                            throw new JsonException("LoadDuration must be an Int64");
                        loadDuration = reader.GetInt64();
                        break;
                    case "prompt_eval_count":
                        if (reader.TokenType != JsonTokenType.Number)
                            throw new JsonException("PromptEvaluationCount must be an Int32");
                        promptEvaluationCount = reader.GetInt32();
                        break;
                    case "prompt_eval_duration":
                        if (reader.TokenType != JsonTokenType.Number)
                            throw new JsonException("PromptEvaluationDuration must be an Int64");
                        promptEvaluationDuration = reader.GetInt64();
                        break;
                    case "eval_count":
                        if (reader.TokenType != JsonTokenType.Number)
                            throw new JsonException("EvaluationCount must be an Int32");
                        evaluationCount = reader.GetInt32();
                        break;
                    case "eval_duration":
                        if (reader.TokenType != JsonTokenType.Number)
                            throw new JsonException("EvaluationDuration must be an Int64");
                        evaluationDuration = reader.GetInt64();
                        break;
                    default:
                        continue;
                }
            }

            throw new JsonException($"Failed to deserialize {nameof(GenerateCompletionResponse)}");
        }

        /// <inheritdoc/>
        public override void Write(
            Utf8JsonWriter writer,
            GenerateCompletionResponse value,
            JsonSerializerOptions options
        ) => throw new NotSupportedException("Serialization is not supported for this response");
    }
}
