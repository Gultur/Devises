using LuccaDevises.Shared;

namespace LuccaDevises.Abstractions;

public interface IFileService
{
    public Result<IEnumerable<string>> GetFileContent(string filePath);
}
