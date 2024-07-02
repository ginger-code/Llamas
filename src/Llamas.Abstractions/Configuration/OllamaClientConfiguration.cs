using System;

namespace Llamas.Configuration;

/// <summary>
/// Configuration for the ollama client
/// </summary>
public sealed record OllamaClientConfiguration
{
    /// Address or hostname of ollama server
    public OllamaHostNameOrAddress HostNameOrAddress { get; init; } = "127.0.0.1";

    /// Port of ollama server
    public OllamaPort Port { get; init; } = 11434;

    /// <summary>
    /// Uri of the ollama server
    /// </summary>
    public Uri Uri => new($"http://{HostNameOrAddress}:{Port.Port}/");

    /// <summary>
    /// Create a new instance of OllamaHostConfiguration
    /// </summary>
    public OllamaClientConfiguration(
        OllamaHostNameOrAddress? hostNameOrAddress = null,
        OllamaPort? port = null
    )
    {
        HostNameOrAddress = hostNameOrAddress ?? HostNameOrAddress;
        Port = port ?? Port;
    }
}
