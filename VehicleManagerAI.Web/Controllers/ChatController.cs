using Microsoft.AspNetCore.Mvc;

namespace VehicleManagerAI.Web.Controllers;

/// <summary>
/// Hosts the Blazor chat component. Interactive chat state lives in the component;
/// this controller only serves the MVC page shell.
/// </summary>
public class ChatController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
