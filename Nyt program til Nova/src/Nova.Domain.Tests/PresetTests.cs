using FluentAssertions;
using Nova.Domain.Models;

namespace Nova.Domain.Tests;

public class PresetTests
{
    [Fact]
    public void FromSysEx_ValidPreset_ParsesCorrectly()
    {
        // Arrange - minimal valid preset SysEx
        var sysex = new byte[520];
        sysex[0] = 0xF0;                    // SysEx start
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F; // TC Electronic ID
        sysex[4] = 0x00;                    // Device ID
        sysex[5] = 0x63;                    // Nova System Model ID
        sysex[6] = 0x20;                    // Message ID (Dump)
        sysex[7] = 0x01;                    // Data Type (Preset)
        sysex[8] = 0x1F;                    // Preset number (31)
        // Bytes 9-32 = preset name (24 ASCII chars)
        var name = "Test Preset             ";
        for (int i = 0; i < 24; i++)
            sysex[9 + i] = (byte)name[i];
        // ... rest of parameters ...
        sysex[519] = 0xF7;                  // SysEx end

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Number.Should().Be(31);
        result.Value.Name.Should().Be("Test Preset");
    }

    [Fact]
    public void FromSysEx_InvalidLength_ReturnsFailure()
    {
        // Arrange
        var sysex = new byte[100]; // Too short

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("520 bytes"));
    }

    [Fact]
    public void FromSysEx_LegacyDoubleF7Length521_ParsesCorrectly()
    {
        // Arrange - 521 bytes with double F7 (legacy capture)
        var sysex = new byte[521];
        sysex[0] = 0xF0;
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F;
        sysex[4] = 0x00;
        sysex[5] = 0x63;
        sysex[6] = 0x20;
        sysex[7] = 0x01;
        sysex[8] = 0x1F;
        var name = "Test Preset             ";
        for (int i = 0; i < 24; i++)
            sysex[9 + i] = (byte)name[i];
        sysex[519] = 0xF7;
        sysex[520] = 0xF7;

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.RawSysEx.Should().HaveCount(520);
    }

    [Fact]
    public void FromSysEx_MissingF0_ReturnsFailure()
    {
        // Arrange
        var sysex = new byte[520];
        sysex[0] = 0x00; // Wrong start
        sysex[519] = 0xF7;

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("F0"));
    }

    [Fact]
    public void FromSysEx_WrongModelId_ReturnsFailure()
    {
        // Arrange
        var sysex = new byte[520];
        sysex[0] = 0xF0;
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F; // TC Electronic
        sysex[4] = 0x00; // Device ID
        sysex[5] = 0xFF; // Wrong model ID (should be 0x63)
        sysex[6] = 0x20; // Message ID
        sysex[7] = 0x01; // Data Type
        sysex[519] = 0xF7;

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("0x63") || e.Message.Contains("Nova System"));
    }

    [Fact]
    public void ToSysEx_ValidPreset_ReturnsOriginalBytes()
    {
        // Arrange - create preset from SysEx
        var originalSysex = CreateValidPresetSysEx(45, "Test Preset");
        var preset = Preset.FromSysEx(originalSysex).Value;

        // Act
        var result = preset.ToSysEx();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(originalSysex);
    }

    [Fact]
    public void ToSysEx_PresetRoundtrip_PreservesData()
    {
        // Arrange
        var sysex1 = CreateValidPresetSysEx(50, "Original Name");
        var preset1 = Preset.FromSysEx(sysex1).Value;

        // Act - serialize back to SysEx
        var sysex2 = preset1.ToSysEx().Value;
        var preset2 = Preset.FromSysEx(sysex2).Value;

        // Assert - data preserved
        preset2.Number.Should().Be(50);
        preset2.Name.Should().Be("Original Name");
        preset2.RawSysEx.Should().BeEquivalentTo(sysex1);
    }

    private static byte[] CreateValidPresetSysEx(int number, string name)
    {
        var sysex = new byte[520];
        sysex[0] = 0xF0;
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F;
        sysex[4] = 0x00;
        sysex[5] = 0x63;
        sysex[6] = 0x20;
        sysex[7] = 0x01;
        sysex[8] = (byte)number;

        // Encode name (24 bytes, space-padded)
        var nameBytes = System.Text.Encoding.ASCII.GetBytes(name.PadRight(24));
        Array.Copy(nameBytes, 0, sysex, 9, 24);

        sysex[519] = 0xF7;
        return sysex;
    }
}
