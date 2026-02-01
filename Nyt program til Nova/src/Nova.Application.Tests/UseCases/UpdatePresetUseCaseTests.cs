using FluentAssertions;
using FluentResults;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Nova.Midi;
using Serilog;
using Xunit;

namespace Nova.Application.Tests.UseCases;

public class UpdatePresetUseCaseTests
{
    private readonly Mock<IMidiPort> _mockMidiPort;
    private readonly Mock<ILogger> _mockLogger;
    private readonly UpdatePresetUseCase _useCase;
    private readonly byte[] _validSysEx;

    public UpdatePresetUseCaseTests()
    {
        _mockMidiPort = new Mock<IMidiPort>();
        _mockLogger = new Mock<ILogger>();
        _useCase = new UpdatePresetUseCase(_mockMidiPort.Object, _mockLogger.Object);
        
        // Create a valid 521-byte SysEx message for testing
        _validSysEx = new byte[521];
        _validSysEx[0] = 0xF0;  // SysEx start
        _validSysEx[1] = 0x00;  // TC Electronic manufacturer ID
        _validSysEx[2] = 0x20;
        _validSysEx[3] = 0x1F;
        _validSysEx[5] = 0x63;  // Nova System model ID
        _validSysEx[6] = 0x20;  // Dump message
        _validSysEx[7] = 0x01;  // Preset data type
        _validSysEx[8] = 0x00;  // Preset number
        // Preset name at bytes 9-32 (24 characters)
        for (int i = 9; i < 33; i++)
            _validSysEx[i] = (byte)'A';
        
        // Set TapTempo to 150 (bytes 38-41) using 4-byte little-endian encoding
        // 150 = 0x96 = 0b10010110
        // Split into 7-bit chunks: 0010110 (0x16) and 0000001 (0x01)
        _validSysEx[38] = 0x16;  // LSB
        _validSysEx[39] = 0x01;  // 
        _validSysEx[40] = 0x00;
        _validSysEx[41] = 0x00;  // MSB
        
        _validSysEx[520] = 0xF7;  // SysEx end
    }

    [Fact]
    public async Task ExecuteAsync_WithNullPreset_ReturnsFailure()
    {
        // Act
        var result = await _useCase.ExecuteAsync(null!);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Preset cannot be null");
    }

    [Fact]
    public async Task ExecuteAsync_WhenMidiPortNotConnected_ReturnsFailure()
    {
        // Arrange
        _mockMidiPort.Setup(m => m.IsConnected).Returns(false);
        var preset = Preset.FromSysEx(_validSysEx).Value;

        // Act
        var result = await _useCase.ExecuteAsync(preset);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "MIDI port is not connected");
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidPresetName_ReturnsFailure()
    {
        // Arrange
        _mockMidiPort.Setup(m => m.IsConnected).Returns(true);
        
        // Create SysEx with empty name
        var sysex = (byte[])_validSysEx.Clone();
        for (int i = 9; i < 33; i++)
            sysex[i] = 0x20;  // Space characters
        
        var preset = Preset.FromSysEx(sysex).Value;

        // Act
        var result = await _useCase.ExecuteAsync(preset);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Preset name cannot be empty");
    }

    [Fact]
    public async Task ExecuteAsync_WithValidPreset_SendsSysExAndReturnsSuccess()
    {
        // Arrange
        _mockMidiPort.Setup(m => m.IsConnected).Returns(true);
        _mockMidiPort.Setup(m => m.SendSysExAsync(It.IsAny<byte[]>()))
            .ReturnsAsync(Result.Ok());
        
        var preset = Preset.FromSysEx(_validSysEx).Value;

        // Act
        var result = await _useCase.ExecuteAsync(preset);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockMidiPort.Verify(m => m.SendSysExAsync(It.Is<byte[]>(b => b.Length == 521)), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSendFails_ReturnsFailure()
    {
        // Arrange
        _mockMidiPort.Setup(m => m.IsConnected).Returns(true);
        _mockMidiPort.Setup(m => m.SendSysExAsync(It.IsAny<byte[]>()))
            .ReturnsAsync(Result.Fail("MIDI send error"));
        
        var preset = Preset.FromSysEx(_validSysEx).Value;

        // Act
        var result = await _useCase.ExecuteAsync(preset);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Failed to send preset");
    }

    [Fact]
    public async Task ExecuteAsync_WhenCancelled_ReturnsFailure()
    {
        // Arrange
        _mockMidiPort.Setup(m => m.IsConnected).Returns(true);
        _mockMidiPort.Setup(m => m.SendSysExAsync(It.IsAny<byte[]>()))
            .ThrowsAsync(new OperationCanceledException());
        
        var preset = Preset.FromSysEx(_validSysEx).Value;
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var result = await _useCase.ExecuteAsync(preset, cts.Token);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("cancelled");
    }

    [Fact]
    public async Task ExecuteAsync_WithValidPreset_LogsSuccess()
    {
        // Arrange
        _mockMidiPort.Setup(m => m.IsConnected).Returns(true);
        _mockMidiPort.Setup(m => m.SendSysExAsync(It.IsAny<byte[]>()))
            .ReturnsAsync(Result.Ok());
        
        var preset = Preset.FromSysEx(_validSysEx).Value;

        // Act
        await _useCase.ExecuteAsync(preset);

        // Assert - Verify that Information was called (Serilog uses generic Information<T> for structured logging)
        _mockLogger.Verify(
            l => l.Information(
                It.Is<string>(s => s.Contains("Successfully sent preset")),
                It.IsAny<int>(),
                It.IsAny<string>()),
            Times.Once);
    }
}
