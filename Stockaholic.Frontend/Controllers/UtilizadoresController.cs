using Microsoft.AspNetCore.Mvc;
using Stockaholic.Frontend.Models;

namespace Stockaholic.Frontend.Controllers;

public class UtilizadoresController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<UtilizadoresController> _logger;

    public UtilizadoresController(IHttpClientFactory clientFactory, ILogger<UtilizadoresController> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var client = _clientFactory.CreateClient("ApiClient");

        try
        {
            var response = await client.GetAsync("utilizadores");
            response.EnsureSuccessStatusCode();

            var utilizadores = await response.Content.ReadFromJsonAsync<List<Utilizador>>();

            var viewDashboard = new ViewUtilizador
            {
                utilizadores = utilizadores ?? new List<Utilizador>()
            };

            return View(viewDashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching products");
            return StatusCode(500, "Internal Server Error");
        }
    }
    [HttpPatch("/utilizadores/{id}")]
    public async Task<IActionResult> Patch(int id, [FromBody] PatchUtilizador utilizador)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.PatchAsJsonAsync($"/utilizadores/{id}", utilizador);
        if(response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.NoContent) {
            return Ok();
        } else
        {
            _logger.LogError("Error updating product with id {Id}. Status code: {StatusCode}", id, response.StatusCode);
            return StatusCode((int)response.StatusCode, "Error updating product");
        }
    }
    [HttpPost("/utilizadores")]
    public async Task<IActionResult> Post([FromBody] CreateUtilizador utilizador)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.PostAsJsonAsync("/utilizadores", utilizador);
        if(response.StatusCode == System.Net.HttpStatusCode.Created) {
            return Ok();
        } else
        {
            _logger.LogError("Error creating product. Status code: {StatusCode}", response.StatusCode);
            return StatusCode((int)response.StatusCode, "Error creating product");
        }
    }
    [HttpDelete("/utilizadores/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.DeleteAsync($"/utilizadores/{id}");
        if(response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.NoContent) {
            return Ok();
        } else
        {
            _logger.LogError("Error deleting product with id {Id}. Status code: {StatusCode}", id, response.StatusCode);
            return StatusCode((int)response.StatusCode, "Error deleting product");
        }
    }
}
