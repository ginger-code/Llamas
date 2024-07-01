using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Llamas.Enums;

namespace Llamas.Primitives;

/// <summary>
/// A span of time to keep a model loaded in the GPU after completing a request
/// </summary>
public readonly record struct KeepAliveTimeSpan
{
    /// <summary>
    /// Total time in seconds to keep a model loaded in the GPU after completing a request
    /// </summary>
    private readonly int _numSeconds;

    /// <summary>
    /// A span of time to keep a model loaded in the GPU after completing a request, in seconds
    /// </summary>
    /// <param name="numSeconds"></param>
    public KeepAliveTimeSpan(int numSeconds)
    {
        _numSeconds = numSeconds;
    }

    /// <summary>
    /// A span of time to keep a model loaded in the GPU after completing a request
    /// </summary>
    /// <param name="units">Amount of time</param>
    /// <param name="timeUnit">Unit of time</param>
    public KeepAliveTimeSpan(int units, UnitOfTime timeUnit)
    {
        _numSeconds = timeUnit.ToSeconds(units);
    }

    /// Returns the string-formatted representation of the time span, in seconds
    public override string ToString() => $"{_numSeconds}s";

    /// <summary>
    /// Custom JSON converter for <see cref="KeepAliveTimeSpan"/>
    /// </summary>
    internal class JsonConverter : JsonConverter<KeepAliveTimeSpan>
    {
        /// <inheritdoc/>
        public override KeepAliveTimeSpan Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            var str = reader.GetString()!.Trim('"');
            var unitOfTime = str[^1] switch
            {
                's' => UnitOfTime.Second,
                'm' => UnitOfTime.Minute,
                'h' => UnitOfTime.Hour,
                _
                    => throw new InvalidOperationException(
                        $"'{str[^1]}' is not a known measure of time"
                    )
            };
            var unitsString = str[..^1];
            if (int.TryParse(unitsString, out var units))
            {
                return new KeepAliveTimeSpan(units, unitOfTime);
            }

            throw new JsonException($"Failed to parse {nameof(KeepAliveTimeSpan)}");
        }

        /// <inheritdoc/>
        public override void Write(
            Utf8JsonWriter writer,
            KeepAliveTimeSpan value,
            JsonSerializerOptions options
        ) => writer.WriteStringValue(value.ToString());
    }
}
