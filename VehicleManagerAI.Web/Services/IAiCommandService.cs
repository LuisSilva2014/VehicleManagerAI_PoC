using VehicleManagerAI.Web.Models;

namespace VehicleManagerAI.Web.Services;

/// <summary>
/// Orchestrates "prompt → AI command → vehicle service".
/// This is the integration point that makes natural language an extra UI layer.
/// </summary>
public interface IAiCommandService
{
    Task<AiCommandResult> ProcessAsync(string userPrompt, CancellationToken cancellationToken = default);
}
