using FluentAssertions;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Nova.Midi;

namespace Nova.Application.Tests.UseCases;

public class SaveSystemDumpUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidSystemDump_SendsToMidi()
    {
        // Arrange
        var sysex = CreateValidSystemDumpSysEx();
        var systemDump = SystemDump.FromSysEx(sysex).Value;
        var mockMidi = new Mock<IMidiPort>();
        mockMidi.Setup(x => x.SendSysExAsync(It.IsAny<byte[]>()))
                .ReturnsAsync(FluentResults.Result.Ok());
        var useCase = new SaveSystemDumpUseCase(mockMidi.Object);

        // Act
        var result = await useCase.ExecuteAsync(systemDump);

        // Assert
        result.IsSuccess.Should().BeTrue();
        mockMidi.Verify(x => x.SendSysExAsync(It.Is<byte[]>(bytes => 
            bytes.Length == 526 && 
            bytes[0] == 0xF0 && 
            bytes[525] == 0xF7)), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenMidiSendFails_ReturnsFailure()
    {
        // Arrange
        var sysex = CreateValidSystemDumpSysEx();
        var systemDump = SystemDump.FromSysEx(sysex).Value;
        var mockMidi = new Mock<IMidiPort>();
        mockMidi.Setup(x => x.SendSysExAsync(It.IsAny<byte[]>()))
                .ReturnsAsync(FluentResults.Result.Fail("MIDI send failed"));
        var useCase = new SaveSystemDumpUseCase(mockMidi.Object);

        // Act
        var result = await useCase.ExecuteAsync(systemDump);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("MIDI send failed");
    }

    private byte[] CreateValidSystemDumpSysEx()
    {
        var sysex = new byte[526];
        sysex[0] = 0xF0;                          // Start
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F; // TC Electronic
        sysex[4] = 0x00;                          // Device ID
        sysex[5] = 0x63;                          // Nova System
        sysex[6] = 0x20;                          // Dump
        sysex[7] = 0x02;                          // System Dump
        sysex[525] = 0xF7;                        // End
        return sysex;
    }
}
