using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Llamas.Models;

namespace Llamas.Library;

/// <summary>
/// A null persistence implementation which does not perform any operations
/// </summary>
public class NullOllamaLibraryPersistence : IOllamaLibraryPersistence
{
    /// <summary>
    /// Queryable root for model listings
    /// </summary>
    public IQueryable<ModelListing> ModelListings =>
        System.Array.Empty<ModelListing>().AsQueryable();

    /// <summary>
    /// Queryable root for model listing details
    /// </summary>
    public IQueryable<ModelListingDetails> ModelListingDetails =>
        System.Array.Empty<ModelListingDetails>().AsQueryable();

    /// <summary>
    /// Deletes a model listing from the persistence store
    /// </summary>
    /// <param name="modelNames">The model names to delete listings for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Any deleted ModelListings</returns>
    public async IAsyncEnumerable<ModelListing> DeleteModelListings(
        IEnumerable<string> modelNames,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        await Task.CompletedTask.ConfigureAwait(false);
        yield break;
    }

    /// <summary>
    /// Deletes model listing details from the persistence store
    /// </summary>
    /// <param name="modelNames">The model names to delete listing details for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Any deleted ModelListingDetails</returns>
    public async IAsyncEnumerable<ModelListingDetails?> DeleteModelListingDetails(
        IEnumerable<string> modelNames,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        await Task.CompletedTask.ConfigureAwait(false);
        yield break;
    }

    /// <summary>
    /// Adds or updates a model listing in the persistence store
    /// </summary>
    /// <param name="listings">The listings collection to add or update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added/updated listing or null if unsuccessful</returns>
    public Task AddOrUpdateModelListings(
        IEnumerable<ModelListing> listings,
        CancellationToken cancellationToken = default
    )
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Adds or updates model listing details in the persistence store
    /// </summary>
    /// <param name="details">The listing details collection to add or update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added/updated details or null if unsuccessful</returns>
    public Task AddOrUpdateModelListingDetails(
        IEnumerable<ModelListingDetails> details,
        CancellationToken cancellationToken = default
    )
    {
        return Task.CompletedTask;
    }
}
