using Llamas.Configuration;
using Llamas.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Llamas.Tests.Integration;

/// <summary>
/// Root class for configuring shared resources for integration tests (e.g. hosted containerized services)
/// </summary>
[SetUpFixture, Parallelizable]
public static class IntegrationTests
{
    /// <summary>
    /// Port exposed on localhost for integration tests.
    /// </summary>
    private const ushort ExternalPort = 8082;

    /// <summary>
    /// Hosted Ollama Container
    /// </summary>
    private static OllamaContainer Container { get; set; } = null!;

    /// <summary>
    /// Generated Ollama client for integration tests.
    /// </summary>
    internal static OllamaClient Client { get; private set; } = null!;

    /// <summary>
    /// Creat and start the Ollama container for all integration tests
    /// </summary>
    [OneTimeSetUp]
    public static async Task Setup()
    {
        Container = await OllamaContainer.Create(
            new OllamaContainerConfiguration(ExternalPort),
            CreateContainerLogger()
        );
        Client = new OllamaClient(new OllamaClientConfiguration("127.0.0.1", ExternalPort));
        ServiceCollection services = new();
        services.AddLogging(b => b.AddConsole());
        services.AddOllamaContainerService();
    }

    /// <summary>
    /// Stop the Ollama container after all integration tests have run.
    /// </summary>
    [OneTimeTearDown]
    public static async Task TearDown()
    {
        await Container.DisposeAsync();
    }

    /// <summary>
    /// Create a console logger the Ollama container
    /// </summary>
    /// <returns></returns>
    private static ILogger<OllamaContainer> CreateContainerLogger()
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
            builder.SetMinimumLevel(LogLevel.Trace).AddConsole()
        );
        return loggerFactory.CreateLogger<OllamaContainer>();
    }
}
