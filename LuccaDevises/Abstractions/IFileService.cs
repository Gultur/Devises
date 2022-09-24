namespace LuccaDevises.Abstractions;

public interface IFileService
{
    public bool IsfileExists(string filePath);

    public IEnumerable<string> GetFile(string filePath);
}
