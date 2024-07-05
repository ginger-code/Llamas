using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Llamas.Models;
using Parser = Llamas.Library.HtmlOllamaLibraryParser;

namespace Llamas.Library;

/// <summary>
/// Ollama library retriever implementation that parses HTML from https://ollama.com/library
/// </summary>
public partial class HtmlOllamaLibraryRetriever : IOllamaLibraryRetriever
{
    private HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Llamas.Library.HtmlOllamaLibraryRetriever" /> class.
    /// </summary>
    public HtmlOllamaLibraryRetriever(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Asynchronously enumerate all models available to pull
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public async IAsyncEnumerable<ModelListing> EnumerateModels(
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        var page = 1;
        var html = await RetrievePagedListingHtml(page, cancellationToken).ConfigureAwait(false);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var finalPage = Parser.ParseFinalPageNumber(doc);
        foreach (var listing in Parser.ParseListingHtml(doc))
        {
            yield return listing;
        }

        while (page <= finalPage)
        {
            page++;
            html = await RetrievePagedListingHtml(page, cancellationToken).ConfigureAwait(false);
            doc.LoadHtml(html);
            foreach (var listing in Parser.ParseListingHtml(doc))
            {
                yield return listing;
            }
        }
    }

    /// <summary>
    /// Retrieve details about a model available to pull
    /// </summary>
    /// <param name="listing">Listing for which to retrieve details about specifications and download options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task<ModelListingDetails> GetModelListingDetails(
        ModelListing listing,
        CancellationToken cancellationToken = default
    )
    {
        var html = await RetrieveModelListingDetailsHtml(listing.Name, cancellationToken)
            .ConfigureAwait(false);
        return Parser.ParseModelListingDetailsHtml(html);
    }

    /// <summary>
    /// Retrieve details about a model available to pull
    /// </summary>
    /// <param name="modelName">Name of model for which to retrieve details about specifications and download options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task<ModelListingDetails> GetModelListingDetails(
        string modelName,
        CancellationToken cancellationToken = default
    )
    {
        var html = await RetrieveModelListingDetailsHtml(modelName, cancellationToken)
            .ConfigureAwait(false);
        return Parser.ParseModelListingDetailsHtml(html);
    }

    /// <summary>
    /// Retrieve the paged listing HTML for a given page number
    /// </summary>
    /// <returns></returns>
    private async Task<string> RetrievePagedListingHtml(
        int page = 1,
        CancellationToken cancellationToken = default
    )
    {
        return await _httpClient
            .GetStringAsync($"https://ollama.com/search?q=&p={page}&sort=newest", cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieve the model listing details HTML for a given model name
    /// </summary>
    private async Task<string> RetrieveModelListingDetailsHtml(
        string modelName,
        CancellationToken cancellationToken = default
    )
    {
        var prefix = modelName.Contains('/') ? "" : "library/";
        return await _httpClient
            .GetStringAsync($"https://ollama.com/{prefix}{modelName}", cancellationToken)
            .ConfigureAwait(false);
    }
}
