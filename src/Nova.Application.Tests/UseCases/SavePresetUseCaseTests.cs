using FluentAssertions;
using FluentResults;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Nova.Midi;
using Serilog;

namespace Nova.Application.Tests.UseCases;

public sealed class SavePresetUseCaseTests
{
    private readonly Mock<IMidiPort> _mockMidiPort;
    private readonly Mock<IRequestPresetUseCase> _mockRequestPresetUseCase;
    private readonly Mock<ILogger> _mockLogger;
    private readonly SavePresetUseCase _useCase;

    public SavePresetUseCaseTests()
    {
        _mockMidiPort = new Mock<IMidiPort>();
        _mockRequestPresetUseCase = new Mock<IRequestPresetUseCase>();
        _mockLogger = new Mock<ILogger>();
        _useCase = new SavePresetUseCase(_mockMidiPort.Object, _mockRequestPresetUseCase.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidPresetAndSlot_SendsToMidi()
    {
        // Arrange
        var preset = CreateValidPreset();
        _mockMidiPort.Setup(x => x.IsConnected).Returns(true);
        _mockMidiPort.Setup(x => x.SendSysExAsync(It.IsAny<byte[]>()))
                     .ReturnsAsync(Result.Ok());

        // Act
        var result = await _useCase.ExecuteAsync(preset, 32);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockMidiPort.Verify(x => x.SendSysExAsync(It.Is<byte[]>(bytes =>
            bytes.Length == 521 &&
            bytes[0] == 0xF0 &&
            bytes[520] == 0xF7 &&
            bytes[8] == 32)), Times.Once); // Verify slot number
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(61)]
    [InlineData(100)]
    public async Task ExecuteAsync_WithInvalidSlotNumber_ReturnsFailure(int invalidSlot)
    {
        // Arrange
        var preset = CreateValidPreset();
        _mockMidiPort.Setup(x => x.IsConnected).Returns(true);

        // Act
        var result = await _useCase.ExecuteAsync(preset, invalidSlot);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("Invalid slot number");
        _mockMidiPort.Verify(x => x.SendSysExAsync(It.IsAny<byte[]>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNotConnected_ReturnsFailure()
    {
        // Arrange
        var preset = CreateValidPreset();
        _mockMidiPort.Setup(x => x.IsConnected).Returns(false);

        // Act
        var result = await _useCase.ExecuteAsync(preset, 32);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("Not connected");
        _mockMidiPort.Verify(x => x.SendSysExAsync(It.IsAny<byte[]>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenMidiSendFails_ReturnsFailure()
    {
        // Arrange
        var preset = CreateValidPreset();
        _mockMidiPort.Setup(x => x.IsConnected).Returns(true);
        _mockMidiPort.Setup(x => x.SendSysExAsync(It.IsAny<byte[]>()))
                     .ReturnsAsync(Result.Fail("MIDI timeout"));

        // Act
        var result = await _useCase.ExecuteAsync(preset, 32);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("MIDI timeout");
    }

    [Fact]
    public async Task ExecuteAsync_UpdatesPresetNumberBeforeSaving()
    {
        // Arrange
        var preset = CreateValidPreset();
        _mockMidiPort.Setup(x => x.IsConnected).Returns(true);
        _mockMidiPort.Setup(x => x.SendSysExAsync(It.IsAny<byte[]>()))
                     .ReturnsAsync(Result.Ok());

        // Act
        var result = await _useCase.ExecuteAsync(preset, 45); // Save to slot 45

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockMidiPort.Verify(x => x.SendSysExAsync(It.Is<byte[]>(bytes =>
            bytes[8] == 45)), Times.Once); // Verify new slot number in SysEx
    }

    [Fact]
    public async Task ExecuteAsync_ValidatesSysExFormat()
    {
        // Arrange
        var preset = CreateValidPreset();
        _mockMidiPort.Setup(x => x.IsConnected).Returns(true);
        _mockMidiPort.Setup(x => x.SendSysExAsync(It.IsAny<byte[]>()))
                     .ReturnsAsync(Result.Ok());

        // Act
        var result = await _useCase.ExecuteAsync(preset, 32);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockMidiPort.Verify(x => x.SendSysExAsync(It.Is<byte[]>(bytes =>
            bytes.Length == 521 &&
            bytes[0] == 0xF0 &&                          // Start marker
            bytes[1] == 0x00 && bytes[2] == 0x20 && bytes[3] == 0x1F && // TC Electronic ID
            bytes[5] == 0x63 &&                          // Nova System
            bytes[6] == 0x20 &&                          // Dump message
            bytes[7] == 0x01 &&                          // Preset type
            bytes[520] == 0xF7)), Times.Once);           // End marker
    }

    private Preset CreateValidPreset()
    {
        return TestHelpers.CreateValidPreset(32, "Test Preset");
    }
}
