using System;

namespace Stockaholic.Frontend.Models;
public class LoginRequest
{
    public string username { get; set; } = null!;
    public string password { get; set; } = null!;
}