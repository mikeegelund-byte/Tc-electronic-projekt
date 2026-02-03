using FluentAssertions;
using Nova.Domain.Models;

namespace Nova.Domain.Tests;

public class SystemDumpPedalMappingTests
{
    private SystemDump CreateValidSystemDump()
    {
        var validSystemDump = new byte[527];
        validSystemDump[0] = 0xF0;  // SysEx start
        validSystemDump[1] = 0x00;  // TC Electronic
        validSystemDump[2] = 0x20;
        validSystemDump[3] = 0x1F;
        validSystemDump[4] = 0x04;  // Bank number
        validSystemDump[5] = 0x63;  // Nova System model ID
        validSystemDump[6] = 0x20;  // Message ID (Dump)
        validSystemDump[7] = 0x02;  // Data Type (System Dump)
        
        validSystemDump[526] = 0xF7;  // SysEx end
        
        return SystemDump.FromSysEx(validSystemDump).Value;
    }

    [Fact]
    public void GetPedalParameter_ReadsBytes54To57()
    {
        // Arrange
        var systemDump = CreateValidSystemDump();
        var rawBytes = systemDump.RawSysEx;
        
        // Set parameter ID to 42 (little-endian: 0x2A 0x00 0x00 0x00)
        rawBytes[54] = 0x2A;
        rawBytes[55] = 0x00;
        rawBytes[56] = 0x00;
        rawBytes[57] = 0x00;

        // Act
        var parameter = systemDump.GetPedalParameter();

        // Assert
        parameter.Should().Be(42);
    }

    [Fact]
    public void GetPedalMin_ReadsBytes58To61()
    {
        // Arrange
        var systemDump = CreateValidSystemDump();
        var rawBytes = systemDump.RawSysEx;
        
        // Set min to 25% (little-endian: 0x19 0x00 0x00 0x00)
        rawBytes[58] = 0x19;
        rawBytes[59] = 0x00;
        rawBytes[60] = 0x00;
        rawBytes[61] = 0x00;

        // Act
        var min = systemDump.GetPedalMin();

        // Assert
        min.Should().Be(25);
    }

    [Fact]
    public void GetPedalMid_ReadsBytes62To65()
    {
        // Arrange
        var systemDump = CreateValidSystemDump();
        var rawBytes = systemDump.RawSysEx;
        
        // Set mid to 50% (little-endian: 0x32 0x00 0x00 0x00)
        rawBytes[62] = 0x32;
        rawBytes[63] = 0x00;
        rawBytes[64] = 0x00;
        rawBytes[65] = 0x00;

        // Act
        var mid = systemDump.GetPedalMid();

        // Assert
        mid.Should().Be(50);
    }

    [Fact]
    public void GetPedalMax_ReadsBytes66To69()
    {
        // Arrange
        var systemDump = CreateValidSystemDump();
        var rawBytes = systemDump.RawSysEx;
        
        // Set max to 75% (little-endian: 0x4B 0x00 0x00 0x00)
        rawBytes[66] = 0x4B;
        rawBytes[67] = 0x00;
        rawBytes[68] = 0x00;
        rawBytes[69] = 0x00;

        // Act
        var max = systemDump.GetPedalMax();

        // Assert
        max.Should().Be(75);
    }

    [Fact]
    public void GetPedalValues_WithTypicalConfiguration_ReturnsExpectedValues()
    {
        // Arrange - typical expression pedal setup
        var systemDump = CreateValidSystemDump();
        var rawBytes = systemDump.RawSysEx;
        
        // Parameter: Volume (ID 10)
        rawBytes[54] = 0x0A;
        rawBytes[55] = 0x00;
        rawBytes[56] = 0x00;
        rawBytes[57] = 0x00;
        
        // Min: 0%
        rawBytes[58] = 0x00;
        rawBytes[59] = 0x00;
        rawBytes[60] = 0x00;
        rawBytes[61] = 0x00;
        
        // Mid: 50%
        rawBytes[62] = 0x32;
        rawBytes[63] = 0x00;
        rawBytes[64] = 0x00;
        rawBytes[65] = 0x00;
        
        // Max: 100%
        rawBytes[66] = 0x64;
        rawBytes[67] = 0x00;
        rawBytes[68] = 0x00;
        rawBytes[69] = 0x00;

        // Act
        var parameter = systemDump.GetPedalParameter();
        var min = systemDump.GetPedalMin();
        var mid = systemDump.GetPedalMid();
        var max = systemDump.GetPedalMax();

        // Assert
        parameter.Should().Be(10, "Volume parameter");
        min.Should().Be(0, "Minimum at 0%");
        mid.Should().Be(50, "Midpoint at 50%");
        max.Should().Be(100, "Maximum at 100%");
    }
}
