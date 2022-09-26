using LuccaDevises.Abstractions;

namespace LuccaDevises.Services;

internal class OutputService : IOutputService
{
    public void OutputErrorMessage(string message)
    {
        Console.WriteLine("Encountered Error : ", ConsoleColor.Red, message, null);
    }
    public void OutputMessage(string message)
    {
        Console.WriteLine(message);
    }
}
