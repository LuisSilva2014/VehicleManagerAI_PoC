namespace VehicleManagerAI.Web.Models;

/// <summary>
/// Bound from the "Ollama" section in appsettings.json.
/// Point this at any locally hosted Ollama instance and model name.
/// </summary>
public class OllamaOptions
{
    public const string SectionName = "Ollama";

    public string BaseUrl { get; set; } = "http://localhost:11434";
    public string Model { get; set; } = "gemma4:e4b";
    public int TimeoutSeconds { get; set; } = 120;
}
