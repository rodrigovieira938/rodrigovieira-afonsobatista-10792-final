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
    [HttpPut("/produtos/{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] CreateProduto produto)
    {
        var client = _clientFactory.CreateClient("ApiClient");
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
        var client = _clientFactory.CreateClient("ApiClient");
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
        var client = _clientFactory.CreateClient("ApiClient");
        var response = await client.DeleteAsync($"/produtos/{id}");
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
}
