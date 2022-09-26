using LuccaDevises.Entities;
using System.Globalization;

namespace LuccaDevisesTests.Helper;

internal static class CurrencyExchangeRequestHelper
{
    public static CurrencyExchangeRequest CreateFromArray(string[] validArray)
    {

        var headerParts = validArray[0].Split(';');

        CurrencyExchangeRequest currencyExchangeRequest = new CurrencyExchangeRequest(
            new CurrencyCode(headerParts[0]), 
            new CurrencyCode(headerParts[2]), 
            int.Parse(headerParts[1]));


        CultureInfo provider = new CultureInfo("en-GB"); // the separator must be  '.'
        NumberStyles numberStyles = NumberStyles.AllowDecimalPoint;

        // we ignore the first two line
        for (int i = 2; i<validArray.Length; i ++)
        {
            var exchangeRateParts = validArray[i].Split(';');

            currencyExchangeRequest.AddExchangeRate(
                new CurrencyCode(exchangeRateParts[0]),
                new CurrencyCode(exchangeRateParts[1]),
                decimal.Parse(exchangeRateParts[2], numberStyles, provider));
        }

        return currencyExchangeRequest;

    }
}
