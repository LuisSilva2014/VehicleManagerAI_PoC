namespace VehicleManagerAI.Web.Models;

/// <summary>
/// One bubble in the Blazor chat transcript.
/// </summary>
public class ChatTurn
{
    public string Role { get; init; } = "assistant";
    public string Text { get; init; } = string.Empty;
    public bool IsError { get; init; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.Now;
}
