using System.Text.Json.Serialization;

namespace VehicleManagerAI.Web.Models;

/// <summary>
/// Structured command produced by the local Ollama model.
/// The web app never lets the model execute code; it only interprets this DTO.
/// </summary>
public class AiCommand
{
    [JsonPropertyName("action")]
    public string Action { get; set; } = "unknown";

    [JsonPropertyName("vehicleId")]
    public int? VehicleId { get; set; }

    [JsonPropertyName("make")]
    public string? Make { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("year")]
    public int? Year { get; set; }

    [JsonPropertyName("explanation")]
    public string? Explanation { get; set; }

    /// <summary>Raw text returned by Ollama, kept for debugging in the chat UI.</summary>
    [JsonIgnore]
    public string? RawResponse { get; set; }

    public AiAction GetAction() =>
        Enum.TryParse<AiAction>(Action, ignoreCase: true, out var action)
            ? action
            : AiAction.Unknown;
}
