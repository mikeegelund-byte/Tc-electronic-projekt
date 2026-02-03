using FluentAssertions;
using Nova.Domain.Models;

namespace Nova.Domain.Tests;

public class SystemDumpTests
{
    [Fact]
    public void FromSysEx_ValidSystemDump_ParsesCorrectly()
    {
        // Arrange - minimal valid System Dump
        var sysex = new byte[526];
        sysex[0] = 0xF0;
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F; // TC Electronic
        sysex[4] = 0x00; // Device ID
        sysex[5] = 0x63; // Nova System Model
        sysex[6] = 0x20; // Message ID: Dump
        sysex[7] = 0x02; // Data Type: System Dump
        sysex[525] = 0xF7;

        // Act
        var result = SystemDump.FromSysEx(sysex);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.RawSysEx.Should().HaveCount(526);
        result.Value.RawSysEx.Should().BeEquivalentTo(sysex);
    }

    [Fact]
    public void FromSysEx_DoubleF7_NormalizesTo526Bytes()
    {
        // Arrange - valid dump with double F7 at end
        var sysex = new byte[527];
        sysex[0] = 0xF0;
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F;
        sysex[4] = 0x00;
        sysex[5] = 0x63;
        sysex[6] = 0x20;
        sysex[7] = 0x02;
        sysex[525] = 0xF7;
        sysex[526] = 0xF7;

        // Act
        var result = SystemDump.FromSysEx(sysex);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.RawSysEx.Should().HaveCount(526);
        result.Value.RawSysEx[^1].Should().Be(0xF7);
    }

    [Fact]
    public void FromSysEx_InvalidLength_ReturnsFailure()
    {
        // Arrange
        var sysex = new byte[500]; // Wrong length

        // Act
        var result = SystemDump.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("526"));
    }

    [Fact]
    public void FromSysEx_MissingF0_ReturnsFailure()
    {
        // Arrange
        var sysex = new byte[526];
        sysex[0] = 0xFF; // Wrong start
        sysex[525] = 0xF7;

        // Act
        var result = SystemDump.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("F0"));
    }

    [Fact]
    public void FromSysEx_WrongDataType_ReturnsFailure()
    {
        // Arrange
        var sysex = new byte[526];
        sysex[0] = 0xF0;
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F;
        sysex[4] = 0x00;
        sysex[5] = 0x63;
        sysex[6] = 0x20;
        sysex[7] = 0x01; // Wrong: 0x01 = Preset, should be 0x02 = System
        sysex[525] = 0xF7;

        // Act
        var result = SystemDump.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("0x02"));
    }

    [Fact]
    public void ToSysEx_ValidSystemDump_ReturnsOriginalBytes()
    {
        // Arrange
        var originalSysex = CreateValidSystemDumpSysEx();
        var systemDump = SystemDump.FromSysEx(originalSysex).Value;

        // Act
        var result = systemDump.ToSysEx();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(originalSysex);
    }

    private static byte[] CreateValidSystemDumpSysEx()
    {
        var sysex = new byte[526];
        sysex[0] = 0xF0;
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F;
        sysex[4] = 0x00;
        sysex[5] = 0x63;
        sysex[6] = 0x20;
        sysex[7] = 0x02; // System Dump data type
        sysex[525] = 0xF7;
        return sysex;
    }
}
