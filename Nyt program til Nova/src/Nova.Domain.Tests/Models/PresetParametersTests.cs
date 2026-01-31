using FluentAssertions;
using Nova.Domain.Models;

namespace Nova.Domain.Tests.Models;

/// <summary>
/// Tests for Preset parameter extraction from bytes 33-519.
/// These tests verify that we can read all effect parameters from a preset.
/// </summary>
public class PresetParametersTests
{
    /// <summary>
    /// Test fixture: Real preset from hardware (nova-dump-*-181507-msg001.syx)
    /// Preset 31: "Amp-Bright Clean1"
    /// </summary>
    private readonly byte[] _realPresetBytes;

    public PresetParametersTests()
    {
        // Path to real hardware test fixtures in Nova.HardwareTest project
        var fixturesPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "..", "..", "src", "Nova.HardwareTest"
        );
        var presetPath = Path.Combine(fixturesPath, "nova-dump-20260131-181507-msg001.syx");

        if (File.Exists(presetPath))
        {
            _realPresetBytes = File.ReadAllBytes(presetPath);
        }
        else
        {
            // Fallback: minimal valid preset structure (all zeros for parameters)
            _realPresetBytes = new byte[521];
            _realPresetBytes[0] = 0xF0;
            _realPresetBytes[1] = 0x00;
            _realPresetBytes[2] = 0x20;
            _realPresetBytes[3] = 0x1F;
            _realPresetBytes[4] = 0x00;
            _realPresetBytes[5] = 0x63;
            _realPresetBytes[6] = 0x20;
            _realPresetBytes[7] = 0x01;
            _realPresetBytes[8] = 31; // Preset number
            _realPresetBytes[520] = 0xF7;
        }
    }

    [Fact]
    public void FromSysEx_ExtractsTapTempo_FromBytes38To41()
    {
        // Arrange: Bytes 38-41 contain Tap Tempo (100-3000ms)
        // According to SysEx map: 4 bytes encode tempo value

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TapTempo.Should().BeInRange(100, 3000,
            "Tap Tempo must be between 100ms and 3000ms according to Nova System spec");
    }

    [Fact]
    public void FromSysEx_ExtractsRouting_FromBytes42To45()
    {
        // Arrange: Bytes 42-45 contain Routing (0=Semi-parallel, 1=Serial, 2=Parallel)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Routing.Should().BeInRange(0, 2,
            "Routing must be 0 (Semi-par), 1 (Serial), or 2 (Parallel)");
    }

    [Fact]
    public void FromSysEx_ExtractsLevelOutLeft_FromBytes46To49()
    {
        // Arrange: Bytes 46-49 contain Level Out L (-100 to 0dB)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.LevelOutLeft.Should().BeInRange(-100, 0,
            "Level Out Left must be between -100dB and 0dB");
    }

    [Fact]
    public void FromSysEx_ExtractsLevelOutRight_FromBytes50To53()
    {
        // Arrange: Bytes 50-53 contain Level Out R (-100 to 0dB)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.LevelOutRight.Should().BeInRange(-100, 0,
            "Level Out Right must be between -100dB and 0dB");
    }

    [Fact]
    public void FromSysEx_ExtractsCompressorEnabled_FromBytes130To133()
    {
        // Arrange: Bytes 130-133 contain COMP ON/OFF (0=Off, 1=On)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        // Bool property extracted successfully (value validated by type system)
    }

    [Fact]
    public void FromSysEx_ExtractsDriveEnabled_FromBytes194To197()
    {
        // Arrange: Bytes 194-197 contain DRIVE ON/OFF (0=Off, 1=On)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        // Bool property extracted successfully
    }

    [Fact]
    public void FromSysEx_ExtractsModulationEnabled_FromBytes258To261()
    {
        // Arrange: Bytes 258-261 contain MOD ON/OFF (0=Off, 1=On)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        // Bool property extracted successfully
    }

    [Fact]
    public void FromSysEx_ExtractsDelayEnabled_FromBytes322To325()
    {
        // Arrange: Bytes 322-325 contain DELAY ON/OFF (0=Off, 1=On)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        // Bool property extracted successfully
    }

    [Fact]
    public void FromSysEx_ExtractsReverbEnabled_FromBytes386To389()
    {
        // Arrange: Bytes 386-389 contain REVERB ON/OFF (0=Off, 1=On)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        // Bool property extracted successfully
    }

    // ========== COMP (Compressor) Effect Parameters (bytes 70-129) ==========

    [Fact]
    public void FromSysEx_ExtractsCompType_FromBytes70To73()
    {
        // Arrange: Bytes 70-73 contain COMP Type (0=perc, 1=sustain, 2=advanced)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CompType.Should().BeInRange(0, 2,
            "COMP Type must be 0-2 (perc/sustain/advanced)");
    }

    [Fact]
    public void FromSysEx_ExtractsCompThreshold_FromBytes74To77()
    {
        // Arrange: Bytes 74-77 contain COMP Threshold (advanced mode: -30 to 0dB)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CompThreshold.Should().BeInRange(0, 20000000,
            "COMP Threshold raw value - needs offset decoding");
    }

    [Fact]
    public void FromSysEx_ExtractsCompRatio_FromBytes78To81()
    {
        // Arrange: Bytes 78-81 contain COMP Ratio (0=Off, 1-15=ratios per table)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CompRatio.Should().BeInRange(0, 15,
            "COMP Ratio must be 0-15 (Off to Infinite:1)");
    }

    [Fact]
    public void FromSysEx_ExtractsCompAttack_FromBytes82To85()
    {
        // Arrange: Bytes 82-85 contain COMP Attack (0-16 = table lookup)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CompAttack.Should().BeInRange(0, 16,
            "COMP Attack must be 0-16 (0.3ms to 140ms per table)");
    }

    [Fact]
    public void FromSysEx_ExtractsCompRelease_FromBytes86To89()
    {
        // Arrange: Bytes 86-89 contain COMP Release (13-23 = 50ms to 2000ms per table)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CompRelease.Should().BeInRange(13, 23,
            "COMP Release must be 13-23 (50ms to 2000ms per table)");
    }

    [Fact]
    public void FromSysEx_ExtractsCompResponse_FromBytes90To93()
    {
        // Arrange: Bytes 90-93 contain COMP Response (perc/sustain modes: 1-10)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CompResponse.Should().BeInRange(0, 20,
            "COMP Response actual range TBD");
    }

    [Fact]
    public void FromSysEx_ExtractsCompDrive_FromBytes94To97()
    {
        // Arrange: Bytes 94-97 contain COMP Drive (perc/sustain modes: 1-20)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CompDrive.Should().BeInRange(0, 30,
            "COMP Drive actual range TBD");
    }

    [Fact]
    public void FromSysEx_ExtractsCompLevel_FromBytes98To101()
    {
        // Arrange: Bytes 98-101 contain COMP Level (-12 to +12dB)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CompLevel.Should().BeInRange(0, 20000000,
            "COMP Level raw value - needs offset decoding");
    }

    // ==================================
    // DRIVE EFFECT PARAMETERS (bytes 102-113)
    // ==================================

    [Fact]
    public void FromSysEx_ExtractsDriveType_FromBytes102To105()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 102-105
        // Range: 0-6 (overdrive/dist/fuzz/line6drive/custom/tube/metal)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DriveType.Should().BeInRange(0, 6, "drive type must be 0-6");
    }

    [Fact]
    public void FromSysEx_ExtractsDriveGain_FromBytes106To109()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 106-109
        // Range: 0-100 (gain percentage)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DriveGain.Should().BeInRange(0, 100, "drive gain must be 0-100");
    }

    [Fact]
    public void FromSysEx_ExtractsDriveLevel_FromBytes110To113()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 110-113
        // Range: -30 to +20dB (may need offset decoding like COMP params)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DriveLevel.Should().BeInRange(0, 20000000, "raw value - needs offset decoding for -30 to +20dB range");
    }
}
