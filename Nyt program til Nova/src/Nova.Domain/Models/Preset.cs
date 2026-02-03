using FluentResults;

namespace Nova.Domain.Models;

/// <summary>
/// Represents a single Nova System preset parsed from a 520-byte SysEx dump.
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
    public int BoostLevel { get; private set; }     // 0 to 10dB (bytes 122-125) - unsigned per SYSEX_MAP_TABLES.md

    // MOD (Modulation) effect parameters (bytes 198-257)
    public int ModType { get; private set; }        // 0-5 (chorus/flanger/vibrato/phaser/tremolo/panner) (bytes 198-201)
    public int ModSpeed { get; private set; }       // 0.050-20Hz table (bytes 202-205)
    public int ModDepth { get; private set; }       // 0-100% (bytes 206-209)
    public int ModTempo { get; private set; }       // 0-16 table (ignore, 2 to 1/32T) (bytes 210-213)
    public int ModHiCut { get; private set; }       // 20Hz-20kHz table (bytes 214-217)
    public int ModFeedback { get; private set; }    // -100 to +100% (bytes 218-221)
    public int ModDelayOrRange { get; private set; } // Multi-function: Delay/Range/Type (bytes 222-225)
    public int ModMix { get; private set; }         // 0-100% (bytes 250-253)

    // DELAY effect parameters (bytes 262-321)
    public int DelayType { get; private set; }      // 0-5 (clean/analog/tape/dynamic/dual/ping-pong) (bytes 262-265)
    public int DelayTime { get; private set; }      // 0-1800ms (bytes 266-269)
    public int DelayTime2 { get; private set; }     // 0-1800ms (dual mode) (bytes 270-273)
    public int DelayTempo { get; private set; }     // 0-16 table (ignore, 2 to 1/32T) (bytes 274-277)
    public int DelayTempo2OrWidth { get; private set; } // Multi-function: Tempo2 (dual: 0-16) or Width (ping: 0-100%) (bytes 278-281)
    public int DelayFeedback { get; private set; }  // 0-120% (bytes 282-285)
    public int DelayClipOrFeedback2 { get; private set; } // Multi-function: Clip (analog/tape: 0-24dB) or Feedback2 (dual: 0-120%) (bytes 286-289)
    public int DelayHiCut { get; private set; }     // 20Hz-20kHz table (bytes 290-293)
    public int DelayLoCut { get; private set; }     // 20Hz-20kHz table (bytes 294-297)
    public int DelayMix { get; private set; }       // 0-100% (bytes 298-301)

    // REVERB effect parameters (bytes 326-385)
    public int ReverbType { get; private set; }     // 0-3 (spring/hall/room/plate) (bytes 326-329)
    public int ReverbDecay { get; private set; }    // 1-200 (0.1-20s by 0.1s) (bytes 330-333)
    public int ReverbPreDelay { get; private set; } // 0-100ms (bytes 334-337)
    public int ReverbShape { get; private set; }    // 0-2 (round/curved/square) (bytes 338-341)
    public int ReverbSize { get; private set; }     // 0-7 (box/.../huge) (bytes 342-345)
    public int ReverbHiColor { get; private set; }  // 0-6 (wool/.../glass) (bytes 346-349)
    public int ReverbHiLevel { get; private set; }  // -25 to +25dB (bytes 350-353)
    public int ReverbLoColor { get; private set; }  // 0-6 (thick/.../nobass) (bytes 354-357)
    public int ReverbLoLevel { get; private set; }  // -25 to +25dB (bytes 358-361)
    public int ReverbRoomLevel { get; private set; } // -100 to 0dB (bytes 362-365)
    public int ReverbLevel { get; private set; }    // -100 to 0dB (bytes 366-369)
    public int ReverbDiffuse { get; private set; }  // -25 to +25dB (bytes 370-373)
    public int ReverbMix { get; private set; }      // 0-100% (bytes 374-377)

    // EQ/GATE parameters (bytes 390-453)
    public int GateType { get; private set; }       // 0-1 (hard/soft) (bytes 390-393)
    public int GateThreshold { get; private set; }  // -60 to 0dB (bytes 394-397)
    public int GateDamp { get; private set; }       // 0-90dB (bytes 398-401)
    public int GateRelease { get; private set; }    // 0-200 dB/s (bytes 402-405)
    public int EqFreq1 { get; private set; }        // 41Hz-20kHz table (bytes 410-413)
    public int EqGain1 { get; private set; }        // -12 to +12dB (bytes 414-417)
    public int EqWidth1 { get; private set; }       // 0.3-1.6 oct table (bytes 418-421)
    public int EqFreq2 { get; private set; }        // 41Hz-20kHz table (bytes 422-425)
    public int EqGain2 { get; private set; }        // -12 to +12dB (bytes 426-429)
    public int EqWidth2 { get; private set; }       // 0.3-1.6 oct table (bytes 430-433)
    public int EqFreq3 { get; private set; }        // 41Hz-20kHz table (bytes 434-437)
    public int EqGain3 { get; private set; }        // -12 to +12dB (bytes 438-441)
    public int EqWidth3 { get; private set; }       // 0.3-1.6 oct table (bytes 442-445)

    // PITCH effect parameters (bytes 454-513)
    public int PitchType { get; private set; }      // 0-4 (shifter/octaver/whammy/detune/intelligent) (bytes 454-457)
    public int PitchVoice1 { get; private set; }    // -100 to +100 cents OR -13 to +13 degrees (bytes 458-461)
    public int PitchVoice2 { get; private set; }    // -100 to +100 cents OR -13 to +13 degrees (bytes 462-465)
    public int PitchPan1 { get; private set; }      // -50 to +50 (50L to 50R) (bytes 466-469)
    public int PitchPan2 { get; private set; }      // -50 to +50 (50L to 50R) (bytes 470-473)
    public int PitchDelay1 { get; private set; }    // 0-50ms (bytes 474-477)
    public int PitchDelay2 { get; private set; }    // 0-50ms (bytes 478-481)
    public int PitchFeedback1OrKey { get; private set; }    // 0-100% OR 0-12 Key (bytes 482-485)
    public int PitchFeedback2OrScale { get; private set; }  // 0-100% OR 0-13 Scale (bytes 486-489)
    public int PitchLevel1 { get; private set; }    // -100 to 0dB (bytes 490-493)
    public int PitchLevel2 { get; private set; }    // -100 to 0dB (bytes 494-497)

    private Preset() { }


    /// <summary>
    /// Parses a 520-byte SysEx message into a Preset.
    /// </summary>
    /// <param name="sysex">Complete SysEx message (F0...F7)</param>
    /// <returns>Success with Preset or Failure with error messages</returns>
    public static Result<Preset> FromSysEx(byte[] sysex)
    {
        var normalizedResult = NormalizeSysEx(sysex);
        if (normalizedResult.IsFailed)
            return Result.Fail(normalizedResult.Errors);

        sysex = normalizedResult.Value;

        // Validate F0/F7 framing
        if (sysex[0] != 0xF0)
            return Result.Fail("SysEx must start with F0");

        if (sysex[519] != 0xF7)
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
        int levelOutLeft = DecodeSignedDbValue(sysex, 46, -100, 0);
        int levelOutRight = DecodeSignedDbValue(sysex, 50, -100, 0);

        // Extract COMP (Compressor) parameters (bytes 70-101)
        int compType = Decode4ByteValue(sysex, 70);
        int compThreshold = DecodeSignedDbValue(sysex, 74, -30, 0);  // -30 to 0dB
        int compRatio = Decode4ByteValue(sysex, 78);
        int compAttack = Decode4ByteValue(sysex, 82);
        int compRelease = Decode4ByteValue(sysex, 86);
        int compResponse = Decode4ByteValue(sysex, 90);
        int compDrive = Decode4ByteValue(sysex, 94);  // 0-12dB (unsigned)
        int compLevel = DecodeSignedDbValue(sysex, 98, -12, 12);  // -12 to +12dB

        // Extract DRIVE effect parameters (bytes 102-113)
        int driveType = Decode4ByteValue(sysex, 102);
        int driveGain = Decode4ByteValue(sysex, 106);
        int driveLevel = DecodeSignedDbValue(sysex, 110, -30, 20);  // -30 to +20dB (signed, simple offset)

        // Extract BOOST effect parameters (bytes 114-125)
        int boostType = Decode4ByteValue(sysex, 114);
        int boostGain = Decode4ByteValue(sysex, 118);
        int boostLevel = Decode4ByteValue(sysex, 122);  // 0-10 dB (unsigned per spec)

        // Extract MOD (Modulation) effect parameters (bytes 198-257)
        int modType = Decode4ByteValue(sysex, 198);
        int modSpeed = Decode4ByteValue(sysex, 202);
        int modDepth = Decode4ByteValue(sysex, 206);
        int modTempo = Decode4ByteValue(sysex, 210);
        int modHiCut = Decode4ByteValue(sysex, 214);
        int modFeedback = DecodeSignedDbValue(sysex, 218, -100, 100);  // -100 to +100%
        int modDelayOrRange = Decode4ByteValue(sysex, 222);
        int modMix = Decode4ByteValue(sysex, 250);

        // Extract DELAY effect parameters (bytes 262-321)
        int delayType = Decode4ByteValue(sysex, 262);
        int delayTime = Decode4ByteValue(sysex, 266);
        int delayTime2 = Decode4ByteValue(sysex, 270);
        int delayTempo = Decode4ByteValue(sysex, 274);
        int delayTempo2OrWidth = Decode4ByteValue(sysex, 278);
        int delayFeedback = Decode4ByteValue(sysex, 282);
        int delayClipOrFeedback2 = Decode4ByteValue(sysex, 286);
        int delayHiCut = Decode4ByteValue(sysex, 290);
        int delayLoCut = Decode4ByteValue(sysex, 294);
        int delayMix = Decode4ByteValue(sysex, 298);

        // Extract REVERB effect parameters (bytes 326-385)
        int reverbType = Decode4ByteValue(sysex, 326);
        int reverbDecay = Decode4ByteValue(sysex, 330);
        int reverbPreDelay = Decode4ByteValue(sysex, 334);
        int reverbShape = Decode4ByteValue(sysex, 338);
        int reverbSize = Decode4ByteValue(sysex, 342);
        int reverbHiColor = Decode4ByteValue(sysex, 346);
        int reverbHiLevel = DecodeSignedDbValue(sysex, 350, -25, 25);  // -25 to +25dB
        int reverbLoColor = Decode4ByteValue(sysex, 354);
        int reverbLoLevel = DecodeSignedDbValue(sysex, 358, -25, 25);  // -25 to +25dB
        int reverbRoomLevel = DecodeSignedDbValue(sysex, 362, -100, 0);  // -100 to 0dB
        int reverbLevel = DecodeSignedDbValue(sysex, 366, -100, 0);  // -100 to 0dB
        int reverbDiffuse = DecodeSignedDbValue(sysex, 370, -25, 25);
        int reverbMix = Decode4ByteValue(sysex, 374);

        // Extract EQ/GATE parameters (bytes 390-453)
        int gateType = Decode4ByteValue(sysex, 390);
        int gateThreshold = DecodeSignedDbValue(sysex, 394, -60, 0);  // -60 to 0dB
        int gateDamp = Decode4ByteValue(sysex, 398);
        int gateRelease = Decode4ByteValue(sysex, 402);
        int eqFreq1 = Decode4ByteValue(sysex, 406);
        int eqGain1 = DecodeSignedDbValue(sysex, 414, -12, 12);  // -12 to +12dB
        int eqWidth1 = Decode4ByteValue(sysex, 418);
        int eqFreq2 = Decode4ByteValue(sysex, 422);
        int eqGain2 = DecodeSignedDbValue(sysex, 426, -12, 12);  // -12 to +12dB
        int eqWidth2 = Decode4ByteValue(sysex, 430);
        int eqFreq3 = Decode4ByteValue(sysex, 434);
        int eqGain3 = DecodeSignedDbValue(sysex, 438, -12, 12);  // -12 to +12dB
        int eqWidth3 = Decode4ByteValue(sysex, 442);

        // Extract PITCH effect parameters (bytes 454-513)
        int pitchType = Decode4ByteValue(sysex, 454);
        int pitchVoice1 = DecodeSignedDbValue(sysex, 458, -100, 100);  // -100 to +100 cents
        int pitchVoice2 = DecodeSignedDbValue(sysex, 462, -100, 100);  // -100 to +100 cents
        int pitchPan1 = DecodeSignedDbValue(sysex, 466, -50, 50);  // -50 to +50
        int pitchPan2 = DecodeSignedDbValue(sysex, 470, -50, 50);  // -50 to +50
        int pitchDelay1 = Decode4ByteValue(sysex, 474);
        int pitchDelay2 = Decode4ByteValue(sysex, 478);
        int pitchFeedback1OrKey = Decode4ByteValue(sysex, 482);
        int pitchFeedback2OrScale = Decode4ByteValue(sysex, 486);
        int pitchLevel1 = DecodeSignedDbValue(sysex, 490, -100, 0);  // -100 to 0dB
        int pitchLevel2 = DecodeSignedDbValue(sysex, 494, -100, 0);  // -100 to 0dB

        // Extract effect on/off switches (4-byte encoded boolean: 0x00=off, 0x01=on)
        bool compressorEnabled = Decode4ByteValue(sysex, 130) == 1;
        bool driveEnabled = Decode4ByteValue(sysex, 194) == 1;
        bool modulationEnabled = Decode4ByteValue(sysex, 258) == 1;
        bool delayEnabled = Decode4ByteValue(sysex, 322) == 1;
        bool reverbEnabled = Decode4ByteValue(sysex, 386) == 1;

        // ========================================
        // PARAMETER VALIDATION
        // ========================================

        if (pitchFeedback1OrKey < 0 || pitchFeedback1OrKey > 100)
            return Result.Fail($"PitchFeedback1OrKey value {pitchFeedback1OrKey} out of range (0-100)");
        if (pitchFeedback2OrScale < 0 || pitchFeedback2OrScale > 100)
            return Result.Fail($"PitchFeedback2OrScale value {pitchFeedback2OrScale} out of range (0-100)");

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
            DelayType = delayType,
            DelayTime = delayTime,
            DelayTime2 = delayTime2,
            DelayTempo = delayTempo,
            DelayTempo2OrWidth = delayTempo2OrWidth,
            DelayFeedback = delayFeedback,
            DelayClipOrFeedback2 = delayClipOrFeedback2,
            DelayHiCut = delayHiCut,
            DelayLoCut = delayLoCut,
            DelayMix = delayMix,
            ReverbType = reverbType,
            ReverbDecay = reverbDecay,
            ReverbPreDelay = reverbPreDelay,
            ReverbShape = reverbShape,
            ReverbSize = reverbSize,
            ReverbHiColor = reverbHiColor,
            ReverbHiLevel = reverbHiLevel,
            ReverbLoColor = reverbLoColor,
            ReverbLoLevel = reverbLoLevel,
            ReverbRoomLevel = reverbRoomLevel,
            ReverbLevel = reverbLevel,
            ReverbDiffuse = reverbDiffuse,
            ReverbMix = reverbMix,
            GateType = gateType,
            GateThreshold = gateThreshold,
            GateDamp = gateDamp,
            GateRelease = gateRelease,
            EqFreq1 = eqFreq1,
            EqGain1 = eqGain1,
            EqWidth1 = eqWidth1,
            EqFreq2 = eqFreq2,
            EqGain2 = eqGain2,
            EqWidth2 = eqWidth2,
            EqFreq3 = eqFreq3,
            EqGain3 = eqGain3,
            EqWidth3 = eqWidth3,
            PitchType = pitchType,
            PitchVoice1 = pitchVoice1,
            PitchVoice2 = pitchVoice2,
            PitchPan1 = pitchPan1,
            PitchPan2 = pitchPan2,
            PitchDelay1 = pitchDelay1,
            PitchDelay2 = pitchDelay2,
            PitchFeedback1OrKey = pitchFeedback1OrKey,
            PitchFeedback2OrScale = pitchFeedback2OrScale,
            PitchLevel1 = pitchLevel1,
            PitchLevel2 = pitchLevel2,
            CompressorEnabled = compressorEnabled,
            DriveEnabled = driveEnabled,
            ModulationEnabled = modulationEnabled,
            DelayEnabled = delayEnabled,
            ReverbEnabled = reverbEnabled
        });
    }

    /// <summary>
    /// Serializes the preset back to a 520-byte SysEx message.
    /// Simply returns the stored RawSysEx since we preserve original bytes.
    /// </summary>
    /// <returns>Result with 520-byte SysEx or error</returns>
    public Result<byte[]> ToSysEx()
    {
        if (RawSysEx == null || RawSysEx.Length != 520)
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

    /// <summary>
    /// Decodes signed dB values that use offset encoding.
    /// Two encoding strategies discovered from hardware investigation:
    /// 1. Large offset (2^24 = 16777216): For symmetric ranges (zero in middle)
    ///    Examples: -12 to +12, -25 to +25, -100 to +100
    /// 2. Simple offset: For asymmetric ranges (zero at end or near end)
    ///    Examples: -100 to 0, -90 to 0, -30 to 0, -30 to +20
    /// </summary>
    /// <param name="minimumValue">The minimum dB value of the parameter range.</param>
    /// <param name="maximumValue">The maximum dB value of the parameter range.</param>
    /// <remarks>
    /// The encoding strategy is selected automatically based on whether the range is symmetric
    /// (minimumValue + maximumValue == 0). Symmetric ranges use the large-offset strategy,
    /// while asymmetric ranges use the simple-offset strategy.
    /// </remarks>
    private static int DecodeSignedDbValue(byte[] sysex, int offset, int minimumValue, int maximumValue)
    {
        const int LARGE_OFFSET = 16777216; // 2^24

        int rawValue = Decode4ByteValue(sysex, offset);

        // Special case: raw value 0 indicates unused/uninitialized parameter
        // (e.g., feedback for chorus when hardware only uses it for flanger/phaser)
        // Map to safe default: 0 for symmetric ranges, minimumValue for asymmetric
        if (rawValue == 0)
        {
            return minimumValue + maximumValue == 0 ? 0 : minimumValue;
        }

        // Check if value looks like it uses the large offset encoding (values near ~16.7M)
        // Heuristic: Valid offsets are within a reasonable range of 16,777,216.
        // Small values (e.g., < 100,000) are likely stored as direct integers (e.g., positive values in some parameter types).
        if (rawValue > 16000000)
        {
            // ALL signed dB parameters use the same large offset strategy:
            // actual = raw - 2^24
            // This works for both symmetric (e.g., -12..+12) and asymmetric (e.g., -30..0) ranges
            return rawValue - LARGE_OFFSET;
        }

        // Return raw value for small integers (likely positive values stored directly)
        return rawValue;
    }

    private static Result<byte[]> NormalizeSysEx(byte[] sysex)
    {
        if (sysex.Length == 520)
            return Result.Ok(sysex);

        if (sysex.Length == 521 && sysex[^1] == 0xF7 && sysex[^2] == 0xF7)
            return Result.Ok(sysex[..^1]);

        return Result.Fail(
            $"Invalid preset length: expected 520 bytes (or 521 with double F7), got {sysex.Length}");
    }
}
