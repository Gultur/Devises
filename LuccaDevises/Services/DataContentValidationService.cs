using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using LuccaDevises.Abstractions;
using LuccaDevises.Entities;
using LuccaDevises.Shared;

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
        if (IsContentNullOrEmpy(content))
        {
            return Result.Failure(NO_CONTENT);
        }


        string[] data = (string[])content;

        if (IsLineMissing(data))
        {
            return Result.Failure(NOT_ENOUGH_LINES);
        }


        // line 1 of type (iso, int > 0, iso)

        string headerLine = data[0];
        if (IsHeaderHasBadFormating(headerLine))
        {
            return Result.Failure(BAD_FORMATING);
        }

        // line 2 of type integer

        string changeLinesCountData = data[1];
        if (IsLineCountInvalid(changeLinesCountData, out int expectedChangeLinesCount))
        {
            return Result.Failure(BAD_FORMATING);
        }

        // n-2 lines of type (iso, iso, decimal x.xxxx)
        string[] exchangeRateLines = content.Skip(2).ToArray();

        if (exchangeRateLines.Length != expectedChangeLinesCount)
        {
            return Result.Failure(BAD_FORMATING);
        }


        if (exchangeRateLines.Any(line => IsChangeLineHasBadFormating(line) == true))
        {
            return Result.Failure(BAD_FORMATING);
        }


        return Result.Success();
    }

    private bool IsLineMissing(string[] data)
    {
        return data.Length < 3;
    }

    private bool IsContentNullOrEmpy(IEnumerable<string> content)
    {
        return content == null || !content.Any();
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

    private bool IsHeaderHasBadFormating(string line)
    {
        // possible vérification par regex : ^[A-Z]{3};[0-9]+;[A-Z]{3}$ mais cela ne vérifie pas que le montant est supérieur à 0
        // sauf si l'on considère qu'il ne peut pas y avoir de 0 en debut du montant : ^[A-Z]{3};[1-9]+[0-9]*;[A-Z]{3}$

        string[] headerLineElements = line.Split(';');

        if (headerLineElements.Length != 3)
        {
            return true;
        }

        if (this.IsInvalidCurrencyCode(headerLineElements[0]) || this.IsInvalidCurrencyCode(headerLineElements[2]))
        {
            return true;
        }

        if (!int.TryParse(headerLineElements[1], out int amount) || amount < 0)
        {
            return true;
        }

        return false;
    }

    /*    La deuxième ligne contient un nombre entier N indiquant le nombre de taux de change qui vont vous être transmis. */
    private bool IsLineCountInvalid(string line, out int expectedLinescount)
    {
        bool isIntValue = int.TryParse(line, out expectedLinescount);
        if (!isIntValue)
        {
            return true;
        }

        if (expectedLinescount < 0)
        {
            return true;
        }

        return false;
    }

    /* S'en suit N lignes représentant les taux de change représentés ainsi :
          La devise de départ DD sous la forme d'un code de 3 caractères 
          La devise d'arrivée DA sous la forme d'un code de 3 caractères 
          Le taux de change T sous la forme d'un nombre à 4 décimales (avec un "." comme séparateur 
         décimal) 
          Les informations vous sont transmises au format DD; DA;T*/

    private bool IsChangeLineHasBadFormating(string line)
    {

        // possible vérification par regex : ^([A-Z]{3};){2}[0-9]+\.[0-9]{4}$ mais cela ne vérifie pas que le taux de change est supérieur à 0
        // du moins je n'ai pas assez de connaissances en regex pour ce tour de passe-passe


        string[] changeLineElements = line.Split(';');

        if (changeLineElements.Length != 3)
        {
            return true;
        }


        if (this.IsInvalidCurrencyCode(changeLineElements[0]) || this.IsInvalidCurrencyCode(changeLineElements[1]))
        {
            return true;
        }


        CultureInfo provider = new CultureInfo("en-GB"); // the separator is '.'
        NumberStyles numberStyles = NumberStyles.AllowDecimalPoint;
        if (!decimal.TryParse(changeLineElements[2], numberStyles,  provider, out decimal exangeRate))
        {
            return true;
        }

        string[] decimalParts = exangeRate.ToString().Split('.');

        if (decimalParts.Length == 2 && decimalParts[1].Length > 4)
        {
            return true;
        }
            

        return false;
    }


    private bool IsInvalidCurrencyCode(string code)
    {
        MatchCollection matches = Regex.Matches(code, CurrencyCode.PATTERN);

        if (matches.Count == 0 || matches.Count > 1)
        {
            return true;
        }

        return false;
    }
}
