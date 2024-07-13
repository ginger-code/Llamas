using System.Collections.Generic;
using System.Threading.Tasks;
using Llamas.Models;

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

    /// <summary>
    /// Enumerate all models available to pull from the persistence store, optionally refreshing the cache if necessary
    /// </summary>
    /// <param name="refreshCache">If true, update cached model listings from the internet</param>
    IAsyncEnumerable<ModelListing> EnumerateModelListings(bool refreshCache = false)
    {
        return refreshCache ? EnumerateAndCacheModelListings() : EnumerateModelListingsCached();
    }

    /// <summary>
    /// Enumerate all models available to pull from the persistence store, refreshing the cache with new or de-listed models
    /// </summary>
    IAsyncEnumerable<ModelListing> EnumerateAndCacheModelListings();

    /// <summary>
    /// Enumerate all models available to pull from the persistence store from the cached persistence store
    /// </summary>
    IAsyncEnumerable<ModelListing> EnumerateModelListingsCached();

    /// <summary>
    /// Retrieve details about a model available to pull from the persistence store
    /// </summary>
    /// <param name="modelName">Name of the model for which details should be retrieved</param>
    Task<ModelListingDetails> GetModelListingDetails(string modelName);
}
