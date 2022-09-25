using LuccaDevises.Shared;

namespace LuccaDevises.Abstractions;

public interface IProgramArgumentValidationService
{
    public Result<string> GetFilePathFromArguments(string[] args);
}
