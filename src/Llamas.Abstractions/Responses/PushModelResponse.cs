using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Llamas.Responses;

/// <summary>
/// A response generated while pushing a model
/// </summary>
[JsonPolymorphic]
[JsonDerivedType(typeof(PushStatusResponse))]
[JsonDerivedType(typeof(UploadingResponse))]
public abstract record PushModelResponse
{
    /// <summary>
    /// A status response containing a status update
    /// </summary>
    public sealed record PushStatusResponse : PushModelResponse
    {
        /// Status update
        [JsonPropertyName("status")]
        public required string Status { get; init; }
    }

    /// <summary>
    /// An uploading response containing information about the file upload
    /// </summary>
    public sealed record UploadingResponse : PushModelResponse
    {
        /// Status update
        [JsonPropertyName("status")]
        public required string Status { get; init; }

        /// Digest being uploaded
        [JsonPropertyName("digest"), JsonRequired]
        public required string Digest { get; init; }

        /// Total bytes to upload
        [JsonPropertyName("total"), JsonRequired]
        public required long Total { get; init; }
    }

    /// <summary>
    /// Custom JSON converter for <see cref="PushModelResponse"/>
    /// </summary>
    internal class JsonConverter : JsonConverter<PushModelResponse>
    {
        /// <inheritdoc />
        public override PushModelResponse Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            string? status = null;
            string? digest = null;
            long? total = null;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    if (status is not null && digest is null && !total.HasValue)
                    {
                        return new PushStatusResponse { Status = status };
                    }

                    if (status is not null && digest is not null && total.HasValue)
                    {
                        return new UploadingResponse
                        {
                            Status = status,
                            Digest = digest,
                            Total = total.Value
                        };
                    }

                    throw new JsonException($"Failed to deserialize {nameof(PushModelResponse)}");
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
                    default:
                        continue;
                }
            }

            throw new JsonException($"Failed to deserialize {nameof(PushModelResponse)}");
        }

        /// <inheritdoc />
        public override void Write(
            Utf8JsonWriter writer,
            PushModelResponse value,
            JsonSerializerOptions options
        ) => throw new NotSupportedException("Serialization is not supported for this request");
    }
}
