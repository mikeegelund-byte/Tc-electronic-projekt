using FluentAssertions;
using Nova.Domain.Models;

namespace Nova.Domain.Tests;

public class SystemDumpPedalMappingTests
{
    private SystemDump CreateValidSystemDump()
    {
        var validSystemDump = CreateValidSystemDumpBytes();
        return SystemDump.FromSysEx(validSystemDump).Value;
    }

    [Fact]
    public void GetPedalParameter_ReadsNibbleIndex6()
    {
        // Arrange
        var bytes = CreateValidSystemDumpBytes();
        WriteNibble(bytes, 6, 42);
        var systemDump = SystemDump.FromSysEx(bytes).Value;

        // Act
        var parameter = systemDump.GetPedalParameter();

        // Assert
        parameter.Should().Be(42);
    }

    [Fact]
    public void GetPedalMin_ReadsNibbleIndex3()
    {
        // Arrange
        var bytes = CreateValidSystemDumpBytes();
        WriteNibble(bytes, 3, 25);
        var systemDump = SystemDump.FromSysEx(bytes).Value;

        // Act
        var min = systemDump.GetPedalMin();

        // Assert
        min.Should().Be(25);
    }

    [Fact]
    public void GetPedalMid_ReadsNibbleIndex4()
    {
        // Arrange
        var bytes = CreateValidSystemDumpBytes();
        WriteNibble(bytes, 4, 50);
        var systemDump = SystemDump.FromSysEx(bytes).Value;

        // Act
        var mid = systemDump.GetPedalMid();

        // Assert
        mid.Should().Be(50);
    }

    [Fact]
    public void GetPedalMax_ReadsNibbleIndex5()
    {
        // Arrange
        var bytes = CreateValidSystemDumpBytes();
        WriteNibble(bytes, 5, 75);
        var systemDump = SystemDump.FromSysEx(bytes).Value;

        // Act
        var max = systemDump.GetPedalMax();

        // Assert
        max.Should().Be(75);
    }

    [Fact]
    public void GetPedalValues_WithTypicalConfiguration_ReturnsExpectedValues()
    {
        // Arrange - typical expression pedal setup
        var bytes = CreateValidSystemDumpBytes();
        WriteNibble(bytes, 6, 10); // parameter
        WriteNibble(bytes, 3, 0);  // min
        WriteNibble(bytes, 4, 50); // mid
        WriteNibble(bytes, 5, 100); // max
        var systemDump = SystemDump.FromSysEx(bytes).Value;

        // Act
        var parameter = systemDump.GetPedalParameter();
        var min = systemDump.GetPedalMin();
        var mid = systemDump.GetPedalMid();
        var max = systemDump.GetPedalMax();

        // Assert
        parameter.Should().Be(10);
        min.Should().Be(0);
        mid.Should().Be(50);
        max.Should().Be(100);
    }

    [Fact]
    public void UpdatePedalParameter_ValidValue_UpdatesBytes()
    {
        // Arrange
        var systemDump = CreateValidSystemDump();

        // Act
        var result = systemDump.UpdatePedalParameter(42);

        // Assert
        result.IsSuccess.Should().BeTrue();
        systemDump.GetPedalParameter().Should().Be(42);
    }

    [Fact]
    public void UpdatePedalParameter_OutOfRange_Fails()
    {
        // Arrange
        var systemDump = CreateValidSystemDump();

        // Act
        var resultNegative = systemDump.UpdatePedalParameter(-1);
        var resultTooHigh = systemDump.UpdatePedalParameter(128);

        // Assert
        resultNegative.IsFailed.Should().BeTrue();
        resultTooHigh.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void UpdatePedalMin_ValidValue_UpdatesBytes()
    {
        // Arrange
        var systemDump = CreateValidSystemDump();

        // Act
        var result = systemDump.UpdatePedalMin(25);

        // Assert
        result.IsSuccess.Should().BeTrue();
        systemDump.GetPedalMin().Should().Be(25);
    }

    [Fact]
    public void UpdatePedalMin_OutOfRange_Fails()
    {
        // Arrange
        var systemDump = CreateValidSystemDump();

        // Act
        var resultNegative = systemDump.UpdatePedalMin(-1);
        var resultTooHigh = systemDump.UpdatePedalMin(101);

        // Assert
        resultNegative.IsFailed.Should().BeTrue();
        resultTooHigh.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void UpdatePedalMid_ValidValue_UpdatesBytes()
    {
        // Arrange
        var systemDump = CreateValidSystemDump();

        // Act
        var result = systemDump.UpdatePedalMid(50);

        // Assert
        result.IsSuccess.Should().BeTrue();
        systemDump.GetPedalMid().Should().Be(50);
    }

    [Fact]
    public void UpdatePedalMax_ValidValue_UpdatesBytes()
    {
        // Arrange
        var systemDump = CreateValidSystemDump();

        // Act
        var result = systemDump.UpdatePedalMax(100);

        // Assert
        result.IsSuccess.Should().BeTrue();
        systemDump.GetPedalMax().Should().Be(100);
    }

    [Fact]
    public void UpdatePedalMapping_Roundtrip_PreservesValues()
    {
        // Arrange
        var systemDump = CreateValidSystemDump();

        // Act
        systemDump.UpdatePedalParameter(20);
        systemDump.UpdatePedalMin(10);
        systemDump.UpdatePedalMid(60);
        systemDump.UpdatePedalMax(90);

        // Assert
        systemDump.GetPedalParameter().Should().Be(20);
        systemDump.GetPedalMin().Should().Be(10);
        systemDump.GetPedalMid().Should().Be(60);
        systemDump.GetPedalMax().Should().Be(90);
    }

    private static byte[] CreateValidSystemDumpBytes()
    {
        var validSystemDump = new byte[526];
        validSystemDump[0] = 0xF0;  // SysEx start
        validSystemDump[1] = 0x00;  // TC Electronic
        validSystemDump[2] = 0x20;
        validSystemDump[3] = 0x1F;
        validSystemDump[4] = 0x04;  // Device ID
        validSystemDump[5] = 0x63;  // Nova System model ID
        validSystemDump[6] = 0x20;  // Message ID (Dump)
        validSystemDump[7] = 0x02;  // Data Type (System Dump)
        validSystemDump[525] = 0xF7;  // SysEx end
        return validSystemDump;
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
