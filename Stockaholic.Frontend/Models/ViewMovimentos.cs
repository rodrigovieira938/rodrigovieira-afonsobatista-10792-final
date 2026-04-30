using System;

namespace Stockaholic.Frontend.Models;
public class ViewMovimentos
{
    public List<ViewMovimento> movimentos {get; set;} = new List<ViewMovimento>();
    public List<Produto> produtos {get; set;} = new List<Produto>();

}