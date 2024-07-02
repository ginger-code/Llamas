using Llamas.LibraryRetrieval;

namespace Llamas;

/// <summary>
/// An Ollama library client implementation for retrieving information about models available to pull
/// </summary>
public interface IOllamaLibraryClient
{
    /// <summary>
    /// Repository for retrieving and caching information about models available to pull
    /// </summary>
    public IOllamaLibraryRepository LibraryRepository { get; }
}
