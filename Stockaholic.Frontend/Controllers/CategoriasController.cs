using Microsoft.AspNetCore.Mvc;

namespace Stockaholic.Frontend.Controllers;

public class CategoriasController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
