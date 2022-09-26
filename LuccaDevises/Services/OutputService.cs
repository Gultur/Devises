using LuccaDevises.Abstractions;

namespace LuccaDevises.Services;

internal class OutputService : IOutputService
{
    public void OutputErrorMessage(string message)
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Encountered Error : {message}");
        Console.ResetColor();
    }
    public void OutputMessage(string message)
    {
        Console.WriteLine(message);
    }
}
