using LuccaDevises.Entities;
using LuccaDevises.Shared;

namespace LuccaDevises.Abstractions;

public interface ICurrencyExchangeRequestValidationService
{
    public Result<CurrencyExchangeRequest> IsCurrencyExchangeRequestContentValid(IEnumerable<string> content);
}
