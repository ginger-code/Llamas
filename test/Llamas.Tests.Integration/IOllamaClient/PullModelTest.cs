using System.Text;
using Llamas.Requests;
using Llamas.Responses;

namespace Llamas.Tests.Integration.IOllamaClient;

public abstract class PullModelTest
{
    public static async Task Run(Llamas.IOllamaClient client, string modelName)
    {
        var responses = client.PullModel(new PullModelRequest { Name = modelName });
        var statusResponseCount = 0;
        var downloadingResponseCount = 0;
        var completedResponseCount = 0;
        var sb = new StringBuilder();
        await foreach (var response in responses)
        {
            switch (response)
            {
                case PullModelResponse.PullStatusResponse pullStatusResponse:
                    sb.AppendLine(pullStatusResponse.Status);
                    statusResponseCount++;
                    break;
                case PullModelResponse.DownloadingResponse downloadingResponse:
                    sb.AppendLine(downloadingResponse.Status);
                    sb.AppendLine($"{downloadingResponse.Completed}/{downloadingResponse.Total}");

                    downloadingResponseCount++;
                    if (downloadingResponse.Completed == downloadingResponse.Total)
                        completedResponseCount++;
                    break;
                default:
                    Assert.Fail("Unknown type?!");
                    break;
            }
        }

        Assert.Multiple(() =>
        {
            Assert.That(statusResponseCount, Is.GreaterThan(0));
            Assert.That(downloadingResponseCount, Is.GreaterThan(0));
            Assert.That(completedResponseCount, Is.GreaterThan(0));
        });
    }
}
