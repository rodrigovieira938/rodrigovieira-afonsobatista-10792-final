using System;

namespace Stockaholic.Frontend.Models;
public class MovimentoTopValor
{
    public int produtoId { get; set; }
    public string nome { get; set; } = null!;
    public int quantidade {get; set;}
    public float valorTotal {get; set;}
}