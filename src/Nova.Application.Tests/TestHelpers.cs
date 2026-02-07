using Nova.Domain.Models;

namespace Nova.Application.Tests;

/// <summary>
/// Shared test utilities for creating valid test data
/// </summary>
public static class TestHelpers
{
    /// <summary>
    /// Creates a valid 520-byte preset SysEx message with all parameters set to valid defaults
    /// </summary>
    public static byte[] CreateValidPresetSysEx(int presetNumber = 31, string name = "Test Preset")
    {
        var bytes = new byte[520];

        // Header
        bytes[0] = 0xF0;  // SysEx start
        bytes[1] = 0x00;  // TC Electronic manufacturer ID
        bytes[2] = 0x20;
        bytes[3] = 0x1F;
        bytes[4] = 0x00;  // Device ID
        bytes[5] = 0x63;  // Nova System model ID
        bytes[6] = 0x20;  // Dump message
        bytes[7] = 0x01;  // Preset data type
        bytes[8] = (byte)presetNumber;  // Preset number

        // Name (bytes 10-33): 24 ASCII characters (byte 9 reserved)
        var paddedName = name.Length > 24 ? name.Substring(0, 24) : name.PadRight(24);
        var nameBytes = System.Text.Encoding.ASCII.GetBytes(paddedName);
        Array.Copy(nameBytes, 0, bytes, 10, Math.Min(24, nameBytes.Length));

        // Set minimum valid parameter values to pass validation
        Encode4ByteValue(bytes, 38, 500);   // TapTempo: 500ms (100-3000)
        Encode4ByteValue(bytes, 42, 0);     // Routing: 0 (0-2)
        Encode4ByteValue(bytes, 70, 0);     // CompType: 0 (0-2)
        Encode4ByteValue(bytes, 78, 0);     // CompRatio: 0 (0-15)
        Encode4ByteValue(bytes, 82, 0);     // CompAttack: 0 (0-16)
        Encode4ByteValue(bytes, 86, 15);    // CompRelease: 15 (13-23)
        Encode4ByteValue(bytes, 90, 1);     // CompResponse: 1 (1-10)
        Encode4ByteValue(bytes, 94, 1);     // CompDrive: 1 (1-20)
        Encode4ByteValue(bytes, 134, 0);    // DriveType: 0 (0-1)
        Encode4ByteValue(bytes, 138, 50);   // DriveGain: 50 (0-100)
        Encode4ByteValue(bytes, 142, 50);   // DriveTone: 50 (0-100)
        Encode4ByteValue(bytes, 182, 5);    // BoostLevel: 5 (0-10)
        Encode4ByteValue(bytes, 186, 0);    // BoostEnabled: 0 (off)
        Encode4ByteValue(bytes, 190, 0);    // DriveLevel: raw 0 (min)
        Encode4ByteValue(bytes, 198, 0);    // ModType: 0 (0-5)
        Encode4ByteValue(bytes, 206, 50);   // ModDepth: 50 (0-100)
        Encode4ByteValue(bytes, 210, 8);    // ModTempo: 8 (0-16)
        Encode4ByteValue(bytes, 250, 50);   // ModMix: 50 (0-100)
        Encode4ByteValue(bytes, 262, 0);    // DelayType: 0 (0-5)
        Encode4ByteValue(bytes, 266, 500);  // DelayTime: 500 (0-1800)
        Encode4ByteValue(bytes, 270, 500);  // DelayTime2: 500 (0-1800)
        Encode4ByteValue(bytes, 274, 8);    // DelayTempo: 8 (0-16)
        Encode4ByteValue(bytes, 278, 0);    // DelayTempo2OrWidth: 0 (unused for clean)
        Encode4ByteValue(bytes, 282, 50);   // DelayFeedback: 50 (0-120)
        Encode4ByteValue(bytes, 286, 0);    // DelayClipOrFeedback2: 0 (unused for clean)
        Encode4ByteValue(bytes, 314, 50);   // DelayMix: 50 (0-100)
        Encode4ByteValue(bytes, 326, 0);    // ReverbType: 0 (0-3)
        Encode4ByteValue(bytes, 330, 50);   // ReverbDecay: 50 (1-200)
        Encode4ByteValue(bytes, 334, 30);   // ReverbPreDelay: 30 (0-100)
        Encode4ByteValue(bytes, 338, 1);    // ReverbShape: 1 (0-2)
        Encode4ByteValue(bytes, 342, 3);    // ReverbSize: 3 (0-7)
        Encode4ByteValue(bytes, 346, 3);    // ReverbHiColor: 3 (0-6)
        Encode4ByteValue(bytes, 354, 3);    // ReverbLoColor: 3 (0-6)
        Encode4ByteValue(bytes, 374, 50);   // ReverbMix: 50 (0-100)
        Encode4ByteValue(bytes, 390, 0);    // GateType: 0 (0-1)
        Encode4ByteValue(bytes, 398, 45);   // GateDamp: 45 (0-90)
        Encode4ByteValue(bytes, 402, 100);  // GateRelease: 100 (0-200)
        Encode4ByteValue(bytes, 410, 25);   // EqFreq1: 25 (25-113)
        Encode4ByteValue(bytes, 418, 8);    // EqWidth1: 8 (5-12)
        Encode4ByteValue(bytes, 422, 35);   // EqFreq2: 35 (25-113)
        Encode4ByteValue(bytes, 430, 8);    // EqWidth2: 8 (5-12)
        Encode4ByteValue(bytes, 434, 45);   // EqFreq3: 45 (25-113)
        Encode4ByteValue(bytes, 442, 8);    // EqWidth3: 8 (5-12)
        Encode4ByteValue(bytes, 454, 0);    // PitchType: 0 (0-4)
        Encode4ByteValue(bytes, 474, 25);   // PitchDelay1: 25 (0-50)
        Encode4ByteValue(bytes, 478, 25);   // PitchDelay2: 25 (0-50)
        Encode4ByteValue(bytes, 482, 50);   // PitchFeedback1OrKey: 50 (0-100)
        Encode4ByteValue(bytes, 486, 50);   // PitchFeedback2OrScale: 50 (0-100)

        // Calculate checksum (sum of bytes 34-517 & 0x7F)
        int checksum = 0;
        for (int i = 34; i <= 517; i++)
        {
            checksum += bytes[i];
        }
        bytes[518] = (byte)(checksum & 0x7F);

        bytes[519] = 0xF7;  // SysEx end

        return bytes;
    }

    /// <summary>
    /// Creates a valid Preset object for testing
    /// </summary>
    public static Preset CreateValidPreset(int presetNumber = 31, string name = "Test Preset")
    {
        var sysex = CreateValidPresetSysEx(presetNumber, name);
        return Preset.FromSysEx(sysex).Value;
    }

    private static void Encode4ByteValue(byte[] sysex, int offset, int value)
    {
        sysex[offset] = (byte)(value & 0x7F);
        sysex[offset + 1] = (byte)((value >> 7) & 0x7F);
        sysex[offset + 2] = (byte)((value >> 14) & 0x7F);
        sysex[offset + 3] = (byte)((value >> 21) & 0x7F);
    }
}
