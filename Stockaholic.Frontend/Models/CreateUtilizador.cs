using System;

namespace Stockaholic.Frontend.Models;
public class CreateUtilizador
{
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}