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
    public class ProdutosController : ControllerBase
    {
        private readonly StockaholicDbContext _context;
        private readonly CacheService _cacheService;

        public ProdutosController(StockaholicDbContext context, CacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(IEnumerable<Produto>))]
        public async Task<ActionResult<IEnumerable<Produto>>> Get()
        {
            var produtos = await _cacheService.GetOrSetAsync(
                "produtos:all",
                async () => await _context.Produtos.ToListAsync(),
                TimeSpan.FromMinutes(5)
            );
            return Ok(produtos);
        }
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(Produto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(NotFoundResult))]
        public async Task<ActionResult<Produto>> Get(int id)
        {
            var produto = await _cacheService.GetOrSetAsync(
                $"produtos:{id}",
                async () => await _context.Produtos.FindAsync(id).AsTask(),
                TimeSpan.FromMinutes(5)
            );
            if (produto == null)
            {
                return NotFound();
            }
            return Ok(produto);
        }
        [HttpGet("numero")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(int))]
        public async Task<ActionResult<int>> Count()
        {
            var count = await _cacheService.GetOrSetAsync(
                "produtos:count",
                async () => await _context.Produtos.CountAsync(),
                TimeSpan.FromMinutes(5)
            );
            return Ok(count);
        }
        [HttpGet("valor")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(double))]
        public async Task<ActionResult<double>> Value()
        {
            var value = await _cacheService.GetOrSetAsync(
                "produtos:value",
                async () => await _context.Movimentos.GroupBy(m => m.ProdutoId)
                    .Select(g => new
                    {
                        ProdutoId = g.Key,
                        Quantidade = g.Sum(m => m.Delta)
                    })
                    .Join(_context.Produtos, m => m.ProdutoId, p => p.Id, (m, p) => new
                    {
                        Valor = p.Preco * m.Quantidade
                    })
                    .SumAsync(x => x.Valor),
                TimeSpan.FromMinutes(5)
            );
            return Ok(value);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created, Type=typeof(Produto))]
        public async Task<ActionResult<Produto>> Post([FromBody] CreateProduto createProduto)
        {
            var produto = new Produto
            {
                Nome = createProduto.Nome,
                CategoriaId = createProduto.CategoriaId,
                Preco = createProduto.Preco
            };
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
            InvalidateProdutoCache(produto.Id);
            return CreatedAtAction(nameof(Get), new { id = produto.Id }, produto);
        }
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type=typeof(NoContentResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type=typeof(BadRequestResult))]
        public async Task<ActionResult<Produto>> Put(int id, [FromBody] CreateProduto produto)
        {
            var updatedProduto = new Produto
            {
                Id = id,
                Nome = produto.Nome,
                CategoriaId = produto.CategoriaId,
                Preco = produto.Preco
            };
            _context.Entry(updatedProduto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            InvalidateProdutoCache(id);
            return NoContent();
        }
        [HttpPatch("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type=typeof(NoContentResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(NotFoundResult))]
        public async Task<ActionResult<Produto>> Patch(int id, [FromBody] ProdutoPatch input)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
                return NotFound();

            if (input.Nome != null)
                produto.Nome = input.Nome;
            if (input.CategoriaId.HasValue)
                produto.CategoriaId = input.CategoriaId.Value;
            if (input.Preco.HasValue)
                produto.Preco = input.Preco.Value;

            _context.Entry(produto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            InvalidateProdutoCache(id);
            return NoContent();
        }
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type=typeof(NoContentResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(NotFoundResult))]
        public async Task<ActionResult<Produto>> Delete(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
                return NotFound();

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
            InvalidateProdutoCache(id);
            return NoContent();
        }

        private void InvalidateProdutoCache(int id)
        {
            _cacheService.Remove("produtos:all");
            _cacheService.Remove("movimentos:stock");
            _cacheService.Remove("produtos:count");
            _cacheService.Remove("produtos:value");
            _cacheService.Remove($"produtos:{id}");
        }
    }
}