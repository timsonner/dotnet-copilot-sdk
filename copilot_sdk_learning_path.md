# Copilot SDK (C#) Learning Path

This document summarizes the architecture and usage of the GitHub Copilot SDK for .NET. It is designed to help agents and developers quickly understand how to build Copilot agents in C#.

## 1. Core Concepts

The SDK enables the creation of "agents" that communicate with the GitHub Copilot platform. It operates on a client-session model.

### Key Components

*   **`CopilotClient`**: The main entry point. It manages the connection to the local Copilot CLI (which acts as a sidecar/server). It implements `IAsyncDisposable`.
*   **`CopilotSession`**: Represents a single conversation thread. A client can manage multiple concurrent sessions.
*   **`SessionConfig`**: Configuration for a session, defining the model (e.g., `gpt-4`), system prompt, and available tools (skills).
*   **`AIFunctionFactory`**: A utility (from `Microsoft.Extensions.AI`) used to wrap C# methods as tools that the LLM can invoke.

## 2. Implementation Pattern

The standard implementation follows this flow:

1.  **Initialize Client**: Create and start a `CopilotClient`.
2.  **Configure Session**: Define capabilities (tools) and behavior (model, system message) in a `SessionConfig`.
3.  **Start Session**: Call `client.CreateSessionAsync()`.
4.  **Event Loop**: Subscribe to session events (`On()`) to handle incoming messages, tool execution requests, and errors.
5.  **Disposal**: Ensure proper cleanup of the client and session.

### Example Code Structure

```csharp
using GitHub.Copilot.Sdk;
using Microsoft.Extensions.AI;
using System.ComponentModel;

// 1. Initialize Client
await using var client = new CopilotClient();
await client.StartAsync();

// 2. Define Tools (Skills)
var tools = new List<AIFunction>
{
    AIFunctionFactory.Create(
        ([Description("The user's name")] string name) => $"Hello, {name}!",
        "greet_user",
        "Greets the user by name")
};

// 3. Configure Session
var config = new SessionConfig
{
    Model = "gpt-4",
    SystemMessage = "You are a helpful assistant.",
    Tools = tools
};

// 4. Start Session
await using var session = await client.CreateSessionAsync(config);

// 5. Handle Events
session.On(evt =>
{
    switch (evt)
    {
        case AssistantMessageEvent msg:
            Console.WriteLine($"[AI]: {msg.Data.Content}");
            break;
        case ToolExecutionStartEvent tool:
            Console.WriteLine($"[Tool]: Executing {tool.Data.Function.Name}...");
            break;
        case SessionErrorEvent err:
            Console.Error.WriteLine($"[Error]: {err.Data.Message}");
            break;
    }
});

// Keep the application alive or manage the input loop
// ...
```

## 3. Events

The SDK is event-driven. Key events to handle:

*   `AssistantMessageEvent`: Text response from the model.
*   `ToolExecutionStartEvent` / `ToolExecutionCompleteEvent`: Lifecycle of tool calls.
*   `SessionIdleEvent`: Indicates the turn is complete (model has finished generating).
*   `SessionErrorEvent`: Critical errors.

## 4. Requirements

*   **Runtime**: .NET 10.0 or later.
*   **Dependencies**: 
    *   `GitHub.Copilot.SDK` (NuGet)
    *   `Microsoft.Extensions.AI` (for tool definitions)
*   **Environment**: The `github-copilot-cli` must be installed and available in the system PATH.

## References

*   **Detailed Instructions & API Docs**: [awesome-copilot/instructions/copilot-sdk-csharp.instructions.md](https://github.com/github/awesome-copilot/blob/main/instructions/copilot-sdk-csharp.instructions.md)
*   **Cookbook (Examples)**: [github/copilot-sdk/tree/main/cookbook/dotnet](https://github.com/github/copilot-sdk/tree/main/cookbook/dotnet)
