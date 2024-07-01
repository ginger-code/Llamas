using System.Diagnostics.CodeAnalysis;

namespace Llamas.Configuration;

/// <summary>
/// Configuration for hosted ollama containers
/// </summary>
public class OllamaContainerConfiguration
{
    /// Create a new instance of OllamaContainerConfiguration
    [SetsRequiredMembers]
    public OllamaContainerConfiguration(OllamaPort? mappedPort = null)
    {
        MappedPort = mappedPort ?? new OllamaPort(11434);
    }

    /// Create a new instance of OllamaContainerConfiguration
    [SetsRequiredMembers]
    public OllamaContainerConfiguration(OllamaClientConfiguration clientConfiguration)
        : this(clientConfiguration.Port) { }

    /// <summary>
    /// Port to bind on host machine to access port 11434 on ollama container for API access
    /// </summary>
    public required OllamaPort MappedPort { get; init; }
}
