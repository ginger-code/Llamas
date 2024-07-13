using System.Collections.Generic;
using System.Linq;
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
    /// Update the cache with new models, optionally removing models that are no longer listed on the web (default is true)
    /// </summary>
    /// <param name="removeUnlistedModels">If true, remove all models from persistence which no longer have a public listing</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task UpdateCache(
        bool removeUnlistedModels = true,
        CancellationToken cancellationToken = default
    )
    {
        HashSet<string> visitedModels = [];
        await foreach (
            var retrievedListing in Retriever
                .EnumerateModels(cancellationToken)
                .ConfigureAwait(false)
        )
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if (
                removeUnlistedModels && visitedModels.Add(retrievedListing.Name)
                || !removeUnlistedModels
            )
            {
                await Persistence
                    .AddOrUpdateModelListings(retrievedListing, cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        if (removeUnlistedModels)
        {
            var unvisitedModels = Persistence
                .ModelListings.Select(listing => listing.Name)
                .Where(name => !visitedModels.Contains(name))
                .AsEnumerable();
            await foreach (
                var _ in Persistence
                    .DeleteModelListings(unvisitedModels, cancellationToken)
                    .ConfigureAwait(false)
            ) { }
        }
    }

    /// <summary>
    /// Enumerate all models available to pull from the persistence store, refreshing the cache with new models
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public async IAsyncEnumerable<ModelListing> EnumerateAndCacheModelListings(
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        await foreach (
            var retrievedListing in Retriever
                .EnumerateModels(cancellationToken)
                .ConfigureAwait(false)
        )
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            await Persistence
                .AddOrUpdateModelListings(retrievedListing, cancellationToken)
                .ConfigureAwait(false);

            yield return retrievedListing;
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
    public async Task<ModelListingDetails> GetModelListingDetails(
        string modelName,
        CancellationToken cancellationToken = default
    )
    {
        var persisted = Persistence.ModelListingDetails.FirstOrDefault(listing =>
            listing.Name == modelName
        );
        if (persisted is not null)
            return persisted;

        var retrieved = await Retriever
            .GetModelListingDetails(modelName, cancellationToken)
            .ConfigureAwait(false);
        await Persistence
            .AddOrUpdateModelListingDetails(retrieved, cancellationToken)
            .ConfigureAwait(false);
        return retrieved;
    }
}
