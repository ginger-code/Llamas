using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Llamas.Models;

namespace Llamas.Library;

/// <summary>
/// Handles parsing of HTML from https://ollama.com/library
/// </summary>
internal static partial class HtmlOllamaLibraryParser
{
    /// <summary>
    /// Parse the final page number from the listing HTML
    /// </summary>
    public static int ParseFinalPageNumber(HtmlDocument html)
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
    public static IEnumerable<ModelListing> ParseListingHtml(
        HtmlDocument html,
        DateTimeOffset currentTime
    )
    {
        var listingNodes = html.QuerySelectorAll("ul.grid > li.flex > a:nth-child(1)");
        foreach (var listingNode in listingNodes)
        {
            yield return ParseListingNode(listingNode, currentTime);
        }
    }

    /// <summary>
    /// Parse the model listing HTML node to retrieve a <see cref="ModelListing" /> instance
    /// </summary>
    public static ModelListing ParseListingNode(HtmlNode node, DateTimeOffset currentTime)
    {
        var name = node.Attributes["href"].Value.TrimStart('/');
        var description = node.QuerySelector("div.flex > p:nth-child(1)").InnerText;
        var tags = node.QuerySelectorAll("div.flex > div.flex > span")
            ?.Select(n => n.InnerText)
            .ToArray();
        var updated = ParseUpdatedTimeOffset(
            node.QuerySelector("div.flex > p.flex > span:last-child")
                .InnerText.Replace(" ago", "")
                .Replace("Updated&nbsp;", "")
                .Trim(),
            currentTime
        );
        return new ModelListing(
            name,
            description,
            new DateOnly(updated.Year, updated.Month, updated.Day),
            tags
        );
    }

    /// <summary>
    /// Parse the model listing details HTML to retrieve a <see cref="ModelListingDetails" /> instance
    /// </summary>
    public static ModelListingDetails ParseModelListingDetailsHtml(
        string html,
        DateTimeOffset currentTime
    )
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var name = doc.QuerySelector("h1.flex > a.font-medium").InnerText;
        var description = doc.QuerySelector("h2.break-words").InnerText;
        var version = doc.QuerySelector("div.px-4 > p:last-child").InnerText;
        var updated = ParseUpdatedTimeOffset(
            doc.QuerySelector(@"p.sm\:hidden")
                .InnerText.Replace(" ago", "")
                .Replace("Updated&nbsp;", "")
                .Trim(),
            currentTime
        );
        var fileSizes = doc.QuerySelectorAll("#primary-tags > a")
            .Select(a =>
            {
                var tag = a.QuerySelector("div > span").InnerText;
                var size = a.QuerySelector("span").InnerText;
                return (tag, size);
            })
            .ToDictionary();
        var tags = fileSizes.Keys.ToArray();
        var readme = doc.QuerySelector("#display").InnerHtml;
        return new ModelListingDetails(
            name,
            description,
            version,
            updated,
            tags,
            fileSizes,
            readme
        );
    }

    /// <summary>
    /// Parse humanized datetime offset using the current time
    /// </summary>
    /// <param name="updated"></param>
    /// <param name="currentTime"></param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    public static DateTimeOffset ParseUpdatedTimeOffset(string updated, DateTimeOffset currentTime)
    {
        var num = NumberRegex().Match(updated);
        if (updated.Contains("second"))
        {
            return currentTime - TimeSpan.FromSeconds(int.Parse(num.Value));
        }

        if (updated.Contains("minute"))
        {
            return currentTime - TimeSpan.FromMinutes(int.Parse(num.Value));
        }

        if (updated.Contains("hour"))
        {
            return currentTime - TimeSpan.FromHours(int.Parse(num.Value));
        }

        if (updated.Contains("day"))
        {
            return currentTime - TimeSpan.FromDays(int.Parse(num.Value));
        }

        if (updated.Contains("week"))
        {
            return currentTime - TimeSpan.FromDays(int.Parse(num.Value) * 7);
        }

        if (updated.Contains("month"))
        {
            return currentTime - TimeSpan.FromDays(int.Parse(num.Value) * 30);
        }

        if (updated.Contains("year"))
        {
            return currentTime - TimeSpan.FromDays(int.Parse(num.Value) * 365);
        }

        throw new FormatException($"Unable to parse updated time offset '{updated}'");
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex NumberRegex();
}
