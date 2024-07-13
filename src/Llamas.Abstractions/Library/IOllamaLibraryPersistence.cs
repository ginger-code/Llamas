using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Llamas.Models;

namespace Llamas.Library;

/// <summary>
/// An in-memory implementation of a method for caching information about models available to pull
/// </summary>
public interface IOllamaLibraryPersistence
{
    /// <summary>
    /// Queryable root for model listings
    /// </summary>
    public IQueryable<ModelListing> ModelListings { get; }

    /// <summary>
    /// Queryable root for model listing details
    /// </summary>
    public IQueryable<ModelListingDetails> ModelListingDetails { get; }

    /// <summary>
    /// Deletes a model listing from the persistence store
    /// </summary>
    /// <param name="modelName">The model name to delete the listing for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The deleted ModelListing, null if not deleted</returns>
    async Task<ModelListing?> DeleteModelListing(
        string modelName,
        CancellationToken cancellationToken = default
    )
    {
        await using var asyncEnumerator = DeleteModelListings([modelName], cancellationToken)
            .ConfigureAwait(false)
            .GetAsyncEnumerator();
        if (await asyncEnumerator.MoveNextAsync())
            return asyncEnumerator.Current;
        return null;
    }

    /// <summary>
    /// Deletes a model listing from the persistence store
    /// </summary>
    /// <param name="modelNames">The model names to delete listings for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Any deleted ModelListings</returns>
    IAsyncEnumerable<ModelListing> DeleteModelListings(
        IEnumerable<string> modelNames,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Deletes model listing details from the persistence store
    /// </summary>
    /// <param name="modelName">The model name to delete the listing details for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The deleted ModelListingDetails, null if not deleted</returns>
    async Task<ModelListingDetails?> DeleteModelListingDetails(
        string modelName,
        CancellationToken cancellationToken = default
    )
    {
        await using var asyncEnumerator = DeleteModelListingDetails([modelName], cancellationToken)
            .ConfigureAwait(false)
            .GetAsyncEnumerator();
        if (await asyncEnumerator.MoveNextAsync())
            return asyncEnumerator.Current;
        return null;
    }

    /// <summary>
    /// Deletes model listing details from the persistence store
    /// </summary>
    /// <param name="modelNames">The model names to delete listing details for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Any deleted ModelListingDetails</returns>
    IAsyncEnumerable<ModelListingDetails?> DeleteModelListingDetails(
        IEnumerable<string> modelNames,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Adds or updates a model listing in the persistence store
    /// </summary>
    /// <param name="listing">The listing to add or update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added/updated listing or null if unsuccessful</returns>
    Task AddOrUpdateModelListings(
        ModelListing listing,
        CancellationToken cancellationToken = default
    ) => AddOrUpdateModelListings([listing], cancellationToken);

    /// <summary>
    /// Adds or updates a model listing in the persistence store
    /// </summary>
    /// <param name="listings">The listings collection to add or update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added/updated listing or null if unsuccessful</returns>
    Task AddOrUpdateModelListings(
        IEnumerable<ModelListing> listings,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Adds or updates model listing details in the persistence store
    /// </summary>
    /// <param name="details">The listing details to add or update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added/updated details or null if unsuccessful</returns>
    Task AddOrUpdateModelListingDetails(
        ModelListingDetails details,
        CancellationToken cancellationToken = default
    ) => AddOrUpdateModelListingDetails([details], cancellationToken);

    /// <summary>
    /// Adds or updates model listing details in the persistence store
    /// </summary>
    /// <param name="details">The listing details collection to add or update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added/updated details or null if unsuccessful</returns>
    Task AddOrUpdateModelListingDetails(
        IEnumerable<ModelListingDetails> details,
        CancellationToken cancellationToken = default
    );
}
