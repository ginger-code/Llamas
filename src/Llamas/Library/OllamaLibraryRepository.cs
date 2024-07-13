using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
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
    /// Enumerate all models available to pull from the persistence store, refreshing the cache with new models
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public async IAsyncEnumerable<ModelListing> EnumerateAndCacheModelListings(
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        await foreach (
            var retrievedModelListing in Retriever
                .EnumerateModels(cancellationToken)
                .ConfigureAwait(false)
        )
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            await Persistence
                .AddOrUpdateModelListings(retrievedModelListing, cancellationToken)
                .ConfigureAwait(false);

            yield return retrievedModelListing;
        }
    }

    /// <summary>
    /// Enumerate all models available to pull from the persistence store from the cached persistence store
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public async IAsyncEnumerable<ModelListing> EnumerateModelListingsCached(
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        foreach (var modelListing in Persistence.ModelListings)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;
            yield return modelListing;
        }
        await Task.CompletedTask.ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieve details about a model available to pull from the persistence store
    /// </summary>
    /// <param name="modelName">Name of the model for which details should be retrieved</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task<ModelListingDetails> GetModelListingDetails(
        string modelName,
        CancellationToken cancellationToken = default
    )
    {
        throw new System.NotImplementedException();
    }
}
