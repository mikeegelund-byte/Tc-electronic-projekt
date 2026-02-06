using FluentAssertions;
using Nova.Domain.Models;

namespace Nova.Domain.Tests.Models;

/// <summary>
/// Tests for offset decoding of signed dB parameters in Preset.
/// Validates that raw encoded values are correctly converted to actual dB values.
/// </summary>
public class PresetOffsetDecodingTests
{
    private readonly byte[] _realPresetBytes;

    public PresetOffsetDecodingTests()
    {
        // Load real hardware fixture
        var fixturePath = Path.Combine(
            "..", "..", "..", "..", "Nova.HardwareTest", "Dumps",
            "nova-dump-20260131-181507-msg001.syx");
        var fullPath = Path.GetFullPath(fixturePath);
        _realPresetBytes = File.ReadAllBytes(fullPath);
    }

    // ========================================
    // COMP BLOCK SIGNED dB PARAMETERS
    // ========================================

    [Fact]
    public void FromSysEx_CompLevel_DecodesCorrectly()
    {
        // Arrange: CompLevel uses large offset (2^24)
        // Hardware bytes [116, 127, 127, 7] → raw 16777204 → -12dB

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CompLevel.Should().Be(-12,
            "CompLevel should decode from raw 16777204 to -12dB");
    }

    [Fact]
    public void FromSysEx_CompThreshold_DecodesCorrectly()
    {
        // Arrange: CompThreshold uses large offset (2^24)
        // Hardware bytes [104, 127, 127, 7] → raw 16777192 → -24dB

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CompThreshold.Should().Be(-24,
            "CompThreshold should decode from raw 16777192 to -24dB");
    }

    [Fact]
    public void FromSysEx_CompDrive_DecodesCorrectly()
    {
        // Arrange: CompDrive likely uses simple offset
        // Range: 0 to 12dB (unsigned, no offset needed)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CompDrive.Should().BeInRange(0, 12,
            "CompDrive is 0-12dB (unsigned)");
    }

    // ========================================
    // DRIVE BLOCK SIGNED dB PARAMETERS
    // ========================================

    [Fact]
    public void FromSysEx_DriveLevel_DecodesCorrectly()
    {
        // Arrange: DriveLevel uses simple offset
        // Hardware bytes [0, 0, 0, 0] → raw 0 → -30dB (minimum)
        // Range: -30 to +20dB

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DriveLevel.Should().Be(-30,
            "DriveLevel raw 0 should decode to -30dB (minimum)");
    }

    // ========================================
    // BOOST BLOCK SIGNED dB PARAMETERS
    // ========================================

    [Fact]
    public void FromSysEx_BoostLevel_DecodesCorrectly()
    {
        // Arrange: BoostLevel likely uses large offset (2^24)
        // Range: -12 to +12dB (zero in middle)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.BoostLevel.Should().BeInRange(0, 10,
            "BoostLevel is 0 to 10dB");
    }

    // ========================================
    // GLOBAL LEVEL PARAMETERS
    // ========================================

    [Fact]
    public void FromSysEx_LevelOutLeft_DecodesCorrectly()
    {
        // Arrange: LevelOutLeft uses simple offset
        // Hardware bytes [0, 0, 0, 0] → raw 0 → -100dB (minimum)
        // Range: -100 to 0dB

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.LevelOutLeft.Should().Be(-100,
            "LevelOutLeft raw 0 should decode to -100dB (minimum)");
    }

    [Fact]
    public void FromSysEx_LevelOutRight_DecodesCorrectly()
    {
        // Arrange: LevelOutRight uses simple offset
        // Hardware bytes [0, 0, 0, 0] → raw 0 → -100dB (minimum)
        // Range: -100 to 0dB

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.LevelOutRight.Should().Be(-100,
            "LevelOutRight raw 0 should decode to -100dB (minimum)");
    }

    // ========================================
    // REVERB BLOCK SIGNED dB PARAMETERS
    // ========================================

    [Fact]
    public void FromSysEx_ReverbHiLevel_DecodesCorrectly()
    {
        // Arrange: ReverbHiLevel uses large offset (2^24)
        // Range: -25 to +25dB (zero in middle)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ReverbHiLevel.Should().BeInRange(-25, 25,
            "ReverbHiLevel is -25 to +25dB");
    }

    [Fact]
    public void FromSysEx_ReverbLoLevel_DecodesCorrectly()
    {
        // Arrange: ReverbLoLevel uses large offset (2^24)
        // Range: -25 to +25dB (zero in middle)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ReverbLoLevel.Should().BeInRange(-25, 25,
            "ReverbLoLevel is -25 to +25dB");
    }

    [Fact]
    public void FromSysEx_ReverbRoomLevel_DecodesCorrectly()
    {
        // Arrange: ReverbRoomLevel uses simple offset
        // Range: -100 to 0dB (zero at end)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ReverbRoomLevel.Should().BeInRange(-100, 0,
            "ReverbRoomLevel is -100 to 0dB");
    }

    [Fact]
    public void FromSysEx_ReverbLevel_DecodesCorrectly()
    {
        // Arrange: ReverbLevel uses simple offset
        // Range: -100 to 0dB (zero at end)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ReverbLevel.Should().BeInRange(-100, 0,
            "ReverbLevel is -100 to 0dB");
    }

    // ========================================
    // EQ/GATE BLOCK SIGNED dB PARAMETERS
    // ========================================

    [Fact]
    public void FromSysEx_GateThreshold_DecodesCorrectly()
    {
        // Arrange: GateThreshold uses simple offset
        // Range: -90 to 0dB (zero at end)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.GateThreshold.Should().BeInRange(-90, 0,
            "GateThreshold is -90 to 0dB");
    }

    [Fact]
    public void FromSysEx_EqGain1_DecodesCorrectly()
    {
        // Arrange: EqGain1 uses large offset (2^24)
        // Range: -12 to +12dB (zero in middle)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.EqGain1.Should().BeInRange(-12, 12,
            "EqGain1 is -12 to +12dB");
    }

    [Fact]
    public void FromSysEx_EqGain2_DecodesCorrectly()
    {
        // Arrange: EqGain2 uses large offset (2^24)
        // Range: -12 to +12dB (zero in middle)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.EqGain2.Should().BeInRange(-12, 12,
            "EqGain2 is -12 to +12dB");
    }

    [Fact]
    public void FromSysEx_EqGain3_DecodesCorrectly()
    {
        // Arrange: EqGain3 uses large offset (2^24)
        // Range: -12 to +12dB (zero in middle)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.EqGain3.Should().BeInRange(-12, 12,
            "EqGain3 is -12 to +12dB");
    }

    // ========================================
    // PITCH BLOCK SIGNED dB PARAMETERS
    // ========================================

    [Fact]
    public void FromSysEx_PitchLevel1_DecodesCorrectly()
    {
        // Arrange: PitchLevel1 uses large offset (2^24)
        // Range: -12 to +12dB (zero in middle)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PitchLevel1.Should().BeInRange(-100, 0,
            "PitchLevel1 is -100 to 0dB");
    }

    [Fact]
    public void FromSysEx_PitchLevel2_DecodesCorrectly()
    {
        // Arrange: PitchLevel2 uses large offset (2^24)
        // Range: -12 to +12dB (zero in middle)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PitchLevel2.Should().BeInRange(-100, 0,
            "PitchLevel2 is -100 to 0dB");
    }

    // ========================================
    // MOD BLOCK SIGNED PARAMETERS
    // ========================================

    [Fact]
    public void FromSysEx_ModFeedback_DecodesCorrectly()
    {
        // Arrange: ModFeedback uses large offset (2^24)
        // Range: -100 to +100% (zero in middle)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ModFeedback.Should().BeInRange(-100, 100,
            "ModFeedback is -100 to +100%");
    }
}
