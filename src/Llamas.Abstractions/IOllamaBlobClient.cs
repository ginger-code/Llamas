using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Llamas.Enums;
using Llamas.Requests.Blobs;

namespace Llamas;

/// <summary>
/// An ollama blob client implementation
/// </summary>
public interface IOllamaBlobClient
{
    /// <summary>
    /// Checks to see if the blob matching the given digest exists
    /// </summary>
    /// <param name="checkBlobExists">SHA256 hash of the blob for which to check</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>true if the blob exists</returns>
    Task<bool> CheckBlobExists(
        CheckBlobExistsRequest checkBlobExists,
        CancellationToken? cancellationToken = null
    );

    /// <summary>
    /// Checks to see if the blob matching the calculated digest exists
    /// </summary>
    /// <param name="data">Data of blob to check for</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>true if the blob exists</returns>
    Task<bool> CheckBlobExists(Stream data, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Checks to see if the blob matching the calculated digest exists
    /// </summary>
    /// <param name="data">Data of blob to check for</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>true if the blob exists</returns>
    Task<bool> CheckBlobExists(
        ReadOnlyMemory<byte> data,
        CancellationToken? cancellationToken = null
    );

    /// <summary>
    /// Checks to see if the blob matching the calculated digest exists
    /// </summary>
    /// <param name="data">Data of blob to check for</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>true if the blob exists</returns>
    Task<bool> CheckBlobExists(byte[] data, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Create a blob on the ollama host server
    /// </summary>
    /// <param name="createBlob">The SHA256 hash of the data</param>
    /// <param name="data">The data stream to upload</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Result status of the request</returns>
    Task<CreateBlobResult> CreateBlob(
        CreateBlobRequest createBlob,
        Stream data,
        CancellationToken? cancellationToken = null
    );

    /// <summary>
    /// Create a blob on the ollama host server using calculated digest
    /// </summary>
    /// <param name="data">The data stream to upload</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Result status of the request</returns>
    Task<CreateBlobResult> CreateBlob(Stream data, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Create a blob on the ollama host server
    /// </summary>
    /// <param name="createBlob">The SHA256 hash of the data</param>
    /// <param name="data">Byte data to upload</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Result status of the request</returns>
    Task<CreateBlobResult> CreateBlob(
        CreateBlobRequest createBlob,
        byte[] data,
        CancellationToken? cancellationToken = null
    );

    /// <summary>
    /// Create a blob on the ollama host server using calculated digest
    /// </summary>
    /// <param name="data">Byte data to upload</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Result status of the request</returns>
    Task<CreateBlobResult> CreateBlob(byte[] data, CancellationToken? cancellationToken = null);

    /// <summary>
    /// Create a blob on the ollama host server
    /// </summary>
    /// <param name="createBlob">The SHA256 hash of the data</param>
    /// <param name="data">Byte data to upload</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Result status of the request</returns>
    Task<CreateBlobResult> CreateBlob(
        CreateBlobRequest createBlob,
        ReadOnlyMemory<byte> data,
        CancellationToken? cancellationToken = null
    );

    /// <summary>
    /// Create a blob on the ollama host server using calculated digest
    /// </summary>
    /// <param name="data">Byte data to upload</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Result status of the request</returns>
    Task<CreateBlobResult> CreateBlob(
        ReadOnlyMemory<byte> data,
        CancellationToken? cancellationToken = null
    );
}
