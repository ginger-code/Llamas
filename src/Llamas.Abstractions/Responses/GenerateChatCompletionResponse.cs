using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Llamas.Models;

namespace Llamas.Responses;

/// <summary>
/// A response generated during a chat conversation
/// </summary>
[JsonPolymorphic]
[JsonDerivedType(typeof(ChatProgressResponse))]
[JsonDerivedType(typeof(ChatCompleteResponse))]
public abstract record GenerateChatCompletionResponse
{
    /// <summary>
    /// A progress response containing message data
    /// </summary>
    public sealed record ChatProgressResponse : GenerateChatCompletionResponse
    {
        /// Model is the model name; it should be a name familiar to Ollama from the library at https://ollama.com/library
        [JsonPropertyName("model"), JsonRequired]
        public required string Model { get; init; }

        /// When the completion was generated
        [JsonPropertyName("created_at"), JsonRequired]
        public required DateTimeOffset Created { get; init; }

        /// The next chunk of the generated chat completion
        [JsonPropertyName("message")]
        public required ChatMessage Message { get; init; }

        /// Should be false if this is not the last response
        [JsonPropertyName("done"), JsonRequired]
        public required bool Done { get; init; }
    }

    /// <summary>
    /// A completion response containing diagnostic and analytical data
    /// </summary>
    public sealed record ChatCompleteResponse : GenerateChatCompletionResponse
    {
        /// Model is the model name; it should be a name familiar to Ollama from the library at https://ollama.com/library
        [JsonPropertyName("model"), JsonRequired]
        public required string Model { get; init; }

        /// When the completion was generated
        [JsonPropertyName("created_at"), JsonRequired]
        public required DateTimeOffset Created { get; init; }

        /// Should be true if this is not the last response
        [JsonPropertyName("done"), JsonRequired]
        public required bool Done { get; init; }

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
    /// Custom JSON converter for <see cref="GenerateChatCompletionResponse"/>
    /// </summary>
    internal class JsonConverter : JsonConverter<GenerateChatCompletionResponse>
    {
        /// <inheritdoc />
        public override GenerateChatCompletionResponse Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            string? model = null;
            DateTimeOffset? createdAt = null;
            ChatMessage? message = null;
            bool? done = null;
            long totalDuration = 0;
            long loadDuration = 0;
            int promptEvaluationCount = 0;
            long promptEvaluationDuration = 0;
            int evaluationCount = 0;
            long evaluationDuration = 0;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    if (
                        model is not null
                        && createdAt.HasValue
                        && message is not null
                        && done.HasValue
                        && !done.Value
                    )
                    {
                        return new ChatProgressResponse
                        {
                            Model = model,
                            Created = createdAt.Value,
                            Message = message,
                            Done = done.Value
                        };
                    }

                    if (model is not null && createdAt.HasValue && done.HasValue)
                    {
                        return new ChatCompleteResponse
                        {
                            Model = model,
                            Created = createdAt.Value,
                            Done = done.Value,
                            TotalDuration = totalDuration,
                            LoadDuration = loadDuration,
                            PromptEvaluationCount = promptEvaluationCount,
                            PromptEvaluationDuration = promptEvaluationDuration,
                            EvaluationCount = evaluationCount,
                            EvaluationDuration = evaluationDuration,
                        };
                    }

                    Debugger.Break();
                    throw new JsonException(
                        $"Failed to deserialize {nameof(GenerateChatCompletionResponse)}"
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
                    case "message":
                        if (reader.TokenType != JsonTokenType.StartObject)
                            throw new JsonException("Message must be an object");
                        message = new ChatMessage.JsonConverter().Read(
                            ref reader,
                            typeof(ChatMessage),
                            options
                        );
                        break;
                    case "done":
                        if (
                            reader.TokenType != JsonTokenType.True
                            && reader.TokenType != JsonTokenType.False
                        )
                            throw new JsonException("Done must be a boolean");
                        done = reader.GetBoolean();
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

            throw new JsonException(
                $"Failed to deserialize {nameof(GenerateChatCompletionResponse)}"
            );
        }

        /// <inheritdoc />
        public override void Write(
            Utf8JsonWriter writer,
            GenerateChatCompletionResponse value,
            JsonSerializerOptions options
        ) => throw new NotSupportedException("Serialization is not supported for this response");
    }
}
