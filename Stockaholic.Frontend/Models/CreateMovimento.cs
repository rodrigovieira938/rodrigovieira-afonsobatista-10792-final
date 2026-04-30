using System;

namespace Stockaholic.Frontend.Models;
public class CreateMovimento
{
    public string Nome { get; set; } = null!;
    public int ProdutoId { get; set; }
    public int Delta { get; set; }
    public string? descricao { get; set; }
    public int UtilizadorId { get; set; }
}