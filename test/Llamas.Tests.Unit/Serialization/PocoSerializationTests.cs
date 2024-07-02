using Llamas.Models;

namespace Llamas.Tests.Unit.Serialization;

[TestFixture, Parallelizable]
public class PocoSerializationTests
{
    [Test]
    public void ModelInfoDetails()
    {
        const string json = """
            {
               "format": "gguf",
               "family": "llama",
               "families": ["llama", "clip"],
               "parameter_size": "7B",
               "quantization_level": "Q4_0"
            }
            """;
        var expected = new ModelInfoDetails
        {
            Format = "gguf",
            Family = "llama",
            Families = ["llama", "clip"],
            ParameterSize = "7B",
            QuantizationLevel = "Q4_0"
        };

        var actual = OllamaJsonSerializer.Deserialize<ModelInfoDetails>(json);
        Assert.That(actual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.Format, Is.EqualTo(expected.Format));
            Assert.That(actual.Family, Is.EqualTo(expected.Family));
            Assert.That(actual.ParameterSize, Is.EqualTo(expected.ParameterSize));
            Assert.That(actual.QuantizationLevel, Is.EqualTo(expected.QuantizationLevel));
            Assert.That(actual.Families, Is.EqualTo(expected.Families));
        });
    }

    [Test]
    public void ModelInfo()
    {
        const string json = """
            {
             "modelfile": "model file",
             "parameters": "num_ctx                        4096",
             "template": "{{ .System }} USER: {{ .Prompt }} ASSISTANT: ",
             "details": {
               "format": "gguf",
               "family": "llama",
               "families": ["llama", "clip"],
               "parameter_size": "7B",
               "quantization_level": "Q4_0"
             },
             "model_info": {
                "general.architecture": "llama"
             }
            }
            """;
        var expected = new ModelInfo
        {
            File = "model file",
            Parameters = "num_ctx                        4096",
            Template = "{{ .System }} USER: {{ .Prompt }} ASSISTANT: ",
            Details = new ModelInfoDetails
            {
                Format = "gguf",
                Family = "llama",
                Families = ["llama", "clip"],
                ParameterSize = "7B",
                QuantizationLevel = "Q4_0"
            },
            Info = new Dictionary<string, object> { { "general.architecture", "llama" } }
        };

        var actual = OllamaJsonSerializer.Deserialize<ModelInfo>(json);
        Assert.That(actual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual.File, Is.EqualTo(expected.File));
            Assert.That(actual.Parameters, Is.EqualTo(expected.Parameters));
            Assert.That(actual.Template, Is.EqualTo(expected.Template));
            Assert.That(actual.Details.Format, Is.EqualTo(expected.Details.Format));
            Assert.That(actual.Details.Family, Is.EqualTo(expected.Details.Family));
            Assert.That(actual.Details.ParameterSize, Is.EqualTo(expected.Details.ParameterSize));
            Assert.That(
                actual.Details.QuantizationLevel,
                Is.EqualTo(expected.Details.QuantizationLevel)
            );
            Assert.That(actual.Details.Families, Is.EqualTo(expected.Details.Families));
        });
    }
}
