using FluentAssertions;
using Moq;

using LuccaDevises.Shared;
using LuccaDevises.Services;

namespace LuccaDevisesTests;

[TestFixture]
public class TestProgramArgumentValidationService
{
    private ProgramArgumentValidationService _programArgumentValidationService;

    [SetUp]
    public void Setup()
    {

        this._programArgumentValidationService = new ProgramArgumentValidationService();
    }

    [Test]
    public void ProgramArgumentValidationService_AreArgumentsValid_When_NoArgument_Then_Return_Failure()
    {
        // Arrange
        string[] arguments = Array.Empty<string>();

        // Act
        Result result = this._programArgumentValidationService.GetFilePathFromArguments(arguments);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be(ProgramArgumentValidationService.NO_ARGUMENT_PROVIDED);

    }

    
    [Test]
    public void ProgramArgumentValidationService_AreArgumentsValid_When_Too_Much_Arguments_Then_Return_Failure()
    {
        // Arrange
        string[] arguments = new string[] { "filePath", "anotherArg"};

        // Act
        Result result = this._programArgumentValidationService.GetFilePathFromArguments(arguments);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be(ProgramArgumentValidationService.TOO_MUCH_ARGUMENTS_PROVIDED);
    }
    
    
    [Test]
    public void ProgramArgumentValidationService_AreArgumentsValid_When_Valid_Arguments_Then_Return_Success()
    {
        // Arrange
        string filePath = "filePath";

        string[] arguments = new string[] { filePath };

        // Act
        Result result = this._programArgumentValidationService.GetFilePathFromArguments(arguments);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
