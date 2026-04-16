using System.ComponentModel.DataAnnotations.Schema;

namespace Stockaholic.API.Models
{
    [Table("produtos")]
    public class Produto
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("nome")]
        public string Nome { get; set; } = null!;
        [Column("categoria_id")]
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = null!;
    }
}