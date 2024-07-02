using Llamas.Enums;
using Llamas.Requests;

namespace Llamas.Tests.Integration.IOllamaClient;

[TestFixture, Parallelizable]
public class OllamaClientTests
{
    #region Constants

    private const string ModelName = "tinyllama";
    private const string FullModelName = $"{ModelName}:latest";
    private const string CopiedModelName = $"{ModelName}-copy";
    private const string Prompt = "Why did the chicken cross the road?";

    private const string EmbeddingPrompt =
        "The chicken crossed the road to get to Jones BBQ and foot massage";

    #endregion

    #region Properties

    private static Llamas.IOllamaClient Client => IntegrationTests.Client;

    #endregion

    [Test, Order(0)]
    public async Task Heartbeat() => Assert.That(await Client.Heartbeat(), Is.True);

    [Test, Order(1)]
    public Task PullModel() => PullModelTest.Run(Client, FullModelName);

    [Test, Order(2)]
    public Task ListLocalModels() => ListLocalModelsTest.Run(Client, FullModelName);

    [Test, Order(3)]
    public Task ShowModelInfo() => ShowModelInfoTest.Run(Client, FullModelName);

    [Test, Order(4)]
    public async Task CopyModel() =>
        Assert.That(
            await Client.CopyModel(new CopyModelRequest(FullModelName, CopiedModelName)),
            Is.EqualTo(CopyModelResult.Copied)
        );

    [Test, Order(5)]
    public async Task GenerateEmbeddings()
    {
        var result = await Client.GenerateEmbeddings(
            new GenerateEmbeddingsRequest(FullModelName, EmbeddingPrompt)
        );

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Embedding, Is.Not.Null.And.Not.Empty);
    }

    [Test, Order(6)]
    public Task GenerateCompletion() => GenerateCompletionTest.Run(Client, FullModelName, Prompt);

    [Test, Order(7)]
    public Task ListRunningModels() => ListRunningModelsTest.Run(Client, FullModelName);

    [Test, Order(8)]
    public Task GenerateChatCompletion() =>
        GenerateChatCompletionTest.Run(Client, FullModelName, Prompt);

    [Test, Order(9)]
    public async Task DeleteModel() =>
        Assert.That(
            await Client.DeleteModel(new DeleteModelRequest(FullModelName)),
            Is.EqualTo(DeleteModelResult.Deleted)
        );
}
