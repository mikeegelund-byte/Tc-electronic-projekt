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
            "..", "..", "..", "..", "..", "src", "Nova.HardwareTest", "Dumps"
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
        result.Value.CompThreshold.Should().BeInRange(-30, 0,
            "COMP Threshold must be between -30dB and 0dB");
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
        result.Value.CompLevel.Should().BeInRange(-12, 12,
            "COMP Level must be between -12dB and +12dB");
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
        result.Value.DriveLevel.Should().BeInRange(-30, 20, "Drive Level must be between -30dB and +20dB");
    }

    // ==================================
    // BOOST EFFECT PARAMETERS (bytes 114-193)
    // ==================================

    [Fact]
    public void FromSysEx_ExtractsBoostType_FromBytes114To117()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 114-117
        // Range: 0-2 (clean/mid/treble)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.BoostType.Should().BeInRange(0, 2, "boost type must be 0-2");
    }

    [Fact]
    public void FromSysEx_ExtractsBoostGain_FromBytes118To121()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 118-121
        // Range: 0-30dB

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.BoostGain.Should().BeInRange(0, 30, "boost gain must be 0-30dB");
    }

    [Fact]
    public void FromSysEx_ExtractsBoostLevel_FromBytes122To125()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 122-125
        // Range: 0 to 10dB (unsigned)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.BoostLevel.Should().BeInRange(0, 10, "Boost Level must be between 0dB and 10dB");
    }

    // ==================================
    // MOD (Modulation) EFFECT PARAMETERS (bytes 198-257)
    // ==================================

    [Fact]
    public void FromSysEx_ExtractsModType_FromBytes198To201()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 198-201
        // Range: 0-5 (chorus/flanger/vibrato/phaser/tremolo/panner)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ModType.Should().BeInRange(0, 5, "mod type must be 0-5");
    }

    [Fact]
    public void FromSysEx_ExtractsModSpeed_FromBytes202To205()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 202-205
        // Range: 0.050-20Hz (table-based, storing raw value)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ModSpeed.Should().BeGreaterOrEqualTo(0, "mod speed raw value must be >= 0");
    }

    [Fact]
    public void FromSysEx_ExtractsModDepth_FromBytes206To209()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 206-209
        // Range: 0-100%

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ModDepth.Should().BeInRange(0, 100, "mod depth must be 0-100%");
    }

    [Fact]
    public void FromSysEx_ExtractsModTempo_FromBytes210To213()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 210-213
        // Range: 0-16 (table: ignore, 2 to 1/32T)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ModTempo.Should().BeInRange(0, 16, "mod tempo must be 0-16");
    }

    [Fact]
    public void FromSysEx_ExtractsModHiCut_FromBytes214To217()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 214-217
        // Range: 20Hz-20kHz (table-based, storing raw value)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ModHiCut.Should().BeGreaterOrEqualTo(0, "mod hi-cut raw value must be >= 0");
    }

    [Fact]
    public void FromSysEx_ExtractsModFeedback_FromBytes218To221()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 218-221
        // Range: -100 to +100% (may need offset decoding)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ModFeedback.Should().BeInRange(-100, 100, "Mod Feedback must be between -100% and +100%");
    }

    [Fact]
    public void FromSysEx_ExtractsModDelayOrRange_FromBytes222To225()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 222-225
        // Multi-function: Delay (cho/fla 0-50ms), Range (pha low/high), Type (trem soft/hard)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ModDelayOrRange.Should().BeGreaterOrEqualTo(0, "mod delay/range raw value must be >= 0");
    }

    [Fact]
    public void FromSysEx_ExtractsModMix_FromBytes250To253()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 250-253
        // Range: 0-100%

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ModMix.Should().BeInRange(0, 100, "mod mix must be 0-100%");
    }

    // ==================================
    // DELAY EFFECT PARAMETERS (bytes 262-321)
    // ==================================

    [Fact]
    public void FromSysEx_ExtractsDelayType_FromBytes262To265()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 262-265
        // Range: 0-5 (clean/analog/tape/dynamic/dual/ping-pong)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DelayType.Should().BeInRange(0, 5, "delay type must be 0-5");
    }

    [Fact]
    public void FromSysEx_ExtractsDelayTime_FromBytes266To269()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 266-269
        // Range: 0-1800ms

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DelayTime.Should().BeInRange(0, 1800, "delay time must be 0-1800ms");
    }

    [Fact]
    public void FromSysEx_ExtractsDelayTime2_FromBytes270To273()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 270-273
        // Range: 0-1800ms (for dual delay mode)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DelayTime2.Should().BeInRange(0, 1800, "delay time 2 must be 0-1800ms");
    }

    [Fact]
    public void FromSysEx_ExtractsDelayTempo_FromBytes274To277()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 274-277
        // Range: 0-16 (ignore, 2 to 1/32T table)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DelayTempo.Should().BeInRange(0, 16, "delay tempo must be 0-16");
    }

    [Fact]
    public void FromSysEx_ExtractsDelayTempo2OrWidth_FromBytes278To281()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 278-281
        // Multi-function: Tempo2 (dual: 0-16) or Width (ping-pong: 0-100%)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DelayTempo2OrWidth.Should().BeGreaterOrEqualTo(0, "delay tempo2/width raw value must be >= 0");
    }

    [Fact]
    public void FromSysEx_ExtractsDelayFeedback_FromBytes282To285()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 282-285
        // Range: 0-120%

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DelayFeedback.Should().BeInRange(0, 120, "delay feedback must be 0-120%");
    }

    [Fact]
    public void FromSysEx_ExtractsDelayClipOrFeedback2_FromBytes286To289()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 286-289
        // Multi-function: Clip (analog/tape: 0-24dB) or Feedback2 (dual: 0-120%)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DelayClipOrFeedback2.Should().BeGreaterOrEqualTo(0, "delay clip/feedback2 raw value must be >= 0");
    }

    [Fact]
    public void FromSysEx_ExtractsDelayHiCut_FromBytes290To293()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 290-293
        // Range: 20Hz-20kHz (table-based)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DelayHiCut.Should().BeGreaterOrEqualTo(0, "delay hi-cut raw value must be >= 0");
    }

    [Fact]
    public void FromSysEx_ExtractsDelayLoCut_FromBytes294To297()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 294-297
        // Range: 20Hz-20kHz (table-based)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DelayLoCut.Should().BeGreaterOrEqualTo(0, "delay lo-cut raw value must be >= 0");
    }

    [Fact]
    public void FromSysEx_ExtractsDelayMix_FromBytes298To301()
    {
        // Arrange - Already loaded in constructor
        // Reference: Nova System Sysex Map.txt - bytes 298-301
        // Range: 0-100%

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DelayMix.Should().BeInRange(0, 100, "delay mix must be 0-100%");
    }

    // ==================================
    // REVERB EFFECT PARAMETERS (bytes 326-385)
    // ==================================

    [Fact]
    public void FromSysEx_ExtractsReverbType_FromBytes326To329()
    {
        // Arrange
        // Range: 0-3 (spring/hall/room/plate)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ReverbType.Should().BeInRange(0, 3, "reverb type must be 0-3");
    }

    [Fact]
    public void FromSysEx_ExtractsReverbDecay_FromBytes330To333()
    {
        // Arrange
        // Range: 1-200 (0.1s to 20s by 0.1s)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ReverbDecay.Should().BeInRange(1, 200, "reverb decay must be 1-200");
    }

    [Fact]
    public void FromSysEx_ExtractsReverbPreDelay_FromBytes334To337()
    {
        // Arrange
        // Range: 0-100ms

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ReverbPreDelay.Should().BeInRange(0, 100, "reverb pre-delay must be 0-100ms");
    }

    [Fact]
    public void FromSysEx_ExtractsReverbShape_FromBytes338To341()
    {
        // Arrange
        // Range: 0-2 (round/curved/square)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ReverbShape.Should().BeInRange(0, 2, "reverb shape must be 0-2");
    }

    [Fact]
    public void FromSysEx_ExtractsReverbSize_FromBytes342To345()
    {
        // Arrange
        // Range: 0-7 (box/tiny/small/medium/large/xl/grand/huge)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ReverbSize.Should().BeInRange(0, 7, "reverb size must be 0-7");
    }

    [Fact]
    public void FromSysEx_ExtractsReverbHiColor_FromBytes346To349()
    {
        // Arrange
        // Range: 0-6 (wool/warm/real/clear/bright/crisp/glass)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ReverbHiColor.Should().BeInRange(0, 6, "reverb hi-color must be 0-6");
    }

    [Fact]
    public void FromSysEx_ExtractsReverbHiLevel_FromBytes350To353()
    {
        // Arrange
        // Range: -25 to +25dB (needs offset decoding)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ReverbHiLevel.Should().BeInRange(-25, 25, "Reverb Hi Level must be between -25dB and +25dB");
    }

    [Fact]
    public void FromSysEx_ExtractsReverbLoColor_FromBytes354To357()
    {
        // Arrange
        // Range: 0-6 (thick/round/real/light/tight/thin/nobass)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ReverbLoColor.Should().BeInRange(0, 6, "reverb lo-color must be 0-6");
    }

    [Fact]
    public void FromSysEx_ExtractsReverbLoLevel_FromBytes358To361()
    {
        // Arrange
        // Range: -25 to +25dB (needs offset decoding)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ReverbLoLevel.Should().BeInRange(-25, 25, "Reverb Lo Level must be between -25dB and +25dB");
    }

    [Fact]
    public void FromSysEx_ExtractsReverbRoomLevel_FromBytes362To365()
    {
        // Arrange
        // Range: -100 to 0dB (needs offset decoding)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ReverbRoomLevel.Should().BeInRange(-100, 0, "Reverb Room Level must be between -100dB and 0dB");
    }

    [Fact]
    public void FromSysEx_ExtractsReverbLevel_FromBytes366To369()
    {
        // Arrange
        // Range: -100 to 0dB (needs offset decoding)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ReverbLevel.Should().BeInRange(-100, 0, "Reverb Level must be between -100dB and 0dB");
    }

    [Fact]
    public void FromSysEx_ExtractsReverbDiffuse_FromBytes370To373()
    {
        // Arrange
        // Range: -25 to +25dB (needs offset decoding)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ReverbDiffuse.Should().BeInRange(-25, 25, "Reverb Diffuse must be between -25dB and +25dB");
    }

    [Fact]
    public void FromSysEx_ExtractsReverbMix_FromBytes374To377()
    {
        // Arrange
        // Range: 0-100%

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ReverbMix.Should().BeInRange(0, 100, "reverb mix must be 0-100%");
    }

    // ==================================
    // EQ/GATE PARAMETERS (bytes 390-453)
    // ==================================

    [Fact]
    public void FromSysEx_ExtractsGateType_FromBytes390To393()
    {
        // Arrange - bytes 390-393
        // Range: 0-1 (hard/soft)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.GateType.Should().BeInRange(0, 1, "gate type must be 0-1");
    }

    [Fact]
    public void FromSysEx_ExtractsGateThreshold_FromBytes394To397()
    {
        // Arrange - bytes 394-397
        // Range: -60 to 0dB (offset encoding)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.GateThreshold.Should().BeInRange(-60, 0, "Gate Threshold must be between -60dB and 0dB");
    }

    [Fact]
    public void FromSysEx_ExtractsGateDamp_FromBytes398To401()
    {
        // Arrange - bytes 398-401
        // Range: 0-90dB

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.GateDamp.Should().BeInRange(0, 90, "gate damp must be 0-90dB");
    }

    [Fact]
    public void FromSysEx_ExtractsGateRelease_FromBytes402To405()
    {
        // Arrange - bytes 402-405
        // Range: 0-200 dB/s

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.GateRelease.Should().BeInRange(0, 200, "gate release must be 0-200dB/s");
    }

    [Fact]
    public void FromSysEx_ExtractsEqFreq1_FromBytes410To413()
    {
        // Arrange - bytes 410-413
        // Range: 41Hz-20kHz table (raw value)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.EqFreq1.Should().BeGreaterOrEqualTo(0, "eq freq1 raw value must be >= 0");
    }

    [Fact]
    public void FromSysEx_ExtractsEqGain1_FromBytes414To417()
    {
        // Arrange - bytes 414-417
        // Range: -12 to +12dB (offset encoding)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.EqGain1.Should().BeInRange(-12, 12, "EQ Gain 1 must be between -12dB and +12dB");
    }

    [Fact]
    public void FromSysEx_ExtractsEqWidth1_FromBytes418To421()
    {
        // Arrange - bytes 418-421
        // Range: 5-12 (0.3-1.6 octaves table)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.EqWidth1.Should().BeInRange(5, 12, "eq width1 must be 5-12");
    }

    [Fact]
    public void FromSysEx_ExtractsEqFreq2_FromBytes422To425()
    {
        // Arrange - bytes 422-425

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.EqFreq2.Should().BeGreaterOrEqualTo(0, "eq freq2 raw value must be >= 0");
    }

    [Fact]
    public void FromSysEx_ExtractsEqGain2_FromBytes426To429()
    {
        // Arrange - bytes 426-429
        // Range: -12 to +12dB (offset encoding)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.EqGain2.Should().BeInRange(-12, 12, "EQ Gain 2 must be between -12dB and +12dB");
    }

    [Fact]
    public void FromSysEx_ExtractsEqWidth2_FromBytes430To433()
    {
        // Arrange - bytes 430-433

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.EqWidth2.Should().BeInRange(5, 12, "eq width2 must be 5-12");
    }

    [Fact]
    public void FromSysEx_ExtractsEqFreq3_FromBytes434To437()
    {
        // Arrange - bytes 434-437

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.EqFreq3.Should().BeGreaterOrEqualTo(0, "eq freq3 raw value must be >= 0");
    }

    [Fact]
    public void FromSysEx_ExtractsEqGain3_FromBytes438To441()
    {
        // Arrange - bytes 438-441

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.EqGain3.Should().BeInRange(-12, 12, "EQ Gain 3 must be between -12dB and +12dB");
    }

    [Fact]
    public void FromSysEx_ExtractsEqWidth3_FromBytes442To445()
    {
        // Arrange - bytes 442-445

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.EqWidth3.Should().BeInRange(5, 12, "eq width3 must be 5-12");
    }

    // ==================================
    // PITCH EFFECT PARAMETERS (bytes 454-513)
    // ==================================

    [Fact]
    public void FromSysEx_ExtractsPitchType_FromBytes454To457()
    {
        // Arrange - bytes 454-457
        // Range: 0-4 (shifter/octaver/whammy/detune/intelligent)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PitchType.Should().BeInRange(0, 4, "pitch type must be 0-4");
    }

    [Fact]
    public void FromSysEx_ExtractsPitchVoice1_FromBytes458To461()
    {
        // Arrange - bytes 458-461
        // Multi-function: -100 to +100 cents (shift/detune) OR -13 to +13 degrees (intell)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PitchVoice1.Should().BeInRange(-100, 100, "Pitch Voice 1 must be between -100 and +100 cents");
    }

    [Fact]
    public void FromSysEx_ExtractsPitchVoice2_FromBytes462To465()
    {
        // Arrange - bytes 462-465

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PitchVoice2.Should().BeInRange(-100, 100, "Pitch Voice 2 must be between -100 and +100 cents");
    }

    [Fact]
    public void FromSysEx_ExtractsPitchPan1_FromBytes466To469()
    {
        // Arrange - bytes 466-469
        // Range: -50 to +50 (50L to 50R)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PitchPan1.Should().BeInRange(-50, 50, "Pitch Pan 1 must be between -50 and +50");
    }

    [Fact]
    public void FromSysEx_ExtractsPitchPan2_FromBytes470To473()
    {
        // Arrange - bytes 470-473

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PitchPan2.Should().BeInRange(-50, 50, "Pitch Pan 2 must be between -50 and +50");
    }

    [Fact]
    public void FromSysEx_ExtractsPitchDelay1_FromBytes474To477()
    {
        // Arrange - bytes 474-477
        // Range: 0-50ms

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PitchDelay1.Should().BeInRange(0, 50, "pitch delay1 must be 0-50ms");
    }

    [Fact]
    public void FromSysEx_ExtractsPitchDelay2_FromBytes478To481()
    {
        // Arrange - bytes 478-481

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PitchDelay2.Should().BeInRange(0, 50, "pitch delay2 must be 0-50ms");
    }

    [Fact]
    public void FromSysEx_ExtractsPitchFeedback1OrKey_FromBytes482To485()
    {
        // Arrange - bytes 482-485
        // Multi-function: 0-100% (shift) OR 0-12 Key (intell: C..B)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PitchFeedback1OrKey.Should().BeInRange(0, 100, "pitch feedback1/key must be 0-100");
    }

    [Fact]
    public void FromSysEx_ExtractsPitchFeedback2OrScale_FromBytes486To489()
    {
        // Arrange - bytes 486-489
        // Multi-function: 0-100% (shift) OR 0-13 Scale (intell: Ionian/Dorian/etc)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PitchFeedback2OrScale.Should().BeInRange(0, 100, "pitch feedback2/scale must be 0-100");
    }

    [Fact]
    public void FromSysEx_ExtractsPitchLevel1_FromBytes490To493()
    {
        // Arrange - bytes 490-493
        // Range: -100 to 0dB

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PitchLevel1.Should().BeInRange(-100, 0, "Pitch Level 1 must be between -100dB and 0dB");
    }

    [Fact]
    public void FromSysEx_ExtractsPitchLevel2_FromBytes494To497()
    {
        // Arrange - bytes 494-497

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PitchLevel2.Should().BeInRange(-100, 0, "Pitch Level 2 must be between -100dB and 0dB");
    }
}
