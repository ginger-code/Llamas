using System.Collections.Generic;
using System.Threading;
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
    /// <param name="cancellationToken">Cancellation token</param>
    IAsyncEnumerable<ModelListing> EnumerateModelListings(
        bool refreshCache = false,
        CancellationToken cancellationToken = default
    )
    {
        return refreshCache
            ? EnumerateAndCacheModelListings(cancellationToken)
            : EnumerateModelListingsCached(cancellationToken);
    }

    /// <summary>
    /// Enumerate all models available to pull from the persistence store, refreshing the cache with new models
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>

    IAsyncEnumerable<ModelListing> EnumerateAndCacheModelListings(
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Enumerate all models available to pull from the persistence store from the cached persistence store
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>

    IAsyncEnumerable<ModelListing> EnumerateModelListingsCached(
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieve details about a model available to pull from the persistence store
    /// </summary>
    /// <param name="modelName">Name of the model for which details should be retrieved</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<ModelListingDetails> GetModelListingDetails(
        string modelName,
        CancellationToken cancellationToken = default
    );
}
