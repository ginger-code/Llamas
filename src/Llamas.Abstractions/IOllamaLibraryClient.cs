using Llamas.Library;

namespace Llamas;

/// <summary>
/// An Ollama library client implementation for retrieving information about models available to pull from https://ollama.com/library
/// </summary>
public interface IOllamaLibraryClient
{
    /// <summary>
    /// Repository for retrieving and caching information about models available to pull
    /// </summary>
    public IOllamaLibraryRepository LibraryRepository { get; }
}