using Llamas.Enums;

namespace Llamas.Test.Unit.Serialization;

[TestFixture, Parallelizable]
public class PrimitiveSerializationTests
{
    [Test]
    public void ResponseFormat()
    {
        const ResponseFormat model = Enums.ResponseFormat.Json;
        var serialized = OllamaJsonSerializer.Serialize(model);
        var deserialized = OllamaJsonSerializer.Deserialize<ResponseFormat>(serialized);
        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.EqualTo(model));
            Assert.That(serialized, Is.EqualTo("\"json\""));
        });
    }
}
