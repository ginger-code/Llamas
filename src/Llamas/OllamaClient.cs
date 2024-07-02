using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Llamas.Configuration;
using Llamas.Enums;
using Llamas.Models;
using Llamas.Requests;
using Llamas.Responses;

namespace Llamas;

/// <summary>
/// Client for interacting with ollama
/// </summary>
public sealed class OllamaClient : IOllamaClient
{
    #region Properties

    /// <summary>
    /// Configuration to enable networking and more
    /// </summary>
    private OllamaClientConfiguration ClientConfiguration { get; }

    /// <summary>
    /// Injected or created <see cref="System.Net.Http.HttpClient"/> to be used for requests
    /// </summary>
    private HttpClient HttpClient { get; }

    /// <summary>
    /// Blob functionality
    /// </summary>
    public IOllamaBlobClient Blobs { get; }

    /// <summary>
    /// Model library functionality
    /// </summary>
    public IOllamaLibraryClient? Library { get; } = null;

    #endregion

    #region Constructors

    /// <summary>
    /// Create a new instance of OllamaClient with optional HttpClient injection
    /// </summary>
    /// <param name="clientConfiguration">ollama host server configuration</param>
    /// <param name="httpClient">Injected HttpClient</param>
    public OllamaClient(
        OllamaClientConfiguration clientConfiguration,
        HttpClient? httpClient = null
    )
    {
        ClientConfiguration = clientConfiguration;
        HttpClient = httpClient ?? new HttpClient();
        HttpClient.BaseAddress = ClientConfiguration.Uri;
        Blobs = new OllamaBlobClient(HttpClient);
    }

    /// <summary>
    /// Create a new instance of OllamaClient with IHttpClientFactory injection
    /// </summary>
    /// <param name="clientConfiguration">ollama host server configuration</param>
    /// <param name="httpClientFactory">Injected IHttpClientFactory</param>
    public OllamaClient(
        OllamaClientConfiguration clientConfiguration,
        IHttpClientFactory httpClientFactory
    )
    {
        ClientConfiguration = clientConfiguration;
        HttpClient = httpClientFactory.CreateClient();
        HttpClient.BaseAddress = ClientConfiguration.Uri;
        Blobs = new OllamaBlobClient(HttpClient);
    }

    #endregion

    #region Request Methods

    /// <summary>
    /// Check whether the ollama server is online and responding
    /// </summary>
    /// <returns>true if ollama is running</returns>
    public async Task<bool> Heartbeat()
    {
        try
        {
            var response = await HttpClient
                .SendAsync(new HttpRequestMessage(HttpMethod.Head, "/"))
                .ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }

    /// <summary>
    /// Retrieve array of models available locally
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Local models</returns>
    public async Task<LocalModel[]?> ListLocalModels(CancellationToken? cancellationToken = null)
    {
        var json = await HttpClient
            .GetStringAsync("api/tags", cancellationToken ?? new CancellationToken())
            .ConfigureAwait(false);

        return OllamaJsonSerializer.Deserialize<LocalModels>(json)?.Models;
    }

    /// <summary>
    /// Retrieve array of models running locally
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Running models</returns>
    public async Task<LocalModel[]?> ListRunningModels(CancellationToken? cancellationToken = null)
    {
        var json = await HttpClient
            .GetStringAsync("api/ps", cancellationToken ?? new CancellationToken())
            .ConfigureAwait(false);

        return OllamaJsonSerializer.Deserialize<LocalModels>(json)?.Models;
    }

    /// <summary>
    /// Retrieve details about the given model
    /// </summary>
    /// <param name="showModel">Optional properties</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Model details</returns>
    public async Task<ModelInfo?> ShowModelInfo(
        ShowModelRequest showModel,
        CancellationToken? cancellationToken = null
    )
    {
        var response = await HttpClient
            .PostAsync(
                "api/show",
                new StringContent(OllamaJsonSerializer.Serialize(showModel)),
                cancellationToken ?? new CancellationToken()
            )
            .ConfigureAwait(false);

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return OllamaJsonSerializer.Deserialize<ModelInfo>(json);
    }

    /// <summary>
    /// Generate a completion to a prompt
    /// </summary>
    /// <param name="generateCompletion">Completion request details</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A stream of completion responses</returns>
    public IAsyncEnumerable<GenerateCompletionResponse> GenerateCompletion(
        GenerateCompletionRequest generateCompletion,
        CancellationToken? cancellationToken = null
    )
    {
        return HttpClient.PostAsyncStreaming<GenerateCompletionResponse>(
            "api/generate",
            new StringContent(OllamaJsonSerializer.Serialize(generateCompletion)),
            cancellationToken ?? new CancellationToken()
        );
    }

    /// <summary>
    /// Generate a chat completion
    /// </summary>
    /// <param name="generateChatCompletion">Chat request details</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A stream of chat completion responses</returns>
    public IAsyncEnumerable<GenerateChatCompletionResponse> GenerateChatCompletion(
        GenerateChatCompletionRequest generateChatCompletion,
        CancellationToken? cancellationToken = null
    )
    {
        return HttpClient.PostAsyncStreaming<GenerateChatCompletionResponse>(
            "api/chat",
            new StringContent(OllamaJsonSerializer.Serialize(generateChatCompletion)),
            cancellationToken ?? new CancellationToken()
        );
    }

    /// <summary>
    /// Pull model for local execution
    /// </summary>
    /// <param name="pullModel">Name of model and optional parameters</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A stream of downloading statuses</returns>
    public IAsyncEnumerable<PullModelResponse> PullModel(
        PullModelRequest pullModel,
        CancellationToken? cancellationToken = null
    )
    {
        return HttpClient.PostAsyncStreaming<PullModelResponse>(
            "api/pull",
            new StringContent(OllamaJsonSerializer.Serialize(pullModel)),
            cancellationToken ?? new CancellationToken()
        );
    }

    /// <summary>
    /// Push a local model to a remote source
    /// </summary>
    /// <param name="pushModel">Name of model and optional parameters</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A stream of uploading statuses</returns>
    public IAsyncEnumerable<PushModelResponse> PushModel(
        PushModelRequest pushModel,
        CancellationToken? cancellationToken = null
    )
    {
        return HttpClient.PostAsyncStreaming<PushModelResponse>(
            "api/push",
            new StringContent(OllamaJsonSerializer.Serialize(pushModel)),
            cancellationToken ?? new CancellationToken()
        );
    }

    /// <summary>
    /// Duplicate a model using a given target name
    /// </summary>
    /// <param name="copyModel">Source and target model names</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Result status of request</returns>
    public async Task<CopyModelResult> CopyModel(
        CopyModelRequest copyModel,
        CancellationToken? cancellationToken = null
    )
    {
        var response = await HttpClient
            .PostAsync(
                "api/copy",
                new StringContent(OllamaJsonSerializer.Serialize(copyModel)),
                cancellationToken ?? new CancellationToken()
            )
            .ConfigureAwait(false);

        return response.StatusCode switch
        {
            HttpStatusCode.OK => CopyModelResult.Copied,
            HttpStatusCode.NotFound => CopyModelResult.NotFound,
            _ => CopyModelResult.NotCopied
        };
    }

    /// <summary>
    /// Delete a model from the ollama server
    /// </summary>
    /// <param name="deleteModel">Model to delete</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Result status of request</returns>
    public async Task<DeleteModelResult> DeleteModel(
        DeleteModelRequest deleteModel,
        CancellationToken? cancellationToken = null
    )
    {
        var response = await HttpClient
            .SendAsync(
                new HttpRequestMessage(HttpMethod.Delete, "api/delete")
                {
                    Content = new StringContent(OllamaJsonSerializer.Serialize(deleteModel))
                },
                cancellationToken ?? new CancellationToken()
            )
            .ConfigureAwait(false);

        return response.StatusCode switch
        {
            HttpStatusCode.OK => DeleteModelResult.Deleted,
            HttpStatusCode.NotFound => DeleteModelResult.NotFound,
            _ => DeleteModelResult.NotDeleted
        };
    }

    /// <summary>
    /// Generate embeddings for the given model
    /// </summary>
    /// <param name="generateEmbeddings">Parameters and prompt to embed</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Generated embedding weights</returns>
    public async Task<GenerateEmbeddingsResponse> GenerateEmbeddings(
        GenerateEmbeddingsRequest generateEmbeddings,
        CancellationToken? cancellationToken = null
    )
    {
        var response = await HttpClient
            .PostAsync(
                "api/embeddings",
                new StringContent(OllamaJsonSerializer.Serialize(generateEmbeddings)),
                cancellationToken ?? new CancellationToken()
            )
            .ConfigureAwait(false);

        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return OllamaJsonSerializer.Deserialize<GenerateEmbeddingsResponse>(json)!;
    }

    #endregion
}

/// <summary>
///Internal collection type for retrieving local models
/// </summary>
file sealed record LocalModels(LocalModel[] Models);
