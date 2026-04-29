using Microsoft.AspNetCore.Mvc;
using Stockaholic.Frontend.Models;

namespace Stockaholic.Frontend.Controllers;

public class CategoriasController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<CategoriasController> _logger;
    
    public CategoriasController(IHttpClientFactory clientFactory, ILogger<CategoriasController> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }

    [HttpPut("/categorias/{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Categoria categoria)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.PutAsJsonAsync($"/categorias/{id}", categoria);
        if(response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.NoContent) {
            return Ok();
        } else
        {
            _logger.LogError("Error updating categoria with id {Id}. Status code: {StatusCode}", id, response.StatusCode);
            return StatusCode((int)response.StatusCode, "Error updating product");
        }
    }
    [HttpPost("/categorias")]
    public async Task<IActionResult> Post([FromBody] CreateCategoria categoria)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.PostAsJsonAsync("/categorias", categoria);
        if(response.StatusCode == System.Net.HttpStatusCode.Created) {
            return Ok();
        } else
        {
            _logger.LogError("Error creating categoria. Status code: {StatusCode}", response.StatusCode);
            return StatusCode((int)response.StatusCode, "Error creating categoria1");
        }
    }
    [HttpDelete("/categorias/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.DeleteAsync($"/categorias/{id}");
        if(response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.NoContent) {
            return Ok();
        } else
        {
            _logger.LogError("Error deleting product with id {Id}. Status code: {StatusCode}", id, response.StatusCode);
            return StatusCode((int)response.StatusCode, "Error deleting product");
        }
    }
    public async Task<IActionResult> Index()
    {
        var client = _clientFactory.CreateClient("ApiClient");
        try
        {
            var response = await client.GetAsync("/categorias");
            response.EnsureSuccessStatusCode();
            var categorias = await response.Content.ReadFromJsonAsync<List<Categoria>>() ?? new List<Categoria>();

            var model = new ViewCategorias
            {
                categorias = categorias
            };
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching products");
            return StatusCode(500, "Internal Server Error");
        }
    }
}
