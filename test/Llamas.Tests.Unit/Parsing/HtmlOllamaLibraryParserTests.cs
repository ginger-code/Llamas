using HtmlAgilityPack;
using Parser = Llamas.Library.HtmlOllamaLibraryParser;

namespace Llamas.Tests.Unit.Parsing;

sealed class HtmlOllamaLibraryParserTests
{
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
            Assert.That(
                listing.ModelTags,
                Is.Not.Null.And.Not.Empty.And.EqualTo(new[] { "9B", "latest" })
            );
        });
    }

    [Test]
    public async Task ParseModelListings()
    {
        var html = await File.ReadAllTextAsync("../../../listing-page.html");
        var listings = Parser.ParseListingHtml(html, DateTimeOffset.Now).ToList();
        Assert.That(listings, Is.Not.Null.And.Not.Empty);
        foreach (var listing in listings)
        {
            Assert.Multiple(() =>
            {
                Assert.That(listing.Name, Is.Not.Null.And.Not.Empty);
                Assert.That(listing.Description, Is.Not.Null.Or.Not.Empty);
                Assert.That(listing.ModelTags, Is.Not.Null.And.Not.Empty);
            });
        }
    }

    [Test]
    public async Task ParseFinalPageNumber()
    {
        var html = await File.ReadAllTextAsync("../../../listing-page.html");
        var finalPage = Parser.ParseFinalPageNumber(html);
        Assert.That(finalPage, Is.EqualTo(34));
    }

    [Test]
    public async Task ParseModelDetails()
    {
        var html = await File.ReadAllTextAsync("../../../listing-detail.html");
        var model = Parser.ParseModelListingDetailsHtml(html, DateTimeOffset.Now);
        Assert.That(model, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(model.Name, Is.Not.Null.And.Not.Empty.And.EqualTo("gemma2"));
            Assert.That(
                model.Description,
                Is.Not.Null.And.Not.Empty.And.EqualTo(
                    "Google Gemma 2 is now available in 2 sizes, 9B and 27B."
                )
            );
            Assert.That(
                model.ModelTags,
                Is.Not.Null.And.Not.Empty.And.Contains("9b")
                    .And.Contains("27b")
                    .And.Contains("27b-instruct-q5_1")
                    .And.Contains("latest")
            );
            Assert.That(
                model.FileSizes,
                Is.Not.Null.And.Not.Empty.And.ContainKey("27b-instruct-q3_K_M")
            );
            Assert.That(model.FileSizes["27b-instruct-q3_K_M"], Is.Not.Null.And.EqualTo("13GB"));
            Assert.That(model.Version, Is.Not.Null.And.Not.Empty.And.EqualTo("c19987e1e6e2"));
            var expectedUpdateDate = (DateTimeOffset.Now - TimeSpan.FromDays(9)).Date;
            Assert.That(
                model.Updated,
                Is.EqualTo(
                    new DateOnly(
                        expectedUpdateDate.Year,
                        expectedUpdateDate.Month,
                        expectedUpdateDate.Day
                    )
                )
            );
            Assert.That(
                model.ReadmeMarkup,
                Is.Not.Null.And.StartsWith(
                        @"<p><img src=""https://ollama.com/assets/library/gemma2/5"
                    )
                    .And.EndsWith(
                        """
                        llm.complete(&#34;Why is the sky blue?&#34;)
                        </code></pre>
                        """
                    )
            );
        });
    }
}
