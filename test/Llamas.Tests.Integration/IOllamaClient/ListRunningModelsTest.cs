namespace Llamas.Tests.Integration.IOllamaClient;

public abstract class ListRunningModelsTest
{
    public static async Task Run(Llamas.IOllamaClient client, string modelName)
    {
        var models = await client.ListRunningModels();
        Assert.Multiple(() =>
        {
            Assert.That(models, Is.Not.Null.And.Length.EqualTo(1));
            Assert.That(models?[0].Name, Is.EqualTo(modelName));
        });
    }
}
