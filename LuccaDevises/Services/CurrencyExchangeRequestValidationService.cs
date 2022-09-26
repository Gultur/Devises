using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using LuccaDevises.Abstractions;
using LuccaDevises.Entities;
using LuccaDevises.Shared;

[assembly: InternalsVisibleTo("LuccaDevisesTests")]
namespace LuccaDevises.Services;

internal class CurrencyExchangeRequestValidationService : ICurrencyExchangeRequestValidationService
{
    internal const string NO_CONTENT = "The specified file is empty";
    internal const string NOT_ENOUGH_LINES = "The file content is incomplete";
    internal const string CURRENCY_CODE_MISFORMAT = "At least one currency code have a bad formatting";
    internal const string FIRST_LINE_INCOMPLETE = "The first line of the file is incomplete or have bad formatting";
    internal const string AMOUNT_NOT_PARSABLE = "The amount to exchange failed to be parsed or is less than 0";
    internal const string LINE_COUNT_NOT_PARSABLE = "The exchange rate line count failed to be parsed or is less than 0";
    internal const string LINE_COUNT_INVALID = "The number of exchange lines does not match the given exchange lines";
    internal const string EXCHANGE_LINE_INVALID = "At least one exchange rate line have bad formatting";
    internal const string EXCHANGE_RATE_NOT_PARSABLE = "At least one exchange rate failed to be parsed or is less than 0";

    // currency code seems like an iso code, further validation should be using an iso code source

    public Result<CurrencyExchangeRequest> IsCurrencyExchangeRequestContentValid(IEnumerable<string> content)
    {

        if (IsContentNullOrEmpy(content))
        {
            return Result<CurrencyExchangeRequest>.Failure(NO_CONTENT);
        }

        // we remove external white space
        string[] data = content.Select(line => line.Trim()).ToArray();

        if (IsLineMissing(data))
        {
            return Result<CurrencyExchangeRequest>.Failure(NOT_ENOUGH_LINES);
        }


        // line 1 of type (iso, int > 0, iso)
        string headerLine = data[0];

        Result<CurrencyExchangeRequest> initializeRequestHeaderResult = AddHeaderDataToRequest(headerLine);
        if (initializeRequestHeaderResult.IsFailure)
        {
            return Result<CurrencyExchangeRequest>.Failure(initializeRequestHeaderResult.Message);
        }

        CurrencyExchangeRequest currencyExchangeRequest = initializeRequestHeaderResult.Value;

        // line 2 of type positive integer
        string changeLinesCountData = data[1];
        if (IsLineCountInvalid(changeLinesCountData, out int expectedChangeLinesCount))
        {
            return Result<CurrencyExchangeRequest>.Failure(LINE_COUNT_NOT_PARSABLE);
        }

        // n-2 lines of type (iso, iso, decimal x.xxxx)
        string[] exchangeRateLines = content.Skip(2).ToArray();

        if (exchangeRateLines.Length != expectedChangeLinesCount)
        {
            return Result<CurrencyExchangeRequest>.Failure(LINE_COUNT_INVALID);
        }

        Result<CurrencyExchangeRequest> addExchangeRateResult = this.AddExchangeRatesToRequest(exchangeRateLines, currencyExchangeRequest);


        if(addExchangeRateResult.IsFailure)
        {
            return Result<CurrencyExchangeRequest>.Failure(addExchangeRateResult.Message);
        }


        return Result<CurrencyExchangeRequest>.Success(currencyExchangeRequest);
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
    
    private Result<CurrencyExchangeRequest> AddHeaderDataToRequest(string line)
    {
        // possible vérification par regex : ^[A-Z]{3};[0-9]+;[A-Z]{3}$ mais cela ne vérifie pas que le montant est supérieur à 0
        // sauf si l'on considère qu'il ne peut pas y avoir de 0 en debut du montant : ^[A-Z]{3};[1-9]+[0-9]*;[A-Z]{3}$

        string[] headerLineElements = line.Split(';');

        if (headerLineElements.Length != 3)
        {
            return Result<CurrencyExchangeRequest>.Failure(FIRST_LINE_INCOMPLETE);
        }

        if (this.IsInvalidCurrencyCode(headerLineElements[0]) || this.IsInvalidCurrencyCode(headerLineElements[2]))
        {
            return Result<CurrencyExchangeRequest>.Failure(CURRENCY_CODE_MISFORMAT);
        }

        if (!int.TryParse(headerLineElements[1], out int amount) || amount < 0)
        {
            return Result<CurrencyExchangeRequest>.Failure(AMOUNT_NOT_PARSABLE);
        }

        CurrencyExchangeRequest currencyExchangeRequest = new CurrencyExchangeRequest(
            new CurrencyCode(headerLineElements[0]),
            new CurrencyCode(headerLineElements[2]),
            amount);

        return Result<CurrencyExchangeRequest>.Success(currencyExchangeRequest); ;
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

    private Result<CurrencyExchangeRequest> AddExchangeRatesToRequest(string[] lines, CurrencyExchangeRequest currencyExchangeRequest)
    {
        // possible vérification par regex : ^([A-Z]{3};){2}[0-9]+\.[0-9]{4}$ mais cela ne vérifie pas que le taux de change est supérieur à 0
        // du moins je n'ai pas assez de connaissances en regex pour ce tour de passe-passe

        foreach (var line in lines)
        {
            string[] changeLineElements = line.Split(';');

            if (changeLineElements.Length != 3)
            {
                return Result<CurrencyExchangeRequest>.Failure(EXCHANGE_LINE_INVALID);
            }


            if (this.IsInvalidCurrencyCode(changeLineElements[0]) || this.IsInvalidCurrencyCode(changeLineElements[1]))
            {
                return Result<CurrencyExchangeRequest>.Failure(CURRENCY_CODE_MISFORMAT);
            }


            CultureInfo provider = new CultureInfo("en-GB"); // the separator must be  '.'
            if (!decimal.TryParse(changeLineElements[2], NumberStyles.AllowDecimalPoint, provider, out decimal exangeRate))
            {
                return Result<CurrencyExchangeRequest>.Failure(EXCHANGE_RATE_NOT_PARSABLE);
            }

            string[] decimalParts = exangeRate.ToString().Split(',');

            if (decimalParts.Length == 2 && decimalParts[1].Length > 4 || exangeRate < 0)
            {
                return Result<CurrencyExchangeRequest>.Failure(EXCHANGE_RATE_NOT_PARSABLE);
            }

            currencyExchangeRequest.AddExchangeRate(new CurrencyCode(changeLineElements[0]), new CurrencyCode(changeLineElements[1]), exangeRate);
        }

        return Result<CurrencyExchangeRequest>.Success(currencyExchangeRequest);
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
