# Llamas

---

![Llamas NuGet Version](https://img.shields.io/nuget/v/Llamas?style=for-the-badge&logo=nuget&label=Llamas)

`Llamas` is a .NET client library for [Ollama](https://github.com/ollama/ollama), enabling .NET developers to interact with and leverage large language models. 
If using the [Llamas.Container](#llamascontainer) package, developers can also host pre-configured instances of Ollama in docker from their own .NET code either directly or using the simple DI patterns they are accustomed to with no configuration knowledge needed.

`Llamas` is a handwritten client library focused on ergonomics and performance, taking full advantage of `IAsyncEnumerable` and `ndjson` to handle and propagate live-streaming data. 
This client handles the functionality exposed by the [Ollama API](https://github.com/ollama/ollama/blob/main/docs/api.md) and therefore requires an instance of Ollama to be accessible over the local network, or hosted using the `Llamas.Container` package.

## Usage

The `IOllamaClient` interface describes the functionality of the Ollama client, such as listing models installed locally, pulling new models, generating chat completions, generating embeddings, pushing models, and retrieving details about  models.
`IOllamaBlobClient` contains definitions for blob functionality including checking for the existence of and creation of a data blob.

Examples of client use can be found both in the `examples` folder, as well as the integration test suite.

## Dependency Injection

`Llamas` comes with several ways to set up a client using the .NET hosting abstractions.

One can inject a client configuration and the client explicitly, or using one of the helper extension methods  on `IServiceCollection`.

```csharp
services.AddHttpClient(); // IHttpClientFactory and HttpClient can both be injected. Otherwise, new HttpClient will be created

#region Manual Addition

/// Add the services manually
var clientConfig = new OllamaClientConfiguration();
services.AddSingleton(clientConfig);
services.AddSingleton<IOllamaClient, OllamaClient>();
#endregion


#region From Configuration

// Automatically inject the configuration and a client
var clientConfig = new OllamaClientConfiguration();
services.AddOllamaClient(clientConfig);

#endregion

#region With Configuration Builder

// Use the lambda parameter to change the default configuration values
services.AddOllamaClient(clientConfig => clientConfig with {Port = 8082});

#endregion
```