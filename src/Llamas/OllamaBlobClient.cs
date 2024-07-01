using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Llamas.Enums;
using Llamas.Requests.Blobs;

namespace Llamas;

/// <summary>
/// Client for interacting with ollama blob functionality
/// </summary>
public sealed class OllamaBlobClient : IOllamaBlobClient
{
    /// <summary>
    /// Injected or constructed <see cref="System.Net.Http.HttpClient"/>.
    /// This client is already configured with a BaseUrl
    /// </summary>
    private HttpClient HttpClient { get; }

    /// <summary>
    ///Create a blob client using the injected <see cref="System.Net.Http.HttpClient"/>
    /// </summary>
    /// <param name="httpClient"><see cref="System.Net.Http.HttpClient"/> to inject</param>
    internal OllamaBlobClient(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    /// <summary>
    /// Checks to see if the blob matching the given digest exists
    /// </summary>
    /// <param name="checkBlobExists">SHA256 hash of the blob for which to check</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>true if the blob exists</returns>
    public async Task<bool> CheckBlobExists(
        CheckBlobExistsRequest checkBlobExists,
        CancellationToken? cancellationToken = null
    )
    {
        try
        {
            var response = await HttpClient
                .SendAsync(
                    new HttpRequestMessage(
                        HttpMethod.Head,
                        $"/api/blobs/sha256:{checkBlobExists.Digest.ToLower()}"
                    ),
                    cancellationToken ?? new CancellationToken()
                )
                .ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }

    /// <summary>
    /// Checks to see if the blob matching the calculated digest exists
    /// </summary>
    /// <param name="data">Data of blob to check for</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>true if the blob exists</returns>
    public Task<bool> CheckBlobExists(Stream data, CancellationToken? cancellationToken = null) =>
        CheckBlobExists(data.CalculateDigest(), cancellationToken);

    /// <summary>
    /// Checks to see if the blob matching the calculated digest exists
    /// </summary>
    /// <param name="data">Data of blob to check for</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>true if the blob exists</returns>
    public Task<bool> CheckBlobExists(byte[] data, CancellationToken? cancellationToken = null) =>
        CheckBlobExists(data.CalculateDigest(), cancellationToken);

    /// <summary>
    /// Checks to see if the blob matching the calculated digest exists
    /// </summary>
    /// <param name="data">Data of blob to check for</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>true if the blob exists</returns>
    public Task<bool> CheckBlobExists(
        ReadOnlyMemory<byte> data,
        CancellationToken? cancellationToken = null
    ) => CheckBlobExists(data.CalculateDigest(), cancellationToken);

    /// <summary>
    /// Create a blob on the ollama host server
    /// </summary>
    /// <param name="createBlob">The SHA256 hash of the data</param>
    /// <param name="data">The data stream to upload</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Result status of the request</returns>
    public async Task<CreateBlobResult> CreateBlob(
        CreateBlobRequest createBlob,
        Stream data,
        CancellationToken? cancellationToken = null
    )
    {
        var response = await HttpClient
            .PostAsync(
                $"/api/blobs/sha256:{createBlob.Digest.ToLower()}",
                new StreamContent(data),
                cancellationToken ?? new CancellationToken()
            )
            .ConfigureAwait(false);
        return response.StatusCode switch
        {
            HttpStatusCode.Created => CreateBlobResult.Created,
            HttpStatusCode.BadRequest => CreateBlobResult.DigestMismatch,
            _ => CreateBlobResult.NotCreated
        };
    }

    /// <summary>
    /// Create a blob on the ollama host server using calculated digest
    /// </summary>
    /// <param name="data">The data stream to upload</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Result status of the request</returns>
    public Task<CreateBlobResult> CreateBlob(
        Stream data,
        CancellationToken? cancellationToken = null
    )
    {
        if (!data.CanSeek)
            throw new ArgumentException(
                "Stream must be seekable to calculate digest automatically",
                nameof(data)
            );
        return CreateBlob(data.CalculateDigest(), data, cancellationToken);
    }

    /// <summary>
    /// Create a blob on the ollama host server
    /// </summary>
    /// <param name="createBlob">The SHA256 hash of the data</param>
    /// <param name="data">Byte data to upload</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Result status of the request</returns>
    public Task<CreateBlobResult> CreateBlob(
        CreateBlobRequest createBlob,
        byte[] data,
        CancellationToken? cancellationToken = null
    ) => CreateBlob(createBlob, data.AsMemory(), cancellationToken);

    /// <summary>
    /// Create a blob on the ollama host server using calculated digest
    /// </summary>
    /// <param name="data">Byte data to upload</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Result status of the request</returns>
    public Task<CreateBlobResult> CreateBlob(
        byte[] data,
        CancellationToken? cancellationToken = null
    ) => CreateBlob(data.AsMemory(), cancellationToken);

    /// <summary>
    /// Create a blob on the ollama host server
    /// </summary>
    /// <param name="createBlob">The SHA256 hash of the data</param>
    /// <param name="data">Byte data to upload</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Result status of the request</returns>
    public async Task<CreateBlobResult> CreateBlob(
        CreateBlobRequest createBlob,
        ReadOnlyMemory<byte> data,
        CancellationToken? cancellationToken = null
    )
    {
        var response = await HttpClient
            .PostAsync(
                $"/api/blobs/sha256:{createBlob.Digest.ToLower()}",
                new ReadOnlyMemoryContent(data),
                cancellationToken ?? new CancellationToken()
            )
            .ConfigureAwait(false);
        return response.StatusCode switch
        {
            HttpStatusCode.Created => CreateBlobResult.Created,
            HttpStatusCode.BadRequest => CreateBlobResult.DigestMismatch,
            _ => CreateBlobResult.NotCreated
        };
    }

    /// <summary>
    /// Create a blob on the ollama host server using calculated digest
    /// </summary>
    /// <param name="data">Byte data to upload</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Result status of the request</returns>
    public Task<CreateBlobResult> CreateBlob(
        ReadOnlyMemory<byte> data,
        CancellationToken? cancellationToken = null
    ) => CreateBlob(data.CalculateDigest(), data, cancellationToken);
}
