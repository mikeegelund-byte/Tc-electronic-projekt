using FluentResults;

namespace Nova.Domain.Models;

/// <summary>
/// Represents a single Nova System preset parsed from a 521-byte SysEx dump.
/// </summary>
public class Preset
{
    public int Number { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public byte[] RawSysEx { get; private set; } = Array.Empty<byte>();

    // Global parameters (bytes 38-53)
    public int TapTempo { get; private set; }  // 100-3000ms (bytes 38-41)
    public int Routing { get; private set; }   // 0=Semi-parallel, 1=Serial, 2=Parallel (bytes 42-45)
    public int LevelOutLeft { get; private set; }  // -100 to 0dB (bytes 46-49)
    public int LevelOutRight { get; private set; } // -100 to 0dB (bytes 50-53)

    // Effect on/off switches
    public bool CompressorEnabled { get; private set; }  // bytes 130-133
    public bool DriveEnabled { get; private set; }       // bytes 194-197
    public bool ModulationEnabled { get; private set; }  // bytes 258-261
    public bool DelayEnabled { get; private set; }       // bytes 322-325
    public bool ReverbEnabled { get; private set; }      // bytes 386-389

    // COMP (Compressor) effect parameters (bytes 70-129)
    public int CompType { get; private set; }       // 0=perc, 1=sustain, 2=advanced (bytes 70-73)
    public int CompThreshold { get; private set; }  // -30 to 0dB (bytes 74-77)
    public int CompRatio { get; private set; }      // 0=Off, 1-15=ratios (bytes 78-81)
    public int CompAttack { get; private set; }     // 0-16 table index (bytes 82-85)
    public int CompRelease { get; private set; }    // 13-23 table index (bytes 86-89)
    public int CompResponse { get; private set; }   // 1-10 (bytes 90-93)
    public int CompDrive { get; private set; }      // 1-20 (bytes 94-97)
    public int CompLevel { get; private set; }      // -12 to +12dB (bytes 98-101)

    // DRIVE effect parameters (bytes 102-193)
    public int DriveType { get; private set; }      // 0-6 (overdrive/dist/fuzz/line6drive/custom/tube/metal) (bytes 102-105)
    public int DriveGain { get; private set; }      // 0-100 (bytes 106-109)
    public int DriveLevel { get; private set; }     // -30 to +20dB (bytes 110-113)

    // BOOST effect parameters (bytes 114-125)
    public int BoostType { get; private set; }      // 0-2 (clean/mid/treble) (bytes 114-117)
    public int BoostGain { get; private set; }      // 0-30dB (bytes 118-121)
    public int BoostLevel { get; private set; }     // -30 to +20dB (bytes 122-125)

    // MOD (Modulation) effect parameters (bytes 198-257)
    public int ModType { get; private set; }        // 0-5 (chorus/flanger/vibrato/phaser/tremolo/panner) (bytes 198-201)
    public int ModSpeed { get; private set; }       // 0.050-20Hz table (bytes 202-205)
    public int ModDepth { get; private set; }       // 0-100% (bytes 206-209)
    public int ModTempo { get; private set; }       // 0-16 table (ignore, 2 to 1/32T) (bytes 210-213)
    public int ModHiCut { get; private set; }       // 20Hz-20kHz table (bytes 214-217)
    public int ModFeedback { get; private set; }    // -100 to +100% (bytes 218-221)
    public int ModDelayOrRange { get; private set; } // Multi-function: Delay/Range/Type (bytes 222-225)
    public int ModMix { get; private set; }         // 0-100% (bytes 250-253)

    private Preset() { }


    /// <summary>
    /// Parses a 521-byte SysEx message into a Preset.
    /// </summary>
    /// <param name="sysex">Complete SysEx message (F0...F7)</param>
    /// <returns>Success with Preset or Failure with error messages</returns>
    public static Result<Preset> FromSysEx(byte[] sysex)
    {
        // Validate length
        if (sysex.Length != 521)
            return Result.Fail($"Invalid preset length: expected 521 bytes, got {sysex.Length}");

        // Validate F0/F7 framing
        if (sysex[0] != 0xF0)
            return Result.Fail("SysEx must start with F0");

        if (sysex[520] != 0xF7)
            return Result.Fail("SysEx must end with F7");

        // Validate TC Electronic manufacturer ID (00 20 1F)
        if (sysex[1] != 0x00 || sysex[2] != 0x20 || sysex[3] != 0x1F)
            return Result.Fail("Invalid manufacturer ID: expected TC Electronic (00 20 1F)");

        // Validate Nova System model ID (0x63)
        if (sysex[5] != 0x63)
            return Result.Fail("Invalid model ID: expected Nova System (0x63)");

        // Validate message type (0x20 = Dump, 0x01 = Preset)
        if (sysex[6] != 0x20)
            return Result.Fail("Invalid message ID: expected Dump (0x20)");

        if (sysex[7] != 0x01)
            return Result.Fail("Invalid data type: expected Preset (0x01)");

        // Extract preset number (byte 8)
        int presetNumber = sysex[8];

        // Extract preset name (bytes 9-32 = 24 ASCII chars)
        var nameBytes = new byte[24];
        Array.Copy(sysex, 9, nameBytes, 0, 24);
        var presetName = System.Text.Encoding.ASCII.GetString(nameBytes).TrimEnd();

        // Extract parameters (Nova System uses 4-byte nibble encoding)
        int tapTempo = Decode4ByteValue(sysex, 38);
        int routing = Decode4ByteValue(sysex, 42);
        int levelOutLeft = Decode4ByteValue(sysex, 46);
        int levelOutRight = Decode4ByteValue(sysex, 50);

        // Extract COMP (Compressor) parameters (bytes 70-101)
        int compType = Decode4ByteValue(sysex, 70);
        int compThreshold = Decode4ByteValue(sysex, 74);
        int compRatio = Decode4ByteValue(sysex, 78);
        int compAttack = Decode4ByteValue(sysex, 82);
        int compRelease = Decode4ByteValue(sysex, 86);
        int compResponse = Decode4ByteValue(sysex, 90);
        int compDrive = Decode4ByteValue(sysex, 94);
        int compLevel = Decode4ByteValue(sysex, 98);

        // Extract DRIVE effect parameters (bytes 102-113)
        int driveType = Decode4ByteValue(sysex, 102);
        int driveGain = Decode4ByteValue(sysex, 106);
        int driveLevel = Decode4ByteValue(sysex, 110);

        // Extract BOOST effect parameters (bytes 114-125)
        int boostType = Decode4ByteValue(sysex, 114);
        int boostGain = Decode4ByteValue(sysex, 118);
        int boostLevel = Decode4ByteValue(sysex, 122);

        // Extract MOD (Modulation) effect parameters (bytes 198-257)
        int modType = Decode4ByteValue(sysex, 198);
        int modSpeed = Decode4ByteValue(sysex, 202);
        int modDepth = Decode4ByteValue(sysex, 206);
        int modTempo = Decode4ByteValue(sysex, 210);
        int modHiCut = Decode4ByteValue(sysex, 214);
        int modFeedback = Decode4ByteValue(sysex, 218);
        int modDelayOrRange = Decode4ByteValue(sysex, 222);
        int modMix = Decode4ByteValue(sysex, 250);

        // Extract effect on/off switches (4-byte encoded boolean: 0x00=off, 0x01=on)
        bool compressorEnabled = Decode4ByteValue(sysex, 130) == 1;
        bool driveEnabled = Decode4ByteValue(sysex, 194) == 1;
        bool modulationEnabled = Decode4ByteValue(sysex, 258) == 1;
        bool delayEnabled = Decode4ByteValue(sysex, 322) == 1;
        bool reverbEnabled = Decode4ByteValue(sysex, 386) == 1;

        return Result.Ok(new Preset
        {
            Number = presetNumber,
            Name = presetName,
            RawSysEx = sysex,
            TapTempo = tapTempo,
            Routing = routing,
            LevelOutLeft = levelOutLeft,
            LevelOutRight = levelOutRight,
            CompType = compType,
            CompThreshold = compThreshold,
            CompRatio = compRatio,
            CompAttack = compAttack,
            CompRelease = compRelease,
            CompResponse = compResponse,
            CompDrive = compDrive,
            CompLevel = compLevel,
            DriveType = driveType,
            DriveGain = driveGain,
            DriveLevel = driveLevel,
            BoostType = boostType,
            BoostGain = boostGain,
            BoostLevel = boostLevel,
            ModType = modType,
            ModSpeed = modSpeed,
            ModDepth = modDepth,
            ModTempo = modTempo,
            ModHiCut = modHiCut,
            ModFeedback = modFeedback,
            ModDelayOrRange = modDelayOrRange,
            ModMix = modMix,
            CompressorEnabled = compressorEnabled,
            DriveEnabled = driveEnabled,
            ModulationEnabled = modulationEnabled,
            DelayEnabled = delayEnabled,
            ReverbEnabled = reverbEnabled
        });
    }

    /// <summary>
    /// Serializes the preset back to a 521-byte SysEx message.
    /// Simply returns the stored RawSysEx since we preserve original bytes.
    /// </summary>
    /// <returns>Result with 521-byte SysEx or error</returns>
    public Result<byte[]> ToSysEx()
    {
        if (RawSysEx == null || RawSysEx.Length != 521)
            return Result.Fail("Preset has no valid RawSysEx data");

        return Result.Ok(RawSysEx);
    }

    /// <summary>
    /// Decodes a 4-byte encoded value from Nova System SysEx format.
    /// Nova System uses 4-byte encoding where 4 consecutive bytes encode a single int value.
    /// Based on reverse engineering: bytes appear to be Little Endian with 7-bit encoding.
    /// </summary>
    private static int Decode4ByteValue(byte[] sysex, int offset)
    {
        if (offset + 3 >= sysex.Length)
            return 0;

        // Nova System uses 4-byte little-endian encoding
        // Each byte is 7-bit (0-127), combined into 28-bit value
        int b0 = sysex[offset];     // LSB
        int b1 = sysex[offset + 1];
        int b2 = sysex[offset + 2];
        int b3 = sysex[offset + 3]; // MSB

        // Combine 4x7-bit values into single int (little-endian)
        int value = b0 | (b1 << 7) | (b2 << 14) | (b3 << 21);
        
        return value;
    }
}
