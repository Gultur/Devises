using LuccaDevises.Entities;
using LuccaDevises.Shared;

namespace LuccaDevises.Abstractions;

public interface IExchangeRequestValidationService
{
    public Result<CurrencyExchangeRequest> IsRequestContentValid(IEnumerable<string> content);
}
