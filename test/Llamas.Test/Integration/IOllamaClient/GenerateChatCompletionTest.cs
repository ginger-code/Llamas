using Llamas.Enums;
using Llamas.Models;
using Llamas.Requests;
using Llamas.Responses;

namespace Llamas.Test.Integration.IOllamaClient;

public abstract class GenerateChatCompletionTest
{
    public static async Task Run(Llamas.IOllamaClient client, string modelName, string prompt)
    {
        var userMessage = new ChatMessage(Roles.User, prompt, null);
        var responses = client.GenerateChatCompletion(
            new GenerateChatCompletionRequest
            {
                Messages = [userMessage],
                Model = modelName,
                Options = new ModelOptions { Temperature = 1f }
            }
        );

        var responseCount = 0;
        var responseCompleteCount = 0;

        List<ChatMessage> messages = [userMessage];

        await foreach (var response in responses)
        {
            switch (response)
            {
                case GenerateChatCompletionResponse.ChatCompleteResponse generateCompleteResponse:
                    responseCount++;
                    responseCompleteCount++;
                    Assert.That(generateCompleteResponse.Done, Is.True);
                    break;
                case GenerateChatCompletionResponse.ChatProgressResponse generateProgressResponse:
                    responseCount++;
                    Assert.That(generateProgressResponse.Done, Is.False);
                    if (messages.Count == 1)
                    {
                        messages.Add(generateProgressResponse.Message);
                        break;
                    }

                    messages[1] = messages[1] with
                    {
                        Content = messages[1].Content + generateProgressResponse.Message.Content
                    };
                    break;
                default:
                    Assert.Fail("Unknown type?!");
                    break;
            }
        }

        Console.WriteLine(messages[1].Content);
        Assert.Multiple(() =>
        {
            Assert.That(responseCount, Is.GreaterThan(0));
            Assert.That(responseCompleteCount, Is.EqualTo(1));
        });
    }
}
