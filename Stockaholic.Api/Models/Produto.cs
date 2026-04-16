namespace Stockaholic.API.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public Categoria Categoria { get; set; } = null!;
    }
}