using System;

namespace Stockaholic.Frontend.Models;
public class LoginRequest
{
    public string email { get; set; } = null!;
    public string password { get; set; } = null!;
}