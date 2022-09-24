using FluentAssertions;
using Moq;

using LuccaDevises.Abstractions;
using LuccaDevises.Shared;
using LuccaDevises.Services;

namespace LuccaDevisesTests;

[TestFixture]
public class TestProgramArgumentValidationService
{
    private ProgramArgumentValidationService _programArgumentValidationService;

    private Mock<IFileService> _fileServiceMock;

    [SetUp]
    public void Setup()
    {
        this._fileServiceMock = new Mock<IFileService>(MockBehavior.Strict);

        this._programArgumentValidationService = new ProgramArgumentValidationService(this._fileServiceMock.Object);
    }

    [Test]
    public void ProgramArgumentValidationService_AreArgumentsValid_When_NoArgument_Then_Return_Failure()
    {
        // Arrange
        string[] arguments = Array.Empty<string>();

        // Act
        Result result = this._programArgumentValidationService.AreArgumentsValid(arguments);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be(ProgramArgumentValidationService.NO_ARGUMENT_PROVIDED);

    }

    [Test]
    public void ProgramArgumentValidationService_AreArgumentsValid_When_No_FilePath_Then_Return_Failure()
    {
        // Arrange
        string[] arguments = new string[] { "ddlName"};

        // Act
        Result result = this._programArgumentValidationService.AreArgumentsValid(arguments);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be(ProgramArgumentValidationService.NO_FILEPATH_PROVIDED);
    }
    
    [Test]
    public void ProgramArgumentValidationService_AreArgumentsValid_When_Too_Much_Arguments_Then_Return_Failure()
    {
        // Arrange
        string[] arguments = new string[] { "ddlName", "filePath", "anotherArg"};

        // Act
        Result result = this._programArgumentValidationService.AreArgumentsValid(arguments);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be(ProgramArgumentValidationService.TOO_MUCH_ARGUMENTS_PROVIDED);
    }
    
    [Test]
    public void ProgramArgumentValidationService_AreArgumentsValid_When_File_Not_Exists_Then_Return_Failure()
    {
        // Arrange
        string filePath = "filePath";

        string[] arguments = new string[] { "ddlName", filePath };

        this._fileServiceMock
            .Setup(service => service.IsfileExists(filePath))
            .Returns(false);

        // Act
        Result result = this._programArgumentValidationService.AreArgumentsValid(arguments);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Message.Should().Be(string.Format(ProgramArgumentValidationService.FILE_NOT_EXIST, filePath));
    }
    
    [Test]
    public void ProgramArgumentValidationService_AreArgumentsValid_When_Valid_Arguments_Then_Return_Success()
    {
        // Arrange
        string filePath = "filePath";

        string[] arguments = new string[] { "ddlName", filePath };

        this._fileServiceMock
            .Setup(service => service.IsfileExists(filePath))
            .Returns(true);

        // Act
        Result result = this._programArgumentValidationService.AreArgumentsValid(arguments);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().BeNull();
    }
}
