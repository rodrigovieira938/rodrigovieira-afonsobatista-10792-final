using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        [JsonIgnore]
        public Produto Produto { get; set; } = null!;
        [Column("delta")]
        public int Delta { get; set; }
        [Column("descricao")]
        public string? descricao { get; set; }
        [Column("utilizador_id")]
        public int UtilizadorId { get; set; }
        [JsonIgnore]
        public Utilizador Utilizador { get; set; } = null!;
    }
    public class PutMovimento
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public DateTime Timestamp { get; set; }
        public int ProdutoId { get; set; }
        public int Delta { get; set; }
        public string? descricao { get; set; }
        public int UtilizadorId { get; set; }
    }

    public class MovimentoPatch
    {
        public string? Nome { get; set; } = null;
        public DateTime? Timestamp { get; set; } = null;
        public int? ProdutoId { get; set; } = null;
        public int? Delta { get; set; } = null;
        public string? descricao { get; set; } = null;
        public int? UtilizadorId { get; set; } = null;
    }
    public class CreateMovimento
    {
        public string Nome { get; set; } = null!;
        public int ProdutoId { get; set; }
        public int Delta { get; set; }
        public string? descricao { get; set; }
        public int UtilizadorId { get; set; }
    }
}