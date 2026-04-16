using System.ComponentModel.DataAnnotations.Schema;

namespace Stockaholic.API.Models
{
    [Table("movimentos")]
    public class Movimento
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("nome")]
        public string Nome { get; set; } = null!;
        [Column("timestamp")]
        public DateTime Timestamp { get; set; }
        [Column("produto_id")]
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; } = null!;
        [Column("delta")]
        public int Delta { get; set; }
        [Column("descricao")]
        public string? descricao { get; set; }
        [Column("utilizador_id")]
        public int UtilizadorId { get; set; }
        public Utilizador Utilizador { get; set; } = null!;
    }
}