using Microsoft.AspNetCore.Mvc;

namespace Stockaholic.Frontend.Controllers;

public class MovimentosController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
