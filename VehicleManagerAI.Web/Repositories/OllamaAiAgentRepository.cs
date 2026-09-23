using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using VehicleManagerAI.Web.Models;
using VehicleManagerAI.Web.Services;

namespace VehicleManagerAI.Web.Repositories;

/// <summary>
/// Sends the user prompt to a local Ollama HTTP API and parses the JSON command.
/// The model is instructed to return data only — it never touches the vehicle store.
/// </summary>
public class OllamaAiAgentRepository : IAiAgentRepository
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly HttpClient _httpClient;
    private readonly OllamaOptions _options;
    private readonly ILogger<OllamaAiAgentRepository> _logger;

    public OllamaAiAgentRepository(
        HttpClient httpClient,
        IOptions<OllamaOptions> options,
        ILogger<OllamaAiAgentRepository> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<AiCommand> InterpretAsync(string userPrompt, CancellationToken cancellationToken = default)
    {
        var request = new OllamaChatRequest
        {
            Model = _options.Model,
            Stream = false,
            Format = "json",
            Messages =
            [
                new OllamaChatMessage { Role = "system", Content = BuildSystemPrompt() },
                new OllamaChatMessage { Role = "user", Content = userPrompt }
            ]
        };

        try
        {
            using var response = await _httpClient.PostAsJsonAsync("/api/chat", request, SerializerOptions, cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Ollama returned {Status}: {Body}", (int)response.StatusCode, body);
                return AiCommandParser.Unknown(
                    $"Ollama returned HTTP {(int)response.StatusCode}. Confirm the model '{_options.Model}' is pulled and Ollama is running at {_options.BaseUrl}.",
                    body);
            }

            var chat = JsonSerializer.Deserialize<OllamaChatResponse>(body, SerializerOptions);
            var content = chat?.Message?.Content;
            var command = AiCommandParser.Parse(content);
            command.RawResponse ??= content;
            return command;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return AiCommandParser.Unknown(
                $"The local Ollama model timed out after {_options.TimeoutSeconds} seconds. The first request after startup is often slow while the model loads.");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Could not reach Ollama at {BaseUrl}", _options.BaseUrl);
            return AiCommandParser.Unknown(
                $"Could not reach the local Ollama agent at {_options.BaseUrl}. Start Ollama, then pull the model with: ollama pull {_options.Model}.");
        }
    }

    internal static string BuildSystemPrompt() =>
        """
        You are a command interpreter for a Vehicle Management System.
        Convert the user's natural language request into ONE JSON object.
        Do not include markdown. Do not include any text outside JSON.

        Schema:
        {
          "action": "create" | "update" | "delete" | "get" | "list" | "unknown",
          "vehicleId": number or null,
          "make": string or null,
          "model": string or null,
          "year": number or null,
          "explanation": string
        }

        Rules:
        - action is required.
        - create: fill make, model, and year when the user provides them. Include vehicleId only if the user specified one.
        - update: vehicleId is required. Only fill fields the user wants to change.
        - delete: vehicleId is required.
        - get: vehicleId is required.
        - list: no other fields are required.
        - If the intent is unclear, use action "unknown" and explain why.
        """;

    private sealed class OllamaChatRequest
    {
        public string Model { get; set; } = string.Empty;
        public bool Stream { get; set; }
        public string Format { get; set; } = "json";
        public List<OllamaChatMessage> Messages { get; set; } = [];
    }

    private sealed class OllamaChatMessage
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    private sealed class OllamaChatResponse
    {
        public OllamaChatMessage? Message { get; set; }
    }
}
