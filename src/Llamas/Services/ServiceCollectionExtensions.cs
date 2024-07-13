using System;
using Llamas.Configuration;
using Llamas.Library;
using Microsoft.Extensions.DependencyInjection;

namespace Llamas.Services;

/// <summary>
/// Extensions for <see cref="IServiceCollection" />
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds in-memory cache for ollama model library retrieval
    /// </summary>
    /// <param name="serviceCollection">Service collection to modify</param>
    public static void AddOllamaLibraryInMemoryCache(this IServiceCollection serviceCollection) =>
        serviceCollection.AddSingleton<InMemoryOllamaLibraryPersistence>();

    /// <summary>
    /// Add an ollama client to the service collection, relying on the <see cref="OllamaClientConfiguration" /> added elsewhere to configure the client
    /// </summary>
    /// <param name="serviceCollection">Service collection to modify</param>
    public static void AddOllamaClient(this IServiceCollection serviceCollection) =>
        serviceCollection.AddSingleton<OllamaClient>();

    /// <summary>
    /// Add an ollama client to the service collection, injecting the supplied <see cref="OllamaClientConfiguration" /> needed to configure the client
    /// </summary>
    /// <param name="serviceCollection">Service collection to modify</param>
    /// <param name="configuration">Container configuration</param>
    public static void AddOllamaClient(
        this IServiceCollection serviceCollection,
        OllamaClientConfiguration configuration
    )
    {
        serviceCollection.AddSingleton(configuration);
        serviceCollection.AddSingleton<OllamaClient>();
    }

    /// <summary>
    /// Add an ollama client to the service collection, injecting the supplied <see cref="OllamaClientConfiguration" /> needed to configure the client
    /// </summary>
    /// <param name="serviceCollection">Service collection to modify</param>
    /// <param name="configure">Function to modify the default container configuration</param>
    public static void AddOllamaClient(
        this IServiceCollection serviceCollection,
        Func<OllamaClientConfiguration, OllamaClientConfiguration> configure
    )
    {
        var configuration = configure(new OllamaClientConfiguration());
        serviceCollection.AddSingleton(configuration);
        serviceCollection.AddSingleton<OllamaClient>();
    }

    /// <summary>
    /// Add configuration for an ollama client
    /// </summary>
    /// <param name="serviceCollection">Service collection to modify</param>
    /// <param name="configuration">Container configuration</param>
    public static void AddOllamaClientConfiguration(
        this IServiceCollection serviceCollection,
        OllamaClientConfiguration? configuration
    )
    {
        serviceCollection.AddSingleton(configuration ?? new OllamaClientConfiguration());
    }

    /// <summary>
    /// Add configuration for an ollama client
    /// </summary>
    /// <param name="serviceCollection">Service collection to modify</param>
    /// <param name="hostNameOrAddress">Hostname or address of the ollama server</param>
    /// <param name="port">Port bound by the ollama server</param>
    public static void AddOllamaClientConfiguration(
        this IServiceCollection serviceCollection,
        OllamaHostNameOrAddress? hostNameOrAddress = null,
        OllamaPort? port = null
    )
    {
        serviceCollection.AddSingleton(new OllamaClientConfiguration(hostNameOrAddress, port));
    }
}
