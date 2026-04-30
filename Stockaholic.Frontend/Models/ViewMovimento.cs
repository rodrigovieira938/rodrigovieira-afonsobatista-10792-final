using System;

namespace Stockaholic.Frontend.Models;
public class ViewMovimento
{
    public int id {get; set;}
    public string nome {get; set;} = null!;
    public string descricao {get; set;} = null!;
    public string produto {get; set;} = null!;
    public string user {get; set;} = null!;
    public int delta {get; set;}
    public DateTime timestamp {get; set;}
}