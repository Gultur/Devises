using LuccaDevises.Abstractions;
using LuccaDevises.Entities;
using LuccaDevises.Shared;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("LuccaDevisesTests")]
namespace LuccaDevises.Services;

internal class DataContentValidationService : IDataContentValidationService
{
    internal const string NO_CONTENT = "No content";
    internal const string NOT_ENOUGH_LINES = "At least three lines are expected";
    internal const string BAD_FORMATING = "Data is not well formated";

    // currency code seems like an iso code, further validation should be using an iso code source

    public Result IsDataContentValid(IEnumerable<string> content)
    {
        if (content == null || !content.Any())
        {
            return Result.Failure(NO_CONTENT);
        }

        string[] data = (string[])content;

        if (data.Length < 3)
        {
            return Result.Failure(NOT_ENOUGH_LINES);
        }

        string headerLine = data[0];
        if (!isHeaderLineWellFormated(headerLine))
        {
            return Result.Failure(BAD_FORMATING);
        }


        // n lines

        // line 1 of type (iso, decimal, iso)

        // line 2 of type integer

        // n lines of type (iso, decimal, iso)

        return Result.Success();
    }

    /*
    La première ligne contient :
         La devise initiale D1 dans laquelle le montant est affiché, sous la forme d'un code de 3 
        caractères 
         Le montant M dans cette devise initiale, sous la forme d'un nombre entier positif > 0 
         La devise cible D2 vers laquelle il veut convertir le montant, sous la forme d'un code de 3 
        caractères 
         Les informations vous sont transmises au format D1;M;D2
    */

    private bool isHeaderLineWellFormated(string line)
    {
        string[] headerLineElements = line.Split(';');

        if (headerLineElements.Length != 3)
        {
            return false;
        }

        if (!this.isValidCurrencyCode(headerLineElements[0]) || !this.isValidCurrencyCode(headerLineElements[2]))
        {
            return false;
        }

        if (!int.TryParse(headerLineElements[1], out int amount))
        {
            return false;
        }

        return true;
    }

    private bool isValidCurrencyCode(string code)
    {
        return Regex.Match(code, CurrencyCode.PATTERN).Groups.Count == 1;
    }

    private bool isChangeLineWellFormated(string line)
    {
        return true;
    }
}
