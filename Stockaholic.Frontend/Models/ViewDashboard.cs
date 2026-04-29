using System;

namespace Stockaholic.Frontend.Models;
public class ViewDashboard
{
    public int total_produtos { get; set; }
    public float total_valor { get; set; }
    public int movimentos_hoje { get; set; }
    public List<Movimento> movimentos_recentes { get; set; } = new List<Movimento>();
    public List<MovimentoTopValor> movimentos_top_valor { get; set; } = new List<MovimentoTopValor>();
}