using LuccaDevises.Entities;
using LuccaDevises.Shared;

namespace LuccaDevises.Abstractions;

public interface IExchangeRequestService
{
    Result<int> CalculateExchange(CurrencyExchangeRequest currencyExchangeRequest);
}
