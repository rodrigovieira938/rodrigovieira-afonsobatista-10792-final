using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Stockaholic.Frontend.Models;

namespace Stockaholic.Frontend.Controllers;

public class LoginController : Controller
{
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
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request == null)
            return BadRequest("Missing credentials");

        var jwt = "TODO";

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, // prevents JS access (safer)
            Secure = true,   // only over HTTPS
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append("auth_token", jwt, cookieOptions);

        return Ok(new { success = true });
    }
}
