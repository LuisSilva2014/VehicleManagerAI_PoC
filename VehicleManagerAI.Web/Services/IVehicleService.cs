using VehicleManagerAI.Web.Models;

namespace VehicleManagerAI.Web.Services;

/// <summary>
/// Application business rules for vehicle CRUD. Both MVC controllers and the
/// AI command processor call this layer — never the repository directly.
/// </summary>
public interface IVehicleService
{
    IReadOnlyList<Vehicle> GetAll();
    Vehicle? GetById(int id);
    OperationResult<Vehicle> Create(Vehicle vehicle);
    OperationResult<Vehicle> Update(Vehicle vehicle);
    OperationResult<Vehicle> Delete(int id);
}
