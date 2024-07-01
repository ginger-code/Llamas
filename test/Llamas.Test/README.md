# Llamas.Test

---

## Unit Tests

Unit tests are defined for any functionality which is atomic and testable without mocking a server connection.
In practice, this applies to custom serialization, stream hashing, etc..

## Integration Tests

Integration tests are defined for all core client functionality, and are supported by a hosted instance of Ollama using
`Llamas.Container`. These tests are ordered, to ensure stateful changes are accounted for.

*Warning: Running integration tests will execute an LLM on your graphics device. It is not recommended to run these
tests on a machine using an integrated graphics device or on a battery. The model used is small (<1GB) but will still
heat up your PC.*