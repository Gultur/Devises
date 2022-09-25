using LuccaDevises.Shared;

namespace LuccaDevises.Abstractions;

public interface IExchangeRequestValidationService
{
    public Result IsRequestContentValid(IEnumerable<string> content);
}
