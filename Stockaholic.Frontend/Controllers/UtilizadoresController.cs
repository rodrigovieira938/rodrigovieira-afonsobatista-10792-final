using System.Net;
using System.Net.Http.Headers;
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
            response = await client.GetAsync("utilizadores");
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
        string? token = Request.Cookies["auth_token"];
        if(token == null)
        {
            return Unauthorized();
        }
        var client = _clientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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
        string? token = Request.Cookies["auth_token"];
        if(token == null)
        {
            return Unauthorized();
        }
        var client = _clientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
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
        string? token = Request.Cookies["auth_token"];
        if(token == null)
        {
            return Unauthorized();
        }
        var client = _clientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
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
