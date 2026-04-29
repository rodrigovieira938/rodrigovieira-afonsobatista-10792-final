using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Stockaholic.Frontend.Controllers;

public class DashboardController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IHttpClientFactory clientFactory, ILogger<DashboardController> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var client = _clientFactory.CreateClient("ApiClient");

        try
        {
            //Just dummy call to test resilience policies. Replace with actual API call and model binding as needed.
            var response = await client.GetAsync("produtos");
            response.EnsureSuccessStatusCode();

            var products = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Produtos: {}", products);
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching products");
            return StatusCode(500, "Internal Server Error");
        }
    }
}
