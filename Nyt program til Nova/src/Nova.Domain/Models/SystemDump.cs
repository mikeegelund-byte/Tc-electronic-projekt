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

    private const int DATA_START_OFFSET = 8;
    private const int NIBBLE_COUNT = 129;
    private const int PROGRAM_MAP_IN_START = 64;
    private const int PROGRAM_MAP_IN_COUNT = 43;
    private const int PROGRAM_MAP_OUT_START = 107;
    private const int PROGRAM_MAP_OUT_COUNT = 20;

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
    /// Gets the incoming Program Change mapping table (PC 1-127).
    /// </summary>
    public List<ProgramMapInEntry> GetProgramMapIn()
    {
        var entries = new List<ProgramMapInEntry>(127);

        for (int i = 0; i < PROGRAM_MAP_IN_COUNT; i++)
        {
            var nibbleIndex = PROGRAM_MAP_IN_START + i;
            var (v1, v2, v3) = ReadMidiMapValues(nibbleIndex);

            var incoming1 = (i * 3) + 1;
            if (incoming1 <= 127)
                entries.Add(new ProgramMapInEntry(incoming1, DecodeProgramMapPreset(v1)));

            var incoming2 = (i * 3) + 2;
            if (incoming2 <= 127)
                entries.Add(new ProgramMapInEntry(incoming2, DecodeProgramMapPreset(v2)));

            var incoming3 = (i * 3) + 3;
            if (incoming3 <= 127)
                entries.Add(new ProgramMapInEntry(incoming3, DecodeProgramMapPreset(v3)));
        }

        return entries;
    }

    public Result UpdateProgramMapIn(int incomingProgram, int? presetNumber)
    {
        if (incomingProgram < 1 || incomingProgram > 127)
            return Result.Fail($"Incoming program out of range: {incomingProgram} (valid range: 1-127)");

        if (presetNumber.HasValue && (presetNumber.Value < 1 || presetNumber.Value > 90))
            return Result.Fail($"Preset number out of range: {presetNumber} (valid range: 1-90 or Off)");

        var index = (incomingProgram - 1) / 3;
        var slot = (incomingProgram - 1) % 3;
        var nibbleIndex = PROGRAM_MAP_IN_START + index;

        var (v1, v2, v3) = ReadMidiMapValues(nibbleIndex);
        var encoded = presetNumber.HasValue ? presetNumber.Value : 0;

        switch (slot)
        {
            case 0: v1 = encoded; break;
            case 1: v2 = encoded; break;
            case 2: v3 = encoded; break;
        }

        return WriteMidiMapValues(nibbleIndex, v1, v2, v3);
    }

    /// <summary>
    /// Gets the outgoing Program Change mapping table for user presets (31-90).
    /// </summary>
    public List<ProgramMapOutEntry> GetProgramMapOut()
    {
        var entries = new List<ProgramMapOutEntry>(60);

        for (int i = 0; i < PROGRAM_MAP_OUT_COUNT; i++)
        {
            var nibbleIndex = PROGRAM_MAP_OUT_START + i;
            var (v1, v2, v3) = ReadMidiMapValues(nibbleIndex);

            var preset1 = 31 + (i * 3);
            entries.Add(new ProgramMapOutEntry(preset1, v1));

            var preset2 = 32 + (i * 3);
            entries.Add(new ProgramMapOutEntry(preset2, v2));

            var preset3 = 33 + (i * 3);
            entries.Add(new ProgramMapOutEntry(preset3, v3));
        }

        return entries;
    }

    public Result UpdateProgramMapOut(int presetNumber, int outgoingProgram)
    {
        if (presetNumber < 31 || presetNumber > 90)
            return Result.Fail($"Preset number out of range: {presetNumber} (valid range: 31-90)");

        if (outgoingProgram < 0 || outgoingProgram > 127)
            return Result.Fail($"Outgoing program out of range: {outgoingProgram} (valid range: 0-127)");

        var index = (presetNumber - 31) / 3;
        var slot = (presetNumber - 31) % 3;
        var nibbleIndex = PROGRAM_MAP_OUT_START + index;

        var (v1, v2, v3) = ReadMidiMapValues(nibbleIndex);

        switch (slot)
        {
            case 0: v1 = outgoingProgram; break;
            case 1: v2 = outgoingProgram; break;
            case 2: v3 = outgoingProgram; break;
        }

        return WriteMidiMapValues(nibbleIndex, v1, v2, v3);
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

    /// <summary>
    /// Updates the expression pedal parameter assignment.
    /// </summary>
    /// <param name="parameterId">Parameter ID to assign (0-127)</param>
    /// <returns>Result indicating success or failure</returns>
    public Result UpdatePedalParameter(int parameterId)
    {
        if (parameterId < 0 || parameterId > 127)
            return Result.Fail($"Parameter ID out of range: {parameterId} (valid range: 0-127)");

        var bytes = BitConverter.GetBytes(parameterId);
        Array.Copy(bytes, 0, RawSysEx, PEDAL_PARAMETER_OFFSET, 4);

        return Result.Ok();
    }

    /// <summary>
    /// Updates the expression pedal minimum position value.
    /// </summary>
    /// <param name="min">Minimum value as percentage (0-100)</param>
    /// <returns>Result indicating success or failure</returns>
    public Result UpdatePedalMin(int min)
    {
        if (min < 0 || min > 100)
            return Result.Fail($"Min value out of range: {min} (valid range: 0-100)");

        var bytes = BitConverter.GetBytes(min);
        Array.Copy(bytes, 0, RawSysEx, PEDAL_MIN_OFFSET, 4);

        return Result.Ok();
    }

    /// <summary>
    /// Updates the expression pedal midpoint position value.
    /// </summary>
    /// <param name="mid">Midpoint value as percentage (0-100)</param>
    /// <returns>Result indicating success or failure</returns>
    public Result UpdatePedalMid(int mid)
    {
        if (mid < 0 || mid > 100)
            return Result.Fail($"Mid value out of range: {mid} (valid range: 0-100)");

        var bytes = BitConverter.GetBytes(mid);
        Array.Copy(bytes, 0, RawSysEx, PEDAL_MID_OFFSET, 4);

        return Result.Ok();
    }

    /// <summary>
    /// Updates the expression pedal maximum position value.
    /// </summary>
    /// <param name="max">Maximum value as percentage (0-100)</param>
    /// <returns>Result indicating success or failure</returns>
    public Result UpdatePedalMax(int max)
    {
        if (max < 0 || max > 100)
            return Result.Fail($"Max value out of range: {max} (valid range: 0-100)");

        var bytes = BitConverter.GetBytes(max);
        Array.Copy(bytes, 0, RawSysEx, PEDAL_MAX_OFFSET, 4);

        return Result.Ok();
    }

    private static int? DecodeProgramMapPreset(int value)
    {
        if (value <= 0 || value > 90)
            return null;

        return value;
    }

    private (int V1, int V2, int V3) ReadMidiMapValues(int nibbleIndex)
    {
        if (RawSysEx == null || RawSysEx.Length != 527)
            return (0, 0, 0);

        if (nibbleIndex < 0 || nibbleIndex >= NIBBLE_COUNT)
            return (0, 0, 0);

        var offset = DATA_START_OFFSET + (nibbleIndex * 4);
        if (offset + 3 >= RawSysEx.Length)
            return (0, 0, 0);

        var b0 = RawSysEx[offset] & 0x7F;
        var b1 = RawSysEx[offset + 1] & 0x7F;
        var b2 = RawSysEx[offset + 2] & 0x7F;
        var b3 = RawSysEx[offset + 3] & 0x7F;

        var v1 = (b2 / 4) + (32 * (b3 % 8));
        var v2 = (b1 / 2) + (64 * (b2 % 4));
        var v3 = b0 + (128 * (b1 % 2));

        return (v1, v2, v3);
    }

    private Result WriteMidiMapValues(int nibbleIndex, int v1, int v2, int v3)
    {
        if (RawSysEx == null || RawSysEx.Length != 527)
            return Result.Fail("SystemDump has no valid RawSysEx data");

        if (nibbleIndex < 0 || nibbleIndex >= NIBBLE_COUNT)
            return Result.Fail($"Nibble index out of range: {nibbleIndex} (valid range: 0-{NIBBLE_COUNT - 1})");

        if (v1 < 0 || v1 > 127 || v2 < 0 || v2 > 127 || v3 < 0 || v3 > 127)
            return Result.Fail("MIDI map values must be between 0 and 127");

        var offset = DATA_START_OFFSET + (nibbleIndex * 4);
        if (offset + 3 >= RawSysEx.Length)
            return Result.Fail("MIDI map values exceed SystemDump buffer length");

        RawSysEx[offset] = (byte)v3;
        RawSysEx[offset + 1] = (byte)((v2 % 64) * 2);
        RawSysEx[offset + 2] = (byte)((v1 % 32) * 4 + (v2 / 64));
        RawSysEx[offset + 3] = (byte)(v1 / 32);

        return Result.Ok();
    }
}
