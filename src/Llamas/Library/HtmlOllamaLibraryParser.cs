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
/// This class is prone to breaking changes to the HTML structure.
/// Working as of 2024-07-09
/// </summary>
internal static partial class HtmlOllamaLibraryParser
{
    /// <summary>
    /// Parse the final page number from the listing HTML
    /// </summary>
    public static int ParseFinalPageNumber(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var anchors = doc.QuerySelectorAll("ul.inline-flex > li");
        var anchor = anchors[^2].QuerySelector("a:nth-child(1)");
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
        string html,
        DateTimeOffset currentTime
    )
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var listingNodes = doc.QuerySelectorAll("ul.grid > li.flex > a:nth-child(1)");
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
        var description = node.QuerySelector("div.flex > p:nth-child(1)")?.InnerText ?? "";
        var tags = node.QuerySelectorAll("div.flex > div.flex > span")
            ?.Select(n => n.InnerText)
            .Prepend("latest")
            .Distinct()
            .Order()
            .ToArray();
        var updated = ParseUpdatedTimeOffset(
            node.QuerySelector("div.flex > p.flex > span:last-child")
                ?.InnerText.Replace(" ago", "")
                .Replace("Updated&nbsp;", "")
                .Trim() ?? "0 seconds",
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
        var name = doc.QuerySelector("h1.flex > a.font-medium").InnerText.Trim();
        var description = doc.QuerySelector("h2.break-words")?.InnerText.Trim() ?? "";
        var versionStr = doc.QuerySelector("div.px-4 > p:last-child")?.InnerText.Trim() ?? "";
        var version = VersionRegex().Match(versionStr).Value;
        var updated = ParseUpdatedTimeOffset(
            doc.QuerySelector("p.hidden")
                .InnerText.Replace(" ago", "")
                .Replace("Updated&nbsp;", "")
                .Trim(),
            currentTime
        );
        var fileSizes = doc.QuerySelectorAll("#primary-tags > a")
            .Union(doc.QuerySelectorAll("#secondary-tags > a"))
            .Select(a =>
            {
                var tag = a.QuerySelector("div > span").InnerText.Trim();
                var size = a.QuerySelector("span.text-xs").InnerText.Trim();
                return (tag, size);
            })
            .ToDictionary();
        var tags = fileSizes.Keys.ToArray();
        var readme = doc.QuerySelector("#display")?.InnerHtml.Trim() ?? "";
        return new ModelListingDetails(
            name,
            description,
            version,
            new DateOnly(updated.Year, updated.Month, updated.Day),
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

    [GeneratedRegex(@"(\S+)")]
    private static partial Regex VersionRegex();
}
