using AutoFixture;
using FluentAssertions;

using LuccaDevises.Services;
using LuccaDevises.Entities;
using LuccaDevisesTests.Helper;

namespace LuccaDevisesTests;

[TestFixture]
public class TestExchangeRequestService
{
    private ExchangeRequestService _exchangeRequestService;


    [SetUp]
    public void Setup()
    {
        this._exchangeRequestService = new ExchangeRequestService();
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

        CurrencyExchangeRequest a = CurrencyExchangeRequestHelper.CreateFromArray(fileContent);

        // Act
        int b = this._exchangeRequestService.CalculateExchange(a).Value;

        // Assert
        b.Should().Be(59033);
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

        CurrencyExchangeRequest a = CurrencyExchangeRequestHelper.CreateFromArray(fileContent);

        // Act
        int b = this._exchangeRequestService.CalculateExchange(a).Value;

        // Assert
        b.Should().Be(59033);
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

        CurrencyExchangeRequest a = CurrencyExchangeRequestHelper.CreateFromArray(fileContent);

        // Act
        int b = this._exchangeRequestService.CalculateExchange(a).Value;

        // Assert
        b.Should().Be(59033);
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

        CurrencyExchangeRequest a = CurrencyExchangeRequestHelper.CreateFromArray(fileContent);

        // Act
        int b = this._exchangeRequestService.CalculateExchange(a).Value;

        // Assert
        b.Should().Be(59033);
    }
}

