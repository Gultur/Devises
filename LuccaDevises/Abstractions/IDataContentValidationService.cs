using LuccaDevises.Shared;

namespace LuccaDevises.Abstractions;

internal interface IDataContentValidationService
{
    public Result IsDataContentValid(IEnumerable<string> content);
}
