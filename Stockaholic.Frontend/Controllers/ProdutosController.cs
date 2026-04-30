using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Stockaholic.Frontend.Models;

namespace Stockaholic.Frontend.Controllers;

public class ProdutosController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<ProdutosController> _logger;

    public ProdutosController(IHttpClientFactory clientFactory, ILogger<ProdutosController> logger)
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
            var response = await client.GetAsync("/movimentos/stock");
            response.EnsureSuccessStatusCode();
            var stock = await response.Content.ReadFromJsonAsync<List<ProdutoStock>>();

            response = await client.GetAsync("/categorias");
            response.EnsureSuccessStatusCode();
            var categorias = await response.Content.ReadFromJsonAsync<List<Categoria>>();

            var model = new ViewProdutos
            {
                Produtos = stock ?? new List<ProdutoStock>(),
                Categorias = categorias ?? new List<Categoria>()
            };
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching products");
            return StatusCode(500, "Internal Server Error");
        }
    }
    [HttpPut("/produtos/{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] CreateProduto produto)
    {
       string? token = Request.Cookies["auth_token"];
        if(token == null)
        {
            return Unauthorized();
        }
        var client = _clientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PutAsJsonAsync($"/produtos/{id}", produto);
        if(response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.NoContent) {
            return Ok();
        } else
        {
            _logger.LogError("Error updating product with id {Id}. Status code: {StatusCode}", id, response.StatusCode);
            return StatusCode((int)response.StatusCode, "Error updating product");
        }
    }
    [HttpPost("/produtos")]
    public async Task<IActionResult> Post([FromBody] CreateProduto produto)
    {
        string? token = Request.Cookies["auth_token"];
        if(token == null)
        {
            return Unauthorized();
        }
        var client = _clientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/produtos", produto);
        if(response.StatusCode == System.Net.HttpStatusCode.Created) {
            return Ok();
        } else
        {
            _logger.LogError("Error creating product. Status code: {StatusCode}", response.StatusCode);
            return StatusCode((int)response.StatusCode, "Error creating product");
        }
    }
    [HttpDelete("/produtos/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        string? token = Request.Cookies["auth_token"];
        if(token == null)
        {
            return Unauthorized();
        }
        var client = _clientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var response = await client.DeleteAsync($"/produtos/{id}");
        if(response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.NoContent) {
            return Ok();
        } else
        {
            _logger.LogError("Error deleting product with id {Id}. Status code: {StatusCode}", id, response.StatusCode);
            return StatusCode((int)response.StatusCode, "Error deleting product");
        }
    }
}
