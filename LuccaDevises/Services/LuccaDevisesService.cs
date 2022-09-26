using LuccaDevises.Abstractions;
using LuccaDevises.Entities;
using LuccaDevises.Shared;
using System.Diagnostics;

namespace LuccaDevises.Services;

internal class LuccaDevisesService : ILuccaDevisesService
{
    private readonly IProgramArgumentValidationService _programArgumentValidationService;
    private readonly IFileService _fileService;
    private readonly IOutputService _outputService;
    private readonly ICurrencyExchangeRequestValidationService _exchangeRequestValidationService;
    private readonly ICurrencyExchangeRequestService _exchangeRequestService;

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
        Result<string> filePathResult = this._programArgumentValidationService.GetFilePathFromArguments(args);

        if (filePathResult.IsFailure)
        {
            this._outputService.OutputErrorMessage(filePathResult.Message);
            return;
        }

        Debug.WriteLine($"Running LuccaDevises with file {filePathResult.Value}");
        Result<IEnumerable<string>> fileContentResult = this._fileService.GetFileContent(filePathResult.Value);
        if (fileContentResult.IsFailure)
        {
            this._outputService.OutputErrorMessage(fileContentResult.Message);
            return;
        }

        IEnumerable<string> fileContent = fileContentResult.Value;

        foreach (string line in fileContent)
        {
            Debug.WriteLine(line);
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
