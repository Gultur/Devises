﻿using FluentAssertions;
using Moq;

using LuccaDevises.Shared;
using LuccaDevises.Services;
using LuccaDevises.Abstractions;

namespace LuccaDevisesTests;

[TestFixture]
public class TestLuccaDevisesService
{
    private const string ERROR_MESSAGE = "Error Message";
    private const string FILE_PATH = "D:\\TestFile.txt";

    private static IEnumerable<string> fileContent = new string[]
    {
        "EUR;550;JPY",
        "6",
        "AUD;CHF;0.9661",
        "JPY;KRW;13.1151",
        "EUR;CHF;1.2053",
        "AUD;JPY;86.0305",
        "EUR;USD;1.2989",
        "JPY;INR;0.6571",
    };

    private LuccaDevisesService _luccaDevisesService;
    private Mock<IProgramArgumentValidationService> _programArgumentValidationServiceMock;
    private Mock<IExchangeRequestValidationService> _exchangeRequestValidationServiceMock;
    private Mock<IFileService> _fileServiceMock;
    private Mock<IOutputService> _outputServiceMock;

    private MockRepository _mockRepository;


    [SetUp]
    public void Setup()
    {
        this._mockRepository = new MockRepository(MockBehavior.Strict);

        this._programArgumentValidationServiceMock = this._mockRepository.Create<IProgramArgumentValidationService>();
        this._exchangeRequestValidationServiceMock = this._mockRepository.Create<IExchangeRequestValidationService>();
        this._fileServiceMock = this._mockRepository.Create<IFileService>();
        this._outputServiceMock = this._mockRepository.Create<IOutputService>();

        this._luccaDevisesService = new LuccaDevisesService(
            this._programArgumentValidationServiceMock.Object,
            this._fileServiceMock.Object,
            this._exchangeRequestValidationServiceMock.Object,
            this._outputServiceMock.Object);
    }

    [Test]
    public void TestLuccaDevisesService_Execute_When_ArgumentsAreInvalid_Then_OutputError()
    {
        // Arrange
        string[] args = new string[] { FILE_PATH, "args"};

        Result<string> filePathResult = Result<string>.Failure(ERROR_MESSAGE);


       _ = this._programArgumentValidationServiceMock
            .Setup(service => service.GetFilePathFromArguments(args))
            .Returns(filePathResult);

        _ = this._outputServiceMock
             .Setup(service => service.OutputError(ERROR_MESSAGE));

        // Act
        this._luccaDevisesService.Execute(args);

        //Assert

        this._programArgumentValidationServiceMock
             .Verify(service => service.GetFilePathFromArguments(args), Times.Once);

        this._outputServiceMock
             .Verify(service => service.OutputError(ERROR_MESSAGE), Times.Once);
    }
    
    [Test]
    public void TestLuccaDevisesService_Execute_When_FileNotExist_Then_OutputError()
    {

        // Arrange
        string[] args = new string[] { FILE_PATH };


        Result<string> filePathResult = Result<string>.Success(FILE_PATH);

        Result<IEnumerable<string>> fileResult = Result<IEnumerable<string>>.Failure(ERROR_MESSAGE);


        _ = this._programArgumentValidationServiceMock
            .Setup(service => service.GetFilePathFromArguments(args))
            .Returns(filePathResult);

        _ = this._fileServiceMock
            .Setup(service => service.GetFileContent(filePathResult.Value))
            .Returns(fileResult);

        _ = this._outputServiceMock
             .Setup(service => service.OutputError(ERROR_MESSAGE));

        // Act
        this._luccaDevisesService.Execute(args);

        //Assert

        this._programArgumentValidationServiceMock
             .Verify(service => service.GetFilePathFromArguments(args), Times.Once);

        this._fileServiceMock
            .Verify(service => service.GetFileContent(filePathResult.Value), Times.Once);

        this._outputServiceMock
             .Verify(service => service.OutputError(ERROR_MESSAGE), Times.Once);
    }
    
    [Test]
    public void TestLuccaDevisesService_Execute_When_FileContent_BadFormat_Then_OutputError()
    {

        // Arrange
        string[] args = new string[] { FILE_PATH };
        string[] fileContent = new string[] { "BadFormating" };


        Result<string> filePathResult = Result<string>.Success(FILE_PATH);

        Result<IEnumerable<string>> fileResult = Result<IEnumerable<string>>.Success(fileContent);

        Result filecontentResult = Result.Failure(ERROR_MESSAGE);


        _ = this._programArgumentValidationServiceMock
            .Setup(service => service.GetFilePathFromArguments(args))
            .Returns(filePathResult);

        _ = this._fileServiceMock
            .Setup(service => service.GetFileContent(filePathResult.Value))
            .Returns(fileResult);

        _ = this._exchangeRequestValidationServiceMock
            .Setup(service => service.IsRequestContentValid(fileContent))
            .Returns(filecontentResult);

        _ = this._outputServiceMock
             .Setup(service => service.OutputError(ERROR_MESSAGE));

        // Act
        this._luccaDevisesService.Execute(args);

        //Assert

        this._programArgumentValidationServiceMock
             .Verify(service => service.GetFilePathFromArguments(args), Times.Once);

        this._fileServiceMock
            .Verify(service => service.GetFileContent(filePathResult.Value), Times.Once);

        this._exchangeRequestValidationServiceMock
            .Verify(service => service.IsRequestContentValid(fileContent), Times.Once);

        this._outputServiceMock
             .Verify(service => service.OutputError(ERROR_MESSAGE), Times.Once);
    }

}
