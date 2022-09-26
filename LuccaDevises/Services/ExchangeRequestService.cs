using System.Diagnostics;
using System.Runtime.CompilerServices;

using LuccaDevises.Abstractions;
using LuccaDevises.Entities;
using LuccaDevises.Shared;

[assembly: InternalsVisibleTo("LuccaDevisesTests")]
namespace LuccaDevises.Services;

internal class ExchangeRequestService : IExchangeRequestService
{
    public Result<int> CalculateExchange(CurrencyExchangeRequest currencyExchangeRequest)
    {

        currencyExchangeRequest = this.CleanUselessExchangeRates(currencyExchangeRequest);

        // we need a graph

        IEnumerable<CurrencyCode> distinctCurrencies = currencyExchangeRequest.GetDistinctCurrencies();

        CurrencyGraph graph = NewMethod(currencyExchangeRequest, distinctCurrencies);
        Result<List<CurrencyCode>> shortestPathResult = NewMethod1(currencyExchangeRequest, graph);

        if (shortestPathResult.IsFailure)
        {
            Debug.WriteLine(shortestPathResult.Message);
            return Result<int>.Failure(shortestPathResult.Message);

        }
        Debug.WriteLine(string.Join("->", shortestPathResult.Value.Select(c => c)));

        decimal initialamount = currencyExchangeRequest.Amount;
        var path = shortestPathResult.Value;

        for (int i = 0; i < path.Count - 1; i++)
        {
            CurrencyRelation relation = new CurrencyRelation(path[i], path[i + 1]);

            if (currencyExchangeRequest.ExchangesRates.ContainsKey(relation))
            {
                initialamount = initialamount * currencyExchangeRequest.ExchangesRates[relation];
            }
            else
            {
                CurrencyRelation reversedRelation = CurrencyRelation.ReverseCurrencyRelation(relation);

                if (currencyExchangeRequest.ExchangesRates.ContainsKey(reversedRelation))
                {
                    decimal currencyRatechange = Math.Round(1 / currencyExchangeRequest.ExchangesRates[reversedRelation], 4, MidpointRounding.AwayFromZero);

                    initialamount = initialamount * currencyRatechange;
                }

            }
        }

        int roundedAmount = (int)Math.Round(initialamount, MidpointRounding.AwayFromZero);

        return Result<int>.Success(roundedAmount);
    }

    private static Result<List<CurrencyCode>> NewMethod1(CurrencyExchangeRequest currencyExchangeRequest, CurrencyGraph graph)
    {
        return graph.GetShortestPath(currencyExchangeRequest.InitialCurrency, currencyExchangeRequest.ExpectedCurrency);
    }

    private static CurrencyGraph NewMethod(CurrencyExchangeRequest currencyExchangeRequest, IEnumerable<CurrencyCode> distinctCurrencies)
    {
        return new CurrencyGraph(
            distinctCurrencies,
            currencyExchangeRequest.ExchangesRates.Keys.ToArray());
    }


    // some currency(other than the initial and final) have only one relation, they don't have use in calculation
    private CurrencyExchangeRequest CleanUselessExchangeRates(CurrencyExchangeRequest currencyExchangeRequest)
    {
        IEnumerable<CurrencyCode> singleCurrencies = currencyExchangeRequest.GetSingleCurrencies();

        while (singleCurrencies.Any())
        {
            foreach (CurrencyCode currency in singleCurrencies)
            {
                currencyExchangeRequest.RemoveExchangeRate(currency);
            }

            singleCurrencies = currencyExchangeRequest.GetSingleCurrencies();
        }

        return currencyExchangeRequest;
    }
}
