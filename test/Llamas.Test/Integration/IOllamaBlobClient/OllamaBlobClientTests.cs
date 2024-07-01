using Llamas.Enums;

namespace Llamas.Test.Integration.IOllamaBlobClient;

[TestFixture, Parallelizable]
public class OllamaBlobClientTests
{
    #region Properties

    private static Llamas.IOllamaBlobClient Client => IntegrationTests.Client.Blobs;
    private static Stream LargeDataStream => new MockFileStream();
    private static Stream NonSeekingDataStream => new MockFileStream(1024, false);

    #endregion

    [Test, Order(0)]
    public async Task CheckBlobExistsSimplePre()
    {
        Assert.That(await Client.CheckBlobExists("a"u8.ToArray()), Is.False);
    }

    [Test, Order(1)]
    public async Task CreateBlobSimpleDigestMismatch()
    {
        Assert.That(
            await Client.CreateBlob("ccccc", "a"u8.ToArray()),
            Is.EqualTo(CreateBlobResult.DigestMismatch)
        );
    }

    [Test, Order(2)]
    public async Task CreateBlobSimpleFirstSucceeds()
    {
        Assert.That(await Client.CreateBlob("a"u8.ToArray()), Is.EqualTo(CreateBlobResult.Created));
    }

    [Test, Order(3)]
    public async Task CreateBlobSimpleSecondFails()
    {
        Assert.That(
            await Client.CreateBlob("a"u8.ToArray()),
            Is.EqualTo(CreateBlobResult.NotCreated)
        );
    }

    [Test, Order(4)]
    public async Task CheckBlobExistsSimplePost()
    {
        Assert.That(await Client.CheckBlobExists("a"u8.ToArray()), Is.True);
    }

    [Test, Order(5)]
    public async Task CreateLargeBlob()
    {
        var digest = LargeDataStream.CalculateDigest();
        Assert.That(await Client.CheckBlobExists(digest), Is.False);
        Assert.That(await Client.CreateBlob(LargeDataStream), Is.EqualTo(CreateBlobResult.Created));
        Assert.That(await Client.CheckBlobExists(digest), Is.True);
    }

    [Test, Order(6)]
    public void CreateBlobWithNonSeekingStreamFails()
    {
        Assert.That(() => Client.CreateBlob(NonSeekingDataStream), Throws.ArgumentException);
    }
}
