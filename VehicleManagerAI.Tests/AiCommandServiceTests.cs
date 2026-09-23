using Moq;
using VehicleManagerAI.Web.Models;
using VehicleManagerAI.Web.Repositories;
using VehicleManagerAI.Web.Services;

namespace VehicleManagerAI.Tests;

public class AiCommandServiceTests
{
    private static (AiCommandService Sut, IVehicleService Vehicles) CreateSut(AiCommand command)
    {
        var ai = new Mock<IAiAgentRepository>();
        ai.Setup(a => a.InterpretAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(command);

        var vehicles = new VehicleService(new InMemoryVehicleRepository());
        return (new AiCommandService(ai.Object, vehicles), vehicles);
    }

    [Fact]
    public async Task ProcessAsync_CreateCommand_AddsVehicle()
    {
        var (sut, vehicles) = CreateSut(new AiCommand
        {
            Action = "create",
            VehicleId = 123,
            Make = "Toyota",
            Model = "Corolla",
            Year = 2024
        });

        var result = await sut.ProcessAsync("Add a new vehicle with ID 123, Toyota Corolla, 2024.");

        Assert.True(result.Success);
        Assert.Equal(AiAction.Create, result.Command.GetAction());
        Assert.Equal("Corolla", vehicles.GetById(123)?.Model);
    }

    [Fact]
    public async Task ProcessAsync_UpdateCommand_ChangesModel()
    {
        var (sut, vehicles) = CreateSut(new AiCommand
        {
            Action = "update",
            VehicleId = 103,
            Model = "Camry"
        });

        var result = await sut.ProcessAsync("Update vehicle 103 and change the model to Camry.");

        Assert.True(result.Success);
        Assert.Equal("Camry", vehicles.GetById(103)?.Model);
        Assert.Equal("Toyota", vehicles.GetById(103)?.Make);
    }

    [Fact]
    public async Task ProcessAsync_DeleteCommand_RemovesVehicle()
    {
        var (sut, vehicles) = CreateSut(new AiCommand
        {
            Action = "delete",
            VehicleId = 101
        });

        var result = await sut.ProcessAsync("Remove vehicle with ID 101.");

        Assert.True(result.Success);
        Assert.Null(vehicles.GetById(101));
    }

    [Fact]
    public async Task ProcessAsync_ListCommand_ReturnsInventory()
    {
        var (sut, _) = CreateSut(new AiCommand { Action = "list" });

        var result = await sut.ProcessAsync("Show all vehicles.");

        Assert.True(result.Success);
        Assert.NotEmpty(result.Vehicles);
        Assert.Contains("Honda", result.Message);
    }
}
