using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Llamas.Enums;

namespace Llamas;

/// <summary>
/// Custom JSON serializer
/// </summary>
internal static class OllamaJsonSerializer
{
    /// <summary>
    /// Json Options configured for custom decoding
    /// </summary>
    private static JsonSerializerOptions JsonSerializerOptions { get; } =
        new()
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters =
            {
                new JsonStringEnumConverter<ResponseFormat>(JsonNamingPolicy.CamelCase, false)
            }
        };

    /// <summary>
    /// Injects all JSON converters into the JSON serializer options via reflection
    /// </summary>
    static OllamaJsonSerializer()
    {
        foreach (var jsonConverter in GetJsonConverters())
        {
            JsonSerializerOptions.Converters.Add(jsonConverter);
        }
    }

    /// <summary>
    /// Retrieves all JSON converters from the Llamas.Abstractions assembly
    /// </summary>
    private static IEnumerable<JsonConverter> GetJsonConverters()
    {
        var thisAssembly = typeof(IOllamaClient).Assembly;
        return thisAssembly
            .GetTypes()
            .SelectMany(t => t.GetNestedTypes(BindingFlags.NonPublic))
            .Where(t => t.Assembly == thisAssembly && t.BaseType?.BaseType == typeof(JsonConverter))
            .Select(t => t.GetConstructor([]))
            .Where(c => c is not null)
            .Select(c => c!.Invoke([]))
            .Cast<JsonConverter>();
    }

    /// <summary>
    /// Serializes an object to JSON using custom encoders
    /// </summary>
    /// <param name="item">Item to encode</param>
    /// <typeparam name="T">Type of item</typeparam>
    /// <returns>JSON string</returns>
    public static string Serialize<T>(T item) =>
        JsonSerializer.Serialize(item, JsonSerializerOptions);

    /// <summary>
    ///Deserializes an object from JSON using custom encoders
    /// </summary>
    /// <param name="json">JSON to decode</param>
    /// <typeparam name="T">Type of item</typeparam>
    /// <returns>Decoded object or null</returns>
    public static T? Deserialize<T>(string json) =>
        JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
}
