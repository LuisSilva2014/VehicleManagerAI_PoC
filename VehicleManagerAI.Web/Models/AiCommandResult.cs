namespace VehicleManagerAI.Web.Models;

/// <summary>
/// Outcome of interpreting a natural-language prompt and running the matching CRUD operation.
/// </summary>
public class AiCommandResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public AiCommand Command { get; init; } = new();
    public Vehicle? Vehicle { get; init; }
    public IReadOnlyList<Vehicle> Vehicles { get; init; } = [];

    public static AiCommandResult Ok(AiCommand command, string message, Vehicle? vehicle = null, IReadOnlyList<Vehicle>? vehicles = null) =>
        new()
        {
            Success = true,
            Message = message,
            Command = command,
            Vehicle = vehicle,
            Vehicles = vehicles ?? []
        };

    public static AiCommandResult Fail(AiCommand command, string message) =>
        new()
        {
            Success = false,
            Message = message,
            Command = command
        };
}
