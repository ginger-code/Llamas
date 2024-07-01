using System.Text.Json.Serialization;

namespace Llamas.Enums;

/// <summary>
/// Roles representing actors in a conversation
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<Roles>))]
public enum Roles
{
    /// System message
    System = 0,

    /// User message
    User = 1,

    /// Model message
    Assistant = 2
}
