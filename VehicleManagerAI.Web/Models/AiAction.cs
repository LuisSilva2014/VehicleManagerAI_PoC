namespace VehicleManagerAI.Web.Models;

/// <summary>
/// CRUD-style operations the local AI agent is allowed to request.
/// Anything the model cannot map cleanly becomes <see cref="Unknown"/>.
/// </summary>
public enum AiAction
{
    Unknown = 0,
    Create,
    Update,
    Delete,
    Get,
    List
}
