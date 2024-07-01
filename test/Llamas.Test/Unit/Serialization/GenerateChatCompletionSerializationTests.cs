using System.Diagnostics;
using Llamas.Enums;
using Llamas.Models;
using Llamas.Responses;

namespace Llamas.Test.Unit.Serialization;

[TestFixture, Parallelizable]
public class GenerateChatCompletionSerializationTests
{
    [Test]
    public void ChatMessage()
    {
        const string json = """
            {
              "role": "assistant",
              "content": "The",
              "images": ["a"]
            }
            """;
        var deserialized = OllamaJsonSerializer.Deserialize<ChatMessage>(json);
        Assert.Multiple(() =>
        {
            Assert.That(deserialized, Is.Not.Null);
            Debug.Assert(deserialized != null, nameof(deserialized) + " != null");
            Assert.That(deserialized.Role, Is.EqualTo(Roles.Assistant));
            Assert.That(deserialized.Content, Is.EqualTo("The"));
            Assert.That(deserialized.Images?.SequenceEqual(["a"]), Is.True);
        });
    }

    [Test]
    public void GenerateChatCompletionProgressResponse()
    {
        const string json = """
            {
              "model": "llama3",
              "created_at": "2023-08-04T08:52:19.385406455-07:00",
              "message": {
                "role": "assistant",
                "content": "The",
                "images": ["a", "b"]
              },
              "done": false
            }
            """;
        var deserialized = OllamaJsonSerializer.Deserialize<GenerateChatCompletionResponse>(json);
        Assert.That(deserialized, Is.Not.Null);
        if (deserialized is GenerateChatCompletionResponse.ChatProgressResponse result)
        {
            Assert.Multiple(() =>
            {
                Assert.That(result.Model, Is.EqualTo("llama3"));
                Assert.That(result.Message.Role, Is.EqualTo(Roles.Assistant));
                Assert.That(result.Message.Content, Is.EqualTo("The"));
                Assert.That(result.Message.Images, Is.Not.Null);
                Assert.That(result.Message.Images!.SequenceEqual(["a", "b"]));
                Assert.That(result.Done, Is.False);
            });
        }
        else
        {
            Assert.Fail(
                "Deserialized object was not of type 'GenerateChatCompletionProgressResponse'."
            );
        }
    }

    [Test]
    public void GenerateChatCompletionFinishedResponse()
    {
        const string json = """
            {
              "model": "llama3",
              "created_at": "2023-08-04T19:22:45.499127Z",
              "done": true,
              "total_duration": 4883583458,
              "load_duration": 1334875,
              "prompt_eval_count": 26,
              "prompt_eval_duration": 342546000,
              "eval_count": 282,
              "eval_duration": 4535599000,
              "message": {
                  "role": "assistant",
                  "content": "",
                  "images": null
                }
            }
            """;
        var deserialized = OllamaJsonSerializer.Deserialize<GenerateChatCompletionResponse>(json);
        Assert.That(deserialized, Is.Not.Null);
        if (deserialized is GenerateChatCompletionResponse.ChatCompleteResponse result)
        {
            Assert.Multiple(() =>
            {
                Assert.That(result.Model, Is.EqualTo("llama3"));
                Assert.That(result.Done, Is.True);
                Assert.That(result.TotalDuration, Is.EqualTo(4883583458));
            });
        }
        else
        {
            Assert.Fail(
                "Deserialized object was not of type 'GenerateChatCompletionFinishedResponse'."
            );
        }
    }
}
