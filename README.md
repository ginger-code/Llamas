# Llamas

---

![Llamas NuGet Version](https://img.shields.io/nuget/v/Llamas?style=for-the-badge&logo=nuget&label=Llamas)
![Llamas.Abstractions NuGet Version](https://img.shields.io/nuget/v/Llamas.Abstractions?style=for-the-badge&logo=nuget&label=Llamas.Abstractions)
![Llamas.Container NuGet Version](https://img.shields.io/nuget/v/Llamas.Container?style=for-the-badge&logo=nuget&label=Llamas.Container)

## Table of Contents

- [About](#about)
- [Usage](#usage)
- [Dependency Injection](#dependency-injection)
- [Testing](#testing)
    - [Unit Tests](#unit-tests)
    - [Integration Tests](#integration-tests)
- [`Llamas.Abstractions`](#llamasabstractions)
- [`Llamas.Container`](#llamascontainer)
    - [Container Dependency Injection](#container-dependency-injection)

## About

`Llamas` is a .NET client library for [Ollama](https://github.com/ollama/ollama), enabling .NET developers to interact
with and leverage large language models.
If using the [Llamas.Container](#llamascontainer) package, developers can also host pre-configured instances of Ollama
in docker from their own .NET code either directly or using the simple DI patterns they are accustomed to with no
configuration knowledge needed.

`Llamas` is a handwritten client library focused on ergonomics and performance, taking full advantage
of `IAsyncEnumerable` and `ndjson` to handle and propagate live-streaming data.
This client handles the functionality exposed by
the [Ollama API](https://github.com/ollama/ollama/blob/main/docs/api.md) and therefore requires an instance of Ollama to
be accessible over the local network, or hosted using the `Llamas.Container` package.

## Usage

The `IOllamaClient` interface describes the functionality of the Ollama client, such as listing models installed
locally, pulling new models, generating chat completions, generating embeddings, pushing models, and retrieving details
about models.
`IOllamaBlobClient` contains definitions for blob functionality including checking for the existence of and creation of
a data blob.

Examples of client use can be found both in the `examples` folder, as well as the integration test suite.

## Dependency Injection

`Llamas` comes with several ways to set up a client using the .NET hosting abstractions.

One can inject a client configuration and the client explicitly, or using one of the helper extension methods
on `IServiceCollection`.

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

## Testing

### Unit Tests

Unit tests are defined for any functionality which is atomic and testable without mocking a server connection.
In practice, this applies to custom serialization, stream hashing, etc..

### Integration Tests

Integration tests are defined for all core client functionality, and are supported by a hosted instance of Ollama
using `Llamas.Container`. These tests are ordered, to ensure stateful changes are accounted for.

*Warning: Running integration tests will execute an LLM on your graphics device. It is not recommended to run these
tests on a machine using an integrated graphics device or on a battery. The model used is small (<1GB) but will still
heat up your PC.*

## Llamas.Abstractions

`Llamas.Abstractions` contains interfaces for `Llamas` Ollama client and blob support, as well as the exported types
needed for parameters and results.
This assembly is provided separately to allow for integration purposes such as DI and client (e.g. Blazor) references.

## Llamas.Container

`Llamas.Container` is a library providing Ollama self-hosting capabilities to .NET applications.
This assembly provides the logic needed to automatically hook into a local docker service, pull the ollama container,
configure the necessary devices, and run it transitively.

Support for persistence and further configuration is planned.

### Container Dependency Injection

Like `Llamas`, `Llamas.Container` extends `IServiceCollection` with methods for easy injection.
These allow for hosting with or without a client, and can be injected using the same configuration as the client for
simplicity.

```csharp
/// Add a container based on the client configuration
var clientConfig = new OllamaClientConfiguration();
services.AddOllamaClient(clientConfig);
services.AddOllamaContainerService();
```