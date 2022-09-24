using System.Runtime.CompilerServices;

using LuccaDevises.Abstractions;
using LuccaDevises.Shared;


[assembly: InternalsVisibleTo("LuccaDevisesTests")]
namespace LuccaDevises.Services;

internal class ProgramArgumentValidationService : IProgramArgumentValidationService
{
    internal const string NO_ARGUMENT_PROVIDED = "No argument provided";
    internal const string NO_FILEPATH_PROVIDED = "A file path is expected";
    internal const string TOO_MUCH_ARGUMENTS_PROVIDED = "Too much arguments provided";
    internal const string FILE_NOT_EXIST = @"{0} is not a valid file path, no file has beed found";

    private IFileService _fileService;


    public ProgramArgumentValidationService(IFileService fileService)
    {
        _fileService = fileService;
    }

    public Result AreArgumentsValid(string[] programArguments)
    {
        // First argument is the ddl - Shouldn't happend
        if (programArguments.Length == 0)
        {
            return Result.Failure(NO_ARGUMENT_PROVIDED);
        }

        if (programArguments.Length < 2)
        {
            return Result.Failure(NO_FILEPATH_PROVIDED);
        }

        if (programArguments.Length > 2)
        {
            return Result.Failure(TOO_MUCH_ARGUMENTS_PROVIDED);
        }

        // The secondth argument must be the file name
        string expectedFilePath = programArguments[1];
        if (!_fileService.IsfileExists(expectedFilePath))
        {
            return Result.Failure(string.Format(FILE_NOT_EXIST, expectedFilePath));
        }

        // We should check if the extension is a valid one, but none are provided in the subject

        return Result.Success();
    }
}
