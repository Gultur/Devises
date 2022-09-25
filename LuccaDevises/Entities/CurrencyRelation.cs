namespace LuccaDevises.Entities;

public record CurrencyRelation
{
    public CurrencyCode InitialCurrency { get; private set; }
    public CurrencyCode FinalCurrency { get; private set; }

    public CurrencyRelation(CurrencyCode initialCurrency, CurrencyCode finalCurrency)
    {
        this.InitialCurrency = initialCurrency;
        this.FinalCurrency = finalCurrency;
    }
}
