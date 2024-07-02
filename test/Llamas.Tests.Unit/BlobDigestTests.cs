using System.Text;

namespace Llamas.Tests.Unit;

public sealed class BlobDigestTests
{
    #region Constants

    private const string SmallInputString = "Hello, world!";

    private const string SmallInputStringHash =
        "315F5BDB76D078C43B8AC0064E4A0164612B1FCE77C869345BFC94C75894EDD3";

    private const string LargeInputFilepath = "../../../large-input.txt";

    private const string LargeInputHash =
        "151B119CA1F93A5E77F0F06B8C0F2291ADCA2924A611BC4E414AC90DF134D7B6";

    #endregion

    #region Properties

    private static byte[] SmallInputBytes => Encoding.UTF8.GetBytes(SmallInputString);

    private static ReadOnlyMemory<byte> SmallInputMemory =>
        Encoding.UTF8.GetBytes(SmallInputString).AsMemory();

    private static MemoryStream SmallInputStream => new(Encoding.UTF8.GetBytes(SmallInputString));

    private static string LargeInputString { get; } = File.ReadAllText(LargeInputFilepath);
    private static byte[] LargeInputBytes => Encoding.UTF8.GetBytes(LargeInputString);

    private static ReadOnlyMemory<byte> LargeInputMemory =>
        Encoding.UTF8.GetBytes(LargeInputString).AsMemory();

    private static MemoryStream LargeInputStream => new(Encoding.UTF8.GetBytes(LargeInputString));

    #endregion

    [Test]
    public void CalculateDigestFromStreamWithSmallInput() =>
        Assert.That(SmallInputStream.CalculateDigest(), Is.EqualTo(SmallInputStringHash));

    [Test]
    public void CalculateDigestFromArrayWithSmallInput() =>
        Assert.That(SmallInputBytes.CalculateDigest(), Is.EqualTo(SmallInputStringHash));

    [Test]
    public void CalculateDigestFromMemoryWithSmallInput() =>
        Assert.That(SmallInputMemory.CalculateDigest(), Is.EqualTo(SmallInputStringHash));

    [Test]
    public void CalculateDigestFromStreamWithLargeInput() =>
        Assert.That(LargeInputStream.CalculateDigest(), Is.EqualTo(LargeInputHash));

    [Test]
    public void CalculateDigestFromArrayWithLargeInput() =>
        Assert.That(LargeInputBytes.CalculateDigest(), Is.EqualTo(LargeInputHash));

    [Test]
    public void CalculateDigestFromMemoryWithLargeInput() =>
        Assert.That(LargeInputMemory.CalculateDigest(), Is.EqualTo(LargeInputHash));

    [Test]
    public void CalculateDigestFromMockFileStream()
    {
        Assert.That(
            new MockFileStream().CalculateDigest(),
            Is.EqualTo("16D4D0C15D9C27F3A858F56471A3304A077FA43B79B8DA89FE4CA7373400FCC8")
        );
    }
}
