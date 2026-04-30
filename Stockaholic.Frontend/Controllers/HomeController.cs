using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace Stockaholic.Frontend.Controllers;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    public HomeController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
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
            if(response.StatusCode != HttpStatusCode.OK)
            {
                return RedirectToAction("Index", "Login");
            }
            return RedirectToAction("Index", "Dashboard");
        } catch(Exception)
        {
            return RedirectToAction("Index", "Login");
        }
    }
}
