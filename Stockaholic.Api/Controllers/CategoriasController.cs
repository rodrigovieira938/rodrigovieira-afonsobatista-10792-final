using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stockaholic.API.Data;
using Stockaholic.API.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace Stockaholic.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly StockaholicDbContext _context;
        public CategoriasController(StockaholicDbContext context)
        {
            // Store the context in a private field for later use
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Categoria>> Get()
        {
            return Ok(_context.Categorias.ToList());
        }
        [HttpGet("{id}")]
        public ActionResult<Categoria> Get(int id)
        {
            var categoria = _context.Categorias.Find(id);
            if (categoria == null)
            {
                return NotFound();
            }
            return Ok(categoria);
        }
        [HttpPost]
        public ActionResult<Categoria> Post([FromBody] Categoria categoria)
        {
            categoria.Id = 0; // Ignorar o ID enviado pelo cliente, a base de dados irá criar outro
            _context.Categorias.Add(categoria);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = categoria.Id }, categoria);
        }
        [HttpPut("{id}")]
        public ActionResult<Categoria> Put(int id, [FromBody] Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return BadRequest();
            }
            _context.Entry(categoria).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }
        [HttpPatch("{id}")]
        public ActionResult<Categoria> Patch(int id, [FromBody] CategoriaPatch input)
        {
            var categoria = _context.Categorias.Find(id);
            if (categoria == null)
                return NotFound();

            if (input.Nome != null)
                categoria.Nome = input.Nome;

            _context.Entry(categoria).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public ActionResult<Categoria> Delete(int id)
        {
            var categoria = _context.Categorias.Find(id);
            if (categoria == null)
                return NotFound();

            _context.Categorias.Remove(categoria);
            _context.SaveChanges();
            return NoContent();
        }
    }
}