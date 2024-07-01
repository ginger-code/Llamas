namespace Llamas.Configuration;

/// <summary>
/// Port for ollama server
/// </summary>
public sealed record OllamaPort(ushort Port)
{
    /// Implicitly convert an integer
    public static implicit operator OllamaPort(ushort port) => new(port);

    /// Implicitly convert from integer
    public static implicit operator ushort(OllamaPort port) => port.Port;
}
