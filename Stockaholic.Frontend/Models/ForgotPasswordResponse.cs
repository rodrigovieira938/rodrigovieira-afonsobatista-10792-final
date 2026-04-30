using System;

namespace Stockaholic.Frontend.Models;
public class ForgotPasswordResponse
{
    public string message { get; set; } = null!;
    public string resetUrl { get; set; } = null!;
}