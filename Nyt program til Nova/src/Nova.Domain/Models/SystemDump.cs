using FluentResults;

namespace Nova.Domain.Models;

/// <summary>
/// Represents the System Dump from the Nova System pedal (global settings, 527 bytes).
/// Contains MIDI configuration, global tempo, routing preferences, and other system-level parameters.
/// </summary>
public class SystemDump
{
    /// <summary>
    /// Raw 527-byte SysEx message including F0 start and F7 end bytes.
    /// </summary>
    public byte[] RawSysEx { get; private init; } = Array.Empty<byte>();

    /// <summary>
    /// MIDI Channel setting (0-15).
    /// Extracted from byte offset 8 in the SysEx data.
    /// </summary>
    public int MidiChannel => RawSysEx.Length > 8 ? RawSysEx[8] : 0;

    /// <summary>
    /// Device ID (0-127).
    /// Extracted from byte offset 4 in the SysEx data.
    /// </summary>
    public int DeviceId => RawSysEx.Length > 4 ? RawSysEx[4] : 0;

    /// <summary>
    /// Indicates if MIDI Clock is enabled.
    /// Extracted from byte offset 9 bit 0 in the SysEx data.
    /// </summary>
    public bool IsMidiClockEnabled => RawSysEx.Length > 9 && (RawSysEx[9] & 0x01) != 0;

    /// <summary>
    /// Indicates if MIDI Program Change is enabled.
    /// Extracted from byte offset 9 bit 1 in the SysEx data.
    /// </summary>
    public bool IsMidiProgramChangeEnabled => RawSysEx.Length > 9 && (RawSysEx[9] & 0x02) != 0;

    /// <summary>
    /// Gets the firmware version string.
    /// Returns "Unknown" if version data is not available.
    /// </summary>
    /// <returns>Version string in format "X.Y" or "Unknown"</returns>
    public string GetVersionString()
    {
        if (RawSysEx.Length < 12)
            return "Unknown";

        // Version info typically stored at bytes 10-11 (major.minor)
        var major = RawSysEx[10];
        var minor = RawSysEx[11];
        
        return $"{major}.{minor}";
    }

    private SystemDump() { }

    /// <summary>
    /// Parses a 527-byte System Dump SysEx message.
    /// </summary>
    /// <param name="sysex">Full SysEx message including F0/F7</param>
    /// <returns>Result with SystemDump or error</returns>
    public static Result<SystemDump> FromSysEx(byte[] sysex)
    {
        // Validate length
        if (sysex.Length != 527)
            return Result.Fail($"Invalid System Dump length: expected 527 bytes, got {sysex.Length}");

        // Validate F0/F7 framing
        if (sysex[0] != 0xF0)
            return Result.Fail("SysEx must start with F0");
        if (sysex[526] != 0xF7)
            return Result.Fail("SysEx must end with F7");

        // Validate TC Electronic manufacturer ID (00 20 1F)
        if (sysex[1] != 0x00 || sysex[2] != 0x20 || sysex[3] != 0x1F)
            return Result.Fail("Invalid manufacturer ID: expected TC Electronic (00 20 1F)");

        // Validate Nova System model ID (0x63)
        if (sysex[5] != 0x63)
            return Result.Fail("Invalid model ID: expected Nova System (0x63)");

        // Validate Message ID (0x20 = Dump)
        if (sysex[6] != 0x20)
            return Result.Fail("Invalid message ID: expected Dump (0x20)");

        // Validate Data Type (0x02 = System Dump)
        if (sysex[7] != 0x02)
            return Result.Fail("Invalid data type: expected System Dump (0x02)");

        return Result.Ok(new SystemDump
        {
            RawSysEx = sysex
        });
    }

    /// <summary>
    /// Serializes the System Dump back to a 527-byte SysEx message.
    /// Simply returns the stored RawSysEx since we preserve original bytes.
    /// </summary>
    /// <returns>Result with 527-byte SysEx or error</returns>
    public Result<byte[]> ToSysEx()
    {
        if (RawSysEx == null || RawSysEx.Length != 527)
            return Result.Fail("SystemDump has no valid RawSysEx data");

        return Result.Ok(RawSysEx);
    }
}
