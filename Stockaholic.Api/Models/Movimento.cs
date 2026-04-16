namespace Stockaholic.API.Models
{
    public class Movimento
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public DateTime Timestamp { get; set; }
        public Produto Produto { get; set; } = null!;
        public int Delta { get; set; }
        public string? descricao { get; set; }
        public Utilizador Utilizador { get; set; } = null!;
    }
}