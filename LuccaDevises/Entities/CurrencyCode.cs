using System.ComponentModel.DataAnnotations;

namespace LuccaDevises.Entities;

internal class CurrencyCode
{
    public const string PATTERN = @"^[A-Z]{3}$";

    [RegularExpression(PATTERN, ErrorMessage = "Bad Format")]
    public string Code { get; set; } 

    public override string ToString() => this.Code;

    public CurrencyCode(string code)
    {
        Code = code;
    }
}
