using LuccaDevises.Shared;
using System.Diagnostics;

namespace LuccaDevises.Entities;


public class CurrencyGraph
{
    const string CAN_NOT_EXCHANGE_CURRENCIES = "The currency {0} can't be exchanged to the currency {1}";

    private readonly Dictionary<CurrencyCode, List<CurrencyCode>> _adjacentCurrenciesByCurrencyCode;

    public CurrencyGraph(CurrencyExchangeRequest currencyExchangeRequest)
    {
        IEnumerable<CurrencyCode> distinctCurrencies = currencyExchangeRequest.GetDistinctCurrencies();
        CurrencyRelation[] currencyRelations = currencyExchangeRequest.ExchangesRates.Keys.ToArray();

        this._adjacentCurrenciesByCurrencyCode = new Dictionary<CurrencyCode, List<CurrencyCode>>();

        foreach (var (currency, index) in distinctCurrencies.Select((currency, index) => (currency, index)))
        {
            this._adjacentCurrenciesByCurrencyCode.Add(currency, new List<CurrencyCode>());
        }

        foreach (CurrencyRelation currencyRelation in currencyRelations)
        {
            this.AddAdjacent(currencyRelation);
        }

        PrintGraph();
    }


    private void AddAdjacent(CurrencyRelation currencyRelation)
    {
        this._adjacentCurrenciesByCurrencyCode[currencyRelation.InitialCurrency].Add(currencyRelation.FinalCurrency);
        this._adjacentCurrenciesByCurrencyCode[currencyRelation.FinalCurrency].Add(currencyRelation.InitialCurrency);
    }

    // A utility function to print the adjacency list
    // representation of graph
    private void PrintGraph()
    {
        foreach (KeyValuePair<CurrencyCode, List<CurrencyCode>> adjacentCurrenciesForCurrency in this._adjacentCurrenciesByCurrencyCode)
        {
            Debug.WriteLine("\nRelation for the Currency "
                              + adjacentCurrenciesForCurrency.Key.ToString());

            foreach (CurrencyCode item in adjacentCurrenciesForCurrency.Value)
            {
                Debug.Write(" -> " + item);
            }
            Debug.WriteLine("");
        }
    }

    public Result<List<CurrencyCode>> GetShortestPath(CurrencyCode sourceCurrencyCode, CurrencyCode destinationCurrencyCode)
    {
        // initialisation of collection
        Dictionary<CurrencyCode, CurrencyPathComputed> currencyPathByCode = new Dictionary<CurrencyCode, CurrencyPathComputed>();
        Queue<CurrencyCode> currenciesToExplore = new Queue<CurrencyCode>();

        bool destinationHaveBeenReached = false;

        // we store the source currency
        CurrencyPathComputed sourceCurrency = new CurrencyPathComputed(sourceCurrencyCode, Array.Empty<CurrencyCode>(), 0);
        currencyPathByCode.Add(sourceCurrency.CurrencyCode, sourceCurrency);
        currenciesToExplore.Enqueue(sourceCurrency.CurrencyCode);

        while (currenciesToExplore.Any() && !destinationHaveBeenReached)
        {
            CurrencyCode dequeuedCurrency = currenciesToExplore.Dequeue();

            // Get linked currencies
            CurrencyCode[] adjacentCurrencies = this._adjacentCurrenciesByCurrencyCode[dequeuedCurrency].Select(c => c).ToArray();

            if (!adjacentCurrencies.Any()) continue;

            CurrencyPathComputed dequeuedCurrencyPathInfo = currencyPathByCode[dequeuedCurrency];

            foreach (CurrencyCode currency in adjacentCurrencies)
            {
                // if currency is already in the dictionnary we should ignore it
                if (!currencyPathByCode.ContainsKey(currency))
                {
                    CurrencyPathComputed currencyPathComputed = CurrencyPathComputed.CreateFromPrevious(dequeuedCurrencyPathInfo, currency);
                    currencyPathByCode.Add(currency, currencyPathComputed);

                    if (currency == destinationCurrencyCode)
                    {
                        destinationHaveBeenReached = true;
                    }
                    else
                    {
                        currenciesToExplore.Enqueue(currency);
                    }
                }
            }
        }

        return destinationHaveBeenReached
            ? Result<List<CurrencyCode>>.Success(currencyPathByCode[destinationCurrencyCode].Path)
            : Result<List<CurrencyCode>>.Failure(string.Format(CAN_NOT_EXCHANGE_CURRENCIES, sourceCurrencyCode, destinationCurrencyCode));
    }
}
