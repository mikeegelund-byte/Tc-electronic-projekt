namespace Nova.Domain.Midi;

/// <summary>
/// Static mapping of MIDI CC numbers to Nova System parameters.
/// Reference: MIDI_PROTOCOL.md - CC assignments are user-configurable in Nova System.
/// These constants represent common MIDI CC standards and recommended mappings.
/// </summary>
public static class MidiCCMap
{
    // Standard MIDI CC Numbers (from General MIDI specification)
    
    // Volume and Mix
    public const byte Volume = 7;
    public const byte Pan = 10;
    public const byte Expression = 11;
    
    // Modulation and Effects
    public const byte ModulationWheel = 1;
    public const byte EffectDepth = 91;
    public const byte ChorusDepth = 93;
    public const byte ReverbDepth = 94;
    
    // Nova System Recommended Mappings (user-assignable via MIDI CC menu)
    // These are suggested CC numbers for Nova-specific parameters
    public const byte CompressorThreshold = 16;
    public const byte DriveGain = 17;
    public const byte ModulationRate = 18;
    public const byte ModulationDepth = 19;
    public const byte DelayTime = 20;
    public const byte DelayFeedback = 21;
    public const byte ReverbTime = 22;
    
    // Nova System Effect Bypass Controls (assignable)
    public const byte TapTempo = 64;
    public const byte DriveOnOff = 65;
    public const byte CompOnOff = 66;
    public const byte NoiseGateOnOff = 67;
    public const byte EqOnOff = 68;
    public const byte BoostOnOff = 69;
    public const byte ModOnOff = 70;
    public const byte PitchOnOff = 71;
    public const byte DelayOnOff = 72;
    public const byte ReverbOnOff = 73;
    
    /// <summary>
    /// Validates if a CC number is within the valid MIDI CC range.
    /// </summary>
    /// <param name="ccNumber">CC number to validate</param>
    /// <returns>True if CC number is 0-127, false otherwise</returns>
    public static bool IsValidCC(byte ccNumber) => ccNumber <= 127;
    
    /// <summary>
    /// Gets a descriptive name for a CC number.
    /// </summary>
    /// <param name="ccNumber">CC number to get name for</param>
    /// <returns>Human-readable name or "Unknown CC" if not mapped</returns>
    public static string GetCCName(byte ccNumber)
    {
        return ccNumber switch
        {
            ModulationWheel => "Modulation Wheel",
            Volume => "Volume",
            Pan => "Pan",
            Expression => "Expression",
            CompressorThreshold => "Compressor Threshold",
            DriveGain => "Drive Gain",
            ModulationRate => "Modulation Rate",
            ModulationDepth => "Modulation Depth",
            DelayTime => "Delay Time",
            DelayFeedback => "Delay Feedback",
            ReverbTime => "Reverb Time",
            TapTempo => "Tap Tempo",
            DriveOnOff => "Drive On/Off",
            CompOnOff => "Compressor On/Off",
            NoiseGateOnOff => "Noise Gate On/Off",
            EqOnOff => "EQ On/Off",
            BoostOnOff => "Boost On/Off",
            ModOnOff => "Modulation On/Off",
            PitchOnOff => "Pitch On/Off",
            DelayOnOff => "Delay On/Off",
            ReverbOnOff => "Reverb On/Off",
            EffectDepth => "Effect Depth",
            ChorusDepth => "Chorus Depth",
            ReverbDepth => "Reverb Depth",
            _ => $"Unknown CC {ccNumber}"
        };
    }
}
