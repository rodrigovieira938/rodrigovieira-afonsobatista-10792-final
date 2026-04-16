using Microsoft.EntityFrameworkCore;

namespace Stockaholic.API.Data
{
    public class StockaholicDbContext : DbContext
    {
        public StockaholicDbContext (DbContextOptions<StockaholicDbContext> options) : base(options)
        {

        }
        public DbSet<Models.Categoria> Categorias { get; set; }
    }
}