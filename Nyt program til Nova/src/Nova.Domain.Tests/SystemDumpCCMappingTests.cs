using FluentAssertions;
using Nova.Domain.Models;

namespace Nova.Domain.Tests;

public class SystemDumpCCMappingTests
{
    [Fact]
    public void GetCCMapping_ValidAssignment_ReturnsCorrectMapping()
    {
        // Arrange - Tap Tempo assignment (index 0) set to CC 20
        var sysex = CreateValidSystemDump();
        WriteNibble(sysex, 8, 21); // CC 20 is stored as 21

        var systemDump = SystemDump.FromSysEx(sysex).Value;

        // Act
        var result = systemDump.GetCCMapping(0);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Assignment.Should().Be("Tap Tempo");
        result.Value.CCNumber.Should().Be(20);
        result.Value.IsAssigned.Should().BeTrue();
    }

    [Fact]
    public void GetAllCCMappings_ValidSystemDump_Returns11Mappings()
    {
        // Arrange
        var sysex = CreateValidSystemDump();
        var systemDump = SystemDump.FromSysEx(sysex).Value;

        // Act
        var result = systemDump.GetAllCCMappings();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(11);
    }

    [Fact]
    public void GetCCMapping_IndexOutOfRange_ReturnsFailure()
    {
        // Arrange
        var sysex = CreateValidSystemDump();
        var systemDump = SystemDump.FromSysEx(sysex).Value;

        // Act
        var result = systemDump.GetCCMapping(11); // Out of bounds

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("index"));
    }

    [Fact]
    public void GetCCMapping_Unassigned_ReturnsOff()
    {
        // Arrange - Tap Tempo set to Off (0)
        var sysex = CreateValidSystemDump();
        WriteNibble(sysex, 8, 0);

        var systemDump = SystemDump.FromSysEx(sysex).Value;

        // Act
        var result = systemDump.GetCCMapping(0);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CCNumber.Should().BeNull();
        result.Value.IsAssigned.Should().BeFalse();
    }

    private static byte[] CreateValidSystemDump()
    {
        var sysex = new byte[526];
        sysex[0] = 0xF0;
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F; // TC Electronic
        sysex[4] = 0x00; // Device ID
        sysex[5] = 0x63; // Nova System
        sysex[6] = 0x20; // Dump message
        sysex[7] = 0x02; // System dump type
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
