using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace Llamas;

/// <summary>
/// Class providing extension methods for <see cref="HttpClient"/>
/// </summary>
internal static class HttpClientExtensions
{
    /// <summary>
    /// Writes to the request stream asynchronously to allow for streaming JSON to be received in the response
    /// </summary>
    /// <param name="httpClient"><see cref="System.Net.Http.HttpClient"/> to use for requests</param>
    /// <param name="requestUri">Uri to request</param>
    /// <param name="content">Content to write to stream</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <typeparam name="T">Type of object being streamed in response</typeparam>
    /// <exception cref="NotSupportedException">The response content-type was not 'application/x-ndjson'</exception>
    internal static async IAsyncEnumerable<T> PostAsyncStreaming<T>(
        this HttpClient httpClient,
        [StringSyntax("Uri")] string requestUri,
        HttpContent content,
        CancellationToken? cancellationToken = null
    )
    {
        var response = await httpClient
            .PostAsync(requestUri, content, cancellationToken ?? new CancellationToken())
            .ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(response.Content);

        if (response.Content.Headers.ContentType?.MediaType != "application/x-ndjson")
        {
            throw new NotSupportedException("MediaType must be \"application/x-ndjson\"");
        }

        var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            var json = await reader.ReadLineAsync().ConfigureAwait(false);
            if (json is null)
                continue;
            var result = OllamaJsonSerializer.Deserialize<T>(json);
            if (result is not null)
                yield return result;
        }
    }
}
