using Microsoft.EntityFrameworkCore;

namespace Stockaholic.API.Data
{
    public class StockaholicDbContext : DbContext
    {
        public StockaholicDbContext (DbContextOptions<StockaholicDbContext> options) : base(options)
        {

        }
        public DbSet<Models.Categoria> Categorias { get; set; }
        public DbSet<Models.Produto> Produtos { get; set; }
        public DbSet<Models.Movimento> Movimentos { get; set; }
        public DbSet<Models.Utilizador> Utilizadores { get; set; }
    }
}