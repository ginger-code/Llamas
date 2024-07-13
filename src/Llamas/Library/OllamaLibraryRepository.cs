using System.Collections.Generic;
using System.Threading.Tasks;
using Llamas.Models;

namespace Llamas.Library;

/// <summary>
/// Implementation of the IOllamaLibraryRepository interface using the provided retriever and persistence methods
/// </summary>
/// <param name="retriever">Implementation of <see cref="IOllamaLibraryRetriever"/></param>
/// <param name="persistence">Implementation of <see cref="IOllamaLibraryPersistence"/></param>
public class OllamaLibraryRepository(
    IOllamaLibraryRetriever retriever,
    IOllamaLibraryPersistence persistence
) : IOllamaLibraryRepository
{
    /// <summary>
    /// Method for retrieving information about models available to pull from the web
    /// </summary>
    public IOllamaLibraryRetriever Retriever { get; init; } = retriever;

    /// <summary>
    /// Method for persisting and retrieving information about models available to pull
    /// </summary>
    public IOllamaLibraryPersistence Persistence { get; init; } = persistence;

    /// <summary>
    /// Enumerate all models available to pull from the persistence store, refreshing the cache with new or de-listed models
    /// </summary>
    public IAsyncEnumerable<ModelListing> EnumerateAndCacheModelListings()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Enumerate all models available to pull from the persistence store from the cached persistence store
    /// </summary>
    public IAsyncEnumerable<ModelListing> EnumerateModelListingsCached()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Retrieve details about a model available to pull from the persistence store
    /// </summary>
    /// <param name="modelName">Name of the model for which details should be retrieved</param>
    public Task<ModelListingDetails> GetModelListingDetails(string modelName)
    {
        throw new System.NotImplementedException();
    }
}
