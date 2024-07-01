using Llamas.Responses;

namespace Llamas.Test.Unit.Serialization;

[TestFixture, Parallelizable]
public class GenerateCompletionSerializationTests
{
    [Test]
    public void GenerateCompletionProgressResponse()
    {
        const string json = """
            {
              "model": "llama3",
              "created_at": "2023-08-04T19:22:45.499127Z",
              "response": "The",
              "done": false
            }
            """;
        var deserialized = OllamaJsonSerializer.Deserialize<GenerateCompletionResponse>(json);
        Assert.That(deserialized, Is.Not.Null);
        if (deserialized is GenerateCompletionResponse.GenerateProgressResponse result)
        {
            Assert.Multiple(() =>
            {
                Assert.That(result.Model, Is.EqualTo("llama3"));
                Assert.That(result.Response, Is.EqualTo("The"));
                Assert.That(result.Done, Is.False);
            });
        }
        else
        {
            Assert.Fail(
                "Deserialized object was not of type 'GenerateCompletionProgressResponse'."
            );
        }
    }

    [Test]
    public void GenerateCompletionFinishedResponse()
    {
        const string json = """
            {
              "model": "llama3",
              "created_at": "2023-08-04T19:22:45.499127Z",
              "response": "",
              "done": true,
              "context": [1, 2, 3],
              "total_duration": 10706818083,
              "load_duration": 6338219291,
              "prompt_eval_count": 26,
              "prompt_eval_duration": 130079000,
              "eval_count": 259,
              "eval_duration": 4232710000
            }
            """;
        var deserialized = OllamaJsonSerializer.Deserialize<GenerateCompletionResponse>(json);
        Assert.That(deserialized, Is.Not.Null);
        if (deserialized is GenerateCompletionResponse.GenerateCompleteResponse result)
        {
            Assert.Multiple(() =>
            {
                Assert.That(result.Model, Is.EqualTo("llama3"));
                Assert.That(result.Response, Is.EqualTo(""));
                Assert.That(result.Done, Is.True);
                Assert.That(result.Context.SequenceEqual([1, 2, 3]));
            });
        }
        else
        {
            Assert.Fail(
                "Deserialized object was not of type 'GenerateCompletionFinishedResponse'."
            );
        }
    }
}
