using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Stockaholic.Frontend.Models;

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
        string? token = Request.Cookies["auth_token"];
        if(token == null)
        {
            return RedirectToAction("Index", "Login");
        }
        var client = _clientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        try
        {
            var response = await client.GetAsync("auth/valid");
            response.EnsureSuccessStatusCode();
            if(response.StatusCode != HttpStatusCode.OK)
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

            response = await client.GetAsync("produtos/numero");
            response.EnsureSuccessStatusCode();

            var total_produtos = await response.Content.ReadFromJsonAsync<int>();

            response = await client.GetAsync("produtos/valor");
            response.EnsureSuccessStatusCode();
            var total_valor = await response.Content.ReadFromJsonAsync<float>();

            response = await client.GetAsync("movimentos/hoje");
            response.EnsureSuccessStatusCode();
            var movimentos_hoje = await response.Content.ReadFromJsonAsync<List<Movimento>>();            

            response = await client.GetAsync("movimentos/recentes");
            response.EnsureSuccessStatusCode();
            var movimentos_recentes = await response.Content.ReadFromJsonAsync<List<Movimento>>();

            response = await client.GetAsync("movimentos/top-valor");
            response.EnsureSuccessStatusCode();
            var movimentos_top_valor = await response.Content.ReadFromJsonAsync<List<MovimentoTopValor>>();

            var viewDashboard = new ViewDashboard
            {
                total_produtos = total_produtos,
                total_valor = total_valor,
                movimentos_hoje = movimentos_hoje?.Count ?? 0,
                movimentos_recentes = movimentos_recentes ?? new List<Movimento>(),
                movimentos_top_valor = movimentos_top_valor ?? new List<MovimentoTopValor>()
            };

            return View(viewDashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching products");
            return StatusCode(500, "Internal Server Error");
        }
    }
}
