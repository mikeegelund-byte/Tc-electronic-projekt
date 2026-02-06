using FluentAssertions;
using FluentResults;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Serilog;

namespace Nova.Application.Tests.UseCases;

public sealed class CopyPresetUseCaseTests
{
    private readonly Mock<ISavePresetUseCase> _mockSavePresetUseCase;
    private readonly Mock<ILogger> _mockLogger;
    private readonly CopyPresetUseCase _useCase;

    public CopyPresetUseCaseTests()
    {
        _mockSavePresetUseCase = new Mock<ISavePresetUseCase>();
        _mockLogger = new Mock<ILogger>();
        _useCase = new CopyPresetUseCase(_mockSavePresetUseCase.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidPresetAndSlot_CopiesPreset()
    {
        // Arrange
        var preset = CreateValidPreset(slotNumber: 5);
        int targetSlot = 10;
        _mockSavePresetUseCase.Setup(x => x.ExecuteAsync(preset, targetSlot, false))
            .ReturnsAsync(Result.Ok());

        // Act
        var result = await _useCase.ExecuteAsync(preset, targetSlot);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockSavePresetUseCase.Verify(x => x.ExecuteAsync(preset, targetSlot, false), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(61)]
    [InlineData(100)]
    public async Task ExecuteAsync_WithInvalidTargetSlot_ReturnsFailed(int invalidSlot)
    {
        // Arrange
        var preset = CreateValidPreset();

        // Act
        var result = await _useCase.ExecuteAsync(preset, invalidSlot);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("between 1 and 60");
        _mockSavePresetUseCase.Verify(x => x.ExecuteAsync(It.IsAny<Preset>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_CopyingToSameSlot_ReturnsFailed()
    {
        // Arrange
        var preset = CreateValidPreset(slotNumber: 15);
        int targetSlot = 15; // Same as source

        // Act
        var result = await _useCase.ExecuteAsync(preset, targetSlot);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("same slot");
        _mockSavePresetUseCase.Verify(x => x.ExecuteAsync(It.IsAny<Preset>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSaveFails_ReturnsFailedResult()
    {
        // Arrange
        var preset = CreateValidPreset(slotNumber: 5);
        int targetSlot = 10;
        _mockSavePresetUseCase.Setup(x => x.ExecuteAsync(preset, targetSlot, false))
            .ReturnsAsync(Result.Fail("MIDI error"));

        // Act
        var result = await _useCase.ExecuteAsync(preset, targetSlot);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("MIDI error");
    }

    private static Preset CreateValidPreset(int slotNumber = 31)
    {
        return TestHelpers.CreateValidPreset(slotNumber, "Test Preset");
    }
}
