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
        var listing = Parser.ParseListingNode(node);
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
}
