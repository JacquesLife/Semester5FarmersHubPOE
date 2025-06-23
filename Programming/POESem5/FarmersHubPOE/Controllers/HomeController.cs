/// <summary>
/// This is pretty much the default controller that is created when you create a new ASP.NET Core MVC project.
/// It is the main controller for the application and handles the home page and privacy page.


using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Programming3A.Models;

namespace Programming3A.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


//---------------------------------------------------End of File-----------------------------------------------------