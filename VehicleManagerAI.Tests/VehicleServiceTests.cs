using VehicleManagerAI.Web.Models;
using VehicleManagerAI.Web.Repositories;
using VehicleManagerAI.Web.Services;

namespace VehicleManagerAI.Tests;

public class VehicleServiceTests
{
    private static VehicleService CreateSut() =>
        new(new InMemoryVehicleRepository());

    [Fact]
    public void Create_AddsVehicleToInventory()
    {
        var sut = CreateSut();
        var vehicle = new Vehicle { Id = 123, Make = "Toyota", Model = "Corolla", Year = 2024 };

        var result = sut.Create(vehicle);

        Assert.True(result.Success);
        var stored = sut.GetById(123);
        Assert.NotNull(stored);
        Assert.Equal("Corolla", stored!.Model);
        Assert.Equal(2024, stored.Year);
    }

    [Fact]
    public void GetById_ReturnsExistingVehicle()
    {
        var sut = CreateSut();

        var civic = sut.GetById(101);

        Assert.NotNull(civic);
        Assert.Equal("Honda", civic!.Make);
        Assert.Equal("Civic", civic.Model);
    }

    [Fact]
    public void Update_ChangesModelOnExistingVehicle()
    {
        var sut = CreateSut();
        var result = sut.Update(new Vehicle { Id = 101, Make = "Honda", Model = "Accord", Year = 2022 });

        Assert.True(result.Success);
        Assert.Equal("Accord", sut.GetById(101)!.Model);
    }

    [Fact]
    public void Delete_RemovesVehicle()
    {
        var sut = CreateSut();

        var result = sut.Delete(102);

        Assert.True(result.Success);
        Assert.Null(sut.GetById(102));
        Assert.DoesNotContain(sut.GetAll(), v => v.Id == 102);
    }

    [Fact]
    public void Create_RejectsDuplicateId()
    {
        var sut = CreateSut();

        var result = sut.Create(new Vehicle { Id = 101, Make = "Toyota", Model = "Corolla", Year = 2024 });

        Assert.False(result.Success);
        Assert.Contains("already exists", result.Message, StringComparison.OrdinalIgnoreCase);
    }
}
