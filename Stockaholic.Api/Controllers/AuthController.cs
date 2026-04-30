using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stockaholic.API.Auth;
using Stockaholic.API.Data;
using Stockaholic.API.Models;

namespace Stockaholic.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly StockaholicDbContext _context;
        private readonly TokenService _tokenService;
        public AuthController(StockaholicDbContext context, TokenService tokenService)
        {
            // Store the context in a private field for later use
            _context = context;
            _tokenService = tokenService;
        }
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(LoginResult))]
        public ActionResult<LoginResult> Login([FromBody] LoginInput input)
        {
            var utilizador = _context.Utilizadores.FirstOrDefault(u => u.Email == input.Email);
            if (utilizador == null)
                return Unauthorized();

            var salt = Convert.FromBase64String(utilizador.PasswordSalt);
            var hash = Rfc2898DeriveBytes.Pbkdf2(input.Password, salt, 10000, HashAlgorithmName.SHA256, 32);
            if (Convert.ToBase64String(hash) != utilizador.PasswordHash)
                return Unauthorized();
            var token = _tokenService.GenerateToken(utilizador.Id.ToString(), utilizador.Email);
            return Ok(new LoginResult { Token = token });
        }
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(MeResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type=typeof(UnauthorizedResult))]
        public ActionResult<MeResult> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);

            if(userId == null || email == null)
                return Unauthorized();

            return Ok(new MeResult{ Id=int.Parse(userId), Name=_context.Utilizadores.FirstOrDefault(u => u.Email == email).Nome, Email=email??"" });
        }
        [HttpGet("valid")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Valid()
        {
            return Ok();
        }
    
    }
}