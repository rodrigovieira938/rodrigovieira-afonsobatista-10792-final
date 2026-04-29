using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stockaholic.API.Data;
using Stockaholic.API.Models;

namespace Stockaholic.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovimentosController : ControllerBase
    {
        private readonly StockaholicDbContext _context;
        public MovimentosController(StockaholicDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(IEnumerable<Movimento>))]
        public ActionResult<IEnumerable<Movimento>> Get()
        {
            return Ok(_context.Movimentos.ToList());
        }
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(Movimento))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(NotFoundResult))]
        public ActionResult<Movimento> Get(int id)
        {
            var movimento = _context.Movimentos.Find(id);
            if (movimento == null)
            {
                return NotFound();
            }
            return Ok(movimento);
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type=typeof(Movimento))]
        public ActionResult<Movimento> Post([FromBody] CreateMovimento createMovimento)
        {
            var movimento = new Movimento
            {
                Nome = createMovimento.Nome,
                Timestamp = DateTime.UtcNow,
                ProdutoId = createMovimento.ProdutoId,
                Delta = createMovimento.Delta,
                descricao = createMovimento.descricao,
                UtilizadorId = createMovimento.UtilizadorId
            };
            _context.Movimentos.Add(movimento);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = movimento.Id }, movimento);
        }
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type=typeof(NoContentResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type=typeof(BadRequestResult))]
        public ActionResult<Movimento> Put(int id, [FromBody] Movimento movimento)
        {
            if (id != movimento.Id)
            {
                return BadRequest("O ID da URL deve ser igual ao ID do corpo da requisição.");
            }
            _context.Entry(movimento).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type=typeof(NoContentResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(NotFoundResult))]
        public ActionResult<Movimento> Patch(int id, [FromBody] MovimentoPatch input)
        {
            var movimento = _context.Movimentos.Find(id);
            if (movimento == null)
                return NotFound();

            if (input.Nome != null)
                movimento.Nome = input.Nome;
            if (input.Timestamp.HasValue)
                movimento.Timestamp = input.Timestamp.Value;
            if (input.ProdutoId.HasValue)
                movimento.ProdutoId = input.ProdutoId.Value;
            if (input.Delta.HasValue)
                movimento.Delta = input.Delta.Value;
            if (input.descricao != null)
                movimento.descricao = input.descricao;
            if (input.UtilizadorId.HasValue)
                movimento.UtilizadorId = input.UtilizadorId.Value;

            _context.Entry(movimento).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type=typeof(NoContentResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(NotFoundResult))]
        public ActionResult<Movimento> Delete(int id)
        {
            var movimento = _context.Movimentos.Find(id);
            if (movimento == null)
                return NotFound();

            _context.Movimentos.Remove(movimento);
            _context.SaveChanges();
            return NoContent();
        }
    }
}