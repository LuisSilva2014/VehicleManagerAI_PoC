using Microsoft.AspNetCore.Mvc;
using VehicleManagerAI.Web.Models;
using VehicleManagerAI.Web.Services;

namespace VehicleManagerAI.Web.Controllers;

/// <summary>
/// Traditional MVC CRUD. Compare this controller with <see cref="ChatController"/>:
/// both call <see cref="IVehicleService"/>, only the input channel is different.
/// </summary>
public class VehiclesController : Controller
{
    private readonly IVehicleService _vehicles;

    public VehiclesController(IVehicleService vehicles)
    {
        _vehicles = vehicles;
    }

    public IActionResult Index()
    {
        return View(_vehicles.GetAll());
    }

    public IActionResult Details(int id)
    {
        var vehicle = _vehicles.GetById(id);
        return vehicle is null ? NotFound() : View(vehicle);
    }

    public IActionResult Create()
    {
        return View(new Vehicle());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Vehicle vehicle)
    {
        if (!ModelState.IsValid)
        {
            return View(vehicle);
        }

        var result = _vehicles.Create(vehicle);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(vehicle);
        }

        TempData["Status"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var vehicle = _vehicles.GetById(id);
        return vehicle is null ? NotFound() : View(vehicle);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Vehicle vehicle)
    {
        if (id != vehicle.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(vehicle);
        }

        var result = _vehicles.Update(vehicle);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(vehicle);
        }

        TempData["Status"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int id)
    {
        var vehicle = _vehicles.GetById(id);
        return vehicle is null ? NotFound() : View(vehicle);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var result = _vehicles.Delete(id);
        if (!result.Success)
        {
            return NotFound();
        }

        TempData["Status"] = result.Message;
        return RedirectToAction(nameof(Index));
    }
}
