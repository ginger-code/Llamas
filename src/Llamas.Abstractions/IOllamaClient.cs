using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Llamas.Enums;
using Llamas.Models;
using Llamas.Requests;
using Llamas.Responses;

namespace Llamas;

/// <summary>
/// An Ollama client implementation
/// </summary>
public interface IOllamaClient
{
    /// Blob functionality
    IOllamaBlobClient Blobs { get; }

    /// <summary>
    /// Check whether the ollama server is online and responding
    /// </summary>
    /// <returns>true if ollama is running</returns>
    Task<bool> Heartbeat();

    /// <summary>
    /// Retrieve array of models available locally
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Local models</returns>
    Task<LocalModel[]?> ListLocalModels(CancellationToken? cancellationToken = null);

    /// <summary>
    /// Retrieve array of models running locally
    /// </summary>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Running models</returns>
    Task<LocalModel[]?> ListRunningModels(CancellationToken? cancellationToken = null);

    /// <summary>
    /// Retrieve details about the given model
    /// </summary>
    /// <param name="showModel">Optional properties</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Model details</returns>
    Task<ModelInfo?> ShowModelInfo(
        ShowModelRequest showModel,
        CancellationToken? cancellationToken = null
    );

    /// <summary>
    /// Generate a completion to a prompt
    /// </summary>
    /// <param name="generateCompletion">Completion request details</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A stream of completion responses</returns>
    IAsyncEnumerable<GenerateCompletionResponse> GenerateCompletion(
        GenerateCompletionRequest generateCompletion,
        CancellationToken? cancellationToken = null
    );

    /// <summary>
    /// Generate a chat completion
    /// </summary>
    /// <param name="generateChatCompletion">Chat request details</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A stream of chat completion responses</returns>
    IAsyncEnumerable<GenerateChatCompletionResponse> GenerateChatCompletion(
        GenerateChatCompletionRequest generateChatCompletion,
        CancellationToken? cancellationToken = null
    );

    /// <summary>
    /// Pull model for local execution
    /// </summary>
    /// <param name="pullModel">Name of model and optional parameters</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A stream of downloading statuses</returns>
    IAsyncEnumerable<PullModelResponse> PullModel(
        PullModelRequest pullModel,
        CancellationToken? cancellationToken = null
    );

    /// <summary>
    /// Push a local model to a remote source
    /// </summary>
    /// <param name="pushModel">Name of model and optional parameters</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A stream of uploading statuses</returns>
    IAsyncEnumerable<PushModelResponse> PushModel(
        PushModelRequest pushModel,
        CancellationToken? cancellationToken = null
    );

    /// <summary>
    /// Duplicate a model using a given target name
    /// </summary>
    /// <param name="copyModel">Source and target model names</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Result status of request</returns>
    Task<CopyModelResult> CopyModel(
        CopyModelRequest copyModel,
        CancellationToken? cancellationToken = null
    );

    /// <summary>
    /// Delete a model from the ollama server
    /// </summary>
    /// <param name="deleteModel">Model to delete</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Result status of request</returns>
    Task<DeleteModelResult> DeleteModel(
        DeleteModelRequest deleteModel,
        CancellationToken? cancellationToken = null
    );

    /// <summary>
    /// Generate embeddings for the given model
    /// </summary>
    /// <param name="generateEmbeddings">Parameters and prompt to embed</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>Generated embedding weights</returns>
    Task<GenerateEmbeddingsResponse> GenerateEmbeddings(
        GenerateEmbeddingsRequest generateEmbeddings,
        CancellationToken? cancellationToken = null
    );
}
