using System;

namespace Stockaholic.Frontend.Models;
public class Utils
{
    public static string Initials(string nome)
    {
        return string.Concat(
            nome
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w[0])
                .Take(2)
        ).ToUpper();
    }

}