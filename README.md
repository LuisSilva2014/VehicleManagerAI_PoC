# Vehicle Manager AI PoC

**Author:** Luis Silva

A small, fully offline proof of concept that shows how an **ASP.NET MVC** application can add a **natural-language interaction layer** on top of ordinary business services.

The vehicle screens are intentionally simple. The interesting part is the path from a chat prompt to a structured command executed by your code, not by the model.

## What you will learn

- Keep MVC, services, repositories, and dependency injection even when AI is involved.
- Treat the local model as an interpreter, never as something that writes to your data store.
- Host a Blazor chat component inside an MVC site.
- Talk to [Ollama](https://ollama.com) over HTTP from a dedicated repository.
- Unit-test AI orchestration by mocking the agent interface.

## Solution layout

```
VehicleManagerAI.slnx
├── VehicleManagerAI.Web          ASP.NET Core MVC + Blazor chat
│   ├── Controllers               MVC endpoints (Home, Vehicles, Chat)
│   ├── Views                     Traditional CRUD screens
│   ├── Components/AiChat.razor   Interactive chat UI
│   ├── Models                    Vehicle, AiCommand, options
│   ├── Repositories              In-memory vehicles + Ollama HTTP client
│   └── Services                  Business rules + AI command execution
└── VehicleManagerAI.Tests        xUnit + Moq
```

Two input channels share one service:

```
MVC form  ──┐
            ├── IVehicleService ── IVehicleRepository (in-memory)
Blazor chat ┘         ▲
                      │
             IAiCommandService
                      ▲
             IAiAgentRepository ── local Ollama
```

## Prerequisites

1. [.NET 10 SDK](https://dotnet.microsoft.com/download)
2. [Ollama](https://ollama.com/download) running locally
3. A pulled model, for example:

```bash
ollama pull gemma4:e4b
```

If you prefer another local model, change `Ollama:Model` in `VehicleManagerAI.Web/appsettings.json`.

MVC CRUD works even when Ollama is not running. Only the chat page needs the local agent.

## Run the web app

```bash
dotnet run --project VehicleManagerAI.Web
```

Open the URL printed by Kestrel (default `http://localhost:5072`).

Try both paths:

- **Vehicles** — classic create / details / edit / delete forms
- **AI Chat** — natural language, for example:
  - `Add a new vehicle with ID 123, Toyota Corolla, 2024.`
  - `Update vehicle 123 and change the model to Camry.`
  - `Remove vehicle with ID 123.`
  - `List all vehicles.`

The first chat request can take a while while Ollama loads the model into memory.

## Run the tests

```bash
dotnet test
```

Tests cover:

| Area | What is verified |
| --- | --- |
| `VehicleServiceTests` | Create, read, update, delete, duplicate-id rejection |
| `AiCommandServiceTests` | Prompt → mocked AI command → same CRUD operations |
| `AiCommandParserTests` | JSON and markdown-fenced model output |
| `VehiclesControllerTests` | MVC actions for list / create / edit / delete |

The AI repository is mocked. Tests never call Ollama.

## How a chat prompt is executed

1. `AiChat` sends the user text to `IAiCommandService`.
2. `OllamaAiAgentRepository` POSTs to `http://localhost:11434/api/chat` with a system prompt that demands JSON.
3. `AiCommandParser` maps the model output to `AiCommand`.
4. `AiCommandService` switches on `create | update | delete | get | list` and calls `IVehicleService`.
5. The chat UI shows the business result, not raw model reasoning.

The model cannot insert a vehicle by itself. If JSON is missing or Ollama is down, the command becomes `unknown` and the inventory is left unchanged.

## Configuration

```json
"Ollama": {
  "BaseUrl": "http://localhost:11434",
  "Model": "gemma4:e4b",
  "TimeoutSeconds": 120
}
```

## Design notes for learners

- **Repositories** isolate I/O (memory + HTTP).
- **Services** own validation and use cases.
- **Controllers / Blazor** only collect input and present results.
- Register the vehicle store as a **singleton** so MVC and Blazor share data for the lifetime of the process. Restarting the app resets the sample inventory.
- Prefer an `IAiAgentRepository` seam so tests replace Ollama with Moq.

This sample is a teaching baseline. Swap the in-memory store for EF Core, or add more command types, without changing the overall shape.
