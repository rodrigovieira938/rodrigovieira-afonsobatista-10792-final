using System;

namespace Stockaholic.Frontend.Models;
public class Movimento
{
    public int id { get; set; }
    public string nome { get; set; } = null!;
    public string descricao { get; set; } = null!;
    public DateTime timestamp { get; set;}
    public int produtoId { get; set; }
    public int delta { get; set; }
    public int utilizadorId { get; set; }
}