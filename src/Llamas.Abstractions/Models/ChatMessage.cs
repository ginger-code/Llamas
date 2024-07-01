using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Llamas.Enums;

namespace Llamas.Models;

/// <summary>
/// A chat conversation message
/// </summary>
public sealed record ChatMessage
{
    /// <summary>
    /// Create chat conversation message entry
    /// </summary>
    public ChatMessage() { }

    /// <summary>
    /// Create chat conversation message entry
    /// </summary>
    [method: SetsRequiredMembers]
    public ChatMessage(Roles role, string content, string[]? images)
    {
        Role = role;
        Content = content;
        Images = images;
    }

    /// The role of the message's sender ("system", "user", or "assistant")
    [JsonPropertyName("role"), JsonRequired,]
    public required Roles Role { get; init; }

    /// The text content of the message
    [JsonPropertyName("content"), JsonRequired]
    public required string Content { get; init; }

    /// Optional list of images
    [JsonPropertyName("images"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required string[]? Images { get; init; }

    /// <summary>
    /// Custom JSON reader for <see cref="ChatMessage"/>
    /// </summary>
    internal sealed class JsonConverter : JsonConverter<ChatMessage>
    {
        /// <inheritdoc />
        public override ChatMessage Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            Roles? role = null;
            string? content = null;
            List<string> images = [];
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    if (role.HasValue && content is not null)
                        return new ChatMessage(
                            role.Value,
                            content,
                            images.Count > 0 ? images.ToArray() : null
                        );

                    throw new JsonException(
                        $"Failed to parse object of type '{nameof(ChatMessage)}'"
                    );
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException("Invalid JSON, expected property name");

                var propertyName = reader.GetString();
                ArgumentException.ThrowIfNullOrEmpty(propertyName);
                reader.Read();
                switch (propertyName)
                {
                    case "role":
                        if (reader.TokenType != JsonTokenType.String)
                            throw new JsonException("Role must be a string");
                        var roleStr = reader.GetString()!;
                        role = roleStr.ToLower() switch
                        {
                            "user" => Roles.User,
                            "system" => Roles.System,
                            "assistant" => Roles.Assistant,
                            _
                                => throw new JsonException(
                                    $"Invalid role '{roleStr}', must be 'user','system', or 'assistant'"
                                )
                        };
                        break;
                    case "content":
                        if (reader.TokenType != JsonTokenType.String)
                            throw new JsonException("Content must be a string");
                        content = reader.GetString()!;
                        break;
                    case "images":
                        if (reader.TokenType == JsonTokenType.Null)
                            break;
                        if (reader.TokenType != JsonTokenType.StartArray)
                            throw new JsonException(
                                "Images must be an array of strings, but did not start with '['"
                            );

                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonTokenType.EndArray)
                                break;
                            if (reader.TokenType != JsonTokenType.String)
                                throw new JsonException(
                                    "Invalid JSON, expected string or end of array"
                                );
                            var i = reader.GetString();
                            if (i is not null)
                                images.Add(i);
                        }

                        break;
                    default:
                        continue;
                }
            }

            throw new JsonException($"Failed to deserialize '{nameof(ChatMessage)}'");
        }

        /// <inheritdoc />
        public override void Write(
            Utf8JsonWriter writer,
            ChatMessage value,
            JsonSerializerOptions options
        )
        {
            writer.WriteStartObject();
            writer.WriteString("role", value.Role.ToString().ToLower());
            writer.WriteString("content", value.Content);
            if (value.Images is not null)
            {
                writer.WritePropertyName("images");
                writer.WriteStartArray();
                foreach (var image in value.Images)
                {
                    writer.WriteStringValue(image);
                }

                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }
    }
}
