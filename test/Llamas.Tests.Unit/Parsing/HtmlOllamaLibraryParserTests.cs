using HtmlAgilityPack;
using Parser = Llamas.Library.HtmlOllamaLibraryParser;

namespace Llamas.Tests.Unit.Parsing;

public class HtmlOllamaLibraryParserTests
{
    [Test]
    public async Task ParseListingNode()
    {
        var html = await File.ReadAllTextAsync("../../../listing-entry.html");
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var node = doc.DocumentNode.FirstChild;
        var listing = Parser.ParseListingNode(node, DateTimeOffset.Now);
        Assert.That(listing, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(
                listing.Name,
                Is.Not.Null.And.Not.Empty.And.EqualTo("wangshenzhi/gemma2-9b-chinese-chat")
            );
            Assert.That(
                listing.Description,
                Is.Not.Null.And.Not.Empty.And.EqualTo(
                    "The official ollama model for Gemma-2-9B-Chinese-Chat\r\n            (https://huggingface.co/shenzhi-wang/Gemma-2-9B-Chinese-Chat)."
                )
            );
            Assert.That(listing.ModelTags, Is.Not.Null.And.Not.Empty.And.EqualTo(new[] { "9B" }));
        });
    }

    private static readonly IEnumerable<(string, TimeSpan)> TimeOffsetCases =
    [
        ("1 second", TimeSpan.FromSeconds(1)),
        ("15 seconds", TimeSpan.FromSeconds(15)),
        ("15 minutes", TimeSpan.FromMinutes(15)),
        ("444 hours", TimeSpan.FromHours(444)),
        ("2 days", TimeSpan.FromDays(2)),
        ("3 weeks", TimeSpan.FromDays(21)),
        ("4 months", TimeSpan.FromDays(120)),
        ("2 years", TimeSpan.FromDays(730)),
    ];

    [TestCaseSource(nameof(TimeOffsetCases))]
    public void ParseUpdatedTimeOffset((string str, TimeSpan ts) input)
    {
        var currentTime = DateTimeOffset.Now;
        var expected = currentTime - input.ts;
        var actual = Parser.ParseUpdatedTimeOffset(input.str, currentTime);
        Assert.That(actual, Is.EqualTo(expected));
    }
}
