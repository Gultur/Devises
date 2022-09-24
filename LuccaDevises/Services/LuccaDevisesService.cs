using LuccaDevises.Abstractions;

namespace LuccaDevises.Services;

internal class LuccaDevisesService : ILuccaDevisesService
{
    private IProgramArgumentValidationService _programArgumentValidationService;

    public LuccaDevisesService(IProgramArgumentValidationService programArgumentValidationService)
    {
        _programArgumentValidationService = programArgumentValidationService;
    }

    public void Execute(string[] args)
    {
        string[] appArgs = Environment.GetCommandLineArgs();


        var result = _programArgumentValidationService.AreArgumentsValid(appArgs);

        if (result.IsFailure)
        {
            Console.WriteLine(result.Message);
        }
        else
        {
            foreach (string arg in appArgs)
            {
                Console.WriteLine(arg);
            }
        }

    }
}
