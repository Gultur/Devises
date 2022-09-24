using LuccaDevises.Abstractions;

namespace LuccaDevises.Services;

internal class LuccaDevisesService : ILuccaDevisesService
{
    private IProgramArgumentValidationService _programArgumentValidationService;
    private IFileService _fileService;

    public LuccaDevisesService(
        IProgramArgumentValidationService programArgumentValidationService,
        IFileService fileService)
    {
        this._programArgumentValidationService = programArgumentValidationService;
        this._fileService = fileService;
    }

    public void Execute(string[] args)
    {
        string[] appArgs = Environment.GetCommandLineArgs();


        var result = this._programArgumentValidationService.AreArgumentsValid(appArgs);

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
