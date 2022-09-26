using AutoFixture;
using FluentAssertions;

using LuccaDevises.Shared;
using LuccaDevises.Services;
using LuccaDevises.Entities;

namespace LuccaDevisesTests;

[TestFixture]
public class TestExchangeRequestValidationService
{
    private ExchangeRequestValidationService _exchangeRequestValidationService;

    private const string VALID_HEADER = "EUR;550;JPY";
    private const string VALID_CHANGE_LINE_COUNT = "1";
    private const string VALID_CHANGE_LINE = "EUR;JPY;0.9661";

    private Fixture _fixture;


    [SetUp]
    public void Setup()
    {
        this._fixture = new Fixture();
        this._exchangeRequestValidationService = new ExchangeRequestValidationService();
    }

    [Test]
    public void ExchangeRequestValidationService_IsDataContentValid_When_NoArgument_Then_Return_Failure()
    {
        // Arrange
        string[] dataContent = Array.Empty<string>();

        // Act
        Result result = this._exchangeRequestValidationService.IsRequestContentValid(dataContent);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be(ExchangeRequestValidationService.NO_CONTENT);

    }

    [TestCase(1)]
    [TestCase(2)]
    public void ExchangeRequestValidationService_IsDataContentValid_When_Not_EnoughLine_Then_Return_Failure(short lineCount)
    {
        // Arrange
        string[] dataContent = new string[lineCount];

        for (int i = 0; i < lineCount; i++)
        {
            dataContent[i] = this._fixture.Create<string>();
        }

        // Act
        Result result = this._exchangeRequestValidationService.IsRequestContentValid(dataContent);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be(ExchangeRequestValidationService.NOT_ENOUGH_LINES);
    }

    #region header
    [Test]
    public void ExchangeRequestValidationService_IsDataContentValid_When_Header_Bad_Format_Then_Return_Failure()
    {
        // Arrange
        string[] dataContent = this.GetValidData();
        dataContent[0] = "invalidHeader";

        // Act
        Result result = this._exchangeRequestValidationService.IsRequestContentValid(dataContent);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be(ExchangeRequestValidationService.BAD_FORMATING);
    }

    [TestCase("NotANumber")]
    [TestCase("-10")]
    [TestCase("10.5")]
    [TestCase("10,5")]
    public void ExchangeRequestValidationService_IsDataContentValid_When_Header_Invalid_Amount_Then_Return_Failure(string amount)
    {
        // Arrange
        string[] dataContent = this.GetValidData();

        string HeaderEletements = $"{GetValidCurrency()};{amount};{GetValidCurrency}";
        dataContent[0] = HeaderEletements;


        // Act
        Result result = this._exchangeRequestValidationService.IsRequestContentValid(dataContent);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be(ExchangeRequestValidationService.BAD_FORMATING);
    }

    [Test]
    public void ExchangeRequestValidationService_IsDataContentValid_When_Header_Invalid_Currency_Then_Return_Failure()
    {
        // Arrange
        string[] dataContent = this.GetValidData();

        string headerElements = $"{GetValidCurrency()};500;FGTR";
        dataContent[0] = headerElements;


        // Act
        Result result = this._exchangeRequestValidationService.IsRequestContentValid(dataContent);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be(ExchangeRequestValidationService.BAD_FORMATING);
    }




    #endregion

    #region exchange rates lines count
    [TestCase("")]
    [TestCase("-5")]
    public void ExchangeRequestValidationService_IsDataContentValid_When_ExchangeRates_LinesCount_InvalidType_Then_Return_Failure(string lineCount)
    {
        // Arrange
        string[] dataContent = this.GetValidData();

        dataContent[1] = lineCount;

        // Act
        Result result = this._exchangeRequestValidationService.IsRequestContentValid(dataContent);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be(ExchangeRequestValidationService.BAD_FORMATING);
    }

    [Test]
    public void ExchangeRequestValidationService_IsDataContentValid_When_ExchangeRates_LinesCount_InvalidNumber_Then_Return_Failure()
    {
        // Arrange
        string[] dataContent = this.GetValidData();

        // There is only one line of exchange rate
        dataContent[1] = "2";

        // Act
        Result result = this._exchangeRequestValidationService.IsRequestContentValid(dataContent);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be(ExchangeRequestValidationService.BAD_FORMATING);
    }

    #endregion

    #region exchangesRatesLines

    [Test]
    public void ExchangeRequestValidationService_IsDataContentValid_When_ExchangeRates_InvalidLine_Then_Return_Failure()
    {
        // Arrange
        string[] dataContent = this.GetValidData();
        dataContent[2] = "Invalid";

        // Act
        Result result = this._exchangeRequestValidationService.IsRequestContentValid(dataContent);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be(ExchangeRequestValidationService.BAD_FORMATING);
    }

    [TestCase("invalidLine", Reason = "the line don't have the expected 3 elements")]
    [TestCase("EUR;EUR;0.8000; oneElementMore", Reason = "the line don't have the expected 3 elements")]
    [TestCase("EUR;EUR;10,5", Reason = "echange rate must have a . not a ,")]
    [TestCase("EURO;EUR;1.5000", Reason = "first currency is invalid")]
    [TestCase("EUR;EURO;1.5000", Reason = "second currency is invalid")]
    [TestCase("EUR;EUR;0.800000", Reason = "echange rate must have only 4 digit after .")]
    public void ExchangeRequestValidationService_IsDataContentValid_When_ExchangeRates_InvalidLine_Then_Return_Failure(string lineElement)
    {
        // Arrange
        // Arrange
        string[] dataContent = this.GetValidData();

        dataContent[0] = lineElement;


        // Act
        Result result = this._exchangeRequestValidationService.IsRequestContentValid(dataContent);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be(ExchangeRequestValidationService.BAD_FORMATING);
    }

    #endregion

    [Test]
    public void ExchangeRequestValidationService_IsDataContentValid_When_Valid_LineCounts_Then_Return_Success()
    {
        // Arrange
        string[] dataContent = new string[3] { VALID_HEADER, VALID_CHANGE_LINE_COUNT, VALID_CHANGE_LINE };


        // Act
        Result result = this._exchangeRequestValidationService.IsRequestContentValid(dataContent);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    private string GetValidCurrency()
    {
        string currencyCode = this._fixture.Create<CurrencyCode>().ToString();

        return currencyCode;
    }

    private string[] GetValidData()
    {
        return new string[]
        {
            VALID_HEADER,
            VALID_CHANGE_LINE_COUNT,
            VALID_CHANGE_LINE,
        };
    }
}
