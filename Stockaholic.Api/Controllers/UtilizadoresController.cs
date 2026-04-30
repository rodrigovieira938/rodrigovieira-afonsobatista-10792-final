using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stockaholic.API.Data;
using Stockaholic.API.Models;
using System.Security.Cryptography;

namespace Stockaholic.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UtilizadoresController : ControllerBase
    {
        private readonly StockaholicDbContext _context;
        public UtilizadoresController(StockaholicDbContext context)
        {
            // Store the context in a private field for later use
            _context = context;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(IEnumerable<Utilizador>))]
        public ActionResult<IEnumerable<Utilizador>> Get()
        {
            return Ok(_context.Utilizadores.ToList());
        }
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(Utilizador))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(NotFoundResult))]
        public ActionResult<Utilizador> Get(int id)
        {
            var utilizador = _context.Utilizadores.Find(id);
            if (utilizador == null)
            {
                return NotFound();
            }
            return Ok(utilizador);
        }
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created, Type=typeof(Utilizador))]
        public ActionResult<Utilizador> Post([FromBody] CreateUtilizador createUtilizador)
        {
            // Generate salt
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            // Hash password
            var hash = Rfc2898DeriveBytes.Pbkdf2(createUtilizador.Password, salt, 10000, HashAlgorithmName.SHA256, 32);
            var utilizador = new Utilizador
            {
                Nome = createUtilizador.Nome,
                Email = createUtilizador.Email,
                PasswordHash = Convert.ToBase64String(hash),
                PasswordSalt = Convert.ToBase64String(salt)
            };
            _context.Utilizadores.Add(utilizador);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = utilizador.Id }, utilizador);
        }
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type=typeof(NoContentResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type=typeof(BadRequestResult))]
        public ActionResult<Utilizador> Put(int id, [FromBody] Utilizador utilizador)
        {
            if (id != utilizador.Id)
            {
                return BadRequest("O ID da URL deve ser igual ao ID do corpo da requisição.");
            }
            _context.Entry(utilizador).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }
        [HttpPatch("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type=typeof(NoContentResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(NotFoundResult))]
        public ActionResult<Utilizador> Patch(int id, [FromBody] UtilizadorPatch input)
        {
            var utilizador = _context.Utilizadores.Find(id);
            if (utilizador == null)
                return NotFound();

            if (input.Nome != null)
                utilizador.Nome = input.Nome;
            if (input.Email != null)
                utilizador.Email = input.Email;
            if (input.PasswordHash != null)
                utilizador.PasswordHash = input.PasswordHash;
            if (input.PasswordSalt != null)
                utilizador.PasswordSalt = input.PasswordSalt;

            _context.Entry(utilizador).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type=typeof(NoContentResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(NotFoundResult))]
        public ActionResult<Utilizador> Delete(int id)
        {
            var utilizador = _context.Utilizadores.Find(id);
            if (utilizador == null)
                return NotFound();

            _context.Utilizadores.Remove(utilizador);
            _context.SaveChanges();
            return NoContent();
        }
    }
}