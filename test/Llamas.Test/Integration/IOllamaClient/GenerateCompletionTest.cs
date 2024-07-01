using System.Text;
using Llamas.Enums;
using Llamas.Models;
using Llamas.Primitives;
using Llamas.Requests;
using Llamas.Responses;

namespace Llamas.Test.Integration.IOllamaClient;

public abstract class GenerateCompletionTest
{
    public static async Task Run(Llamas.IOllamaClient client, string modelName, string prompt)
    {
        var responses = client.GenerateCompletion(
            new GenerateCompletionRequest
            {
                Prompt = prompt,
                Model = modelName,
                KeepAlive = new KeepAliveTimeSpan(5, UnitOfTime.Second),
                Options = new ModelOptions { Temperature = 1f }
            }
        );

        var responseCount = 0;
        var responseCompleteCount = 0;

        var message = new StringBuilder();

        await foreach (var response in responses)
        {
            switch (response)
            {
                case GenerateCompletionResponse.GenerateCompleteResponse generateCompleteResponse:
                    responseCount++;
                    responseCompleteCount++;
                    message.Append(generateCompleteResponse.Response);
                    Assert.That(generateCompleteResponse.Done, Is.True);
                    break;
                case GenerateCompletionResponse.GenerateProgressResponse generateProgressResponse:
                    responseCount++;
                    message.Append(generateProgressResponse.Response);
                    Assert.That(generateProgressResponse.Done, Is.False);
                    break;
                default:
                    Assert.Fail("Unknown type?!");
                    break;
            }
        }

        Console.WriteLine(message.ToString());
        Assert.Multiple(() =>
        {
            Assert.That(responseCount, Is.GreaterThan(0));
            Assert.That(responseCompleteCount, Is.EqualTo(1));
        });
    }
}
