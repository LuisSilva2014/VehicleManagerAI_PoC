using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using VehicleManagerAI.Web.Controllers;
using VehicleManagerAI.Web.Models;
using VehicleManagerAI.Web.Services;

namespace VehicleManagerAI.Tests;

public class VehiclesControllerTests
{
    private static VehiclesController CreateController(IVehicleService service)
    {
        return new VehiclesController(service)
        {
            TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
        };
    }

    [Fact]
    public void Index_ReturnsAllVehicles()
    {
        var vehicles = new List<Vehicle>
        {
            new() { Id = 1, Make = "Toyota", Model = "Corolla", Year = 2024 }
        };
        var service = new Mock<IVehicleService>();
        service.Setup(s => s.GetAll()).Returns(vehicles);
        var controller = CreateController(service.Object);

        var result = Assert.IsType<ViewResult>(controller.Index());

        Assert.Same(vehicles, result.Model);
    }

    [Fact]
    public void Create_Post_RedirectsWhenServiceSucceeds()
    {
        var vehicle = new Vehicle { Id = 123, Make = "Toyota", Model = "Corolla", Year = 2024 };
        var service = new Mock<IVehicleService>();
        service.Setup(s => s.Create(It.IsAny<Vehicle>()))
            .Returns(OperationResult<Vehicle>.Ok(vehicle, "Created"));
        var controller = CreateController(service.Object);

        var result = Assert.IsType<RedirectToActionResult>(controller.Create(vehicle));

        Assert.Equal(nameof(VehiclesController.Index), result.ActionName);
        service.Verify(s => s.Create(It.Is<Vehicle>(v => v.Id == 123)), Times.Once);
    }

    [Fact]
    public void Edit_Post_ReturnsViewWhenVehicleIsMissing()
    {
        var vehicle = new Vehicle { Id = 999, Make = "Ford", Model = "Focus", Year = 2020 };
        var service = new Mock<IVehicleService>();
        service.Setup(s => s.Update(It.IsAny<Vehicle>()))
            .Returns(OperationResult<Vehicle>.Fail("Vehicle 999 was not found."));
        var controller = CreateController(service.Object);

        var result = Assert.IsType<ViewResult>(controller.Edit(999, vehicle));

        Assert.Same(vehicle, result.Model);
        Assert.False(controller.ModelState.IsValid);
    }

    [Fact]
    public void DeleteConfirmed_RemovesVehicleAndRedirects()
    {
        var service = new Mock<IVehicleService>();
        service.Setup(s => s.Delete(123))
            .Returns(OperationResult<Vehicle>.Ok(new Vehicle { Id = 123 }, "Removed"));
        var controller = CreateController(service.Object);

        var result = Assert.IsType<RedirectToActionResult>(controller.DeleteConfirmed(123));

        Assert.Equal(nameof(VehiclesController.Index), result.ActionName);
        service.Verify(s => s.Delete(123), Times.Once);
    }
}
