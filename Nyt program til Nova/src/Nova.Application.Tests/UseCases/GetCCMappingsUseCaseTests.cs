using FluentAssertions;
using Nova.Application.UseCases;
using Nova.Domain.Models;

namespace Nova.Application.Tests.UseCases;

public class GetCCMappingsUseCaseTests
{
    private readonly GetCCMappingsUseCase _useCase;

    public GetCCMappingsUseCaseTests()
    {
        _useCase = new GetCCMappingsUseCase();
    }

    [Fact]
    public async Task ExecuteAsync_WhenSystemDumpIsNull_ReturnsFailure()
    {
        // Act
        var result = await _useCase.ExecuteAsync(null!);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("System Dump cannot be null");
    }

    [Fact]
    public async Task ExecuteAsync_WhenSystemDumpIsValid_Returns11Assignments()
    {
        // Arrange
        byte[] validSystemDump = CreateValidSystemDumpBytes();
        WriteNibble(validSystemDump, 8, 21); // Tap Tempo -> CC 20

        var systemDump = SystemDump.FromSysEx(validSystemDump).Value;

        // Act
        var result = await _useCase.ExecuteAsync(systemDump);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(11);
        result.Value[0].Assignment.Should().Be("Tap Tempo");
        result.Value[0].CCNumber.Should().Be(20);
    }

    [Fact]
    public async Task ExecuteAsync_WithUnassignedMappings_ReturnsCorrectIsAssignedStatus()
    {
        // Arrange
        byte[] validSystemDump = CreateValidSystemDumpBytes();
        // Tap Tempo assigned, others default Off
        WriteNibble(validSystemDump, 8, 11); // CC 10

        var systemDump = SystemDump.FromSysEx(validSystemDump).Value;

        // Act
        var result = await _useCase.ExecuteAsync(systemDump);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.First().IsAssigned.Should().BeTrue();
        result.Value.Skip(1).Should().AllSatisfy(mapping => mapping.IsAssigned.Should().BeFalse());
    }

    private static byte[] CreateValidSystemDumpBytes()
    {
        var sysex = new byte[526];
        sysex[0] = 0xF0;  // SysEx start
        sysex[1] = 0x00;  // TC Electronic
        sysex[2] = 0x20;
        sysex[3] = 0x1F;
        sysex[4] = 0x04;  // Device ID
        sysex[5] = 0x63;  // Nova System
        sysex[6] = 0x20;  // Dump message
        sysex[7] = 0x02;  // System dump type
        sysex[525] = 0xF7; // SysEx end
        return sysex;
    }

    private static void WriteNibble(byte[] data, int nibbleIndex, int value)
    {
        var offset = 8 + (nibbleIndex * 4);
        if (value >= 0)
        {
            data[offset] = (byte)(value % 128);
            data[offset + 1] = (byte)(value / 128);
            data[offset + 2] = 0;
            data[offset + 3] = 0;
        }
        else
        {
            data[offset] = (byte)(128 - ((-value) % 128));
            data[offset + 1] = (byte)((value / 128) + 127);
            data[offset + 2] = 127;
            data[offset + 3] = 7;
        }
    }
}
