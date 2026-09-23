using VehicleManagerAI.Web.Models;

namespace VehicleManagerAI.Web.Repositories;

/// <summary>
/// Persistence for vehicles. This PoC uses an in-memory implementation so the
/// sample can run fully offline without a database.
/// </summary>
public interface IVehicleRepository
{
    IReadOnlyList<Vehicle> GetAll();
    Vehicle? GetById(int id);
    void Add(Vehicle vehicle);
    void Update(Vehicle vehicle);
    bool Delete(int id);
    bool Exists(int id);
    int GetNextId();
}
