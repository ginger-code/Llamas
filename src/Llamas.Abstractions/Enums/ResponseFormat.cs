using System.Text.Json.Serialization;

namespace Llamas.Enums;

/// <summary>
/// Data format of a response
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ResponseFormat
{
    /// Format result as JSON
    Json
}
