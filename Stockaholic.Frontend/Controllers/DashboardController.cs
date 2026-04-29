using Microsoft.AspNetCore.Mvc;

namespace Stockaholic.Frontend.Controllers;

public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
