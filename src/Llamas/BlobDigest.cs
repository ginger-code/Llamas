using System;
using System.IO;
using System.Security.Cryptography;

namespace Llamas;

/// <summary>
/// Methods for calculating SHA256 digests of data
/// </summary>
internal static class BlobDigest
{
    /// <summary>
    /// Calculate the SHA256 digest of a stream of data
    /// </summary>
    /// <param name="data">Data to hash</param>
    public static string CalculateDigest(this Stream data)
    {
        var position = data.Position;
        var hash = Convert.ToHexString(SHA256.HashData(data));
        data.Position = position;
        return hash;
    }

    /// <summary>
    /// Calculate the SHA256 digest of a stream of data
    /// </summary>
    /// <param name="data">Data to hash</param>
    public static string CalculateDigest(this byte[] data) =>
        Convert.ToHexString(SHA256.HashData(data.AsSpan()));

    /// <summary>
    /// Calculate the SHA256 digest of a stream of data
    /// </summary>
    /// <param name="data">Data to hash</param>
    public static string CalculateDigest(this ReadOnlyMemory<byte> data) =>
        Convert.ToHexString(SHA256.HashData(data.Span));
}
