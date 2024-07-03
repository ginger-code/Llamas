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
        var finalPage = ParseFinalPageNumber(doc);
        foreach (var listing in ParseListingHtml(doc))
        {
            yield return listing;
        }

        while (page <= finalPage)
        {
            page++;
            html = await RetrievePagedListingHtml(page, cancellationToken).ConfigureAwait(false);
            doc.LoadHtml(html);
            foreach (var listing in ParseListingHtml(doc))
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
        return ParseModelListingDetailsHtml(html);
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
        return ParseModelListingDetailsHtml(html);
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

    /// <summary>
    /// Parse the final page number from the listing HTML
    /// </summary>
    internal static int ParseFinalPageNumber(HtmlDocument html)
    {
        var anchor = html.QuerySelector("ul.inline-flex > li:nth-last-child(2) > a:nth-child(1)");
        if (anchor is null)
            throw new NullReferenceException(
                "No matching anchor found to determine final page number"
            );
        if (!int.TryParse(anchor.InnerText, out var pageNumber))
            throw new FormatException(
                $"Unable to parse final page number from anchor text '{anchor.InnerText}'"
            );
        return pageNumber;
    }

    /// <summary>
    /// Parse the listing HTML to retrieve a collection of <see cref="ModelListing" /> instances
    /// </summary>
    internal static IEnumerable<ModelListing> ParseListingHtml(HtmlDocument html)
    {
        var listingNodes = html.QuerySelectorAll("ul.grid > li.flex > a:nth-child(1)");
        foreach (var listingNode in listingNodes)
        {
            yield return ParseListingNode(listingNode);
        }
    }

    internal static ModelListing ParseListingNode(HtmlNode node)
    {
        var name = node.Attributes["href"].Value;
        var description = node.QuerySelector("div.flex > p:nth-child(1)").InnerText;
        var tags = node.QuerySelectorAll("div.flex > div.flex > span")
            .Select(n => n.InnerText)
            .ToArray();
        var updated = ParseUpdatedTimeOffset(
            node.QuerySelector("div.flex > p:nth-child(2) > span:nth-child(2)")
                .InnerText.Replace(" ago", "")
        );
        return new ModelListing(name, description, updated, tags);
    }

    /// <summary>
    /// Parse the model listing details HTML to retrieve a <see cref="ModelListingDetails" /> instance
    /// </summary>
    internal static ModelListingDetails ParseModelListingDetailsHtml(string html)
    {
        throw new System.NotImplementedException();
    }

    internal static DateTimeOffset ParseUpdatedTimeOffset(string updated)
    {
        var num = NumberRegex().Match(updated);
        if (updated.Contains("second"))
        {
            return DateTimeOffset.Now - TimeSpan.FromSeconds(int.Parse(num.Value));
        }

        if (updated.Contains("minute"))
        {
            return DateTimeOffset.Now - TimeSpan.FromMinutes(int.Parse(num.Value));
        }

        if (updated.Contains("hour"))
        {
            return DateTimeOffset.Now - TimeSpan.FromHours(int.Parse(num.Value));
        }

        if (updated.Contains("day"))
        {
            return DateTimeOffset.Now - TimeSpan.FromDays(int.Parse(num.Value));
        }

        if (updated.Contains("week"))
        {
            return DateTimeOffset.Now - TimeSpan.FromDays(int.Parse(num.Value) * 7);
        }

        if (updated.Contains("month"))
        {
            return DateTimeOffset.Now - TimeSpan.FromDays(int.Parse(num.Value) * 30);
        }

        if (updated.Contains("year"))
        {
            return DateTimeOffset.Now - TimeSpan.FromDays(int.Parse(num.Value) * 365);
        }

        throw new FormatException($"Unable to parse updated time offset '{updated}'");
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex NumberRegex();
}
