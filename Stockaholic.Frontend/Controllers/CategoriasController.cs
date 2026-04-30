using System.Net;
using System.Net.Http.Headers;
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

    public async Task<IActionResult> Index()
    {
        string? token = Request.Cookies["auth_token"];
        if(token == null)
        {
            return RedirectToAction("Index", "Login");
        }
        var client = _clientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        try
        {
            var _response = await client.GetAsync("auth/valid");
            if(_response.StatusCode != HttpStatusCode.OK)
            {
                return RedirectToAction("Index", "Login");
            }
        } catch(Exception)
        {
            return RedirectToAction("Index", "Login");
        }
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

    [HttpPut("/categorias/{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Categoria categoria)
    {
        string? token = Request.Cookies["auth_token"];
        if(token == null)
        {
            return Unauthorized();
        }
        var client = _clientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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
        string? token = Request.Cookies["auth_token"];
        if(token == null)
        {
            return Unauthorized();
        }
        var client = _clientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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
        string? token = Request.Cookies["auth_token"];
        if(token == null)
        {
            return Unauthorized();
        }
        var client = _clientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.DeleteAsync($"/categorias/{id}");
        if(response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.NoContent) {
            return Ok();
        } else
        {
            _logger.LogError("Error deleting product with id {Id}. Status code: {StatusCode}", id, response.StatusCode);
            return StatusCode((int)response.StatusCode, "Error deleting product");
        }
    }
}
