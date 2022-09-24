using LuccaDevises.Abstractions;

namespace LuccaDevises.Services;

internal class FileService : IFileService
{
    public IEnumerable<string> GetFile(string filePath)
    {
        return File.ReadLines(filePath);
    }

    public bool IsfileExists(string filePath)
    {
        return File.Exists(filePath);
    }
}
