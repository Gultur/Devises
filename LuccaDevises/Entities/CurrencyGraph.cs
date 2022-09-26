﻿using LuccaDevises.Shared;
using System.Diagnostics;

namespace LuccaDevises.Entities;


public class CurrencyGraph
{
    // number of currencies / node of the graph
    private int _currenciesCount;

    private Dictionary<CurrencyCode, int> _indexByCurrencyCode;

    private LinkedList<CurrencyCode>[] _currencyAdjencyList; // un simple tableau suffira

    public CurrencyGraph(IEnumerable<CurrencyCode> distinctCurrencies, CurrencyRelation[] currencyRelations)
    {
        _currenciesCount = distinctCurrencies.Count();

        _currencyAdjencyList = new LinkedList<CurrencyCode>[_currenciesCount];
        _indexByCurrencyCode = new Dictionary<CurrencyCode, int>();

        foreach (var (currency, index) in distinctCurrencies.Select((currency, index) => (currency, index)))
        {
            _currencyAdjencyList[index] = new LinkedList<CurrencyCode>();
            _indexByCurrencyCode.Add(currency, index);
        }

        foreach (CurrencyRelation currencyRelation in currencyRelations)
        {
            AddNeigbours(currencyRelation);
        }

        PrintGraph();
    }


    private void AddNeigbours(CurrencyRelation currencyRelation)
    {
        // un lien de taux de change est bidirectionnel
        _currencyAdjencyList[_indexByCurrencyCode[currencyRelation.InitialCurrency]].AddLast(currencyRelation.FinalCurrency);
        _currencyAdjencyList[_indexByCurrencyCode[currencyRelation.FinalCurrency]].AddLast(currencyRelation.InitialCurrency);
    }

    private LinkedList<CurrencyCode> GetAdjacentCurrency(CurrencyCode currencyCode)
    {
        var index = _indexByCurrencyCode[currencyCode];
        return _currencyAdjencyList[index];
    }


    // A utility function to print the adjacency list
    // representation of graph
    private void PrintGraph()
    {
        for (int i = 0; i < _currencyAdjencyList.Length; i++)
        {
            Debug.WriteLine("\nRelation for the Currency "
                              + _indexByCurrencyCode.First(kv => kv.Value == i));

            foreach (CurrencyCode item in _currencyAdjencyList[i])
            {
                Debug.Write(" -> " + item);
            }
            Debug.WriteLine("");
        }
    }

    public Result<List<CurrencyCode>> GetShortestPath(CurrencyCode sourceCurrencyCode, CurrencyCode destinationCurrencyCode)
    {
        WriteDebug(sourceCurrencyCode, destinationCurrencyCode);

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
            CurrencyCode[] adjacentCurrency = GetAdjacentCurrency(dequeuedCurrency).Select(c => c).ToArray();

            if (!adjacentCurrency.Any())
            {
                continue;
            }

            CurrencyPathComputed dequeuedCurrencyPathInfo = currencyPathByCode[dequeuedCurrency];

            foreach (CurrencyCode currency in adjacentCurrency)
            {

                // if currency is already in the dictionnary we should ignore it
                if (currencyPathByCode.ContainsKey(currency))
                {
                    continue;
                }

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

        if (destinationHaveBeenReached)
        {
            Debug.WriteLine(currencyPathByCode[destinationCurrencyCode].ToString());
            return Result<List<CurrencyCode>>.Success(currencyPathByCode[destinationCurrencyCode].Path);
        }

        return Result<List<CurrencyCode>>.Failure("destination have not been reached");
    }

    private void WriteDebug(CurrencyCode sourceCurrencyCode, CurrencyCode destinationCurrencyCode)
    {
        int sourceIndex = _indexByCurrencyCode[sourceCurrencyCode];
        Debug.WriteLine("Source currency " + sourceCurrencyCode + " at index " + sourceIndex);

        int destinationIndex = _indexByCurrencyCode[destinationCurrencyCode];
        Debug.WriteLine("Destination currency " + destinationCurrencyCode + " at index " + destinationIndex);
    }

    private class CurrencyPathComputed
    {
        public CurrencyCode CurrencyCode { get; private set; }
        public List<CurrencyCode> Path { get; private set; }
        public int Distance { get; private set; }

        public CurrencyPathComputed(CurrencyCode currencyCode, IEnumerable<CurrencyCode> existingPath, int distance)
        {
            CurrencyCode = currencyCode;
            Path = new List<CurrencyCode>(existingPath);
            Path.Add(currencyCode);
            Distance = distance;
        }

        public static CurrencyPathComputed CreateFromPrevious(CurrencyPathComputed previous, CurrencyCode code)
        {
            return new CurrencyPathComputed(code, previous.Path, previous.Distance + 1);
        }

        public new string ToString()
        {
            return string.Join(" -> ", Path.Select(c => c.ToString()));
        }
    }
}