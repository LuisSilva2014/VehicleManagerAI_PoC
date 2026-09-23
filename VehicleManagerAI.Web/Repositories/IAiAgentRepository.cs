using VehicleManagerAI.Web.Models;

namespace VehicleManagerAI.Web.Repositories;

/// <summary>
/// Talks to the locally hosted Ollama agent. The rest of the app depends on this
/// interface so unit tests can mock the model instead of calling HTTP.
/// </summary>
public interface IAiAgentRepository
{
    Task<AiCommand> InterpretAsync(string userPrompt, CancellationToken cancellationToken = default);
}
