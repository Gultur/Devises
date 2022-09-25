using LuccaDevises.Abstractions;
using LuccaDevises.Shared;

namespace LuccaDevises.Services;

internal class FileService : IFileService
{
    internal const string FILE_NOT_EXIST = @"{0} is not a valid file path, no file has beed found";

    Result<IEnumerable<string>> IFileService.GetFileContent(string filePath)
    {
        if(!DoesFileExist(filePath))
        {
            return Result<IEnumerable<string>>.Failure(string.Format(FILE_NOT_EXIST, filePath));
        }

        // We should check if the extension is a valid one, but none are provided

        return Result<IEnumerable<string>>.Success(File.ReadLines(filePath));
    }

    private bool DoesFileExist(string filePath)
    {
        return File.Exists(filePath);
    }
}
