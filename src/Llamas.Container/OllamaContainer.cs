using System;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Llamas.Configuration;
using Microsoft.Extensions.Logging;

namespace Llamas;

/// <summary>
/// Docker container for Ollama
/// </summary>
public sealed class OllamaContainer : IAsyncDisposable
{
    /// <summary>
    /// Port exposed by the container internally
    /// </summary>
    private const int OllamaContainerPort = 11434;

    /// <summary>
    /// Port exposed on the localhost mapped to the internal port
    /// </summary>
    private readonly ushort _exposedPort;

    /// <summary>
    /// Hosted Ollama container in docker
    /// </summary>
    private IContainer Container { get; }

    /// <summary>
    /// Create an instance of <see cref="OllamaContainer"/> using container configuration
    /// </summary>
    /// <param name="configuration">Container configuration</param>
    /// <param name="logger">Logging functionality</param>
    public OllamaContainer(OllamaContainerConfiguration configuration, ILogger? logger = null)
    {
        _exposedPort = configuration.MappedPort;
        Container = Configure(logger);
    }

    /// <summary>
    /// Create an instance of <see cref="OllamaContainer"/> using client configuration
    /// </summary>
    /// <param name="configuration">Client configuration</param>
    /// <param name="logger">Logging functionality</param>
    public OllamaContainer(OllamaClientConfiguration configuration, ILogger? logger = null)
        : this(new OllamaContainerConfiguration(configuration), logger) { }

    /// <summary>
    /// Configuration for an <see cref="OllamaClient"/> matching the container's configuration
    /// </summary>
    public OllamaClientConfiguration ClientConfiguration => new("127.0.0.1", _exposedPort);

    /// <summary>
    /// Configuration for an <see cref="OllamaContainer"/> matching the container's configuration
    /// </summary>
    public OllamaContainerConfiguration ContainerConfiguration => new(_exposedPort);

    /// <summary>
    /// Create and start a new ollama container
    /// </summary>
    /// <param name="configuration">Configuration for container setup</param>
    /// <param name="logger">Logger</param>
    /// <param name="cancellationToken">Token for canceling and shutting down the container gracefully</param>
    public static async Task<OllamaContainer> Create(
        OllamaContainerConfiguration configuration,
        ILogger? logger = null,
        CancellationToken cancellationToken = default
    )
    {
        var container = new OllamaContainer(configuration, logger);
        await container.Container.StartAsync(cancellationToken).ConfigureAwait(false);
        return container;
    }

    /// <summary>
    /// Configures the ollama docker container
    /// </summary>
    private IContainer Configure(ILogger? logger)
    {
        return new ContainerBuilder()
            .WithImage("ollama/ollama")
            // Bind internal port 11434 to available port on host
            .WithPortBinding(_exposedPort, OllamaContainerPort)
            // Configure device requirements
            .WithCreateParameterModifier(parameters =>
            {
                parameters.HostConfig.DeviceRequests =
                [
                    new DeviceRequest
                    {
                        // Enable GPU utilization
                        Capabilities =
                        [
                            ["gpu"]
                        ],
                        // Enable all GPUs, instead of indexing single device
                        Count = -1
                    }
                ];
            })
            .WithLogger(logger)
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    // Wait until graphics card diagnostics are complete
                    .UntilMessageIsLogged("msg=\"inference compute\"")
            )
            .Build();
    }

    /// <summary>
    ///Prevents disposal twice
    /// </summary>
    private bool _disposed;

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            await Container.StopAsync().ConfigureAwait(false);
            await Container.DisposeAsync().ConfigureAwait(false);
            _disposed = true;
        }
    }
}
