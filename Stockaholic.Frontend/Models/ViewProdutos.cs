using System;

namespace Stockaholic.Frontend.Models;
public class ViewProdutos
{
    public List<ProdutoStock> Produtos { get; set; } = new List<ProdutoStock>();
    public List<Categoria> Categorias { get; set; } = new List<Categoria>();
}