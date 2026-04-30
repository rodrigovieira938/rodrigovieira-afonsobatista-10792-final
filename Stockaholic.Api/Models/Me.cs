using System.ComponentModel.DataAnnotations.Schema;

namespace Stockaholic.API.Models
{
    public class MeResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}