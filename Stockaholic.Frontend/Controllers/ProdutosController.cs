using Microsoft.AspNetCore.Mvc;

namespace Stockaholic.Frontend.Controllers;

public class ProdutosController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
