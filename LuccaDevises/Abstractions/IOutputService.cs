namespace LuccaDevises.Abstractions;

public interface IOutputService
{
    void OutputErrorMessage(string message);

    void OutputMessage(string message);
}
