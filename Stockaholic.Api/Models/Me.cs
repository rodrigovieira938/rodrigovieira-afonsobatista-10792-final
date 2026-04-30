using System.ComponentModel.DataAnnotations.Schema;

namespace Stockaholic.API.Models
{
    public class MeResult
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
    }
}