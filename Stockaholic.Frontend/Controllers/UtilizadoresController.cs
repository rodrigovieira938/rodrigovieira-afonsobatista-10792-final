using Microsoft.AspNetCore.Mvc;

namespace Stockaholic.Frontend.Controllers;

public class UtilizadoresController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
