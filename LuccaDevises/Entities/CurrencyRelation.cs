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

    // TODO : après ajout du taux de change, cette methode doit calculer celui de l'inversion aussi
    public static CurrencyRelation ReverseCurrencyRelation(CurrencyRelation currencyRelation)
    {
        return new CurrencyRelation(currencyRelation.FinalCurrency, currencyRelation.InitialCurrency);
    }
}
