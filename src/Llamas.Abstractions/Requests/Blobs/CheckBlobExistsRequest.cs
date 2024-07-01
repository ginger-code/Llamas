using System.Diagnostics.CodeAnalysis;

namespace Llamas.Requests.Blobs;

/// <summary>
/// Request to check for the existence of a data blob
/// </summary>
public sealed record CheckBlobExistsRequest
{
    /// <summary>
    /// Request to check for the existence of a data blob
    /// </summary>
    public CheckBlobExistsRequest() { }

    /// <summary>
    /// Request to check for the existence of a data blob
    /// </summary>
    [SetsRequiredMembers]
    public CheckBlobExistsRequest(string digest)
    {
        Digest = digest;
    }

    /// SHA256 hash of blob to check for
    public required string Digest { get; init; }

    /// <summary>
    /// Implicitly convert a string to a <see cref="CheckBlobExistsRequest"/> instance.
    /// </summary>
    /// <param name="digest">SHA256 hash of the blob</param>
    public static implicit operator CheckBlobExistsRequest(string digest) => new(digest);
}
