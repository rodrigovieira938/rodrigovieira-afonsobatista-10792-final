namespace Stockaholic.API.Models
{
    public class ProdutoStock
    {
        public int ProdutoId { get; set; }
        public string Nome { get; set; } = null!;
        public int Quantidade { get; set; }
        public float ValorTotal { get; set; }
    }
}