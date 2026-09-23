namespace VehicleManagerAI.Web.Models;

/// <summary>
/// Simple success/failure wrapper used by the vehicle service.
/// Chosen over exceptions for expected cases such as "id already exists".
/// </summary>
public class OperationResult<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Value { get; init; }

    public static OperationResult<T> Ok(T value, string message) =>
        new() { Success = true, Value = value, Message = message };

    public static OperationResult<T> Fail(string message) =>
        new() { Success = false, Message = message };
}
