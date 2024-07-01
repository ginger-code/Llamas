# Llamas.Container

---

![Llamas.Container NuGet Version](https://img.shields.io/nuget/v/Llamas.Container?style=for-the-badge&logo=nuget&label=Llamas.Container)

`Llamas.Container` is a library providing Ollama self-hosting capabilities to .NET applications. 
This package provides the logic needed to automatically hook into a local docker service, pull the ollama container, configure the necessary devices, and run it transitively.

Support for persistence and further configuration is planned.

## Container Dependency Injection

Like `Llamas`, `Llamas.Container` extends `IServiceCollection` with methods for easy injection. 
These allow for hosting with or without a client, and can be injected using the same configuration as the client for simplicity.

```csharp
/// Add a container based on the client configuration
var clientConfig = new OllamaClientConfiguration();
services.AddOllamaClient(clientConfig);
services.AddOllamaContainerService();
```