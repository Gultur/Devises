using FluentAssertions;

using LuccaDevises.Services;
using LuccaDevises.Entities;
using LuccaDevisesTests.Helper;
using LuccaDevises.Shared;

namespace LuccaDevisesTests;

[TestFixture]
public class TestCurrencyExchangeRequestService
{
    private CurrencyExchangeRequestService _exchangeRequestService;


    [SetUp]
    public void Setup()
    {
        this._exchangeRequestService = new CurrencyExchangeRequestService();
    }

    [Test]
    public void ExchangeRequestService_IsDataContentValid_LinearGraphe_Then_Return_ValidAmount()
    {
        // Arrange
        string[] fileContent = new string[]
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

        CurrencyExchangeRequest currencyExchangeRequest = CurrencyExchangeRequestTestHelper.CreateFromArray(fileContent);

        // Act
        int calculatedAmount = this._exchangeRequestService.CalculateExchange(currencyExchangeRequest).Value;

        // Assert
        calculatedAmount.Should().Be(59033);
    }

    [Test]
    public void ExchangeRequestService_IsDataContentValid_CyclicGraphe_Then_Return_ValidAmount()
    {
        // Arrange
        string[] fileContent = new string[]
        {
            "EUR;550;JPY",
            "7",
            "AUD;CHF;0.9661",
            "JPY;KRW;13.1151",
            "EUR;CHF;1.2053",
            "AUD;JPY;86.0305",
            "EUR;USD;1.2989",
            "JPY;INR;0.6571",
            "USD;JPY;82.6336", // EUR and JPY are connected with two path, shorter by USD
        };

        CurrencyExchangeRequest currencyExchangeRequest = CurrencyExchangeRequestTestHelper.CreateFromArray(fileContent);

        // Act
        int calculatedAmount = this._exchangeRequestService.CalculateExchange(currencyExchangeRequest).Value;

        // Assert
        calculatedAmount.Should().Be(59033);
    }

    [Test]
    public void ExchangeRequestService_IsDataContentValid_Linear_BidirectionalRelation_Then_Return_ValidAmount()
    {
        // Arrange
        string[] fileContent = new string[]
        {
            "EUR;550;JPY",
            "7",
            "AUD;CHF;0.9661",
            "CHF;AUD;1.0351", // inversion of precedent line
            "JPY;KRW;13.1151",
            "EUR;CHF;1.2053",
            "AUD;JPY;86.0305",
            "EUR;USD;1.2989",
            "JPY;INR;0.6571",
        };

        CurrencyExchangeRequest currencyExchangeRequest = CurrencyExchangeRequestTestHelper.CreateFromArray(fileContent);

        // Act
        int calculatedAmount = this._exchangeRequestService.CalculateExchange(currencyExchangeRequest).Value;

        // Assert
        calculatedAmount.Should().Be(59033);
    }

    [Test]
    public void ExchangeRequestService_IsDataContentValid_Linear_Double_Then_Return_ValidAmount()
    {
        // Arrange
        string[] fileContent = new string[]
        {
            "EUR;550;JPY",
            "7",
            "AUD;CHF;0.9661",
            "AUD;CHF;0.9661", // double
            "JPY;KRW;13.1151",
            "EUR;CHF;1.2053",
            "AUD;JPY;86.0305",
            "EUR;USD;1.2989",
            "JPY;INR;0.6571",
        };

        CurrencyExchangeRequest currencyExchangeRequest = CurrencyExchangeRequestTestHelper.CreateFromArray(fileContent);

        // Act
        int calculatedAmount = this._exchangeRequestService.CalculateExchange(currencyExchangeRequest).Value;

        // Assert
        calculatedAmount.Should().Be(59033);
    }

    [Test]
    public void ExchangeRequestService_IsDataContentValid_Cycle_Separated_Then_Return_Failure()
    {
        // Arrange
        string[] fileContent = new string[]
        {
            "EUR;550;JPY",
            "7",
            "AUD;CHF;0.9661", // EUR is in a cycle
            "EUR;CHF;1.2053",
            "USD;AUD;0.9605",
            "EUR;USD;1.2989",
            "JPY;INR;0.6571", // JPY is on a second cycle
            "JPY;KRW;13.1151",
            "INR;KRW;19.9591",
        };

        CurrencyExchangeRequest currencyExchangeRequest = CurrencyExchangeRequestTestHelper.CreateFromArray(fileContent);

        // Act
        Result<int> calculateResult = this._exchangeRequestService.CalculateExchange(currencyExchangeRequest);

        // Assert
        calculateResult.IsFailure.Should().BeTrue();
    }
}

