using System;
using Llamas.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Llamas.Services;

/// <summary>
/// Extensions for <see cref="IServiceCollection" />
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add an ollama client to the service collection, relying on the <see cref="OllamaContainerConfiguration" /> added elsewhere to configure the client
    /// </summary>
    /// <param name="serviceCollection"></param>
    public static void AddOllamaClientFromContainerConfiguration(
        this IServiceCollection serviceCollection
    )
    {
        serviceCollection.AddSingleton<OllamaClientConfiguration>(
            sp => new OllamaClientConfiguration(
                "127.0.0.1",
                sp.GetRequiredService<OllamaContainerConfiguration>().MappedPort
            )
        );
        serviceCollection.AddOllamaClient();
    }

    /// <summary>
    /// Add an ollama container service to the service collection, relying on the <see cref="OllamaContainerConfiguration" /> added elsewhere
    /// </summary>
    /// <param name="serviceCollection">Service collection to modify</param>
    public static IServiceCollection AddOllamaContainerService(
        this IServiceCollection serviceCollection
    ) => serviceCollection.AddHostedService<OllamaContainerService>();

    /// <summary>
    /// Add an ollama container service to the service collection, relying on the <see cref="OllamaClientConfiguration" /> added elsewhere
    /// </summary>
    /// <param name="serviceCollection">Service collection to modify</param>
    public static IServiceCollection AddOllamaContainerServiceFromClientConfiguration(
        this IServiceCollection serviceCollection
    ) =>
        serviceCollection.AddHostedService<OllamaContainerService>(sp => new OllamaContainerService(
            new OllamaContainerConfiguration(
                sp.GetRequiredService<OllamaClientConfiguration>().Port
            ),
            sp.GetService<ILogger<OllamaContainerService>>()
        ));

    /// <summary>
    /// Add an ollama container service to the service collection, injecting the supplied <see cref="OllamaContainerConfiguration" /> needed to configure the service
    /// </summary>
    /// <param name="serviceCollection">Service collection to modify</param>
    /// <param name="configuration">Container configuration</param>
    public static IServiceCollection AddOllamaContainerService(
        this IServiceCollection serviceCollection,
        OllamaContainerConfiguration configuration
    )
    {
        serviceCollection.AddSingleton(configuration);
        serviceCollection.AddHostedService<OllamaContainerService>();
        return serviceCollection;
    }

    /// <summary>
    /// Add an ollama container service to the service collection, injecting the supplied <see cref="OllamaContainerConfiguration" /> needed to configure the service
    /// </summary>
    /// <param name="serviceCollection">Service collection to modify</param>
    /// <param name="configure">Function to modify the default container configuration</param>
    public static IServiceCollection AddOllamaContainerService(
        this IServiceCollection serviceCollection,
        Func<OllamaContainerConfiguration, OllamaClientConfiguration> configure
    )
    {
        var configuration = configure(new OllamaContainerConfiguration());
        serviceCollection.AddSingleton(configuration);
        serviceCollection.AddHostedService<OllamaContainerService>();
        return serviceCollection;
    }

    /// <summary>
    /// Add configuration for an ollama container service
    /// </summary>
    /// <param name="serviceCollection">Service collection to modify</param>
    /// <param name="configuration">Container configuration</param>
    /// <returns></returns>
    public static IServiceCollection AddOllamaContainerConfiguration(
        this IServiceCollection serviceCollection,
        OllamaContainerConfiguration? configuration = null
    ) => serviceCollection.AddSingleton(configuration ?? new OllamaContainerConfiguration());

    /// <summary>
    /// Add configuration for an ollama container service
    /// </summary>
    /// <param name="serviceCollection">Service collection to modify</param>
    /// <param name="portToBind">Port to bind on the host machine to the ollama port in the container</param>
    public static IServiceCollection AddOllamaContainerConfiguration(
        this IServiceCollection serviceCollection,
        OllamaPort portToBind
    ) => serviceCollection.AddSingleton(new OllamaContainerConfiguration(portToBind));
}
