using System.Collections.Concurrent;
using VehicleManagerAI.Web.Models;

namespace VehicleManagerAI.Web.Repositories;

/// <summary>
/// Thread-safe in-memory store. Registered as a singleton so MVC pages and the
/// Blazor chat circuit share the same vehicle list during the app lifetime.
/// </summary>
public class InMemoryVehicleRepository : IVehicleRepository
{
    private readonly ConcurrentDictionary<int, Vehicle> _vehicles = new();
    private int _nextId = 104;

    public InMemoryVehicleRepository()
    {
        Add(new Vehicle { Id = 101, Make = "Honda", Model = "Civic", Year = 2022 });
        Add(new Vehicle { Id = 102, Make = "Ford", Model = "Ranger", Year = 2021 });
        Add(new Vehicle { Id = 103, Make = "Toyota", Model = "Corolla", Year = 2023 });
    }

    public IReadOnlyList<Vehicle> GetAll() =>
        _vehicles.Values.OrderBy(v => v.Id).Select(Clone).ToList();

    public Vehicle? GetById(int id) =>
        _vehicles.TryGetValue(id, out var vehicle) ? Clone(vehicle) : null;

    public void Add(Vehicle vehicle)
    {
        var stored = Clone(vehicle);
        if (!_vehicles.TryAdd(stored.Id, stored))
        {
            throw new InvalidOperationException($"Vehicle {stored.Id} already exists.");
        }

        Interlocked.Exchange(ref _nextId, Math.Max(_nextId, stored.Id + 1));
    }

    public void Update(Vehicle vehicle)
    {
        if (!_vehicles.ContainsKey(vehicle.Id))
        {
            throw new KeyNotFoundException($"Vehicle {vehicle.Id} was not found.");
        }

        _vehicles[vehicle.Id] = Clone(vehicle);
    }

    public bool Delete(int id) => _vehicles.TryRemove(id, out _);

    public bool Exists(int id) => _vehicles.ContainsKey(id);

    public int GetNextId() => _nextId;

    private static Vehicle Clone(Vehicle vehicle) => new()
    {
        Id = vehicle.Id,
        Make = vehicle.Make,
        Model = vehicle.Model,
        Year = vehicle.Year
    };
}
