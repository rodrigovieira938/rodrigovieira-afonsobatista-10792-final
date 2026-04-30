using System.ComponentModel.DataAnnotations.Schema;

namespace Stockaholic.API.Models
{
    public class LoginInput
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
    public class LoginResult
    {
        public string? Token { get; set; } = null!;
    
    }
}