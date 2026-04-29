using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stockaholic.API.Data;
using Stockaholic.API.Models;

namespace Stockaholic.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly StockaholicDbContext _context;
        public ProdutosController(StockaholicDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(IEnumerable<Produto>))]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            return Ok(_context.Produtos.ToList());
        }
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(Produto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(NotFoundResult))]
        public ActionResult<Produto> Get(int id)
        {
            var produto = _context.Produtos.Find(id);
            if (produto == null)
            {
                return NotFound();
            }
            return Ok(produto);
        }
        [HttpGet("numero")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(int))]
        public ActionResult<int> Count()
        {
            return Ok(_context.Produtos.Count());
        }
        [HttpGet("valor")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(double))]
        public ActionResult<double> Value()
        {
            var value = _context.Movimentos.GroupBy(m => m.ProdutoId)
                .Select(g => new
                {
                    ProdutoId = g.Key,
                    Quantidade = g.Sum(m => m.Delta)
                })
                .Join(_context.Produtos, m => m.ProdutoId, p => p.Id, (m, p) => new
                {
                    Valor = p.Preco * m.Quantidade
                })
                .Sum(x => x.Valor);
            return Ok(value);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type=typeof(Produto))]
        public ActionResult<Produto> Post([FromBody] CreateProduto createProduto)
        {
            var produto = new Produto
            {
                Nome = createProduto.Nome,
                CategoriaId = createProduto.CategoriaId,
                Preco = createProduto.Preco
            };
            _context.Produtos.Add(produto);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = produto.Id }, produto);
        }
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type=typeof(NoContentResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type=typeof(BadRequestResult))]
        public ActionResult<Produto> Put(int id, [FromBody] Produto produto)
        {
            if (id != produto.Id)
            {
                return BadRequest("O ID da URL deve ser igual ao ID do corpo da requisição.");
            }
            _context.Entry(produto).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type=typeof(NoContentResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(NotFoundResult))]
        public ActionResult<Produto> Patch(int id, [FromBody] ProdutoPatch input)
        {
            var produto = _context.Produtos.Find(id);
            if (produto == null)
                return NotFound();

            if (input.Nome != null)
                produto.Nome = input.Nome;
            if (input.CategoriaId != null)
                produto.CategoriaId = input.CategoriaId.Value;
            if (input.Preco != null)
                produto.Preco = input.Preco.Value;


            _context.Entry(produto).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type=typeof(NoContentResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(NotFoundResult))]
        public ActionResult<Produto> Delete(int id)
        {
            var produto = _context.Produtos.Find(id);
            if (produto == null)
                return NotFound();

            _context.Produtos.Remove(produto);
            _context.SaveChanges();
            return NoContent();
        }
    }
}