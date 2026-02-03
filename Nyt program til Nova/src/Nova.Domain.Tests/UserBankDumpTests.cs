using FluentAssertions;
using Nova.Domain.Models;

namespace Nova.Domain.Tests;

public class UserBankDumpTests
{
    [Fact]
    public void Empty_Creates60EmptySlots()
    {
        // Act
        var bank = UserBankDump.Empty();

        // Assert
        bank.Presets.Should().HaveCount(60);
        bank.Presets.Should().AllSatisfy(p => p.Should().BeNull());
    }

    [Fact]
    public void WithPreset_ValidIndex_SetsPreset()
    {
        // Arrange
        var bank = UserBankDump.Empty();
        var preset = CreateTestPreset(31);

        // Act
        var result = bank.WithPreset(31, preset);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Presets[0].Should().NotBeNull();
        result.Value.Presets[0]!.Number.Should().Be(31);
    }

    [Theory]
    [InlineData(30)]  // Below range
    [InlineData(91)]  // Above range
    public void WithPreset_InvalidPresetNumber_ReturnsFailure(int number)
    {
        // Arrange
        var bank = UserBankDump.Empty();
        var preset = CreateTestPreset(number);

        // Act
        var result = bank.WithPreset(number, preset);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("31") && e.Message.Contains("90"));
    }

    [Fact]
    public void WithPreset_MismatchedNumber_ReturnsFailure()
    {
        // Arrange
        var bank = UserBankDump.Empty();
        var preset = CreateTestPreset(45);

        // Act - trying to put preset #45 in slot for preset #50
        var result = bank.WithPreset(50, preset);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("mismatch"));
    }

    [Fact]
    public void FromPresets_AllPresets_CreatesFullBank()
    {
        // Arrange - create 60 presets
        var presets = Enumerable.Range(31, 60)
            .Select(n => CreateTestPreset(n))
            .ToList();

        // Act
        var result = UserBankDump.FromPresets(presets);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Presets.Should().HaveCount(60);
        result.Value.Presets.Should().AllSatisfy(p => p.Should().NotBeNull());
    }

    private static Preset CreateTestPreset(int number)
    {
        var sysex = CreateValidPresetSysEx(number);
        return Preset.FromSysEx(sysex).Value;
    }

    /// <summary>
    /// Creates a valid preset SysEx byte array with all parameters at safe values.
    /// </summary>
    private static byte[] CreateValidPresetSysEx(int presetNumber)
    {
        var sysex = new byte[520];
        sysex[0] = 0xF0;                                    // SysEx start
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F; // TC Electronic ID
        sysex[4] = 0x00;                                    // Device ID
        sysex[5] = 0x63;                                    // Nova System Model ID
        sysex[6] = 0x20;                                    // Message ID (Dump)
        sysex[7] = 0x01;                                    // Data Type (Preset)
        sysex[8] = (byte)presetNumber;                      // Preset number

        // Preset name (24 bytes ASCII)
        var name = $"Preset {presetNumber}".PadRight(24);
        for (int i = 0; i < 24; i++)
            sysex[9 + i] = (byte)name[i];

        // Set safe default values for parameters that need validation
        Encode4ByteValue(sysex, 38, 500);   // TapTempo: 500ms (100-3000)
        Encode4ByteValue(sysex, 42, 0);     // Routing: 0 (0-2)
        Encode4ByteValue(sysex, 86, 15);    // CompRelease: 15 (13-23)
        Encode4ByteValue(sysex, 330, 50);   // ReverbDecay: 50 (1-200)
        Encode4ByteValue(sysex, 418, 8);    // EqWidth1: 8 (5-12)
        Encode4ByteValue(sysex, 430, 8);    // EqWidth2: 8 (5-12)
        Encode4ByteValue(sysex, 442, 8);    // EqWidth3: 8 (5-12)

        sysex[519] = 0xF7; // SysEx end
        return sysex;
    }

    private static void Encode4ByteValue(byte[] sysex, int offset, int value)
    {
        sysex[offset] = (byte)(value & 0x7F);
        sysex[offset + 1] = (byte)((value >> 7) & 0x7F);
        sysex[offset + 2] = (byte)((value >> 14) & 0x7F);
        sysex[offset + 3] = (byte)((value >> 21) & 0x7F);
    }
}
