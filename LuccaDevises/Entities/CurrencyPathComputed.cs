namespace LuccaDevises.Entities;

internal class CurrencyPathComputed
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