using System.ComponentModel.DataAnnotations.Schema;

namespace Stockaholic.API.Models
{
    [Table("categorias")]
    public class Categoria
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("nome")]
        public string Nome { get; set; } = null!;
    }
}