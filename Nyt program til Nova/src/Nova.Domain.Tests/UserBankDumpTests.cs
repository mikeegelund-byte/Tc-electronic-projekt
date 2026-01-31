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
        var sysex = new byte[521];
        sysex[0] = 0xF0;
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F;
        sysex[4] = 0x00;
        sysex[5] = 0x63;
        sysex[6] = 0x20;
        sysex[7] = 0x01;
        sysex[8] = (byte)number;
        var name = $"Preset {number}            ";
        for (int i = 0; i < 24; i++)
            sysex[9 + i] = (byte)(i < name.Length ? name[i] : ' ');
        sysex[520] = 0xF7;

        return Preset.FromSysEx(sysex).Value;
    }
}
