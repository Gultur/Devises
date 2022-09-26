using LuccaDevises.Entities;
using LuccaDevises.Shared;

namespace LuccaDevises.Abstractions;

public interface ICurrencyExchangeRequestService
{
    Result<int> CalculateExchange(CurrencyExchangeRequest currencyExchangeRequest);
}
