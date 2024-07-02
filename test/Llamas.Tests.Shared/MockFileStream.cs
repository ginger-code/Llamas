namespace Llamas.Tests;

/// <summary>
/// A mock stream simulating a file stream of arbitrary size.
/// </summary>
public class MockFileStream(long length = 1_073_741_824, bool canSeek = true) : Stream
{
    /// <inheritdoc />
    public override bool CanRead => true;

    /// <inheritdoc />
    public override bool CanSeek => canSeek;

    /// <inheritdoc />
    public override bool CanWrite => false;

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
    {
        int added;
        for (
            added = 0;
            added < count && added + offset < buffer.Length && Position < Length;
            added++, Position++
        )
        {
            buffer[added + offset] = (byte)(Position % 255);
        }

        return added;
    }

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin)
    {
        if (!CanSeek)
            throw new NotSupportedException();
        return Position = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => Math.Min(Length, Math.Max(0, Position + offset)),
            SeekOrigin.End => Length - offset,
            _ => throw new ArgumentOutOfRangeException(nameof(origin), origin, null)
        };
    }

    /// <inheritdoc />
    public override long Length { get; } = length;

    /// <inheritdoc />
    public override long Position { get; set; }

    #region Unsupported

    /// <inheritdoc />
    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public override void Flush()
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    #endregion
}
