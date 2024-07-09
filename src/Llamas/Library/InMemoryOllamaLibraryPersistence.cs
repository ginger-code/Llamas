using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Llamas.Models;

namespace Llamas.Library;

/// <summary>
/// Synchronous, in-memory implementation of IOllamaLibraryPersistence
/// </summary>
public sealed class InMemoryOllamaLibraryPersistence : IOllamaLibraryPersistence
{
    private readonly List<ModelListing> _modelListings = [];
    private readonly List<ModelListingDetails> _modelListingDetails = [];

    /// <summary>
    /// Queryable root for model listings
    /// </summary>
    public IQueryable<ModelListing> ModelListings => _modelListings.AsQueryable();

    /// <summary>
    /// Queryable root for model listing details
    /// </summary>
    public IQueryable<ModelListingDetails> ModelListingDetails =>
        _modelListingDetails.AsQueryable();

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
        foreach (var modelName in modelNames)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            var modelListing = _modelListings.FirstOrDefault(l => l.Name == modelName);

            if (modelListing is null)
                continue;

            _modelListings.Remove(modelListing);
            yield return modelListing;
        }
        await Task.CompletedTask.ConfigureAwait(false);
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
        foreach (var modelName in modelNames)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            var modelListing = _modelListingDetails.FirstOrDefault(l => l.Name == modelName);

            if (modelListing is null)
                continue;

            _modelListingDetails.Remove(modelListing);
            yield return modelListing;
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }

    /// <summary>
    /// Adds or updates a model listing in the persistence store
    /// </summary>
    /// <param name="listings">The listings collection to add or update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added/updated listing or null if unsuccessful</returns>
    public async Task AddOrUpdateModelListings(
        IEnumerable<ModelListing> listings,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var modelListing in listings.Where(l => _modelListings.All(m => m.Name != l.Name)))
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            _modelListings.Add(modelListing);
        }
        await Task.CompletedTask.ConfigureAwait(false);
    }

    /// <summary>
    /// Adds or updates model listing details in the persistence store
    /// </summary>
    /// <param name="details">The listing details collection to add or update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added/updated details or null if unsuccessful</returns>
    public async Task AddOrUpdateModelListingDetails(
        IEnumerable<ModelListingDetails> details,
        CancellationToken cancellationToken = default
    )
    {
        foreach (
            var modelListingDetails in details.Where(l =>
                _modelListingDetails.All(m => m.Name != l.Name)
            )
        )
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            _modelListingDetails.Add(modelListingDetails);
        }
        await Task.CompletedTask.ConfigureAwait(false);
    }
}
