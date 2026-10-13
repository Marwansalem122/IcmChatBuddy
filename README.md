# IcM Buddy — Incident Management AI Assistant

IcM Buddy is an API-first AI assistant designed to help engineers investigate incidents using relevant incident-management and on-call documentation. It combines **.NET 10**, **.NET Aspire**, and **Microsoft.Extensions.AI** with locally hosted language models through **Ollama**, with an architecture designed to support a cloud-based Azure configuration as well.

The goal is to provide useful, structured answers grounded in available incident data—rather than relying on a model's general knowledge alone.

## Key goals

- Answer incident-related questions using relevant technical documentation.
- Ground responses in retrieved source material and clearly indicate when the available information is insufficient.
- Return actionable output, such as a diagnosis, recommended actions, and related incidents.
- Keep application logic independent from the specific chat-model provider where practical.
- Support local development without requiring an Azure AI subscription.
- Use Aspire to coordinate services and provide a consistent local development experience.

## Technology stack

- **.NET 10 / ASP.NET Core** — HTTP API and application services
- **C#** — application implementation
- **.NET Aspire** — local distributed application orchestration and service configuration
- **Microsoft.Extensions.AI (`IChatClient`)** — chat-model abstraction
- **Ollama** — local model serving
- **Llama 3.2** — local chat model used in the development setup
- **OpenAPI / Swagger** — API exploration and testing
- **Docker** — container runtime for supporting services

The architecture can be extended to use **Azure AI Search** for vector search and **Azure AI Foundry** for hosted models. These integrations require their corresponding Azure resources and configuration.

## Architecture overview

```text
API Client / Swagger UI
          |
          v
   ASP.NET Core API
          |
          v
 Microsoft.Extensions.AI
       IChatClient
          |
          v
    Ollama service
          |
          v
      Llama 3.2
```

For a retrieval-augmented generation (RAG) workflow, the intended data path is:

```text
Incident documents
        |
        v
 Ingestion and parsing
        |
        v
 Chunking and embeddings
        |
        v
    Vector storage
        |
        v
 Retrieve relevant chunks <--- User question
        |
        v
 Context-grounded model response
        |
        v
 Structured API response
```

.NET Aspire coordinates the development resources and service connections. It does not replace the API's business logic or the retrieval pipeline itself.

## Current development setup

The solution currently contains these main projects:

- `IcmChat` — ASP.NET Core chat API
- `IcmChatBuddy.AppHost` — Aspire AppHost used to configure and run the application resources
- `IcmChatBuddy.ServiceDefaults` — shared service defaults for cross-cutting concerns such as telemetry, health checks, and service configuration

The local Aspire setup configures an Ollama service, registers the `llama3.2` model, and connects the API to the model through the `IChatClient` abstraction. Open WebUI can also be run as an optional interface for interacting directly with Ollama; it is separate from the application API and is not the RAG pipeline.

> **Implementation note:** The local chat integration is the foundation of the project. Features such as document ingestion, semantic chunking, embedding generation, vector retrieval, grounded refusal behavior, incident-specific structured responses, Redis caching, and Azure provider parity should be considered implemented only when their corresponding code and configuration are present in the repository.

## Getting started

### Prerequisites

- .NET 10 SDK
- Docker Desktop (Windows or macOS)
- Visual Studio with .NET and Aspire support, or a compatible .NET CLI setup
- Sufficient RAM/CPU for the selected local model; GPU acceleration may improve inference performance when supported

### Run locally

1. Clone the repository and open `IcmChatBuddy.slnx` in Visual Studio.
2. Ensure Docker Desktop is running.
3. Set `IcmChatBuddy.AppHost` as the startup project.
4. Run the AppHost. Aspire starts the configured resources and provides a dashboard for inspecting service status and logs.
5. Open the API's Swagger/OpenAPI endpoint from the Aspire dashboard and send a test request.

The exact endpoint and API route depend on the current application configuration; use the URLs shown by Aspire rather than assuming a fixed local port.

### Model setup

The Aspire configuration registers the `llama3.2` model with Ollama. If the model has not been downloaded to the Ollama data volume yet, pull it using the Ollama CLI from the running container, or use the corresponding Ollama command for your local setup:

```bash
ollama pull llama3.2
```

Model downloads are stored separately from the application source code. Keep Ollama's data volume persistent so that the model does not need to be downloaded again after a container restart.

## Configuration and secrets

- Keep credentials, API keys, and connection strings containing secrets out of source control.
- Use .NET configuration providers and environment variables for environment-specific settings.
- Do not expose Ollama's API directly to the public internet. In a deployed environment, allow access only from trusted application services or a secured private network.
- Configure production HTTPS, authentication, request limits, health checks, and monitoring before exposing the API to external users.

## Production considerations

A production deployment should treat the API and model runtime as separate services:

- Build and publish the ASP.NET Core API as a container image.
- Run Ollama on a host with adequate CPU/RAM or GPU resources, or use a hosted model provider.
- Store model files in persistent storage.
- Configure private service-to-service networking, HTTPS at the public entry point, and secure configuration management.
- Add health checks, structured logs, OpenTelemetry tracing/metrics, and resilience policies appropriate for model latency.
- Load-test inference latency and concurrency before choosing server capacity.

The Aspire AppHost is useful for development orchestration. Production deployment should use a deployment target supported by the solution or explicitly define the equivalent service, networking, storage, and configuration settings in the chosen platform.

## Roadmap

The following are natural next steps for evolving the project into a complete incident-focused RAG system:

- [ ] Incident-document ingestion and parsing
- [ ] Semantic chunking and embedding generation
- [ ] Local vector storage and similarity-based retrieval
- [ ] Context-grounded responses with explicit insufficient-evidence handling
- [ ] Structured diagnosis, recommended actions, and related-incident output
- [ ] Tool/function calling for retrieval workflows
- [ ] Resilience policies, retries, timeouts, and circuit breaking tuned for AI workloads
- [ ] OpenTelemetry instrumentation for model requests and retrieval operations
- [ ] Redis-based distributed caching where appropriate
- [ ] Optional Azure AI Search and Azure AI Foundry provider configuration
- [ ] Automated tests and deployment automation

## Project scope

IcM Buddy focuses on backend and AI application architecture: connecting an API to language models, building a retrieval pipeline, and returning responses grounded in incident documentation. It is not intended to train foundation models or provide a frontend application.

## License

Add a license file and update this section before distributing the project publicly.
