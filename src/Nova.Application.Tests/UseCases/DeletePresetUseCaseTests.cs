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
    public async Task ExecuteAsync_WithValidSlot_DeletesPreset()
    {
        // Arrange
        const int slotNumber = 5;
        _mockSavePresetUseCase.Setup(x => x.ExecuteAsync(It.IsAny<Preset>(), slotNumber, false))
            .ReturnsAsync(Result.Ok());

        // Act
        var result = await _useCase.ExecuteAsync(slotNumber);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockSavePresetUseCase.Verify(x => x.ExecuteAsync(
            It.Is<Preset>(p => p.Number == slotNumber && p.Name.Trim() == $"Init {slotNumber:D2}"), 
            slotNumber, 
            false), 
            Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(61)]
    [InlineData(100)]
    public async Task ExecuteAsync_WithInvalidSlot_ReturnsFailed(int invalidSlot)
    {
        // Act
        var result = await _useCase.ExecuteAsync(invalidSlot);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("must be between 1 and 60");
        _mockSavePresetUseCase.Verify(x => x.ExecuteAsync(It.IsAny<Preset>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSaveFails_ReturnsFailedResult()
    {
        // Arrange
        const int slotNumber = 10;
        _mockSavePresetUseCase.Setup(x => x.ExecuteAsync(It.IsAny<Preset>(), slotNumber, false))
            .ReturnsAsync(Result.Fail("MIDI communication error"));

        // Act
        var result = await _useCase.ExecuteAsync(slotNumber);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("MIDI communication error");
    }

    [Fact]
    public async Task ExecuteAsync_DeletesMultipleDifferentSlots_EachGetsUniqueInitName()
    {
        // Arrange
        _mockSavePresetUseCase.Setup(x => x.ExecuteAsync(It.IsAny<Preset>(), It.IsAny<int>(), false))
            .ReturnsAsync(Result.Ok());

        // Act
        var result1 = await _useCase.ExecuteAsync(1);
        var result5 = await _useCase.ExecuteAsync(5);
        var result60 = await _useCase.ExecuteAsync(60);

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result5.IsSuccess.Should().BeTrue();
        result60.IsSuccess.Should().BeTrue();

        _mockSavePresetUseCase.Verify(x => x.ExecuteAsync(
            It.Is<Preset>(p => p.Name.Trim() == "Init 01"), 1, false), Times.Once);
        _mockSavePresetUseCase.Verify(x => x.ExecuteAsync(
            It.Is<Preset>(p => p.Name.Trim() == "Init 05"), 5, false), Times.Once);
        _mockSavePresetUseCase.Verify(x => x.ExecuteAsync(
            It.Is<Preset>(p => p.Name.Trim() == "Init 60"), 60, false), Times.Once);
    }
}
