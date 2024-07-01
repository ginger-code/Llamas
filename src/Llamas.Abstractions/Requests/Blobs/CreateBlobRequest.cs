using System.Diagnostics.CodeAnalysis;

namespace Llamas.Requests.Blobs;

/// <summary>
/// Request to create a data blob
/// </summary>
public sealed record CreateBlobRequest
{
    /// <summary>
    /// Request to create a data blob
    /// </summary>
    public CreateBlobRequest() { }

    /// <summary>
    /// Request to create a data blob
    /// </summary>
    [method: SetsRequiredMembers]
    public CreateBlobRequest(string digest)
    {
        Digest = digest;
    }

    /// SHA256 hash of the blob to create
    public required string Digest { get; init; }

    /// <summary>
    /// Implicitly convert a string to a <see cref="CreateBlobRequest"/> instance.
    /// </summary>
    /// <param name="digest">SHA256 hash of the blob</param>
    public static implicit operator CreateBlobRequest(string digest) => new(digest);
}
