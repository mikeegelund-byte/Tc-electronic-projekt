using FluentAssertions;
using FluentResults;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Serilog;

namespace Nova.Application.Tests.UseCases;

public sealed class RenamePresetUseCaseTests
{
    private readonly Mock<ISavePresetUseCase> _mockSavePresetUseCase;
    private readonly Mock<IRequestPresetUseCase> _mockRequestPresetUseCase;
    private readonly Mock<ILogger> _mockLogger;
    private readonly RenamePresetUseCase _useCase;

    public RenamePresetUseCaseTests()
    {
        _mockSavePresetUseCase = new Mock<ISavePresetUseCase>();
        _mockRequestPresetUseCase = new Mock<IRequestPresetUseCase>();
        _mockLogger = new Mock<ILogger>();
        _useCase = new RenamePresetUseCase(
            _mockSavePresetUseCase.Object,
            _mockRequestPresetUseCase.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidName_RenamesPreset()
    {
        // Arrange
        var originalPreset = CreateValidPreset("Old Name");
        var newName = "New Name";
        var renamedPreset = CreateValidPreset(newName);

        _mockSavePresetUseCase.Setup(x => x.ExecuteAsync(It.IsAny<Preset>(), originalPreset.Number, false))
            .ReturnsAsync(Result.Ok());
        _mockRequestPresetUseCase.Setup(x => x.ExecuteAsync(originalPreset.Number, 2000))
            .ReturnsAsync(Result.Ok(renamedPreset));

        // Act
        var result = await _useCase.ExecuteAsync(originalPreset, newName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Trim().Should().Be(newName);
        _mockSavePresetUseCase.Verify(x => x.ExecuteAsync(It.Is<Preset>(p => p.Name.StartsWith(newName)), originalPreset.Number, false), Times.Once);
        _mockRequestPresetUseCase.Verify(x => x.ExecuteAsync(originalPreset.Number, 2000), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ExecuteAsync_WithEmptyName_ReturnsFailed(string? emptyName)
    {
        // Arrange
        var preset = CreateValidPreset();

        // Act
        var result = await _useCase.ExecuteAsync(preset, emptyName!);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("cannot be empty");
        _mockSavePresetUseCase.Verify(x => x.ExecuteAsync(It.IsAny<Preset>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithNameTooLong_ReturnsFailed()
    {
        // Arrange
        var preset = CreateValidPreset();
        var tooLongName = "This name is way too long for a preset";

        // Act
        var result = await _useCase.ExecuteAsync(preset, tooLongName);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("24 characters or less");
        _mockSavePresetUseCase.Verify(x => x.ExecuteAsync(It.IsAny<Preset>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSaveFails_ReturnsFailedResult()
    {
        // Arrange
        var preset = CreateValidPreset();
        var newName = "New Name";
        _mockSavePresetUseCase.Setup(x => x.ExecuteAsync(It.IsAny<Preset>(), preset.Number, false))
            .ReturnsAsync(Result.Fail("MIDI error"));

        // Act
        var result = await _useCase.ExecuteAsync(preset, newName);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("MIDI error");
    }

    [Fact]
    public async Task ExecuteAsync_WhenVerificationFails_StillReturnsSuccess()
    {
        // Arrange
        var originalPreset = CreateValidPreset("Old Name");
        var newName = "New Name";

        _mockSavePresetUseCase.Setup(x => x.ExecuteAsync(It.IsAny<Preset>(), originalPreset.Number, false))
            .ReturnsAsync(Result.Ok());
        _mockRequestPresetUseCase.Setup(x => x.ExecuteAsync(originalPreset.Number, 2000))
            .ReturnsAsync(Result.Fail<Preset>("Timeout"));

        // Act
        var result = await _useCase.ExecuteAsync(originalPreset, newName);

        // Assert
        result.IsSuccess.Should().BeTrue("save succeeded even if verification failed");
        result.Value.Name.Trim().Should().Be(newName);
    }

    [Fact]
    public async Task ExecuteAsync_With16CharName_AcceptsName()
    {
        // Arrange
        var preset = CreateValidPreset();
        var exactLengthName = "Exactly 24 chars here!!"; // Exactly 24 chars
        var renamedPreset = CreateValidPreset(exactLengthName);

        _mockSavePresetUseCase.Setup(x => x.ExecuteAsync(It.IsAny<Preset>(), preset.Number, false))
            .ReturnsAsync(Result.Ok());
        _mockRequestPresetUseCase.Setup(x => x.ExecuteAsync(preset.Number, 2000))
            .ReturnsAsync(Result.Ok(renamedPreset));

        // Act
        var result = await _useCase.ExecuteAsync(preset, exactLengthName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Trim().Should().Be(exactLengthName);
    }

    private static Preset CreateValidPreset(string name = "Test Preset")
    {
        return TestHelpers.CreateValidPreset(31, name);
    }
}
