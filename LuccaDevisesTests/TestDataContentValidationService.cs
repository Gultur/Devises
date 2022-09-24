using AutoFixture;
using FluentAssertions;

using LuccaDevises.Shared;
using LuccaDevises.Services;
using AutoFixture.Kernel;
using LuccaDevises.Entities;

namespace LuccaDevisesTests;

[TestFixture]
public class TestDataContentValidationService
{
    private DataContentValidationService _dataContentValidationService;

    private const string VALID_HEADER = "EUR;550;JPY";
    private const string VALID_CHANGE_LINE_COUNT = "1";
    private const string VALID_CHANGE_LINE = "AUD;CHF;0.9661";

    private Fixture _fixture;


    [SetUp]
    public void Setup()
    {
        this._fixture = new Fixture();
        this._dataContentValidationService = new DataContentValidationService();
    }

    [Test]
    public void DataContentValidationService_IsDataContentValid_When_NoArgument_Then_Return_Failure()
    {
        // Arrange
        string[] dataContent = Array.Empty<string>();

        // Act
        Result result = this._dataContentValidationService.IsDataContentValid(dataContent);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be(DataContentValidationService.NO_CONTENT);

    }

    [TestCase(1)]
    [TestCase(2)]
    public void DataContentValidationService_IsDataContentValid_When_Not_EnoughLine_Then_Return_Failure(short lineCount)
    {
        // Arrange
        string[] dataContent = new string[lineCount];

        for (int i = 0; i < lineCount; i ++)
        {
            dataContent[i] = this._fixture.Create<string>();
        }

        // Act
        Result result = this._dataContentValidationService.IsDataContentValid(dataContent);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be(DataContentValidationService.NOT_ENOUGH_LINES);
    }

    [Test]
    public void DataContentValidationService_IsDataContentValid_When_Header_Bad_Format_Then_Return_Failure()
    {
        // Arrange
        string[] dataContent = new string[3];
        dataContent[0] = "invalid";


        for (int i = 1; i <= 2; i ++)
        {
            dataContent[i] = this._fixture.Create<string>();
        }

        // Act
        Result result = this._dataContentValidationService.IsDataContentValid(dataContent);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be(DataContentValidationService.BAD_FORMATING);
    }
    
    [Test]
    public void DataContentValidationService_IsDataContentValid_When_Valid_LineCounts_Then_Return_Success()
    {
        // Arrange
        string[] dataContent = new string[3] { VALID_HEADER, VALID_CHANGE_LINE_COUNT, VALID_CHANGE_LINE };


        // Act
        Result result = this._dataContentValidationService.IsDataContentValid(dataContent);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    private string GetValidCurrency()
    {
        return this._fixture.Build<string>()
            .With(
                code => code,
                new SpecimenContext(this._fixture).Resolve(new RegularExpressionRequest(CurrencyCode.PATTERN)))
            .Create();
    }

}
