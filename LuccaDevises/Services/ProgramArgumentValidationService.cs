using LuccaDevises.Abstractions;
using LuccaDevises.Shared;

namespace LuccaDevises.Services;

internal class ProgramArgumentValidationService : IProgramArgumentValidationService
{
    internal const string NO_ARGUMENT_PROVIDED = "No argument provided";
    internal const string NO_FILEPATH_PROVIDED = "A file path is expected";
    internal const string TOO_MUCH_ARGUMENTS_PROVIDED = "Too much arguments provided";


    public Result<string> GetFilePathFromArguments(string[] programArguments)
    {
        if (programArguments.Length == 0)
        {
            return Result<string>.Failure(NO_ARGUMENT_PROVIDED);
        }

        if (programArguments.Length > 1)
        {
            return Result<string>.Failure(TOO_MUCH_ARGUMENTS_PROVIDED);
        }

        return Result<string>.Success(programArguments[0]);
    }
}
