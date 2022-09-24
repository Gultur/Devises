using System.ComponentModel.DataAnnotations;

namespace LuccaDevises.Entities;

internal class CurrencyCode
{
    public const string PATTERN = @"^[A-Z]{3}$";

    [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Bad Format")]
    public string? Code { get; set; }
}
