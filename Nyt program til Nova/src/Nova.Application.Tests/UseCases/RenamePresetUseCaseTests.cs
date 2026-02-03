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
        // Create minimal valid SysEx for preset
        var sysex = new byte[520];
        sysex[0] = 0xF0; // SysEx start
        sysex[1] = 0x00; // TC Electronic ID
        sysex[2] = 0x20;
        sysex[3] = 0x1F;
        sysex[4] = 0x00; // Device ID
        sysex[5] = 0x63; // Nova System
        sysex[6] = 0x20; // Preset dump
        sysex[7] = 0x01; // Data type: Preset
        sysex[8] = 31; // Preset number (user preset 31)
        
        // Set preset name at bytes 9-32 (24 chars)
        var nameBytes = System.Text.Encoding.ASCII.GetBytes(name.PadRight(24));
        Array.Copy(nameBytes, 0, sysex, 9, 24);

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
