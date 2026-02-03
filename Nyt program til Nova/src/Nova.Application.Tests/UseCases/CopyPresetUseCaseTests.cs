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
    public async Task ExecuteAsync_WithValidPresetAndTargetNumber_CopiesPreset()
    {
        // Arrange
        var preset = CreateValidPreset(presetNumber: 31);
        int targetPresetNumber = 40;
        _mockSavePresetUseCase.Setup(x => x.ExecuteAsync(preset, targetPresetNumber, false))
            .ReturnsAsync(Result.Ok());

        // Act
        var result = await _useCase.ExecuteAsync(preset, targetPresetNumber);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockSavePresetUseCase.Verify(x => x.ExecuteAsync(preset, targetPresetNumber, false), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(30)]
    [InlineData(91)]
    public async Task ExecuteAsync_WithInvalidTargetPresetNumber_ReturnsFailed(int invalidPresetNumber)
    {
        // Arrange
        var preset = CreateValidPreset();

        // Act
        var result = await _useCase.ExecuteAsync(preset, invalidPresetNumber);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("between 31 and 90");
        _mockSavePresetUseCase.Verify(x => x.ExecuteAsync(It.IsAny<Preset>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_CopyingToSamePreset_ReturnsFailed()
    {
        // Arrange
        var preset = CreateValidPreset(presetNumber: 45);
        int targetPresetNumber = 45; // Same as source

        // Act
        var result = await _useCase.ExecuteAsync(preset, targetPresetNumber);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("same preset");
        _mockSavePresetUseCase.Verify(x => x.ExecuteAsync(It.IsAny<Preset>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSaveFails_ReturnsFailedResult()
    {
        // Arrange
        var preset = CreateValidPreset(presetNumber: 31);
        int targetPresetNumber = 40;
        _mockSavePresetUseCase.Setup(x => x.ExecuteAsync(preset, targetPresetNumber, false))
            .ReturnsAsync(Result.Fail("MIDI error"));

        // Act
        var result = await _useCase.ExecuteAsync(preset, targetPresetNumber);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("MIDI error");
    }

    private static Preset CreateValidPreset(int presetNumber = 31)
    {
        // Create minimal valid SysEx for preset
        var sysex = new byte[520];
        sysex[0] = 0xF0; // SysEx start
        sysex[1] = 0x00; // TC Electronic ID
        sysex[2] = 0x20;
        sysex[3] = 0x1F;
        sysex[4] = 0x00; // Device ID
        sysex[5] = 0x63; // Nova System
        sysex[6] = 0x20; // Preset dump
        sysex[7] = 0x01; // Data type: Preset (0x01)
        sysex[8] = (byte)presetNumber; // Preset number
        
        // Set preset name "Test Preset" at bytes 10-25 (16 chars)
        var nameBytes = System.Text.Encoding.ASCII.GetBytes("Test Preset".PadRight(16));
        Array.Copy(nameBytes, 0, sysex, 10, 16);

        // Calculate checksum (sum of bytes 34-517 & 0x7F)
        int checksum = 0;
        for (int i = 34; i <= 517; i++)
        {
            checksum += sysex[i];
        }
        sysex[518] = (byte)(checksum & 0x7F);
        
        sysex[519] = 0xF7; // SysEx end

        return Preset.FromSysEx(sysex).Value;
    }
}
