using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llamas.Models;

namespace Llamas.Library;

/// <summary>
/// An implementation of a method for retrieving information about models available to pull from https://ollama.com/library
/// </summary>
public interface IOllamaLibraryRetriever
{
    /// <summary>
    /// Asynchronously enumerate all models available to pull
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    IAsyncEnumerable<ModelListing> EnumerateModels(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve details about a model available to pull
    /// </summary>
    /// <param name="listing">Listing for which to retrieve details about specifications and download options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<ModelListingDetails> GetModelListingDetails(
        ModelListing listing,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieve details about a model available to pull
    /// </summary>
    /// <param name="modelName">Name of model for which to retrieve details about specifications and download options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<ModelListingDetails> GetModelListingDetails(
        string modelName,
        CancellationToken cancellationToken = default
    );
}
