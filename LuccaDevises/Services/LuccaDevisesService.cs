using LuccaDevises.Abstractions;
using LuccaDevises.Entities;
using LuccaDevises.Shared;

namespace LuccaDevises.Services;

internal class LuccaDevisesService : ILuccaDevisesService
{
    private IProgramArgumentValidationService _programArgumentValidationService;
    private IFileService _fileService;
    private IOutputService _outputService;
    private ICurrencyExchangeRequestValidationService _exchangeRequestValidationService;
    private ICurrencyExchangeRequestService _exchangeRequestService;

    public LuccaDevisesService(
        IProgramArgumentValidationService programArgumentValidationService,
        IFileService fileService,
        ICurrencyExchangeRequestValidationService exchangeRequestValidationService,
        IOutputService outputService,
        ICurrencyExchangeRequestService exchangeRequestService)
    {
        this._programArgumentValidationService = programArgumentValidationService;
        this._fileService = fileService;
        this._exchangeRequestValidationService = exchangeRequestValidationService;
        this._outputService = outputService;
        this._exchangeRequestService = exchangeRequestService;
    }

    // methode de nouilles
    public void Execute(string[] args)
    {
        //string[] appArgs = Environment.GetCommandLineArgs();

        //args = new string[] { "F:\\Programmation\\Lucca\\testLuccaOk.txt" };

        Result<string> filePathResult = this._programArgumentValidationService.GetFilePathFromArguments(args);

        if (filePathResult.IsFailure)
        {
            this._outputService.OutputErrorMessage(filePathResult.Message);
        }
        else
        {
            Console.WriteLine(filePathResult.Value);
            Result<IEnumerable<string>> fileContentResult = this._fileService.GetFileContent(filePathResult.Value);

            if (fileContentResult.IsFailure)
            {
                this._outputService.OutputErrorMessage(fileContentResult.Message);
                return;
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
            var fileContent = fileContentResult.Value;

            foreach (string line in fileContent)
            {
                Console.WriteLine(line);
            }

            Result<CurrencyExchangeRequest> requestValidationResult = this._exchangeRequestValidationService.IsCurrencyExchangeRequestContentValid(fileContent);

            if (requestValidationResult.IsFailure)
            {
                this._outputService.OutputErrorMessage(requestValidationResult.Message);
                return;
            }

            Result<int> calculationResult = this._exchangeRequestService.CalculateExchange(requestValidationResult.Value);

            if (calculationResult.IsFailure)
            {
                this._outputService.OutputErrorMessage(calculationResult.Message);
                return;
            }

            this._outputService.OutputMessage(calculationResult.Value.ToString());

        }

    }
}
