using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Stockaholic.Frontend.Models;

namespace Stockaholic.Frontend.Controllers;

public class MovimentosController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<MovimentosController> _logger;

    public MovimentosController(IHttpClientFactory clientFactory, ILogger<MovimentosController> logger)
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
            var response = await client.GetAsync("/movimentos");
            response.EnsureSuccessStatusCode();
            var movimentos = (await response.Content.ReadFromJsonAsync<List<Movimento>>()) ?? new List<Movimento>();
            
            response = await client.GetAsync("/produtos");
            response.EnsureSuccessStatusCode();
            var produtos = (await response.Content.ReadFromJsonAsync<List<Produto>>()) ?? new List<Produto>();

            response = await client.GetAsync("/utilizadores");
            response.EnsureSuccessStatusCode();
            var utilizadores = (await response.Content.ReadFromJsonAsync<List<Utilizador>>()) ?? new List<Utilizador>();

            List<ViewMovimento> viewMovimentos = movimentos
            .Join(produtos,
                m => m.produtoId,
                p => p.Id,
                (m, p) => new { m, p })
            .Join(utilizadores,
                mp => mp.m.utilizadorId,
                u => u.Id,
                (mp, u) => new ViewMovimento
                {
                    id = mp.m.id,
                    nome = mp.m.nome,
                    descricao = mp.m.descricao,
                    produto = mp.p.Nome,
                    delta = mp.m.delta,
                    timestamp = mp.m.timestamp,
                    user = u.Nome
                })
            .ToList();

            var viewDashboard = new ViewMovimentos
            {
                movimentos = viewMovimentos,
                produtos = produtos
            };

            return View(viewDashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching products");
            return StatusCode(500, "Internal Server Error");
        }
    }
    [HttpPut("/movimentos/{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Movimento movimento)
    {
        string? token = Request.Cookies["auth_token"];
        if(token == null)
        {
            return Unauthorized();
        }
        var client = _clientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        try
        {
            var _response = await client.GetAsync("/auth/me");
            _response.EnsureSuccessStatusCode();
            var _me = await _response.Content.ReadFromJsonAsync<MeResult>();
            movimento.utilizadorId = _me.Id;
        } catch
        {
            return Unauthorized();
        }

        var response = await client.GetAsync("auth/me");
        response.EnsureSuccessStatusCode();
        var me = await response.Content.ReadFromJsonAsync<MeResult>();

        if(me == null)
        {
            return Unauthorized();
        }
        ViewData["FullName"] = me.Name;
        ViewData["Initials"] = Utils.Initials(me.Name);
        ViewData["Email"] = me.Email;
        response = await client.PutAsJsonAsync($"/movimentos/{id}", movimento);
        if(response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.NoContent) {
            return Ok();
        } else
        {
            _logger.LogError("Error updating product with id {Id}. Status code: {StatusCode}", id, response.StatusCode);
            return StatusCode((int)response.StatusCode, "Error updating product");
        }
    }
    [HttpPost("/movimentos")]
    public async Task<IActionResult> Post([FromBody] CreateMovimento movimento)
    {
        string? token = Request.Cookies["auth_token"];
        if(token == null)
        {
            return Unauthorized();
        }
        var client = _clientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        try
        {
            var _response = await client.GetAsync("/auth/me");
            _response.EnsureSuccessStatusCode();
            var me = await _response.Content.ReadFromJsonAsync<MeResult>();
            movimento.UtilizadorId = me.Id;
        } catch
        {
            return Unauthorized();
        }

        var response = await client.PostAsJsonAsync("/movimentos", movimento);
        if(response.StatusCode == System.Net.HttpStatusCode.Created) {
            return Ok();
        } else
        {
            _logger.LogError("Error creating product. Status code: {StatusCode}", response.StatusCode);
            return StatusCode((int)response.StatusCode, "Error creating product");
        }
    }
    [HttpDelete("/movimentos/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        string? token = Request.Cookies["auth_token"];
        if(token == null)
        {
            return Unauthorized();
        }
        var client = _clientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var response = await client.DeleteAsync($"/movimentos/{id}");
        if(response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.NoContent) {
            return Ok();
        } else
        {
            _logger.LogError("Error deleting product with id {Id}. Status code: {StatusCode}", id, response.StatusCode);
            return StatusCode((int)response.StatusCode, "Error deleting product");
        }
    }
}
