using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VehicleManagerAI.Web.Models;
using VehicleManagerAI.Web.Services;

namespace VehicleManagerAI.Web.Controllers;

public class HomeController : Controller
{
    private readonly IVehicleService _vehicles;

    public HomeController(IVehicleService vehicles)
    {
        _vehicles = vehicles;
    }

    public IActionResult Index()
    {
        ViewBag.VehicleCount = _vehicles.GetAll().Count;
        return View();
    }

    public IActionResult HowItWorks()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
