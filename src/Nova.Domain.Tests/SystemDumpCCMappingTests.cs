using FluentAssertions;
using Nova.Domain.Models;

namespace Nova.Domain.Tests;

public class SystemDumpCCMappingTests
{
    [Fact]
    public void GetCCMapping_ValidByteRange_ReturnsCorrectMapping()
    {
        // Arrange - System dump with CC #1 mapped to parameter 5
        var sysex = CreateValidSystemDumpWithCCMappings();
        sysex[34] = 0x01; // CC number 1
        sysex[35] = 0x05; // mapped to parameter 5
        
        var systemDump = SystemDump.FromSysEx(sysex).Value;

        // Act
        var result = systemDump.GetCCMapping(0); // First CC mapping at index 0

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CCNumber.Should().Be(1);
        result.Value.ParameterId.Should().Be(5);
    }

    [Fact]
    public void GetAllCCMappings_ValidSystemDump_Returns64Mappings()
    {
        // Arrange
        var sysex = CreateValidSystemDumpWithCCMappings();
        var systemDump = SystemDump.FromSysEx(sysex).Value;

        // Act
        var result = systemDump.GetAllCCMappings();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(64); // 64 CC mapping slots
    }

    [Fact]
    public void GetCCMapping_IndexOutOfRange_ReturnsFailure()
    {
        // Arrange
        var sysex = CreateValidSystemDumpWithCCMappings();
        var systemDump = SystemDump.FromSysEx(sysex).Value;

        // Act
        var result = systemDump.GetCCMapping(64); // Out of bounds

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("index"));
    }

    [Fact]
    public void GetCCMapping_UnassignedCC_ReturnsEmptyMapping()
    {
        // Arrange - CC slot with 0xFF (unassigned marker)
        var sysex = CreateValidSystemDumpWithCCMappings();
        sysex[34] = 0xFF; // Unassigned CC
        sysex[35] = 0xFF; // Unassigned parameter
        
        var systemDump = SystemDump.FromSysEx(sysex).Value;

        // Act
        var result = systemDump.GetCCMapping(0);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CCNumber.Should().Be(0xFF); // Unassigned marker
        result.Value.ParameterId.Should().Be(0xFF);
    }

    private static byte[] CreateValidSystemDumpWithCCMappings()
    {
        var sysex = new byte[527];
        sysex[0] = 0xF0;
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F; // TC Electronic
        sysex[4] = 0x00; // Device ID
        sysex[5] = 0x63; // Nova System
        sysex[6] = 0x20; // Dump message
        sysex[7] = 0x02; // System dump type
        
        // Initialize all CC mappings to unassigned (0xFF)
        for (int i = 34; i < 34 + 128; i++) // 64 mappings × 2 bytes each = 128 bytes
        {
            sysex[i] = 0xFF;
        }
        
        sysex[526] = 0xF7; // SysEx end
        return sysex;
    }
}
