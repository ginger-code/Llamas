namespace Llamas.Library;

/// <summary>
/// A repository implementation for retrieving and caching information about models available to pull
/// </summary>
public interface IOllamaLibraryRepository
{
    /// <summary>
    /// Method for retrieving information about models available to pull from the web
    /// </summary>
    IOllamaLibraryRetriever Retriever { get; init; }

    /// <summary>
    /// Method for persisting and retrieving information about models available to pull
    /// </summary>
    IOllamaLibraryPersistence Persistence { get; init; }
}
