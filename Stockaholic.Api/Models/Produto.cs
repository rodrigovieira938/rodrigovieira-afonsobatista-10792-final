using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Stockaholic.API.Models
{
    [Table("produtos")]
    public class Produto
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("preco")]
        public float Preco { get; set; }
        [Column("nome")]
        public string Nome { get; set; } = null!;
        [Column("categoria_id")]
        public int CategoriaId { get; set; }
        [JsonIgnore]
        public Categoria Categoria { get; set; } = null!;
    }

    public class ProdutoPatch
    {
        public string? Nome { get; set; } = null;
        public int? CategoriaId { get; set; } = null;
        public float? Preco { get; set; } = null;
    }
    public class CreateProduto
    {
        public string Nome { get; set; } = null!;
        public int CategoriaId { get; set; }
        public float Preco { get; set; }
    }
}