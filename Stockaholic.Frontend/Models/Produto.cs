using System;

namespace Stockaholic.Frontend.Models;
public class Produto
{
    public int Id { get; set; }
    public float Preco { get; set; }
    public string Nome { get; set; } = null!;
    public int CategoriaId { get; set; }
}