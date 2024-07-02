using System.Diagnostics;
using Llamas.Requests;

namespace Llamas.Tests.Integration.IOllamaClient;

public abstract class ShowModelInfoTest
{
    public static async Task Run(Llamas.IOllamaClient client, string modelName)
    {
        var modelInfo = await client.ShowModelInfo(new ShowModelRequest { Name = modelName });
        Assert.That(modelInfo, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(modelInfo.Info, Contains.Key("general.architecture"));
            Debug.Assert(modelInfo.Info != null, "modelInfo.Info != null");
            Assert.That(
                modelInfo.Info["general.architecture"].ToString()?.Trim(),
                Is.EqualTo("llama")
            );
        });
    }
}
