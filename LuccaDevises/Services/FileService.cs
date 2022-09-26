using LuccaDevises.Abstractions;
using LuccaDevises.Shared;

namespace LuccaDevises.Services;

internal class FileService : IFileService
{
    internal const string FILE_NOT_EXIST = @"{0} is not a valid file path, no file has been found";

    public Result<IEnumerable<string>> GetFileContent(string filePath)
    {
        if(!File.Exists(filePath))
        {
            return Result<IEnumerable<string>>.Failure(string.Format(FILE_NOT_EXIST, filePath));
        }

        // We should check if the extension is a valid one, but none are provided

        return Result<IEnumerable<string>>.Success(File.ReadLines(filePath));
    }
}
