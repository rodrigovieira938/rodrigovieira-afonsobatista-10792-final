using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stockaholic.API.Cache;
using Stockaholic.API.Data;
using Stockaholic.API.Models;

namespace Stockaholic.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly StockaholicDbContext _context;
        private readonly CacheService _cacheService;
        public CategoriasController(StockaholicDbContext context, CacheService cacheService)
        {
            // Store the context in a private field for later use
            _context = context;
            _cacheService = cacheService;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(IEnumerable<Categoria>))]
        public async Task<ActionResult<IEnumerable<Categoria>>> Get()
        {
            var categorias = await _cacheService.GetOrSetAsync(
                "categorias:all",
                async () => _context.Produtos.ToList(),
                TimeSpan.FromMinutes(5)
            );
            return Ok(_context.Categorias.ToList());
        }
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(Categoria))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(NotFoundResult))]
        public async Task<ActionResult<Categoria>> Get(int id)
        {
            var categoria = await _cacheService.GetOrSetAsync(
                $"categorias:{id}",
                async () => _context.Categorias.Find(id) ?? default,
                TimeSpan.FromMinutes(5)
            );
            if (categoria == null)
            {
                return NotFound();
            }
            return Ok(categoria);
        }
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created, Type=typeof(Categoria))]
        public ActionResult<Categoria> Post([FromBody] CreateCategoria createCategoria)
        {
            _cacheService.Remove("categorias:all");
            var categoria = new Categoria
            {
                Nome = createCategoria.Nome
            };
            _context.Categorias.Add(categoria);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = categoria.Id }, categoria);
        }
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type=typeof(NoContentResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type=typeof(BadRequestResult))]
        public ActionResult<Categoria> Put(int id, [FromBody] Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return BadRequest("O ID da URL deve ser igual ao ID do corpo da requisição.");
            }
            _cacheService.Remove("categorias:all");
            _cacheService.Remove($"categorias:{id}");
            _context.Entry(categoria).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }
        [HttpPatch("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type=typeof(NoContentResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(NotFoundResult))]
        public ActionResult<Categoria> Patch(int id, [FromBody] CategoriaPatch input)
        {
            var categoria = _context.Categorias.Find(id);
            if (categoria == null)
                return NotFound();

            if (input.Nome != null)
                categoria.Nome = input.Nome;
            _cacheService.Remove("categorias:all");
            _cacheService.Remove($"categorias:{id}");
            _context.Entry(categoria).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type=typeof(NoContentResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(NotFoundResult))]
        public ActionResult<Categoria> Delete(int id)
        {
            var categoria = _context.Categorias.Find(id);
            if (categoria == null)
                return NotFound();
            _cacheService.Remove("categorias:all");
            _cacheService.Remove($"categorias:{id}");
            _context.Categorias.Remove(categoria);
            _context.SaveChanges();
            return NoContent();
        }
    }
}