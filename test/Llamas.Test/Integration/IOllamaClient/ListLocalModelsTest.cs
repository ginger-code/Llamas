namespace Llamas.Test.Integration.IOllamaClient;

public abstract class ListLocalModelsTest
{
    public static async Task Run(Llamas.IOllamaClient client, string modelName)
    {
        var models = await client.ListLocalModels();
        Assert.Multiple(() =>
        {
            Assert.That(models, Is.Not.Null.And.Length.EqualTo(1));
            Assert.That(models?[0].Name, Is.EqualTo(modelName));
        });
    }
}
