using FluentResults;

namespace Nova.Domain.Models;

/// <summary>
/// Represents the System Dump from the Nova System pedal (global settings, 527 bytes).
/// Contains MIDI configuration, global tempo, routing preferences, CC mappings, and other system-level parameters.
/// </summary>
public class SystemDump
{
    /// <summary>
    /// Raw 527-byte SysEx message including F0 start and F7 end bytes.
    /// </summary>
    public byte[] RawSysEx { get; private init; } = Array.Empty<byte>();

    /// <summary>
    /// Byte offset where CC mappings start in the System Dump.
    /// Each CC mapping consists of 2 bytes: CC number + Parameter ID.
    /// Total: 64 mappings × 2 bytes = 128 bytes (offsets 34-161).
    /// </summary>
    private const int CC_MAPPING_START_OFFSET = 34;
    private const int CC_MAPPING_COUNT = 64;
    private const int BYTES_PER_CC_MAPPING = 2;

    /// <summary>
    /// Expression pedal mapping offsets (bytes 54-69 in System Dump).
    /// </summary>
    private const int PEDAL_PARAMETER_OFFSET = 54;  // Map Parameter (4 bytes)
    private const int PEDAL_MIN_OFFSET = 58;        // Map Min (4 bytes, 0-100%)
    private const int PEDAL_MID_OFFSET = 62;        // Map Mid (4 bytes, 0-100%)
    private const int PEDAL_MAX_OFFSET = 66;        // Map Max (4 bytes, 0-100%)

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

    /// <summary>
    /// Gets the CC mapping at the specified index (0-63).
    /// </summary>
    /// <param name="index">CC mapping slot index (0-63)</param>
    /// <returns>Result with CCMapping or error</returns>
    public Result<CCMapping> GetCCMapping(int index)
    {
        if (index < 0 || index >= CC_MAPPING_COUNT)
            return Result.Fail($"CC mapping index out of range: {index} (valid range: 0-{CC_MAPPING_COUNT - 1})");

        int offset = CC_MAPPING_START_OFFSET + (index * BYTES_PER_CC_MAPPING);
        byte ccNumber = RawSysEx[offset];
        byte parameterId = RawSysEx[offset + 1];

        return Result.Ok(new CCMapping(ccNumber, parameterId));
    }

    /// <summary>
    /// Gets all 64 CC mappings from the System Dump.
    /// </summary>
    /// <returns>Result with list of all CC mappings or error</returns>
    public Result<List<CCMapping>> GetAllCCMappings()
    {
        var mappings = new List<CCMapping>(CC_MAPPING_COUNT);

        for (int i = 0; i < CC_MAPPING_COUNT; i++)
        {
            var result = GetCCMapping(i);
            if (result.IsFailed)
                return Result.Fail($"Failed to read CC mapping at index {i}");

            mappings.Add(result.Value);
        }

        return Result.Ok(mappings);
    }

    /// <summary>
    /// Updates a CC mapping at the specified index.
    /// </summary>
    /// <param name="index">CC mapping slot index (0-63)</param>
    /// <param name="ccNumber">New CC number (0-127 or 0xFF for unassigned)</param>
    /// <param name="parameterId">New parameter ID (0xFF for unassigned)</param>
    /// <returns>Result indicating success or failure</returns>
    public Result UpdateCCMapping(int index, byte ccNumber, byte parameterId)
    {
        if (index < 0 || index >= CC_MAPPING_COUNT)
            return Result.Fail($"CC mapping index out of range: {index} (valid range: 0-{CC_MAPPING_COUNT - 1})");

        int offset = CC_MAPPING_START_OFFSET + (index * BYTES_PER_CC_MAPPING);
        RawSysEx[offset] = ccNumber;
        RawSysEx[offset + 1] = parameterId;

        return Result.Ok();
    }

    /// <summary>
    /// Gets the expression pedal parameter assignment (which parameter the pedal controls).
    /// </summary>
    /// <returns>Parameter ID (4 bytes little-endian)</returns>
    public int GetPedalParameter()
    {
        return BitConverter.ToInt32(RawSysEx, PEDAL_PARAMETER_OFFSET);
    }

    /// <summary>
    /// Gets the expression pedal minimum position value (0-100%).
    /// </summary>
    /// <returns>Minimum value as percentage (0-100)</returns>
    public int GetPedalMin()
    {
        return BitConverter.ToInt32(RawSysEx, PEDAL_MIN_OFFSET);
    }

    /// <summary>
    /// Gets the expression pedal midpoint position value (0-100%).
    /// </summary>
    /// <returns>Midpoint value as percentage (0-100)</returns>
    public int GetPedalMid()
    {
        return BitConverter.ToInt32(RawSysEx, PEDAL_MID_OFFSET);
    }

    /// <summary>
    /// Gets the expression pedal maximum position value (0-100%).
    /// </summary>
    /// <returns>Maximum value as percentage (0-100)</returns>
    public int GetPedalMax()
    {
        return BitConverter.ToInt32(RawSysEx, PEDAL_MAX_OFFSET);
    }
}
