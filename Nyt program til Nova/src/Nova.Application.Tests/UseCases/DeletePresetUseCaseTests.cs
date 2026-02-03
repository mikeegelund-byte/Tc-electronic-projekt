using FluentAssertions;
using FluentResults;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Serilog;
using Xunit;

namespace Nova.Application.Tests.UseCases;

public sealed class DeletePresetUseCaseTests
{
    private readonly Mock<ISavePresetUseCase> _mockSavePresetUseCase;
    private readonly Mock<ILogger> _mockLogger;
    private readonly DeletePresetUseCase _useCase;

    public DeletePresetUseCaseTests()
    {
        _mockSavePresetUseCase = new Mock<ISavePresetUseCase>();
        _mockLogger = new Mock<ILogger>();
        _useCase = new DeletePresetUseCase(_mockSavePresetUseCase.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidPresetNumber_DeletesPreset()
    {
        // Arrange
        const int presetNumber = 31;
        _mockSavePresetUseCase.Setup(x => x.ExecuteAsync(It.IsAny<Preset>(), presetNumber, false))
            .ReturnsAsync(Result.Ok());

        // Act
        var result = await _useCase.ExecuteAsync(presetNumber);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockSavePresetUseCase.Verify(x => x.ExecuteAsync(
            It.Is<Preset>(p => p.Number == presetNumber && p.Name.Trim() == $"Init {presetNumber:D2}"), 
            presetNumber, 
            false), 
            Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(30)]
    [InlineData(91)]
    public async Task ExecuteAsync_WithInvalidPresetNumber_ReturnsFailed(int invalidPresetNumber)
    {
        // Act
        var result = await _useCase.ExecuteAsync(invalidPresetNumber);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("between 31 and 90");
        _mockSavePresetUseCase.Verify(x => x.ExecuteAsync(It.IsAny<Preset>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSaveFails_ReturnsFailedResult()
    {
        // Arrange
        const int presetNumber = 40;
        _mockSavePresetUseCase.Setup(x => x.ExecuteAsync(It.IsAny<Preset>(), presetNumber, false))
            .ReturnsAsync(Result.Fail("MIDI communication error"));

        // Act
        var result = await _useCase.ExecuteAsync(presetNumber);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("MIDI communication error");
    }

    [Fact]
    public async Task ExecuteAsync_DeletesMultipleDifferentPresets_EachGetsUniqueInitName()
    {
        // Arrange
        _mockSavePresetUseCase.Setup(x => x.ExecuteAsync(It.IsAny<Preset>(), It.IsAny<int>(), false))
            .ReturnsAsync(Result.Ok());

        // Act
        var result31 = await _useCase.ExecuteAsync(31);
        var result45 = await _useCase.ExecuteAsync(45);
        var result90 = await _useCase.ExecuteAsync(90);

        // Assert
        result31.IsSuccess.Should().BeTrue();
        result45.IsSuccess.Should().BeTrue();
        result90.IsSuccess.Should().BeTrue();

        _mockSavePresetUseCase.Verify(x => x.ExecuteAsync(
            It.Is<Preset>(p => p.Name.Trim() == "Init 31"), 31, false), Times.Once);
        _mockSavePresetUseCase.Verify(x => x.ExecuteAsync(
            It.Is<Preset>(p => p.Name.Trim() == "Init 45"), 45, false), Times.Once);
        _mockSavePresetUseCase.Verify(x => x.ExecuteAsync(
            It.Is<Preset>(p => p.Name.Trim() == "Init 90"), 90, false), Times.Once);
    }
}
