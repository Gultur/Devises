using LuccaDevises.Abstractions;
using LuccaDevises.Shared;

namespace LuccaDevises.Services;

internal class LuccaDevisesService : ILuccaDevisesService
{
    private IProgramArgumentValidationService _programArgumentValidationService;
    private IFileService _fileService;
    private IOutputService _outputService;
    private IExchangeRequestValidationService _exchangeRequestValidationService;

    public LuccaDevisesService(
        IProgramArgumentValidationService programArgumentValidationService,
        IFileService fileService,
        IExchangeRequestValidationService exchangeRequestValidationService,
        IOutputService outputService)
    {
        this._programArgumentValidationService = programArgumentValidationService;
        this._fileService = fileService;
        this._exchangeRequestValidationService = exchangeRequestValidationService;
        this._outputService = outputService;
    }

    // methode de nouilles
    public void Execute(string[] args)
    {
        //string[] appArgs = Environment.GetCommandLineArgs();

        Result<string> filePathResult = this._programArgumentValidationService.GetFilePathFromArguments(args);

        if (filePathResult.IsFailure)
        {
            this._outputService.OutputError(filePathResult.Message);
        }
        else
        {
            Console.WriteLine(filePathResult.Value);
            Result<IEnumerable<string>> fileContentResult = this._fileService.GetFileContent(filePathResult.Value);

            if (fileContentResult.IsFailure)
            {
                this._outputService.OutputError(fileContentResult.Message);
            }

            //IEnumerable<string> fileContent = new string[]
            //{
            //    "EUR;550;JPY",
            //    "6",
            //    "AUD;CHF;0.9661",
            //    "JPY;KRW;13.1151",
            //    "EUR;CHF;1.2053",
            //    "AUD;JPY;86.0305",
            //    "EUR;USD;1.2989",
            //    "JPY;INR;0.6571",
            //};
            else
            {
                var fileContent = fileContentResult.Value;

                foreach (string line in fileContent)
                {
                    Console.WriteLine(line);
                }

                var dataresult = this._exchangeRequestValidationService.IsRequestContentValid(fileContent);

                if (dataresult.IsFailure)
                {
                    this._outputService.OutputError(dataresult.Message);
                }
            }
        }

    }
}
