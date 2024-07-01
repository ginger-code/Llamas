using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Llamas.Responses;

/// <summary>
/// A response generated while pulling a model
/// </summary>
[JsonPolymorphic]
[JsonDerivedType(typeof(PullStatusResponse))]
[JsonDerivedType(typeof(DownloadingResponse))]
public abstract record PullModelResponse
{
    /// <summary>
    /// A status response containing a status update
    /// </summary>
    public sealed record PullStatusResponse : PullModelResponse
    {
        /// Status update
        [JsonPropertyName("status"), JsonRequired]
        public required string Status { get; init; }
    }

    /// <summary>
    /// A downloading response containing information about the file download
    /// </summary>
    public sealed record DownloadingResponse : PullModelResponse
    {
        /// Status update
        [JsonPropertyName("status"), JsonRequired]
        public required string Status { get; init; }

        /// Digest being downloaded
        [JsonPropertyName("digest"), JsonRequired]
        public required string Digest { get; init; }

        /// Total bytes to download
        [JsonPropertyName("total"), JsonRequired]
        public required long Total { get; init; }

        /// Bytes downloaded
        [JsonPropertyName("completed")]
        public required long Completed { get; init; }
    }

    /// <summary>
    /// Custom JSON converter for <see cref="PullModelResponse"/>
    /// </summary>
    internal class JsonConverter : JsonConverter<PullModelResponse>
    {
        /// <inheritdoc/>
        public override PullModelResponse Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            string? status = null;
            string? digest = null;
            long? total = null;
            long completed = 0;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    if (status is not null && digest is null && !total.HasValue)
                    {
                        return new PullStatusResponse { Status = status };
                    }

                    if (status is not null && digest is not null && total.HasValue)
                    {
                        return new DownloadingResponse
                        {
                            Status = status,
                            Digest = digest,
                            Total = total.Value,
                            Completed = completed
                        };
                    }

                    throw new JsonException($"Failed to deserialize {nameof(PullModelResponse)}");
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException("Invalid JSON");

                var propertyName = reader.GetString();
                ArgumentNullException.ThrowIfNull(propertyName);
                reader.Read();
                switch (propertyName)
                {
                    case "status":
                        if (reader.TokenType != JsonTokenType.String)
                            throw new JsonException("Status must be a string");
                        status = reader.GetString();
                        break;
                    case "digest":
                        if (reader.TokenType != JsonTokenType.String)
                            throw new JsonException("Digest must be a string");
                        digest = reader.GetString();
                        break;
                    case "total":
                        if (reader.TokenType != JsonTokenType.Number)
                            throw new JsonException("Total must be a number");
                        total = reader.GetInt64();
                        break;
                    case "completed":
                        if (reader.TokenType != JsonTokenType.Number)
                            throw new JsonException("Completed must be a number");
                        completed = reader.GetInt64();
                        break;
                    default:
                        continue;
                }
            }

            throw new JsonException($"Failed to deserialize {nameof(PullModelResponse)}");
        }

        /// <inheritdoc/>
        public override void Write(
            Utf8JsonWriter writer,
            PullModelResponse value,
            JsonSerializerOptions options
        ) => throw new NotSupportedException("Serialization is not supported for this request");
    }
}
