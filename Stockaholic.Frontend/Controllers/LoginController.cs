using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Stockaholic.Frontend.Models;

namespace Stockaholic.Frontend.Controllers;

public class LoginController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<LoginController> _logger;
    public LoginController(IHttpClientFactory clientFactory, ILogger<LoginController> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }
    // POST /login
    [HttpPost]
    [AllowAnonymous]
    [Route("/login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var client = _clientFactory.CreateClient("ApiClient");

        try
        {
            var response = await client.PostAsJsonAsync("/auth/login", request);
            response.EnsureSuccessStatusCode();
            var token = await response.Content.ReadFromJsonAsync<LoginResult>();
            if(token == null)
            {
                return Unauthorized();
            }
            if(token.Token == null)
            {
                return Unauthorized();
            }
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // prevents JS access (safer)
                Secure = true,   // only over HTTPS
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("auth_token", token.Token, cookieOptions);
            _logger.LogInformation("Login successful! email={} token={}", request.email, token.Token??"");
            return Ok(new {sucess = true});
        } catch (Exception ex)
        {
            _logger.LogError(ex, "Failed attempted login attempt! email={}", request.email);
            return Unauthorized();
        }
    }
}
