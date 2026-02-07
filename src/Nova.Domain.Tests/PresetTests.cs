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
        // Bytes 10-33 = preset name (24 ASCII chars; byte 9 reserved)
        var name = "Test Preset             ";
        for (int i = 0; i < 24; i++)
            sysex[10 + i] = (byte)name[i];

        // Set minimum valid parameter values to pass validation
        Encode4ByteValue(sysex, 38, 500);   // TapTempo: 500ms (100-3000)
        Encode4ByteValue(sysex, 42, 0);     // Routing: 0 (0-2)
        Encode4ByteValue(sysex, 70, 0);     // CompType: 0 (0-2)
        Encode4ByteValue(sysex, 78, 0);     // CompRatio: 0 (0-15)
        Encode4ByteValue(sysex, 82, 0);     // CompAttack: 0 (0-16)
        Encode4ByteValue(sysex, 86, 15);    // CompRelease: 15 (13-23)
        Encode4ByteValue(sysex, 90, 1);     // CompResponse: 1 (1-10)
        Encode4ByteValue(sysex, 94, 1);     // CompDrive: 1 (1-20)
        Encode4ByteValue(sysex, 134, 0);    // DriveType: 0 (0-1)
        Encode4ByteValue(sysex, 138, 50);   // DriveGain: 50 (0-100)
        Encode4ByteValue(sysex, 142, 50);   // DriveTone: 50 (0-100)
        Encode4ByteValue(sysex, 182, 5);    // BoostLevel: 5 (0-10)
        Encode4ByteValue(sysex, 186, 0);    // BoostEnabled: 0 (off)
        Encode4ByteValue(sysex, 190, 0);    // DriveLevel: raw 0 (min)
        Encode4ByteValue(sysex, 198, 0);    // ModType: 0 (0-5)
        Encode4ByteValue(sysex, 206, 50);   // ModDepth: 50 (0-100)
        Encode4ByteValue(sysex, 210, 8);    // ModTempo: 8 (0-16)
        Encode4ByteValue(sysex, 250, 50);   // ModMix: 50 (0-100)
        Encode4ByteValue(sysex, 262, 0);    // DelayType: 0 (0-5)
        Encode4ByteValue(sysex, 266, 500);  // DelayTime: 500 (0-1800)
        Encode4ByteValue(sysex, 270, 500);  // DelayTime2: 500 (0-1800)
        Encode4ByteValue(sysex, 274, 8);    // DelayTempo: 8 (0-16)
        Encode4ByteValue(sysex, 282, 50);   // DelayFeedback: 50 (0-120)
        Encode4ByteValue(sysex, 314, 50);   // DelayMix: 50 (0-100)
        Encode4ByteValue(sysex, 326, 0);    // ReverbType: 0 (0-3)
        Encode4ByteValue(sysex, 330, 50);   // ReverbDecay: 50 (1-200)
        Encode4ByteValue(sysex, 334, 30);   // ReverbPreDelay: 30 (0-100)
        Encode4ByteValue(sysex, 338, 1);    // ReverbShape: 1 (0-2)
        Encode4ByteValue(sysex, 342, 3);    // ReverbSize: 3 (0-7)
        Encode4ByteValue(sysex, 346, 3);    // ReverbHiColor: 3 (0-6)
        Encode4ByteValue(sysex, 354, 3);    // ReverbLoColor: 3 (0-6)
        Encode4ByteValue(sysex, 374, 50);   // ReverbMix: 50 (0-100)
        Encode4ByteValue(sysex, 390, 0);    // GateType: 0 (0-1)
        Encode4ByteValue(sysex, 398, 45);   // GateDamp: 45 (0-90)
        Encode4ByteValue(sysex, 402, 100);  // GateRelease: 100 (0-200)
        Encode4ByteValue(sysex, 410, 25);   // EqFreq1: 25 (25-113)
        Encode4ByteValue(sysex, 418, 8);    // EqWidth1: 8 (5-12)
        Encode4ByteValue(sysex, 422, 35);   // EqFreq2: 35 (25-113)
        Encode4ByteValue(sysex, 430, 8);    // EqWidth2: 8 (5-12)
        Encode4ByteValue(sysex, 434, 45);   // EqFreq3: 45 (25-113)
        Encode4ByteValue(sysex, 442, 8);    // EqWidth3: 8 (5-12)
        Encode4ByteValue(sysex, 454, 0);    // PitchType: 0 (0-4)
        Encode4ByteValue(sysex, 474, 25);   // PitchDelay1: 25 (0-50)
        Encode4ByteValue(sysex, 478, 25);   // PitchDelay2: 25 (0-50)
        Encode4ByteValue(sysex, 482, 50);   // PitchFeedback1OrKey: 50 (0-100)
        Encode4ByteValue(sysex, 486, 50);   // PitchFeedback2OrScale: 50 (0-100)

        sysex[519] = 0xF7;                  // SysEx end

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Number.Should().Be(31);
        result.Value.Name.Should().Be("Test Preset");
    }

    private static void Encode4ByteValue(byte[] sysex, int offset, int value)
    {
        sysex[offset] = (byte)(value & 0x7F);
        sysex[offset + 1] = (byte)((value >> 7) & 0x7F);
        sysex[offset + 2] = (byte)((value >> 14) & 0x7F);
        sysex[offset + 3] = (byte)((value >> 21) & 0x7F);
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
        Array.Copy(nameBytes, 0, sysex, 10, 24);

        // Set minimum valid parameter values to pass validation
        Encode4ByteValue(sysex, 38, 500);   // TapTempo: 500ms (100-3000)
        Encode4ByteValue(sysex, 42, 0);     // Routing: 0 (0-2)
        Encode4ByteValue(sysex, 70, 0);     // CompType: 0 (0-2)
        Encode4ByteValue(sysex, 78, 0);     // CompRatio: 0 (0-15)
        Encode4ByteValue(sysex, 82, 0);     // CompAttack: 0 (0-16)
        Encode4ByteValue(sysex, 86, 15);    // CompRelease: 15 (13-23)
        Encode4ByteValue(sysex, 90, 1);     // CompResponse: 1 (1-10)
        Encode4ByteValue(sysex, 94, 1);     // CompDrive: 1 (1-20)
        Encode4ByteValue(sysex, 134, 0);    // DriveType: 0 (0-1)
        Encode4ByteValue(sysex, 138, 50);   // DriveGain: 50 (0-100)
        Encode4ByteValue(sysex, 142, 50);   // DriveTone: 50 (0-100)
        Encode4ByteValue(sysex, 182, 5);    // BoostLevel: 5 (0-10)
        Encode4ByteValue(sysex, 186, 0);    // BoostEnabled: 0 (off)
        Encode4ByteValue(sysex, 190, 0);    // DriveLevel: raw 0 (min)
        Encode4ByteValue(sysex, 198, 0);    // ModType: 0 (0-5)
        Encode4ByteValue(sysex, 206, 50);   // ModDepth: 50 (0-100)
        Encode4ByteValue(sysex, 210, 8);    // ModTempo: 8 (0-16)
        Encode4ByteValue(sysex, 250, 50);   // ModMix: 50 (0-100)
        Encode4ByteValue(sysex, 262, 0);    // DelayType: 0 (0-5)
        Encode4ByteValue(sysex, 266, 500);  // DelayTime: 500 (0-1800)
        Encode4ByteValue(sysex, 270, 500);  // DelayTime2: 500 (0-1800)
        Encode4ByteValue(sysex, 274, 8);    // DelayTempo: 8 (0-16)
        Encode4ByteValue(sysex, 282, 50);   // DelayFeedback: 50 (0-120)
        Encode4ByteValue(sysex, 314, 50);   // DelayMix: 50 (0-100)
        Encode4ByteValue(sysex, 326, 0);    // ReverbType: 0 (0-3)
        Encode4ByteValue(sysex, 330, 50);   // ReverbDecay: 50 (1-200)
        Encode4ByteValue(sysex, 334, 30);   // ReverbPreDelay: 30 (0-100)
        Encode4ByteValue(sysex, 338, 1);    // ReverbShape: 1 (0-2)
        Encode4ByteValue(sysex, 342, 3);    // ReverbSize: 3 (0-7)
        Encode4ByteValue(sysex, 346, 3);    // ReverbHiColor: 3 (0-6)
        Encode4ByteValue(sysex, 354, 3);    // ReverbLoColor: 3 (0-6)
        Encode4ByteValue(sysex, 374, 50);   // ReverbMix: 50 (0-100)
        Encode4ByteValue(sysex, 390, 0);    // GateType: 0 (0-1)
        Encode4ByteValue(sysex, 398, 45);   // GateDamp: 45 (0-90)
        Encode4ByteValue(sysex, 402, 100);  // GateRelease: 100 (0-200)
        Encode4ByteValue(sysex, 410, 25);   // EqFreq1: 25 (25-113)
        Encode4ByteValue(sysex, 418, 8);    // EqWidth1: 8 (5-12)
        Encode4ByteValue(sysex, 422, 35);   // EqFreq2: 35 (25-113)
        Encode4ByteValue(sysex, 430, 8);    // EqWidth2: 8 (5-12)
        Encode4ByteValue(sysex, 434, 45);   // EqFreq3: 45 (25-113)
        Encode4ByteValue(sysex, 442, 8);    // EqWidth3: 8 (5-12)
        Encode4ByteValue(sysex, 454, 0);    // PitchType: 0 (0-4)
        Encode4ByteValue(sysex, 474, 25);   // PitchDelay1: 25 (0-50)
        Encode4ByteValue(sysex, 478, 25);   // PitchDelay2: 25 (0-50)
        Encode4ByteValue(sysex, 482, 50);   // PitchFeedback1OrKey: 50 (0-100)
        Encode4ByteValue(sysex, 486, 50);   // PitchFeedback2OrScale: 50 (0-100)

        sysex[519] = 0xF7;
        return sysex;
    }
}
