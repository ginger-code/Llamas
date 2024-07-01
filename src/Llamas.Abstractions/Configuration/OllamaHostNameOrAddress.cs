namespace Llamas.Configuration;

/// <summary>
/// Address or hostname of ollama server
/// </summary>
public sealed record OllamaHostNameOrAddress(string HostNameOrAddress)
{
    /// Implicitly convert a string
    public static implicit operator OllamaHostNameOrAddress(string hostNameOrAddress) =>
        new(hostNameOrAddress);

    /// <inheritdoc />
    public override string ToString()
    {
        return HostNameOrAddress;
    }
}
