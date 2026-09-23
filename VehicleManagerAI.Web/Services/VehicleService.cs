using VehicleManagerAI.Web.Models;
using VehicleManagerAI.Web.Repositories;

namespace VehicleManagerAI.Web.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _repository;

    public VehicleService(IVehicleRepository repository)
    {
        _repository = repository;
    }

    public IReadOnlyList<Vehicle> GetAll() => _repository.GetAll();

    public Vehicle? GetById(int id) => _repository.GetById(id);

    public OperationResult<Vehicle> Create(Vehicle vehicle)
    {
        var errors = Validate(vehicle, requireId: vehicle.Id != 0);
        if (errors.Count > 0)
        {
            return OperationResult<Vehicle>.Fail(string.Join(" ", errors));
        }

        if (vehicle.Id == 0)
        {
            vehicle.Id = _repository.GetNextId();
        }

        if (_repository.Exists(vehicle.Id))
        {
            return OperationResult<Vehicle>.Fail($"A vehicle with ID {vehicle.Id} already exists.");
        }

        _repository.Add(vehicle);
        return OperationResult<Vehicle>.Ok(vehicle, $"Created {vehicle}.");
    }

    public OperationResult<Vehicle> Update(Vehicle vehicle)
    {
        var errors = Validate(vehicle, requireId: true);
        if (errors.Count > 0)
        {
            return OperationResult<Vehicle>.Fail(string.Join(" ", errors));
        }

        if (!_repository.Exists(vehicle.Id))
        {
            return OperationResult<Vehicle>.Fail($"Vehicle {vehicle.Id} was not found.");
        }

        _repository.Update(vehicle);
        return OperationResult<Vehicle>.Ok(vehicle, $"Updated {vehicle}.");
    }

    public OperationResult<Vehicle> Delete(int id)
    {
        if (id <= 0)
        {
            return OperationResult<Vehicle>.Fail("A positive vehicle ID is required to delete.");
        }

        var existing = _repository.GetById(id);
        if (existing is null || !_repository.Delete(id))
        {
            return OperationResult<Vehicle>.Fail($"Vehicle {id} was not found.");
        }

        return OperationResult<Vehicle>.Ok(existing, $"Removed {existing}.");
    }

    private static List<string> Validate(Vehicle vehicle, bool requireId)
    {
        var errors = new List<string>();

        if (requireId && vehicle.Id <= 0)
        {
            errors.Add("A positive vehicle ID is required.");
        }

        if (string.IsNullOrWhiteSpace(vehicle.Make))
        {
            errors.Add("Make is required.");
        }

        if (string.IsNullOrWhiteSpace(vehicle.Model))
        {
            errors.Add("Model is required.");
        }

        if (vehicle.Year is < 1900 or > 2100)
        {
            errors.Add("Year must be between 1900 and 2100.");
        }

        return errors;
    }
}
