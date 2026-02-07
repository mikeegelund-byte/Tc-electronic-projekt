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

    // Global parameters (bytes 38-69)
    public int TapTempo { get; private set; }  // 100-3000ms (bytes 38-41)
    public int Routing { get; private set; }   // 0=Semi-parallel, 1=Serial, 2=Parallel (bytes 42-45)
    public int LevelOutLeft { get; private set; }  // -100 to 0dB (bytes 46-49)
    public int LevelOutRight { get; private set; } // -100 to 0dB (bytes 50-53)
    public int MapParameter { get; private set; }  // Pedal mapping parameter (bytes 54-57)
    public int MapMin { get; private set; }        // 0-100% (bytes 58-61)
    public int MapMid { get; private set; }        // 0-100% (bytes 62-65)
    public int MapMax { get; private set; }        // 0-100% (bytes 66-69)

    // Effect on/off switches
    public bool CompressorEnabled { get; private set; }  // bytes 130-133
    public bool DriveEnabled { get; private set; }       // bytes 194-197
    public bool ModulationEnabled { get; private set; }  // bytes 258-261
    public bool DelayEnabled { get; private set; }       // bytes 322-325
    public bool ReverbEnabled { get; private set; }      // bytes 386-389
    public bool EqEnabled { get; private set; }          // bytes 406-409
    public bool GateEnabled { get; private set; }        // bytes 450-453
    public bool PitchEnabled { get; private set; }       // bytes 514-517

    // COMP (Compressor) effect parameters (bytes 70-129)
    public int CompType { get; private set; }       // 0=perc, 1=sustain, 2=advanced (bytes 70-73)
    public int CompThreshold { get; private set; }  // -30 to 0dB (bytes 74-77)
    public int CompRatio { get; private set; }      // 0=Off, 1-15=ratios (bytes 78-81)
    public int CompAttack { get; private set; }     // 0-16 table index (bytes 82-85)
    public int CompRelease { get; private set; }    // 13-23 table index (bytes 86-89)
    public int CompResponse { get; private set; }   // 1-10 (bytes 90-93)
    public int CompDrive { get; private set; }      // 1-20 (bytes 94-97)
    public int CompLevel { get; private set; }      // -12 to +12dB (bytes 98-101)

    // DRIVE effect parameters (bytes 134-193)
    public int DriveType { get; private set; }      // 0-1 (overdrive/distortion) (bytes 134-137)
    public int DriveGain { get; private set; }      // 0-100 (bytes 138-141)
    public int DriveTone { get; private set; }      // 0-100 (bytes 142-145)
    public int DriveLevel { get; private set; }     // -100 to 0dB (bytes 190-193)
    public int BoostLevel { get; private set; }     // 0 to 10dB (bytes 182-185)
    public bool BoostEnabled { get; private set; }  // bytes 186-189

    // MOD (Modulation) effect parameters (bytes 198-257)
    public int ModType { get; private set; }        // 0-5 (chorus/flanger/vibrato/phaser/tremolo/panner) (bytes 198-201)
    public int ModSpeed { get; private set; }       // 0.050-20Hz table (bytes 202-205)
    public int ModDepth { get; private set; }       // 0-100% (bytes 206-209)
    public int ModTempo { get; private set; }       // 0-16 table (ignore, 2 to 1/32T) (bytes 210-213)
    public int ModHiCut { get; private set; }       // 20Hz-20kHz table (bytes 214-217)
    public int ModFeedback { get; private set; }    // -100 to +100% (bytes 218-221)
    public int ModDelayOrRange { get; private set; } // Multi-function: Delay/Range/Type (bytes 222-225)
    public int ModWidth { get; private set; }       // 0-100% (bytes 238-241)
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
    public int DelayOffsetOrPan1 { get; private set; } // Dynamic Offset (-200..200) or Dual Pan1 (-50..50) (bytes 298-301)
    public int DelaySenseOrPan2 { get; private set; }  // Dynamic Sense (-50..0) or Dual Pan2 (-50..50) (bytes 302-305)
    public int DelayDamp { get; private set; }      // 0-100 dB (bytes 306-309)
    public int DelayRelease { get; private set; }   // 20-1000 ms table index (bytes 310-313)
    public int DelayMix { get; private set; }       // 0-100% (bytes 314-317)

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
    public int PitchDirection { get; private set; } // 0=Down, 1=Up (octaver/whammy) (bytes 494-497)
    public int PitchRange { get; private set; }     // 1-2 octaves (octaver/whammy) (bytes 498-501)
    public int PitchMix { get; private set; }       // 0-100% (bytes 502-505)

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

        // Extract preset name (bytes 10-33 = 24 ASCII chars; byte 9 is reserved)
        var nameBytes = new byte[24];
        Array.Copy(sysex, 10, nameBytes, 0, 24);
        var presetName = System.Text.Encoding.ASCII.GetString(nameBytes).TrimEnd('\0', ' ');

        // Extract parameters (Nova System uses 4-byte nibble encoding)
        int tapTempo = Decode4ByteValue(sysex, 38);
        int routing = Decode4ByteValue(sysex, 42);
        int levelOutLeft = DecodeSignedDbValue(sysex, 46, -100, 0);
        int levelOutRight = DecodeSignedDbValue(sysex, 50, -100, 0);
        int mapParameter = Decode4ByteValue(sysex, 54);
        if (mapParameter > 127)
        {
            // Some dumps use 0x00FFFFFF as an "unset" sentinel for map parameter.
            mapParameter = 0;
        }
        int mapMin = Decode4ByteValue(sysex, 58);
        int mapMid = Decode4ByteValue(sysex, 62);
        int mapMax = Decode4ByteValue(sysex, 66);

        // Extract COMP (Compressor) parameters (bytes 70-101)
        int compType = Decode4ByteValue(sysex, 70);
        int compThreshold = DecodeSignedDbValue(sysex, 74, -30, 0);  // -30 to 0dB
        int compRatio = Decode4ByteValue(sysex, 78);
        int compAttack = Decode4ByteValue(sysex, 82);
        int compRelease = Decode4ByteValue(sysex, 86);
        int compResponse = Decode4ByteValue(sysex, 90);
        int compDrive = Decode4ByteValue(sysex, 94);  // 1-20 (unsigned)
        int compLevel = DecodeSignedDbValue(sysex, 98, -12, 12);  // -12 to +12dB

        // Extract DRIVE effect parameters (bytes 134-193)
        int driveType = Decode4ByteValue(sysex, 134);
        int driveGain = Decode4ByteValue(sysex, 138);
        int driveTone = Decode4ByteValue(sysex, 142);
        int boostLevel = Decode4ByteValue(sysex, 182);
        bool boostEnabled = Decode4ByteValue(sysex, 186) == 1;
        int driveLevel = DecodeSignedDbValue(sysex, 190, -100, 0);

        // Extract MOD (Modulation) effect parameters (bytes 198-257)
        int modType = Decode4ByteValue(sysex, 198);
        int modSpeed = Decode4ByteValue(sysex, 202);
        int modDepth = Decode4ByteValue(sysex, 206);
        int modTempo = Decode4ByteValue(sysex, 210);
        int modHiCut = Decode4ByteValue(sysex, 214);
        int modFeedback = DecodeSignedDbValue(sysex, 218, -100, 100);  // -100 to +100%
        int modDelayOrRangeRaw = Decode4ByteValue(sysex, 222);
        int modDelayOrRange = modType switch
        {
            0 or 1 => modDelayOrRangeRaw,                 // Chorus/Flanger: Delay (0-500)
            3 or 4 => Math.Clamp(modDelayOrRangeRaw, 0, 1), // Phaser/Tremolo: Range/Type (0-1)
            _ => 0                                         // Vibrato/Panner: unused
        };
        int modWidth = Decode4ByteValue(sysex, 238);
        int modMix = Decode4ByteValue(sysex, 250);

        // Extract DELAY effect parameters (bytes 262-321)
        int delayType = Decode4ByteValue(sysex, 262);
        int delayTime = Decode4ByteValue(sysex, 266);
        int delayTime2 = Decode4ByteValue(sysex, 270);
        int delayTempo = Decode4ByteValue(sysex, 274);
        int delayTempo2OrWidthRaw = Decode4ByteValue(sysex, 278);
        int delayTempo2OrWidth = delayType switch
        {
            4 => Math.Clamp(delayTempo2OrWidthRaw, 0, 16),  // Dual: Tempo2
            5 => Math.Clamp(delayTempo2OrWidthRaw, 0, 100), // Ping-Pong: Width
            _ => 0
        };
        int delayFeedback = Decode4ByteValue(sysex, 282);
        int delayClipOrFeedback2Raw = Decode4ByteValue(sysex, 286);
        int delayClipOrFeedback2 = delayType switch
        {
            1 or 2 => Math.Clamp(delayClipOrFeedback2Raw, 0, 24),   // Analog/Tape: Clip
            4 => Math.Clamp(delayClipOrFeedback2Raw, 0, 120),       // Dual: Feedback2
            _ => 0
        };
        int delayHiCut = Decode4ByteValue(sysex, 290);
        int delayLoCut = Decode4ByteValue(sysex, 294);
        int delayOffsetOrPan1;
        int delaySenseOrPan2;
        if (delayType == 3)
        {
            delayOffsetOrPan1 = DecodeSignedDbValue(sysex, 298, -200, 200);
            delaySenseOrPan2 = DecodeSignedDbValue(sysex, 302, -50, 0);
        }
        else if (delayType == 4)
        {
            delayOffsetOrPan1 = DecodeSignedDbValue(sysex, 298, -50, 50);
            delaySenseOrPan2 = DecodeSignedDbValue(sysex, 302, -50, 50);
        }
        else
        {
            delayOffsetOrPan1 = 0;
            delaySenseOrPan2 = 0;
        }

        int delayDamp = Decode4ByteValue(sysex, 306);
        int delayRelease = Decode4ByteValue(sysex, 310);
        int delayMix = Decode4ByteValue(sysex, 314);

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
        bool eqEnabled = Decode4ByteValue(sysex, 406) == 1;
        int eqFreq1 = Decode4ByteValue(sysex, 410);
        int eqGain1 = DecodeSignedDbValue(sysex, 414, -12, 12);  // -12 to +12dB
        int eqWidth1 = Decode4ByteValue(sysex, 418);
        int eqFreq2 = Decode4ByteValue(sysex, 422);
        int eqGain2 = DecodeSignedDbValue(sysex, 426, -12, 12);  // -12 to +12dB
        int eqWidth2 = Decode4ByteValue(sysex, 430);
        int eqFreq3 = Decode4ByteValue(sysex, 434);
        int eqGain3 = DecodeSignedDbValue(sysex, 438, -12, 12);  // -12 to +12dB
        int eqWidth3 = Decode4ByteValue(sysex, 442);
        bool gateEnabled = Decode4ByteValue(sysex, 450) == 1;

        // Extract PITCH effect parameters (bytes 454-513)
        int pitchType = Decode4ByteValue(sysex, 454);
        int pitchVoice1Raw = DecodeSignedDbValue(sysex, 458, -100, 100);  // -100 to +100 cents
        int pitchVoice2Raw = DecodeSignedDbValue(sysex, 462, -100, 100);  // -100 to +100 cents
        int pitchVoice1 = pitchType == 4
            ? Math.Clamp(pitchVoice1Raw, -13, 13)
            : Math.Clamp(pitchVoice1Raw, -100, 100);
        int pitchVoice2 = pitchType == 4
            ? Math.Clamp(pitchVoice2Raw, -13, 13)
            : Math.Clamp(pitchVoice2Raw, -100, 100);
        int pitchPan1 = DecodeSignedDbValue(sysex, 466, -50, 50);  // -50 to +50
        int pitchPan2 = DecodeSignedDbValue(sysex, 470, -50, 50);  // -50 to +50
        int pitchDelay1 = Decode4ByteValue(sysex, 474);
        int pitchDelay2 = Decode4ByteValue(sysex, 478);
        int pitchFeedback1OrKeyRaw = Decode4ByteValue(sysex, 482);
        int pitchFeedback2OrScaleRaw = Decode4ByteValue(sysex, 486);
        int pitchFeedback1OrKey = pitchFeedback1OrKeyRaw;
        int pitchFeedback2OrScale = pitchFeedback2OrScaleRaw;
        int pitchLevel1 = DecodeSignedDbValue(sysex, 490, -100, 0);  // -100 to 0dB
        int pitchLevel2 = pitchType == 1 || pitchType == 2
            ? 0
            : DecodeSignedDbValue(sysex, 494, -100, 0);
        int pitchDirection = pitchType == 1 || pitchType == 2
            ? Decode4ByteValue(sysex, 494)
            : 0;
        int pitchRange = pitchType == 1 || pitchType == 2
            ? Decode4ByteValue(sysex, 498)
            : 0;
        int pitchMix = Decode4ByteValue(sysex, 502);

        // Extract effect on/off switches (4-byte encoded boolean: 0x00=off, 0x01=on)
        bool compressorEnabled = Decode4ByteValue(sysex, 130) == 1;
        bool driveEnabled = Decode4ByteValue(sysex, 194) == 1;
        bool modulationEnabled = Decode4ByteValue(sysex, 258) == 1;
        bool delayEnabled = Decode4ByteValue(sysex, 322) == 1;
        bool reverbEnabled = Decode4ByteValue(sysex, 386) == 1;
        bool pitchEnabled = Decode4ByteValue(sysex, 514) == 1;

        // ========================================
        // PARAMETER VALIDATION
        // ========================================

        var validationResult = ValidateAllParameters(
            tapTempo, routing, levelOutLeft, levelOutRight,
            mapParameter, mapMin, mapMid, mapMax,
            compType, compThreshold, compRatio, compAttack, compRelease, compResponse, compDrive, compLevel,
            driveType, driveGain, driveTone, driveLevel, boostLevel,
            modType, modSpeed, modDepth, modTempo, modHiCut, modFeedback, modDelayOrRange, modWidth, modMix,
            delayType, delayTime, delayTime2, delayTempo, delayTempo2OrWidth, delayFeedback, delayClipOrFeedback2,
            delayHiCut, delayLoCut, delayOffsetOrPan1, delaySenseOrPan2, delayDamp, delayRelease, delayMix,
            reverbType, reverbDecay, reverbPreDelay, reverbShape, reverbSize, reverbHiColor, reverbHiLevel,
            reverbLoColor, reverbLoLevel, reverbRoomLevel, reverbLevel, reverbDiffuse, reverbMix,
            gateType, gateThreshold, gateDamp, gateRelease,
            eqFreq1, eqGain1, eqWidth1, eqFreq2, eqGain2, eqWidth2, eqFreq3, eqGain3, eqWidth3,
            pitchType, pitchVoice1, pitchVoice2, pitchPan1, pitchPan2, pitchDelay1, pitchDelay2,
            pitchFeedback1OrKey, pitchFeedback2OrScale, pitchLevel1, pitchLevel2, pitchDirection, pitchRange, pitchMix
        );

        if (validationResult.IsFailed)
            return Result.Fail<Preset>(validationResult.Errors.Select(e => e.Message).ToList());

        return Result.Ok(new Preset
        {
            Number = presetNumber,
            Name = presetName,
            RawSysEx = sysex,
            TapTempo = tapTempo,
            Routing = routing,
            LevelOutLeft = levelOutLeft,
            LevelOutRight = levelOutRight,
            MapParameter = mapParameter,
            MapMin = mapMin,
            MapMid = mapMid,
            MapMax = mapMax,
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
            DriveTone = driveTone,
            DriveLevel = driveLevel,
            BoostLevel = boostLevel,
            BoostEnabled = boostEnabled,
            ModType = modType,
            ModSpeed = modSpeed,
            ModDepth = modDepth,
            ModTempo = modTempo,
            ModHiCut = modHiCut,
            ModFeedback = modFeedback,
            ModDelayOrRange = modDelayOrRange,
            ModWidth = modWidth,
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
            DelayOffsetOrPan1 = delayOffsetOrPan1,
            DelaySenseOrPan2 = delaySenseOrPan2,
            DelayDamp = delayDamp,
            DelayRelease = delayRelease,
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
            EqEnabled = eqEnabled,
            EqFreq1 = eqFreq1,
            EqGain1 = eqGain1,
            EqWidth1 = eqWidth1,
            EqFreq2 = eqFreq2,
            EqGain2 = eqGain2,
            EqWidth2 = eqWidth2,
            EqFreq3 = eqFreq3,
            EqGain3 = eqGain3,
            EqWidth3 = eqWidth3,
            GateEnabled = gateEnabled,
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
            PitchDirection = pitchDirection,
            PitchRange = pitchRange,
            PitchMix = pitchMix,
            CompressorEnabled = compressorEnabled,
            DriveEnabled = driveEnabled,
            ModulationEnabled = modulationEnabled,
            DelayEnabled = delayEnabled,
            ReverbEnabled = reverbEnabled,
            PitchEnabled = pitchEnabled
        });
    }

    private static Result ValidateAllParameters(
        int tapTempo, int routing, int levelOutLeft, int levelOutRight,
        int mapParameter, int mapMin, int mapMid, int mapMax,
        int compType, int compThreshold, int compRatio, int compAttack, int compRelease,
        int compResponse, int compDrive, int compLevel,
        int driveType, int driveGain, int driveTone, int driveLevel, int boostLevel,
        int modType, int modSpeed, int modDepth, int modTempo, int modHiCut, int modFeedback, int modDelayOrRange, int modWidth, int modMix,
        int delayType, int delayTime, int delayTime2, int delayTempo, int delayTempo2OrWidth, int delayFeedback, int delayClipOrFeedback2,
        int delayHiCut, int delayLoCut, int delayOffsetOrPan1, int delaySenseOrPan2, int delayDamp, int delayRelease, int delayMix,
        int reverbType, int reverbDecay, int reverbPreDelay, int reverbShape, int reverbSize, int reverbHiColor, int reverbHiLevel,
        int reverbLoColor, int reverbLoLevel, int reverbRoomLevel, int reverbLevel, int reverbDiffuse, int reverbMix,
        int gateType, int gateThreshold, int gateDamp, int gateRelease,
        int eqFreq1, int eqGain1, int eqWidth1, int eqFreq2, int eqGain2, int eqWidth2, int eqFreq3, int eqGain3, int eqWidth3,
        int pitchType, int pitchVoice1, int pitchVoice2, int pitchPan1, int pitchPan2, int pitchDelay1, int pitchDelay2,
        int pitchFeedback1OrKey, int pitchFeedback2OrScale, int pitchLevel1, int pitchLevel2, int pitchDirection, int pitchRange, int pitchMix)
    {
        var errors = new List<string>();

        // Global parameters
        if (tapTempo < 100 || tapTempo > 3000)
            errors.Add($"TapTempo value {tapTempo} out of range (100-3000)");
        if (routing < 0 || routing > 2)
            errors.Add($"Routing value {routing} out of range (0-2)");
        if (levelOutLeft < -100 || levelOutLeft > 0)
            errors.Add($"LevelOutLeft value {levelOutLeft} out of range (-100 to 0)");
        if (levelOutRight < -100 || levelOutRight > 0)
            errors.Add($"LevelOutRight value {levelOutRight} out of range (-100 to 0)");
        if (mapParameter < 0 || mapParameter > 127)
            errors.Add($"MapParameter value {mapParameter} out of range (0-127)");
        if (mapMin < 0 || mapMin > 100)
            errors.Add($"MapMin value {mapMin} out of range (0-100)");
        if (mapMid < 0 || mapMid > 100)
            errors.Add($"MapMid value {mapMid} out of range (0-100)");
        if (mapMax < 0 || mapMax > 100)
            errors.Add($"MapMax value {mapMax} out of range (0-100)");

        // COMP block
        if (compType < 0 || compType > 2)
            errors.Add($"CompType value {compType} out of range (0-2)");
        if (compThreshold < -30 || compThreshold > 0)
            errors.Add($"CompThreshold value {compThreshold} out of range (-30 to 0)");
        if (compRatio < 0 || compRatio > 15)
            errors.Add($"CompRatio value {compRatio} out of range (0-15)");
        if (compAttack < 0 || compAttack > 16)
            errors.Add($"CompAttack value {compAttack} out of range (0-16)");
        if (compRelease < 13 || compRelease > 23)
            errors.Add($"CompRelease value {compRelease} out of range (13-23)");
        if (compResponse < 0 || compResponse > 10)
            errors.Add($"CompResponse value {compResponse} out of range (0-10)");
        if (compDrive < 0 || compDrive > 20)
            errors.Add($"CompDrive value {compDrive} out of range (0-20)");
        if (compLevel < -12 || compLevel > 12)
            errors.Add($"CompLevel value {compLevel} out of range (-12 to 12)");

        // DRIVE block
        if (driveType < 0 || driveType > 1)
            errors.Add($"DriveType value {driveType} out of range (0-1)");
        if (driveGain < 0 || driveGain > 100)
            errors.Add($"DriveGain value {driveGain} out of range (0-100)");
        if (driveTone < 0 || driveTone > 100)
            errors.Add($"DriveTone value {driveTone} out of range (0-100)");
        if (driveLevel < -100 || driveLevel > 0)
            errors.Add($"DriveLevel value {driveLevel} out of range (-100 to 0)");
        if (boostLevel < 0 || boostLevel > 10)
            errors.Add($"BoostLevel value {boostLevel} out of range (0-10)");

        // MOD block
        if (modType < 0 || modType > 5)
            errors.Add($"ModType value {modType} out of range (0-5)");
        if (modSpeed < 0 || modSpeed > 81)
            errors.Add($"ModSpeed value {modSpeed} out of range (0-81)");
        if (modDepth < 0 || modDepth > 100)
            errors.Add($"ModDepth value {modDepth} out of range (0-100)");
        if (modTempo < 0 || modTempo > 16)
            errors.Add($"ModTempo value {modTempo} out of range (0-16)");
        if (modHiCut < 0 || modHiCut > 61)
            errors.Add($"ModHiCut value {modHiCut} out of range (0-61)");
        if (modFeedback < -100 || modFeedback > 100)
            errors.Add($"ModFeedback value {modFeedback} out of range (-100 to 100)");
        if (modType == 0 || modType == 1)
        {
            // Chorus/Flanger: Delay 0-50ms in 0.1ms steps (0-500)
            if (modDelayOrRange < 0 || modDelayOrRange > 500)
                errors.Add($"ModDelayOrRange value {modDelayOrRange} out of range (0-500)");
        }
        else if (modType == 3)
        {
            // Phaser: Range (0=Low, 1=High)
            if (modDelayOrRange < 0 || modDelayOrRange > 1)
                errors.Add($"ModDelayOrRange value {modDelayOrRange} out of range (0-1) for Phaser");
        }
        else if (modType == 4)
        {
            // Tremolo: Type (0=Soft, 1=Hard)
            if (modDelayOrRange < 0 || modDelayOrRange > 1)
                errors.Add($"ModDelayOrRange value {modDelayOrRange} out of range (0-1) for Tremolo");
        }
        else
        {
            // Vibrato/Panner: unused in spec, allow 0 to avoid invalidating real dumps
            if (modDelayOrRange != 0)
                errors.Add($"ModDelayOrRange value {modDelayOrRange} out of range (expected 0 for Mod type {modType})");
        }
        if (modWidth < 0 || modWidth > 100)
            errors.Add($"ModWidth value {modWidth} out of range (0-100)");
        if (modMix < 0 || modMix > 100)
            errors.Add($"ModMix value {modMix} out of range (0-100)");

        // DELAY block
        if (delayType < 0 || delayType > 5)
            errors.Add($"DelayType value {delayType} out of range (0-5)");
        if (delayTime < 0 || delayTime > 1800)
            errors.Add($"DelayTime value {delayTime} out of range (0-1800)");
        if (delayTime2 < 0 || delayTime2 > 1800)
            errors.Add($"DelayTime2 value {delayTime2} out of range (0-1800)");
        if (delayTempo < 0 || delayTempo > 16)
            errors.Add($"DelayTempo value {delayTempo} out of range (0-16)");
        if (delayType == 4)
        {
            if (delayTempo2OrWidth < 0 || delayTempo2OrWidth > 16)
                errors.Add($"DelayTempo2OrWidth value {delayTempo2OrWidth} out of range (0-16) for Dual");
        }
        else if (delayType == 5)
        {
            if (delayTempo2OrWidth < 0 || delayTempo2OrWidth > 100)
                errors.Add($"DelayTempo2OrWidth value {delayTempo2OrWidth} out of range (0-100) for Ping-Pong");
        }
        else if (delayTempo2OrWidth != 0)
        {
            errors.Add($"DelayTempo2OrWidth value {delayTempo2OrWidth} out of range (expected 0 for Delay type {delayType})");
        }
        if (delayFeedback < 0 || delayFeedback > 120)
            errors.Add($"DelayFeedback value {delayFeedback} out of range (0-120)");
        if (delayType == 1 || delayType == 2)
        {
            if (delayClipOrFeedback2 < 0 || delayClipOrFeedback2 > 24)
                errors.Add($"DelayClipOrFeedback2 value {delayClipOrFeedback2} out of range (0-24) for Analog/Tape");
        }
        else if (delayType == 4)
        {
            if (delayClipOrFeedback2 < 0 || delayClipOrFeedback2 > 120)
                errors.Add($"DelayClipOrFeedback2 value {delayClipOrFeedback2} out of range (0-120) for Dual");
        }
        else if (delayClipOrFeedback2 != 0)
        {
            errors.Add($"DelayClipOrFeedback2 value {delayClipOrFeedback2} out of range (expected 0 for Delay type {delayType})");
        }
        if (delayHiCut < 0 || delayHiCut > 61)
            errors.Add($"DelayHiCut value {delayHiCut} out of range (0-61)");
        if (delayLoCut < 0 || delayLoCut > 61)
            errors.Add($"DelayLoCut value {delayLoCut} out of range (0-61)");
        if (delayType == 3)
        {
            if (delayOffsetOrPan1 < -200 || delayOffsetOrPan1 > 200)
                errors.Add($"DelayOffsetOrPan1 value {delayOffsetOrPan1} out of range (-200 to 200) for Dynamic");
            if (delaySenseOrPan2 < -50 || delaySenseOrPan2 > 0)
                errors.Add($"DelaySenseOrPan2 value {delaySenseOrPan2} out of range (-50 to 0) for Dynamic");
        }
        else if (delayType == 4)
        {
            if (delayOffsetOrPan1 < -50 || delayOffsetOrPan1 > 50)
                errors.Add($"DelayOffsetOrPan1 value {delayOffsetOrPan1} out of range (-50 to 50) for Dual");
            if (delaySenseOrPan2 < -50 || delaySenseOrPan2 > 50)
                errors.Add($"DelaySenseOrPan2 value {delaySenseOrPan2} out of range (-50 to 50) for Dual");
        }
        else
        {
            if (delayOffsetOrPan1 != 0)
                errors.Add($"DelayOffsetOrPan1 value {delayOffsetOrPan1} out of range (expected 0 for Delay type {delayType})");
            if (delaySenseOrPan2 != 0)
                errors.Add($"DelaySenseOrPan2 value {delaySenseOrPan2} out of range (expected 0 for Delay type {delayType})");
        }
        if (delayDamp < 0 || delayDamp > 100)
            errors.Add($"DelayDamp value {delayDamp} out of range (0-100)");
        if (delayType == 3)
        {
            if (delayRelease < 11 || delayRelease > 21)
                errors.Add($"DelayRelease value {delayRelease} out of range (11-21)");
        }
        else if (delayRelease != 0)
        {
            errors.Add($"DelayRelease value {delayRelease} out of range (0 for non-dynamic)");
        }
        if (delayMix < 0 || delayMix > 100)
            errors.Add($"DelayMix value {delayMix} out of range (0-100)");

        // REVERB block
        if (reverbType < 0 || reverbType > 3)
            errors.Add($"ReverbType value {reverbType} out of range (0-3)");
        if (reverbDecay < 1 || reverbDecay > 200)
            errors.Add($"ReverbDecay value {reverbDecay} out of range (1-200)");
        if (reverbPreDelay < 0 || reverbPreDelay > 100)
            errors.Add($"ReverbPreDelay value {reverbPreDelay} out of range (0-100)");
        if (reverbShape < 0 || reverbShape > 2)
            errors.Add($"ReverbShape value {reverbShape} out of range (0-2)");
        if (reverbSize < 0 || reverbSize > 7)
            errors.Add($"ReverbSize value {reverbSize} out of range (0-7)");
        if (reverbHiColor < 0 || reverbHiColor > 6)
            errors.Add($"ReverbHiColor value {reverbHiColor} out of range (0-6)");
        if (reverbHiLevel < -25 || reverbHiLevel > 25)
            errors.Add($"ReverbHiLevel value {reverbHiLevel} out of range (-25 to 25)");
        if (reverbLoColor < 0 || reverbLoColor > 6)
            errors.Add($"ReverbLoColor value {reverbLoColor} out of range (0-6)");
        if (reverbLoLevel < -25 || reverbLoLevel > 25)
            errors.Add($"ReverbLoLevel value {reverbLoLevel} out of range (-25 to 25)");
        if (reverbRoomLevel < -100 || reverbRoomLevel > 0)
            errors.Add($"ReverbRoomLevel value {reverbRoomLevel} out of range (-100 to 0)");
        if (reverbLevel < -100 || reverbLevel > 0)
            errors.Add($"ReverbLevel value {reverbLevel} out of range (-100 to 0)");
        if (reverbDiffuse < -25 || reverbDiffuse > 25)
            errors.Add($"ReverbDiffuse value {reverbDiffuse} out of range (-25 to 25)");
        if (reverbMix < 0 || reverbMix > 100)
            errors.Add($"ReverbMix value {reverbMix} out of range (0-100)");

        // EQ/GATE block
        if (gateType < 0 || gateType > 1)
            errors.Add($"GateType value {gateType} out of range (0-1)");
        if (gateThreshold < -60 || gateThreshold > 0)
            errors.Add($"GateThreshold value {gateThreshold} out of range (-60 to 0)");
        if (gateDamp < 0 || gateDamp > 90)
            errors.Add($"GateDamp value {gateDamp} out of range (0-90)");
        if (gateRelease < 0 || gateRelease > 200)
            errors.Add($"GateRelease value {gateRelease} out of range (0-200)");
        if (eqFreq1 < 25 || eqFreq1 > 210)
            errors.Add($"EqFreq1 value {eqFreq1} out of range (25-210)");
        if (eqGain1 < -12 || eqGain1 > 12)
            errors.Add($"EqGain1 value {eqGain1} out of range (-12 to 12)");
        if (eqWidth1 < 5 || eqWidth1 > 12)
            errors.Add($"EqWidth1 value {eqWidth1} out of range (5-12)");
        if (eqFreq2 < 25 || eqFreq2 > 210)
            errors.Add($"EqFreq2 value {eqFreq2} out of range (25-210)");
        if (eqGain2 < -12 || eqGain2 > 12)
            errors.Add($"EqGain2 value {eqGain2} out of range (-12 to 12)");
        if (eqWidth2 < 5 || eqWidth2 > 12)
            errors.Add($"EqWidth2 value {eqWidth2} out of range (5-12)");
        if (eqFreq3 < 25 || eqFreq3 > 210)
            errors.Add($"EqFreq3 value {eqFreq3} out of range (25-210)");
        if (eqGain3 < -12 || eqGain3 > 12)
            errors.Add($"EqGain3 value {eqGain3} out of range (-12 to 12)");
        if (eqWidth3 < 5 || eqWidth3 > 12)
            errors.Add($"EqWidth3 value {eqWidth3} out of range (5-12)");

        // PITCH block
        if (pitchType < 0 || pitchType > 4)
            errors.Add($"PitchType value {pitchType} out of range (0-4)");
        if (pitchType == 4)
        {
            if (pitchVoice1 < -13 || pitchVoice1 > 13)
                errors.Add($"PitchVoice1 value {pitchVoice1} out of range (-13 to 13) for Intelligent");
            if (pitchVoice2 < -13 || pitchVoice2 > 13)
                errors.Add($"PitchVoice2 value {pitchVoice2} out of range (-13 to 13) for Intelligent");
        }
        else
        {
            if (pitchVoice1 < -100 || pitchVoice1 > 100)
                errors.Add($"PitchVoice1 value {pitchVoice1} out of range (-100 to 100)");
            if (pitchVoice2 < -100 || pitchVoice2 > 100)
                errors.Add($"PitchVoice2 value {pitchVoice2} out of range (-100 to 100)");
        }
        if (pitchPan1 < -50 || pitchPan1 > 50)
            errors.Add($"PitchPan1 value {pitchPan1} out of range (-50 to 50)");
        if (pitchPan2 < -50 || pitchPan2 > 50)
            errors.Add($"PitchPan2 value {pitchPan2} out of range (-50 to 50)");
        if (pitchDelay1 < 0 || pitchDelay1 > 50)
            errors.Add($"PitchDelay1 value {pitchDelay1} out of range (0-50)");
        if (pitchDelay2 < 0 || pitchDelay2 > 50)
            errors.Add($"PitchDelay2 value {pitchDelay2} out of range (0-50)");
        if (pitchType == 4)
        {
            if (pitchFeedback1OrKey < 0 || pitchFeedback1OrKey > 12)
                errors.Add($"PitchFeedback1OrKey value {pitchFeedback1OrKey} out of range (0-12)");
            if (pitchFeedback2OrScale < 0 || pitchFeedback2OrScale > 13)
                errors.Add($"PitchFeedback2OrScale value {pitchFeedback2OrScale} out of range (0-13)");
        }
        else
        {
            if (pitchFeedback1OrKey < 0 || pitchFeedback1OrKey > 100)
                errors.Add($"PitchFeedback1OrKey value {pitchFeedback1OrKey} out of range (0-100)");
            if (pitchFeedback2OrScale < 0 || pitchFeedback2OrScale > 100)
                errors.Add($"PitchFeedback2OrScale value {pitchFeedback2OrScale} out of range (0-100)");
        }
        if (pitchLevel1 < -100 || pitchLevel1 > 0)
            errors.Add($"PitchLevel1 value {pitchLevel1} out of range (-100 to 0)");
        if (pitchType == 1 || pitchType == 2)
        {
            if (pitchDirection < 0 || pitchDirection > 1)
                errors.Add($"PitchDirection value {pitchDirection} out of range (0-1)");
            if (pitchRange < 1 || pitchRange > 2)
                errors.Add($"PitchRange value {pitchRange} out of range (1-2) for Octaver/Whammy");
            if (pitchLevel2 != 0)
                errors.Add($"PitchLevel2 value {pitchLevel2} out of range (expected 0 for Octaver/Whammy)");
        }
        else
        {
            if (pitchLevel2 < -100 || pitchLevel2 > 0)
                errors.Add($"PitchLevel2 value {pitchLevel2} out of range (-100 to 0)");
            if (pitchDirection != 0)
                errors.Add($"PitchDirection value {pitchDirection} out of range (expected 0 for non-Octaver/Whammy)");
            if (pitchRange != 0)
                errors.Add($"PitchRange value {pitchRange} out of range (expected 0 for non-Octaver/Whammy)");
        }
        if (pitchMix < 0 || pitchMix > 100)
            errors.Add($"PitchMix value {pitchMix} out of range (0-100)");

        if (errors.Any())
            return Result.Fail(string.Join("; ", errors));

        return Result.Ok();
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
}
