namespace LuccaDevises.Entities;

public class CurrencyExchangeRequest
{
    public CurrencyCode InitialCurrency { get; private set; }
    public CurrencyCode ExpectedCurrency { get; private set; }
    public int Amount  { get; private set; }

    public Dictionary<CurrencyRelation, decimal> ExchangesRates { get; private set; } // TODO : mettre le taux de change dans CurrencyRelation et remplacer par un hashset

    public CurrencyExchangeRequest(CurrencyCode initialCurrency, CurrencyCode expectedCurrency, int amount)
    {
        InitialCurrency = initialCurrency;
        ExpectedCurrency = expectedCurrency;
        Amount = amount;
        ExchangesRates = new Dictionary<CurrencyRelation, decimal>();
    }

    public void AddExchangeRate(CurrencyCode initial, CurrencyCode final, decimal changeRate)
    {
        CurrencyRelation exchangedCurrency = new CurrencyRelation(initial, final);

        if(!ExchangesRates.ContainsKey(exchangedCurrency))
        {
            ExchangesRates.Add(exchangedCurrency, changeRate);
        }
    }

    public IEnumerable<CurrencyCode> GetSingleCurrencies()
    {
        List<CurrencyCode> initialCurrencies = this.ExchangesRates.Keys.Select(key => key.InitialCurrency).ToList();
        List<CurrencyCode> finalCurrencies = this.ExchangesRates.Keys.Select(key => key.FinalCurrency).ToList();

        initialCurrencies.AddRange(finalCurrencies);


        // whe get currencies (excluding the exchanged currencies)  with only one link to an other, they can't be used to exchange
        IEnumerable<CurrencyCode> singleOnes = initialCurrencies
            .GroupBy(c => c)
            .Where(g => g.Count() == 1)
            .Select(g => g.Key)
            .Where(k => k != this.InitialCurrency && k != this.ExpectedCurrency);

        return singleOnes.ToArray();
    }
    
    public IEnumerable<CurrencyCode> GetDistinctCurrencies()
    {
        IEnumerable<CurrencyCode> currencies = 
            this.ExchangesRates.Keys.Select(key => key.InitialCurrency)
            .Concat(this.ExchangesRates.Keys.Select(key => key.FinalCurrency));

        return currencies.Distinct();
    }

    public void RemoveExchangeRate(CurrencyCode currencyCode)
    {
        CurrencyRelation? entryToRemove = this.ExchangesRates.Keys.SingleOrDefault(k => k.InitialCurrency == currencyCode || k.FinalCurrency == currencyCode);
        if (entryToRemove != null)
        {
            this.ExchangesRates.Remove(entryToRemove);
        }
    }
}
