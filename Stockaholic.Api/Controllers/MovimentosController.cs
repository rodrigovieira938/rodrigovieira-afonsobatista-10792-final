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
    public class MovimentosController : ControllerBase
    {
        private readonly StockaholicDbContext _context;
        private readonly CacheService _cacheService;
        public MovimentosController(StockaholicDbContext context, CacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(IEnumerable<Movimento>))]
        public async Task<ActionResult<IEnumerable<Movimento>>> Get()
        {
            var movimentos = await _cacheService.GetOrSetAsync(
                "movimentos:all",
                async () => await _context.Movimentos.ToListAsync(),
                TimeSpan.FromMinutes(5)
            );
            return Ok(movimentos);
        }
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(Movimento))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(NotFoundResult))]
        public async Task<ActionResult<Movimento>> Get(int id)
        {
            var movimento = await _cacheService.GetOrSetAsync(
                $"movimentos:{id}",
                async () => await _context.Movimentos.FindAsync(id).AsTask(),
                TimeSpan.FromMinutes(5)
            );

            if (movimento == null)
            {
                return NotFound();
            }
            return Ok(movimento);
        }
        [HttpGet("recentes")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(IEnumerable<Movimento>))]
        public async Task<ActionResult<IEnumerable<Movimento>>> GetRecentes()
        {
            var recentes = await _cacheService.GetOrSetAsync(
                "movimentos:recentes",
                async () => await _context.Movimentos
                    .OrderByDescending(m => m.Timestamp)
                    .Take(8)
                    .ToListAsync(),
                TimeSpan.FromMinutes(5)
            );
            return Ok(recentes);
        }
        [HttpGet("hoje")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(IEnumerable<Movimento>))]
        public async Task<ActionResult<IEnumerable<Movimento>>> GetHoje()
        {
            var hoje = DateTime.UtcNow.Date;
            var movimentosHoje = await _cacheService.GetOrSetAsync(
                $"movimentos:hoje:{hoje:yyyy-MM-dd}",
                async () => await _context.Movimentos
                    .Where(m => m.Timestamp.Date == hoje)
                    .ToListAsync(),
                TimeSpan.FromMinutes(5)
            );
            return Ok(movimentosHoje);
        }
        [HttpGet("stock")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(IEnumerable<ProdutoStock>))]
        public async Task<ActionResult<IEnumerable<ProdutoStock>>> GetStock()
        {
            var produtosStock = await _cacheService.GetOrSetAsync(
                "movimentos:stock",
                async () => await _context.Produtos
                    .GroupJoin(
                        _context.Movimentos,
                        p => p.Id,
                        m => m.ProdutoId,
                        (p, movimentos) => new
                        {
                            Produto = p,
                            Quantidade = movimentos.Sum(m => (int?)m.Delta) ?? 0
                        }
                    )
                    .Select(x => new ProdutoStock
                    {
                        ProdutoId = x.Produto.Id,
                        Nome = x.Produto.Nome,
                        CategoriaId = x.Produto.CategoriaId,
                        Categoria = x.Produto.Categoria.Nome,
                        Quantidade = x.Quantidade,
                        Valor = x.Produto.Preco
                    })
                    .ToListAsync(),
                TimeSpan.FromMinutes(5)
            );

            return Ok(produtosStock);
        }
        [HttpGet("top-valor")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(IEnumerable<ProdutoStock>))]
        public async Task<ActionResult<IEnumerable<ProdutoStock>>> GetTopValor()
        {
            var res = await _cacheService.GetOrSetAsync(
                "movimentos:top-valor",
                async () => await _context.Movimentos.GroupBy(m => m.ProdutoId)
                    .Select(g => new ProdutoStock
                    {
                        ProdutoId = g.Key,
                        Nome = _context.Produtos.Where(p => p.Id == g.Key).Select(p => p.Nome).FirstOrDefault() ?? string.Empty,
                        CategoriaId = _context.Produtos.Where(p => p.Id == g.Key).Select(p => p.CategoriaId).FirstOrDefault(),
                        Categoria = _context.Categorias.Where(c => c.Id == _context.Produtos.Where(p => p.Id == g.Key).Select(p => p.CategoriaId).FirstOrDefault()).Select(c => c.Nome).FirstOrDefault() ?? string.Empty,
                        Quantidade = g.Sum(m => m.Delta),
                        Valor = g.Sum(m => m.Delta) * _context.Produtos.Where(p => p.Id == g.Key).Select(p => p.Preco).FirstOrDefault()
                    })
                    .OrderByDescending(ps => ps.Valor)
                    .Take(8)
                    .ToListAsync(),
                TimeSpan.FromMinutes(5)
            );
            return Ok(res);
        }
        [HttpPost]
        [Authorize]
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
            InvalidateMovimentosCache(movimento.Timestamp.Date, movimento.Id);
            return CreatedAtAction(nameof(Get), new { id = movimento.Id }, movimento);
        }
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type=typeof(NoContentResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type=typeof(BadRequestResult))]
        public ActionResult<Movimento> Put(int id, [FromBody] PutMovimento movimento)
        {
            if (id != movimento.Id)
            {
                return BadRequest("O ID da URL deve ser igual ao ID do corpo da requisição.");
            }
            _context.Entry(new Movimento
            {
                Id=movimento.Id,
                Nome=movimento.Nome,
                Timestamp=movimento.Timestamp,
                ProdutoId=movimento.ProdutoId,
                Delta=movimento.Delta,
                descricao=movimento.descricao,
                UtilizadorId=movimento.UtilizadorId,
            }).State = EntityState.Modified;
            _context.SaveChanges();
            InvalidateMovimentosCache(movimento.Timestamp.Date, movimento.Id);
            return NoContent();
        }
        [HttpPatch("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type=typeof(NoContentResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(NotFoundResult))]
        public ActionResult<Movimento> Patch(int id, [FromBody] MovimentoPatch input)
        {
            var movimento = _context.Movimentos.Find(id);
            if (movimento == null)
                return NotFound();

            var originalDate = movimento.Timestamp.Date;
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
            InvalidateMovimentosCache(originalDate, movimento.Timestamp.Date, movimento.Id);
            return NoContent();
        }
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type=typeof(NoContentResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(NotFoundResult))]
        public ActionResult<Movimento> Delete(int id)
        {
            var movimento = _context.Movimentos.Find(id);
            if (movimento == null)
                return NotFound();

            _context.Movimentos.Remove(movimento);
            _context.SaveChanges();
            InvalidateMovimentosCache(movimento.Timestamp.Date, movimento.Id);
            return NoContent();
        }
        private void InvalidateMovimentosCache(DateTime date, int? id = null)
        {
            _cacheService.Remove("movimentos:all");
            _cacheService.Remove("movimentos:recentes");
            _cacheService.Remove("movimentos:stock");
            _cacheService.Remove("movimentos:top-valor");
            _cacheService.Remove($"movimentos:hoje:{date:yyyy-MM-dd}");
            if (id.HasValue)
            {
                _cacheService.Remove($"movimentos:{id.Value}");
            }
        }

        private void InvalidateMovimentosCache(DateTime originalDate, DateTime updatedDate, int? id = null)
        {
            InvalidateMovimentosCache(originalDate, id);
            if (updatedDate != originalDate)
            {
                _cacheService.Remove($"movimentos:hoje:{updatedDate:yyyy-MM-dd}");
            }
        }
    }
}