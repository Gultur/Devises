using LuccaDevises.Abstractions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("LuccaDevisesTests")]
namespace LuccaDevises.Services;

internal class OutputService : IOutputService
{
    public void OutputError(string message)
    {
        Console.WriteLine(message);
    }
}
