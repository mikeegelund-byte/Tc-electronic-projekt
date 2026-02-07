using FluentAssertions;
using Nova.Domain.Models;

namespace Nova.Domain.Tests.Models;

/// <summary>
/// Tests for Preset parameter validation.
/// Validates that FromSysEx rejects out-of-range parameter values.
/// Following TDD RED→GREEN→REFACTOR cycle.
/// </summary>
public class PresetValidationTests
{
    /// <summary>
    /// Creates a valid preset SysEx byte array with all parameters at safe values.
    /// </summary>
    private static byte[] CreateValidPresetSysEx()
    {
        var sysex = new byte[521];
        sysex[0] = 0xF0;                                    // SysEx start
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F; // TC Electronic ID
        sysex[4] = 0x00;                                    // Device ID
        sysex[5] = 0x63;                                    // Nova System Model ID
        sysex[6] = 0x20;                                    // Message ID (Dump)
        sysex[7] = 0x01;                                    // Data Type (Preset)
        sysex[8] = 0x00;                                    // Preset number (0)

        // Preset name (24 bytes ASCII)
        var name = "Test Preset             ";
        for (int i = 0; i < 24; i++)
            sysex[10 + i] = (byte)name[i];

        // Set safe default values for parameters that need validation
        // TapTempo (bytes 38-41): 500ms (safe mid-range value)
        Encode4ByteValue(sysex, 38, 500);

        // Routing (bytes 42-45): 0 (Semi-parallel)
        Encode4ByteValue(sysex, 42, 0);

        // LevelOutLeft/Right (bytes 46-53): 0dB
        EncodeSignedDbValue(sysex, 46, 0);
        EncodeSignedDbValue(sysex, 50, 0);
        Encode4ByteValue(sysex, 54, 0);   // MapParameter: 0
        Encode4ByteValue(sysex, 58, 0);   // MapMin: 0
        Encode4ByteValue(sysex, 62, 50);  // MapMid: 50
        Encode4ByteValue(sysex, 66, 100); // MapMax: 100

        // COMP parameters with safe values
        Encode4ByteValue(sysex, 70, 0);   // CompType: 0 (perc)
        EncodeSignedDbValue(sysex, 74, 0);   // CompThreshold: 0dB
        Encode4ByteValue(sysex, 78, 1);   // CompRatio: 1
        Encode4ByteValue(sysex, 82, 5);   // CompAttack: 5
        Encode4ByteValue(sysex, 86, 15);  // CompRelease: 15
        Encode4ByteValue(sysex, 90, 5);   // CompResponse: 5
        Encode4ByteValue(sysex, 94, 10);  // CompDrive: 10
        EncodeSignedDbValue(sysex, 98, 0);  // CompLevel: 0dB

        // DRIVE parameters
        Encode4ByteValue(sysex, 134, 0);  // DriveType: 0
        Encode4ByteValue(sysex, 138, 50); // DriveGain: 50%
        Encode4ByteValue(sysex, 142, 50); // DriveTone: 50%
        EncodeSignedDbValue(sysex, 190, 0);  // DriveLevel: 0dB

        // BOOST parameters
        Encode4ByteValue(sysex, 182, 5);  // BoostLevel: 5dB
        Encode4ByteValue(sysex, 186, 0);  // BoostEnabled: off

        // Effect enable flags (4-byte encoded boolean: 0=off, 1=on)
        Encode4ByteValue(sysex, 130, 0);  // CompressorEnabled: off
        Encode4ByteValue(sysex, 194, 0);  // DriveEnabled: off
        Encode4ByteValue(sysex, 258, 0);  // ModulationEnabled: off
        Encode4ByteValue(sysex, 322, 0);  // DelayEnabled: off
        Encode4ByteValue(sysex, 386, 0);  // ReverbEnabled: off

        // MOD parameters
        Encode4ByteValue(sysex, 198, 0);  // ModType: 0 (chorus)
        Encode4ByteValue(sysex, 202, 50); // ModSpeed: 50 (table lookup)
        Encode4ByteValue(sysex, 206, 50); // ModDepth: 50%
        Encode4ByteValue(sysex, 210, 8);  // ModTempo: 8
        Encode4ByteValue(sysex, 214, 50); // ModHiCut: 50 (table lookup)
        EncodeSignedDbValue(sysex, 218, 0); // ModFeedback: 0%
        Encode4ByteValue(sysex, 222, 25); // ModDelayOrRange: 25
        Encode4ByteValue(sysex, 250, 50); // ModMix: 50%

        // DELAY parameters
        Encode4ByteValue(sysex, 262, 0);  // DelayType: 0 (clean)
        Encode4ByteValue(sysex, 266, 500); // DelayTime: 500ms
        Encode4ByteValue(sysex, 270, 500); // DelayTime2: 500ms
        Encode4ByteValue(sysex, 274, 8);  // DelayTempo: 8
        Encode4ByteValue(sysex, 278, 0);  // DelayTempo2OrWidth: 0 (unused for clean)
        Encode4ByteValue(sysex, 282, 50); // DelayFeedback: 50%
        Encode4ByteValue(sysex, 286, 0); // DelayClipOrFeedback2: 0 (unused for clean)
        Encode4ByteValue(sysex, 290, 50); // DelayHiCut: 50
        Encode4ByteValue(sysex, 294, 50); // DelayLoCut: 50
        Encode4ByteValue(sysex, 314, 50); // DelayMix: 50%

        // REVERB parameters
        Encode4ByteValue(sysex, 326, 0);  // ReverbType: 0 (spring)
        Encode4ByteValue(sysex, 330, 50); // ReverbDecay: 50
        Encode4ByteValue(sysex, 334, 30); // ReverbPreDelay: 30ms
        Encode4ByteValue(sysex, 338, 1);  // ReverbShape: 1
        Encode4ByteValue(sysex, 342, 3);  // ReverbSize: 3
        Encode4ByteValue(sysex, 346, 3);  // ReverbHiColor: 3
        EncodeSignedDbValue(sysex, 350, 0); // ReverbHiLevel: 0dB
        Encode4ByteValue(sysex, 354, 3);  // ReverbLoColor: 3
        EncodeSignedDbValue(sysex, 358, 0); // ReverbLoLevel: 0dB
        EncodeSignedDbValue(sysex, 362, 0);  // ReverbRoomLevel: 0dB
        EncodeSignedDbValue(sysex, 366, 0);  // ReverbLevel: 0dB
        EncodeSignedDbValue(sysex, 370, 0); // ReverbDiffuse: 0dB
        Encode4ByteValue(sysex, 374, 50); // ReverbMix: 50%

        // EQ/GATE parameters
        Encode4ByteValue(sysex, 390, 0);  // GateType: 0 (hard)
        EncodeSignedDbValue(sysex, 394, -30); // GateThreshold: -30dB
        Encode4ByteValue(sysex, 398, 45); // GateDamp: 45dB
        Encode4ByteValue(sysex, 402, 100); // GateRelease: 100 dB/s
        Encode4ByteValue(sysex, 410, 25); // EqFreq1: 25 (table)
        EncodeSignedDbValue(sysex, 414, 0); // EqGain1: 0dB
        Encode4ByteValue(sysex, 418, 8);  // EqWidth1: 8
        Encode4ByteValue(sysex, 422, 35); // EqFreq2: 35
        EncodeSignedDbValue(sysex, 426, 0); // EqGain2: 0dB
        Encode4ByteValue(sysex, 430, 8);  // EqWidth2: 8
        Encode4ByteValue(sysex, 434, 45); // EqFreq3: 45
        EncodeSignedDbValue(sysex, 438, 0); // EqGain3: 0dB
        Encode4ByteValue(sysex, 442, 8);  // EqWidth3: 8

        // PITCH parameters
        Encode4ByteValue(sysex, 454, 0);  // PitchType: 0 (shifter)
        EncodeSignedDbValue(sysex, 458, 0); // PitchVoice1: 0 cents
        EncodeSignedDbValue(sysex, 462, 0); // PitchVoice2: 0 cents
        EncodeSignedDbValue(sysex, 466, 0); // PitchPan1: center
        EncodeSignedDbValue(sysex, 470, 0); // PitchPan2: center
        Encode4ByteValue(sysex, 474, 10); // PitchDelay1: 10ms
        Encode4ByteValue(sysex, 478, 10); // PitchDelay2: 10ms
        Encode4ByteValue(sysex, 482, 50); // PitchFeedback1OrKey: 50
        Encode4ByteValue(sysex, 486, 50); // PitchFeedback2OrScale: 50
        EncodeSignedDbValue(sysex, 490, 0);  // PitchLevel1: 0dB
        EncodeSignedDbValue(sysex, 494, 0);  // PitchLevel2: 0dB

        sysex[520] = 0xF7; // SysEx end
        return sysex;
    }

    /// <summary>
    /// Encodes an integer value into 4-byte 7-bit little-endian format.
    /// Inverse of Decode4ByteValue in Preset.cs.
    /// </summary>
    private static void Encode4ByteValue(byte[] sysex, int offset, int value)
    {
        sysex[offset] = (byte)(value & 0x7F);           // LSB
        sysex[offset + 1] = (byte)((value >> 7) & 0x7F);
        sysex[offset + 2] = (byte)((value >> 14) & 0x7F);
        sysex[offset + 3] = (byte)((value >> 21) & 0x7F); // MSB
    }

    private static void EncodeSignedDbValue(byte[] sysex, int offset, int value)
    {
        const int largeOffset = 16777216; // 2^24
        Encode4ByteValue(sysex, offset, value + largeOffset);
    }

    // ========================================
    // COMP BLOCK VALIDATION TESTS (8 params)
    // ========================================

    [Theory]
    [InlineData(3)]   // Above max (0-2)
    [InlineData(10)]  // Way above max
    [InlineData(255)] // Edge case
    public void FromSysEx_CompType_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 70, invalidValue); // CompType at bytes 70-73

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("CompType {0} is out of valid range 0-2", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("CompType") || e.Message.Contains("COMP") || e.Message.Contains("type"));
    }

    [Theory]
    [InlineData(16)]  // Above max (0-15)
    [InlineData(20)]  // Way above max
    public void FromSysEx_CompRatio_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 78, invalidValue); // CompRatio at bytes 78-81

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("CompRatio {0} is out of valid range 0-15", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("CompRatio") || e.Message.Contains("ratio"));
    }

    [Theory]
    [InlineData(17)]  // Above max (0-16)
    [InlineData(100)] // Way above max
    public void FromSysEx_CompAttack_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 82, invalidValue); // CompAttack at bytes 82-85

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("CompAttack {0} is out of valid range 0-16", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("CompAttack") || e.Message.Contains("attack"));
    }

    [Theory]
    [InlineData(12)]  // Below min (13-23)
    [InlineData(0)]   // Way below min
    [InlineData(24)]  // Above max
    [InlineData(50)]  // Way above max
    public void FromSysEx_CompRelease_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 86, invalidValue); // CompRelease at bytes 86-89

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("CompRelease {0} is out of valid range 13-23", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("CompRelease") || e.Message.Contains("release"));
    }

    // ========================================
    // DRIVE BLOCK VALIDATION TESTS (3 params)
    // ========================================

    [Theory]
    [InlineData(2)]   // Above max (0-1)
    [InlineData(10)]  // Way above max
    public void FromSysEx_DriveType_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 134, invalidValue); // DriveType at bytes 134-137

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("DriveType {0} is out of valid range 0-1", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("DriveType") || e.Message.Contains("drive") || e.Message.Contains("type"));
    }

    [Theory]
    [InlineData(101)] // Above max (0-100)
    [InlineData(200)] // Way above max
    public void FromSysEx_DriveGain_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 138, invalidValue); // DriveGain at bytes 138-141

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("DriveGain {0} is out of valid range 0-100", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("DriveGain") || e.Message.Contains("gain"));
    }

    // ========================================
    // DRIVE TONE VALIDATION TEST
    // ========================================

    [Theory]
    [InlineData(101)] // Above max (0-100)
    [InlineData(200)] // Way above max
    public void FromSysEx_DriveTone_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 142, invalidValue); // DriveTone at bytes 142-145

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("DriveTone {0} is out of valid range 0-100", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("DriveTone") || e.Message.Contains("drive") || e.Message.Contains("tone"));
    }

    [Theory]
    [InlineData(11)]  // Above max (0-10)
    [InlineData(100)] // Way above max
    public void FromSysEx_BoostLevel_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 182, invalidValue); // BoostLevel at bytes 182-185

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("BoostLevel {0} is out of valid range 0-10", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("BoostLevel") || e.Message.Contains("boost") || e.Message.Contains("level"));
    }

    // ========================================
    // MOD BLOCK VALIDATION TESTS (8 params)
    // ========================================

    [Theory]
    [InlineData(6)]   // Above max (0-5)
    [InlineData(20)]  // Way above max
    public void FromSysEx_ModType_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 198, invalidValue); // ModType at bytes 198-201

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("ModType {0} is out of valid range 0-5", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("ModType") || e.Message.Contains("mod") || e.Message.Contains("type"));
    }

    [Theory]
    [InlineData(101)] // Above max (0-100)
    [InlineData(200)] // Way above max
    public void FromSysEx_ModDepth_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 206, invalidValue); // ModDepth at bytes 206-209

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("ModDepth {0} is out of valid range 0-100", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("ModDepth") || e.Message.Contains("depth"));
    }

    [Theory]
    [InlineData(17)]  // Above max (0-16)
    [InlineData(50)]  // Way above max
    public void FromSysEx_ModTempo_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 210, invalidValue); // ModTempo at bytes 210-213

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("ModTempo {0} is out of valid range 0-16", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("ModTempo") || e.Message.Contains("tempo"));
    }

    [Theory]
    [InlineData(101)] // Above max (0-100)
    [InlineData(200)] // Way above max
    public void FromSysEx_ModMix_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 250, invalidValue); // ModMix at bytes 250-253

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("ModMix {0} is out of valid range 0-100", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("ModMix") || e.Message.Contains("mix"));
    }

    // ========================================
    // DELAY BLOCK VALIDATION TESTS (10 params)
    // ========================================

    [Theory]
    [InlineData(6)]   // Above max (0-5)
    [InlineData(20)]  // Way above max
    public void FromSysEx_DelayType_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 262, invalidValue); // DelayType at bytes 262-265

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("DelayType {0} is out of valid range 0-5", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("DelayType") || e.Message.Contains("delay") || e.Message.Contains("type"));
    }

    [Theory]
    [InlineData(1801)] // Above max (0-1800)
    [InlineData(5000)] // Way above max
    public void FromSysEx_DelayTime_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 266, invalidValue); // DelayTime at bytes 266-269

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("DelayTime {0} is out of valid range 0-1800", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("DelayTime") || e.Message.Contains("time"));
    }

    [Theory]
    [InlineData(1801)] // Above max (0-1800)
    [InlineData(5000)] // Way above max
    public void FromSysEx_DelayTime2_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 270, invalidValue); // DelayTime2 at bytes 270-273

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("DelayTime2 {0} is out of valid range 0-1800", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("DelayTime2") || e.Message.Contains("time"));
    }

    [Theory]
    [InlineData(17)]  // Above max (0-16)
    [InlineData(50)]  // Way above max
    public void FromSysEx_DelayTempo_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 274, invalidValue); // DelayTempo at bytes 274-277

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("DelayTempo {0} is out of valid range 0-16", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("DelayTempo") || e.Message.Contains("tempo"));
    }

    [Theory]
    [InlineData(121)] // Above max (0-120)
    [InlineData(200)] // Way above max
    public void FromSysEx_DelayFeedback_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 282, invalidValue); // DelayFeedback at bytes 282-285

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("DelayFeedback {0} is out of valid range 0-120", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("DelayFeedback") || e.Message.Contains("feedback"));
    }

    [Theory]
    [InlineData(101)] // Above max (0-100)
    [InlineData(200)] // Way above max
    public void FromSysEx_DelayMix_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 314, invalidValue); // DelayMix at bytes 314-317

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("DelayMix {0} is out of valid range 0-100", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("DelayMix") || e.Message.Contains("mix"));
    }

    // ========================================
    // REVERB BLOCK VALIDATION TESTS (13 params)
    // ========================================

    [Theory]
    [InlineData(4)]   // Above max (0-3)
    [InlineData(20)]  // Way above max
    public void FromSysEx_ReverbType_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 326, invalidValue); // ReverbType at bytes 326-329

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("ReverbType {0} is out of valid range 0-3", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("ReverbType") || e.Message.Contains("reverb") || e.Message.Contains("type"));
    }

    [Theory]
    [InlineData(0)]   // Below min (1-200)
    [InlineData(201)] // Above max
    [InlineData(500)] // Way above max
    public void FromSysEx_ReverbDecay_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 330, invalidValue); // ReverbDecay at bytes 330-333

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("ReverbDecay {0} is out of valid range 1-200", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("ReverbDecay") || e.Message.Contains("decay"));
    }

    [Theory]
    [InlineData(101)] // Above max (0-100)
    [InlineData(200)] // Way above max
    public void FromSysEx_ReverbPreDelay_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 334, invalidValue); // ReverbPreDelay at bytes 334-337

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("ReverbPreDelay {0} is out of valid range 0-100", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("ReverbPreDelay") || e.Message.Contains("pre-delay") || e.Message.Contains("predelay"));
    }

    [Theory]
    [InlineData(3)]   // Above max (0-2)
    [InlineData(10)]  // Way above max
    public void FromSysEx_ReverbShape_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 338, invalidValue); // ReverbShape at bytes 338-341

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("ReverbShape {0} is out of valid range 0-2", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("ReverbShape") || e.Message.Contains("shape"));
    }

    [Theory]
    [InlineData(8)]   // Above max (0-7)
    [InlineData(20)]  // Way above max
    public void FromSysEx_ReverbSize_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 342, invalidValue); // ReverbSize at bytes 342-345

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("ReverbSize {0} is out of valid range 0-7", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("ReverbSize") || e.Message.Contains("size"));
    }

    [Theory]
    [InlineData(7)]   // Above max (0-6)
    [InlineData(20)]  // Way above max
    public void FromSysEx_ReverbHiColor_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 346, invalidValue); // ReverbHiColor at bytes 346-349

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("ReverbHiColor {0} is out of valid range 0-6", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("ReverbHiColor") || e.Message.Contains("hi-color") || e.Message.Contains("hicolor"));
    }

    [Theory]
    [InlineData(7)]   // Above max (0-6)
    [InlineData(20)]  // Way above max
    public void FromSysEx_ReverbLoColor_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 354, invalidValue); // ReverbLoColor at bytes 354-357

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("ReverbLoColor {0} is out of valid range 0-6", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("ReverbLoColor") || e.Message.Contains("lo-color") || e.Message.Contains("locolor"));
    }

    [Theory]
    [InlineData(101)] // Above max (0-100)
    [InlineData(200)] // Way above max
    public void FromSysEx_ReverbMix_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 374, invalidValue); // ReverbMix at bytes 374-377

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("ReverbMix {0} is out of valid range 0-100", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("ReverbMix") || e.Message.Contains("mix"));
    }

    // ========================================
    // EQ/GATE BLOCK VALIDATION TESTS (13 params)
    // ========================================

    [Theory]
    [InlineData(2)]   // Above max (0-1)
    [InlineData(10)]  // Way above max
    public void FromSysEx_GateType_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 390, invalidValue); // GateType at bytes 390-393

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("GateType {0} is out of valid range 0-1", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("GateType") || e.Message.Contains("gate") || e.Message.Contains("type"));
    }

    [Theory]
    [InlineData(91)]  // Above max (0-90)
    [InlineData(200)] // Way above max
    public void FromSysEx_GateDamp_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 398, invalidValue); // GateDamp at bytes 398-401

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("GateDamp {0} is out of valid range 0-90", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("GateDamp") || e.Message.Contains("damp"));
    }

    [Theory]
    [InlineData(201)] // Above max (0-200)
    [InlineData(500)] // Way above max
    public void FromSysEx_GateRelease_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 402, invalidValue); // GateRelease at bytes 402-405

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("GateRelease {0} is out of valid range 0-200", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("GateRelease") || e.Message.Contains("release"));
    }

    [Theory]
    [InlineData(4)]   // Below min (5-12)
    [InlineData(0)]   // Way below min
    [InlineData(13)]  // Above max
    [InlineData(50)]  // Way above max
    public void FromSysEx_EqWidth1_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 418, invalidValue); // EqWidth1 at bytes 418-421

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("EqWidth1 {0} is out of valid range 5-12", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("EqWidth1") || e.Message.Contains("width"));
    }

    [Theory]
    [InlineData(4)]   // Below min (5-12)
    [InlineData(13)]  // Above max
    public void FromSysEx_EqWidth2_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 430, invalidValue); // EqWidth2 at bytes 430-433

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("EqWidth2 {0} is out of valid range 5-12", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("EqWidth2") || e.Message.Contains("width"));
    }

    [Theory]
    [InlineData(4)]   // Below min (5-12)
    [InlineData(13)]  // Above max
    public void FromSysEx_EqWidth3_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 442, invalidValue); // EqWidth3 at bytes 442-445

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("EqWidth3 {0} is out of valid range 5-12", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("EqWidth3") || e.Message.Contains("width"));
    }

    // ========================================
    // PITCH BLOCK VALIDATION TESTS (11 params)
    // ========================================

    [Theory]
    [InlineData(5)]   // Above max (0-4)
    [InlineData(20)]  // Way above max
    public void FromSysEx_PitchType_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 454, invalidValue); // PitchType at bytes 454-457

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("PitchType {0} is out of valid range 0-4", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("PitchType") || e.Message.Contains("pitch") || e.Message.Contains("type"));
    }

    [Theory]
    [InlineData(51)]  // Above max (0-50)
    [InlineData(100)] // Way above max
    public void FromSysEx_PitchDelay1_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 474, invalidValue); // PitchDelay1 at bytes 474-477

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("PitchDelay1 {0} is out of valid range 0-50", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("PitchDelay1") || e.Message.Contains("delay"));
    }

    [Theory]
    [InlineData(51)]  // Above max (0-50)
    [InlineData(100)] // Way above max
    public void FromSysEx_PitchDelay2_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 478, invalidValue); // PitchDelay2 at bytes 478-481

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("PitchDelay2 {0} is out of valid range 0-50", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("PitchDelay2") || e.Message.Contains("delay"));
    }

    [Theory]
    [InlineData(101)] // Above max (0-100)
    [InlineData(200)] // Way above max
    public void FromSysEx_PitchFeedback1OrKey_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 482, invalidValue); // PitchFeedback1OrKey at bytes 482-485

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("PitchFeedback1OrKey {0} is out of valid range 0-100", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("PitchFeedback1") || e.Message.Contains("feedback") || e.Message.Contains("key"));
    }

    [Theory]
    [InlineData(101)] // Above max (0-100)
    [InlineData(200)] // Way above max
    public void FromSysEx_PitchFeedback2OrScale_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 486, invalidValue); // PitchFeedback2OrScale at bytes 486-489

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("PitchFeedback2OrScale {0} is out of valid range 0-100", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("PitchFeedback2") || e.Message.Contains("feedback") || e.Message.Contains("scale"));
    }

    // ========================================
    // BASIC/GLOBAL VALIDATION TESTS (9 params)
    // ========================================

    [Theory]
    [InlineData(99)]    // Below min (100-3000)
    [InlineData(0)]     // Way below min
    [InlineData(3001)]  // Above max
    [InlineData(10000)] // Way above max
    public void FromSysEx_TapTempo_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 38, invalidValue); // TapTempo at bytes 38-41

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("TapTempo {0} is out of valid range 100-3000", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("TapTempo") || e.Message.Contains("tempo"));
    }

    [Theory]
    [InlineData(3)]   // Above max (0-2)
    [InlineData(10)]  // Way above max
    public void FromSysEx_Routing_OutOfRange_ReturnsFailure(int invalidValue)
    {
        // Arrange
        var sysex = CreateValidPresetSysEx();
        Encode4ByteValue(sysex, 42, invalidValue); // Routing at bytes 42-45

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsFailed.Should().BeTrue("Routing {0} is out of valid range 0-2", invalidValue);
        result.Errors.Should().Contain(e => e.Message.Contains("Routing") || e.Message.Contains("routing"));
    }

    // ========================================
    // POSITIVE TESTS: Valid preset should parse successfully
    // ========================================

    [Fact]
    public void FromSysEx_AllParametersValid_ReturnsSuccess()
    {
        // Arrange - all parameters set to valid values by CreateValidPresetSysEx()
        var sysex = CreateValidPresetSysEx();

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsSuccess.Should().BeTrue("preset with all valid parameters should parse successfully");
    }

    [Fact]
    public void FromSysEx_EdgeCase_MinimumValidValues_ReturnsSuccess()
    {
        // Arrange - set parameters to minimum valid values
        var sysex = CreateValidPresetSysEx();

        // Minimum values for various params
        Encode4ByteValue(sysex, 38, 100);  // TapTempo min
        Encode4ByteValue(sysex, 70, 0);    // CompType min
        Encode4ByteValue(sysex, 86, 13);   // CompRelease min
        Encode4ByteValue(sysex, 330, 1);   // ReverbDecay min
        Encode4ByteValue(sysex, 418, 5);   // EqWidth1 min

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsSuccess.Should().BeTrue("preset with minimum valid values should parse successfully");
    }

    [Fact]
    public void FromSysEx_EdgeCase_MaximumValidValues_ReturnsSuccess()
    {
        // Arrange - set parameters to maximum valid values
        var sysex = CreateValidPresetSysEx();

        // Maximum values for various params
        Encode4ByteValue(sysex, 38, 3000); // TapTempo max
        Encode4ByteValue(sysex, 42, 2);    // Routing max
        Encode4ByteValue(sysex, 70, 2);    // CompType max
        Encode4ByteValue(sysex, 86, 23);   // CompRelease max
        Encode4ByteValue(sysex, 330, 200); // ReverbDecay max
        Encode4ByteValue(sysex, 418, 12);  // EqWidth1 max
        Encode4ByteValue(sysex, 266, 1800); // DelayTime max

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsSuccess.Should().BeTrue("preset with maximum valid values should parse successfully");
    }
}
