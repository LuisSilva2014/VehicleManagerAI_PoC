using VehicleManagerAI.Web.Models;
using VehicleManagerAI.Web.Repositories;

namespace VehicleManagerAI.Web.Services;

public class AiCommandService : IAiCommandService
{
    private readonly IAiAgentRepository _aiAgent;
    private readonly IVehicleService _vehicles;

    public AiCommandService(IAiAgentRepository aiAgent, IVehicleService vehicles)
    {
        _aiAgent = aiAgent;
        _vehicles = vehicles;
    }

    public async Task<AiCommandResult> ProcessAsync(string userPrompt, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userPrompt))
        {
            return AiCommandResult.Fail(
                AiCommandParser.Unknown("The prompt was empty."),
                "Please enter a command such as \"Add a new vehicle with ID 123, Toyota Corolla, 2024.\"");
        }

        var command = await _aiAgent.InterpretAsync(userPrompt.Trim(), cancellationToken);
        return Execute(command);
    }

    internal AiCommandResult Execute(AiCommand command) => command.GetAction() switch
    {
        AiAction.Create => Create(command),
        AiAction.Update => Update(command),
        AiAction.Delete => Delete(command),
        AiAction.Get => Get(command),
        AiAction.List => List(command),
        _ => AiCommandResult.Fail(command, command.Explanation ?? "I could not determine which vehicle action you wanted.")
    };

    private AiCommandResult Create(AiCommand command)
    {
        var vehicle = new Vehicle
        {
            Id = command.VehicleId ?? 0,
            Make = command.Make ?? string.Empty,
            Model = command.Model ?? string.Empty,
            Year = command.Year ?? 0
        };

        var result = _vehicles.Create(vehicle);
        return result.Success
            ? AiCommandResult.Ok(command, result.Message, result.Value)
            : AiCommandResult.Fail(command, result.Message);
    }

    private AiCommandResult Update(AiCommand command)
    {
        if (command.VehicleId is null or <= 0)
        {
            return AiCommandResult.Fail(command, "An update requires a vehicle ID.");
        }

        var existing = _vehicles.GetById(command.VehicleId.Value);
        if (existing is null)
        {
            return AiCommandResult.Fail(command, $"Vehicle {command.VehicleId} was not found.");
        }

        if (!string.IsNullOrWhiteSpace(command.Make))
        {
            existing.Make = command.Make;
        }

        if (!string.IsNullOrWhiteSpace(command.Model))
        {
            existing.Model = command.Model;
        }

        if (command.Year is > 0)
        {
            existing.Year = command.Year.Value;
        }

        var result = _vehicles.Update(existing);
        return result.Success
            ? AiCommandResult.Ok(command, result.Message, result.Value)
            : AiCommandResult.Fail(command, result.Message);
    }

    private AiCommandResult Delete(AiCommand command)
    {
        if (command.VehicleId is null or <= 0)
        {
            return AiCommandResult.Fail(command, "A delete requires a vehicle ID.");
        }

        var result = _vehicles.Delete(command.VehicleId.Value);
        return result.Success
            ? AiCommandResult.Ok(command, result.Message, result.Value)
            : AiCommandResult.Fail(command, result.Message);
    }

    private AiCommandResult Get(AiCommand command)
    {
        if (command.VehicleId is null or <= 0)
        {
            return AiCommandResult.Fail(command, "A lookup requires a vehicle ID.");
        }

        var vehicle = _vehicles.GetById(command.VehicleId.Value);
        return vehicle is null
            ? AiCommandResult.Fail(command, $"Vehicle {command.VehicleId} was not found.")
            : AiCommandResult.Ok(command, $"Found {vehicle}.", vehicle);
    }

    private AiCommandResult List(AiCommand command)
    {
        var vehicles = _vehicles.GetAll();
        var summary = vehicles.Count == 0
            ? "There are no vehicles in the inventory."
            : "Current inventory:\n" + string.Join("\n", vehicles.Select(v => $"• {v}"));

        return AiCommandResult.Ok(command, summary, vehicles: vehicles);
    }
}
