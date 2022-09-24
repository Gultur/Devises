using LuccaDevises.Shared;

namespace LuccaDevises.Abstractions;

public interface IProgramArgumentValidationService
{
    public Result AreArgumentsValid(string[] args);
}
