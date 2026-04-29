using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Stockaholic.Frontend.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
