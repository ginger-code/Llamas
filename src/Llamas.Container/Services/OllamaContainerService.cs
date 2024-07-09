using System.Threading;
using System.Threading.Tasks;
using Llamas.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Llamas.Services;

/// <summary>
/// Service for hosting ollama containers
/// </summary>
/// <param name="containerConfiguration">Configuration used to set up the container</param>
/// <param name="logger">Logger</param>
public sealed class OllamaContainerService(
    OllamaContainerConfiguration containerConfiguration,
    ILogger<OllamaContainerService>? logger
) : IHostedService
{
    /// <summary>
    /// The hosted container running ollama
    /// </summary>
    private OllamaContainer? Container { get; set; }

    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous Start operation.</returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Container = await OllamaContainer
            .Create(containerConfiguration, logger, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous Stop operation.</returns>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (Container is not null)
            await Container.DisposeAsync().ConfigureAwait(false);
    }
}
