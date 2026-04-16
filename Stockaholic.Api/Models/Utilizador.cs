using System.ComponentModel.DataAnnotations.Schema;

namespace Stockaholic.API.Models
{
    [Table("utilizadores")]
    public class Utilizador
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("nome")]
        public string Nome { get; set; } = null!;
        [Column("email")]
        public string Email { get; set; } = null!;
        [Column("password_hash")]
        public string PasswordHash { get; set; } = null!;
        [Column("password_salt")]
        public string PasswordSalt { get; set; } = null!;
    }
}