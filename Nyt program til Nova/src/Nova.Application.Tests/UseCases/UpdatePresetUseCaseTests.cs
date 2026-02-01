using FluentAssertions;
using FluentResults;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Nova.Midi;
using Xunit;

namespace Nova.Application.Tests.UseCases;

public class UpdatePresetUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidPreset_SendsPresetAndReturnsSuccess()
    {
        // Arrange
        var mockMidiPort = new Mock<IMidiPort>();
        mockMidiPort.Setup(m => m.SendSysExAsync(It.IsAny<byte[]>()))
            .ReturnsAsync(Result.Ok());

        var useCase = new UpdatePresetUseCase();
        var preset = CreateValidPreset(31);

        // Act
        var result = await useCase.ExecuteAsync(preset, mockMidiPort.Object, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        mockMidiPort.Verify(m => m.SendSysExAsync(It.IsAny<byte[]>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidPreset_ReturnsValidationError()
    {
        // Arrange
        var mockMidiPort = new Mock<IMidiPort>();
        var useCase = new UpdatePresetUseCase();
        
        // Create a preset with invalid RawSysEx
        var invalidPreset = CreatePresetWithInvalidSysEx();

        // Act
        var result = await useCase.ExecuteAsync(invalidPreset, mockMidiPort.Object, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("valid RawSysEx"));
        mockMidiPort.Verify(m => m.SendSysExAsync(It.IsAny<byte[]>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenMidiSendFails_ReturnsError()
    {
        // Arrange
        var mockMidiPort = new Mock<IMidiPort>();
        mockMidiPort.Setup(m => m.SendSysExAsync(It.IsAny<byte[]>()))
            .ReturnsAsync(Result.Fail("MIDI connection lost"));

        var useCase = new UpdatePresetUseCase();
        var preset = CreateValidPreset(31);

        // Act
        var result = await useCase.ExecuteAsync(preset, mockMidiPort.Object, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("MIDI connection lost"));
    }

    [Fact]
    public async Task ExecuteAsync_WhenCancelled_ReturnsError()
    {
        // Arrange
        var mockMidiPort = new Mock<IMidiPort>();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var useCase = new UpdatePresetUseCase();
        var preset = CreateValidPreset(31);

        // Act
        var result = await useCase.ExecuteAsync(preset, mockMidiPort.Object, cts.Token);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("cancelled"));
    }

    private Preset CreateValidPreset(int presetNumber)
    {
        // Create a valid 521-byte preset SysEx
        var sysex = new byte[521];
        sysex[0] = 0xF0;
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F; // TC Electronic ID
        sysex[4] = 0x00; // Device ID
        sysex[5] = 0x63; // Nova System model ID
        sysex[6] = 0x20; // Dump message
        sysex[7] = 0x01; // Preset data type
        sysex[8] = (byte)presetNumber;
        
        // Set preset name (bytes 9-32, 24 chars)
        var nameBytes = System.Text.Encoding.ASCII.GetBytes("Test Preset         ");
        Array.Copy(nameBytes, 0, sysex, 9, Math.Min(nameBytes.Length, 24));
        
        sysex[520] = 0xF7; // End of SysEx

        var presetResult = Preset.FromSysEx(sysex);
        return presetResult.Value;
    }

    private Preset CreatePresetWithInvalidSysEx()
    {
        // Create a preset with valid initial structure but empty RawSysEx for ToSysEx validation
        var validPreset = CreateValidPreset(31);
        
        // Use reflection to set RawSysEx to invalid data (empty array)
        var rawSysExProperty = typeof(Preset).GetProperty("RawSysEx");
        rawSysExProperty?.SetValue(validPreset, Array.Empty<byte>());
        
        return validPreset;
    }
}
