using FluentResults;

namespace Nova.Domain.Models;

/// <summary>
/// Represents the System Dump from the Nova System pedal (global settings, 526 bytes).
/// Contains MIDI configuration, global tempo, routing preferences, CC mappings, and other system-level parameters.
/// </summary>
public class SystemDump
{
    /// <summary>
    /// Raw 526-byte SysEx message including F0 start and F7 end bytes.
    /// </summary>
    public byte[] RawSysEx { get; private init; } = Array.Empty<byte>();

    private const int DATA_START_OFFSET = 8;
    private const int NIBBLE_COUNT = 129;
    private const int CHECKSUM_OFFSET = DATA_START_OFFSET + (NIBBLE_COUNT * 4);
    private const int EXPECTED_LENGTH = CHECKSUM_OFFSET + 2; // checksum + F7

    // Nibble indices (DATA_START_OFFSET + 4 * index)
    private const int NIBBLE_PEDAL_MIN = 3;
    private const int NIBBLE_PEDAL_MID = 4;
    private const int NIBBLE_PEDAL_MAX = 5;
    private const int NIBBLE_PEDAL_TYPE = 6;
    private const int NIBBLE_MIDI_CHANNEL = 19;
    private const int NIBBLE_PROGRAM_CHANGE_IN = 20;
    private const int NIBBLE_PROGRAM_CHANGE_OUT = 21;
    private const int NIBBLE_MIDI_CLOCK = 22;
    private const int NIBBLE_SYSEX_ID = 23;

    private const int MIDI_CC_ASSIGNMENT_COUNT = 11;
    private const int PROGRAM_MAP_IN_START = 64;
    private const int PROGRAM_MAP_IN_COUNT = 43;
    private const int PROGRAM_MAP_OUT_START = 107;
    private const int PROGRAM_MAP_OUT_COUNT = 20;

    private static readonly (string Name, int NibbleIndex)[] MidiCcAssignments =
    {
        ("Tap Tempo", 8),
        ("Drive", 10),
        ("Compressor", 9),
        ("Noise Gate", 14),
        ("EQ", 16),
        ("Boost", 17),
        ("Modulation", 11),
        ("Pitch", 15),
        ("Delay", 12),
        ("Reverb", 13),
        ("Expression", 18)
    };
    private SystemDump() { }

    /// <summary>
    /// Parses a 526-byte System Dump SysEx message (accepts 527 with double F7).
    /// </summary>
    /// <param name="sysex">Full SysEx message including F0/F7</param>
    /// <returns>Result with SystemDump or error</returns>
    public static Result<SystemDump> FromSysEx(byte[] sysex)
    {
        if (!TryNormalizeSysEx(sysex, out var normalized, out var errorMessage))
            return Result.Fail(errorMessage);

        sysex = normalized;

        // Validate F0/F7 framing
        if (sysex[0] != 0xF0)
            return Result.Fail("SysEx must start with F0");
        if (sysex[^1] != 0xF7)
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
    /// Serializes the System Dump back to a 526-byte SysEx message.
    /// Simply returns the stored RawSysEx since we preserve original bytes.
    /// </summary>
    /// <returns>Result with 526-byte SysEx or error</returns>
    public Result<byte[]> ToSysEx()
    {
        if (RawSysEx == null || RawSysEx.Length != EXPECTED_LENGTH)
            return Result.Fail("SystemDump has no valid RawSysEx data");

        UpdateChecksum(RawSysEx);
        return Result.Ok(RawSysEx);
    }

    /// <summary>
    /// Gets the fixed MIDI CC assignment at the specified index (0-10).
    /// </summary>
    /// <param name="index">Assignment slot index (0-10)</param>
    /// <returns>Result with CCMapping or error</returns>
    public Result<CCMapping> GetCCMapping(int index)
    {
        if (index < 0 || index >= MIDI_CC_ASSIGNMENT_COUNT)
            return Result.Fail($"CC mapping index out of range: {index} (valid range: 0-{MIDI_CC_ASSIGNMENT_COUNT - 1})");

        var slot = MidiCcAssignments[index];
        var rawValue = GetNibbleValue(slot.NibbleIndex);
        var ccNumber = DecodeMidiCcValue(rawValue);

        return Result.Ok(new CCMapping(slot.Name, ccNumber));
    }

    /// <summary>
    /// Gets all fixed MIDI CC assignments from the System Dump.
    /// </summary>
    /// <returns>Result with list of all CC assignments or error</returns>
    public Result<List<CCMapping>> GetAllCCMappings()
    {
        var mappings = new List<CCMapping>(MIDI_CC_ASSIGNMENT_COUNT);

        for (int i = 0; i < MIDI_CC_ASSIGNMENT_COUNT; i++)
        {
            var result = GetCCMapping(i);
            if (result.IsFailed)
                return Result.Fail($"Failed to read CC mapping at index {i}");

            mappings.Add(result.Value);
        }

        return Result.Ok(mappings);
    }

    /// <summary>
    /// Updates a fixed MIDI CC assignment at the specified index.
    /// </summary>
    /// <param name="index">Assignment slot index (0-10)</param>
    /// <param name="ccNumber">New CC number (0-127 or 0xFF for Off)</param>
    /// <returns>Result indicating success or failure</returns>
    public Result UpdateCCMapping(int index, int? ccNumber)
    {
        if (index < 0 || index >= MIDI_CC_ASSIGNMENT_COUNT)
            return Result.Fail($"CC mapping index out of range: {index} (valid range: 0-{MIDI_CC_ASSIGNMENT_COUNT - 1})");

        if (ccNumber.HasValue && (ccNumber.Value < 0 || ccNumber.Value > 127))
            return Result.Fail($"CC number out of range: {ccNumber} (valid range: 0-127 or Off)");

        var slot = MidiCcAssignments[index];
        var encoded = EncodeMidiCcValue(ccNumber);
        return SetNibbleValue(slot.NibbleIndex, encoded);
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
        // Note: System dump stores Pedal Type here (not per-preset map parameter).
        return GetNibbleValue(NIBBLE_PEDAL_TYPE);
    }

    /// <summary>
    /// Gets the expression pedal minimum position value (0-100%).
    /// </summary>
    /// <returns>Minimum value as percentage (0-100)</returns>
    public int GetPedalMin()
    {
        return GetNibbleValue(NIBBLE_PEDAL_MIN);
    }

    /// <summary>
    /// Gets the expression pedal midpoint position value (0-100%).
    /// </summary>
    /// <returns>Midpoint value as percentage (0-100)</returns>
    public int GetPedalMid()
    {
        return GetNibbleValue(NIBBLE_PEDAL_MID);
    }

    /// <summary>
    /// Gets the expression pedal maximum position value (0-100%).
    /// </summary>
    /// <returns>Maximum value as percentage (0-100)</returns>
    public int GetPedalMax()
    {
        return GetNibbleValue(NIBBLE_PEDAL_MAX);
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

        return SetNibbleValue(NIBBLE_PEDAL_TYPE, parameterId);
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

        return SetNibbleValue(NIBBLE_PEDAL_MIN, min);
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

        return SetNibbleValue(NIBBLE_PEDAL_MID, mid);
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

        return SetNibbleValue(NIBBLE_PEDAL_MAX, max);
    }

    public int GetMidiChannel()
    {
        return GetNibbleValue(NIBBLE_MIDI_CHANNEL);
    }

    public Result UpdateMidiChannel(int channel)
    {
        if (channel < 0 || channel > 17)
            return Result.Fail($"MIDI channel out of range: {channel} (valid range: 0-17)");

        return SetNibbleValue(NIBBLE_MIDI_CHANNEL, channel);
    }

    public bool GetProgramChangeInEnabled()
    {
        return GetNibbleValue(NIBBLE_PROGRAM_CHANGE_IN) == 1;
    }

    public Result UpdateProgramChangeInEnabled(bool enabled)
    {
        return SetNibbleValue(NIBBLE_PROGRAM_CHANGE_IN, enabled ? 1 : 0);
    }

    public bool GetProgramChangeOutEnabled()
    {
        return GetNibbleValue(NIBBLE_PROGRAM_CHANGE_OUT) == 1;
    }

    public Result UpdateProgramChangeOutEnabled(bool enabled)
    {
        return SetNibbleValue(NIBBLE_PROGRAM_CHANGE_OUT, enabled ? 1 : 0);
    }

    public bool GetMidiClockEnabled()
    {
        return GetNibbleValue(NIBBLE_MIDI_CLOCK) == 1;
    }

    public Result UpdateMidiClockEnabled(bool enabled)
    {
        return SetNibbleValue(NIBBLE_MIDI_CLOCK, enabled ? 1 : 0);
    }

    public int GetSysExId()
    {
        return GetNibbleValue(NIBBLE_SYSEX_ID);
    }

    public Result UpdateSysExId(int sysExId)
    {
        if (sysExId < 0 || sysExId > 127)
            return Result.Fail($"SysEx ID out of range: {sysExId} (valid range: 0-127)");

        return SetNibbleValue(NIBBLE_SYSEX_ID, sysExId);
    }

    private int GetNibbleValue(int nibbleIndex)
    {
        if (RawSysEx == null || RawSysEx.Length != EXPECTED_LENGTH)
            return 0;

        if (nibbleIndex < 0 || nibbleIndex >= NIBBLE_COUNT)
            return 0;

        var offset = DATA_START_OFFSET + (nibbleIndex * 4);
        return DecodeNibbleValue(RawSysEx, offset);
    }

    private Result SetNibbleValue(int nibbleIndex, int value)
    {
        if (RawSysEx == null || RawSysEx.Length != EXPECTED_LENGTH)
            return Result.Fail("SystemDump har ingen gyldig RawSysEx data");

        if (nibbleIndex < 0 || nibbleIndex >= NIBBLE_COUNT)
            return Result.Fail($"Nibble index out of range: {nibbleIndex} (valid range: 0-{NIBBLE_COUNT - 1})");

        if (value < -16384 || value > 16383)
            return Result.Fail($"Nibble value out of range: {value} (valid range: -16384 to 16383)");

        var offset = DATA_START_OFFSET + (nibbleIndex * 4);
        EncodeNibbleValue(RawSysEx, offset, value);
        return Result.Ok();
    }

    private static int? DecodeMidiCcValue(int value)
    {
        if (value <= 0)
            return null;

        if (value > 128)
            return null;

        return value - 1;
    }

    private static int EncodeMidiCcValue(int? ccNumber)
    {
        if (!ccNumber.HasValue)
            return 0;

        return ccNumber.Value + 1;
    }

    private static int? DecodeProgramMapPreset(int value)
    {
        if (value <= 0 || value > 90)
            return null;

        return value;
    }

    private (int V1, int V2, int V3) ReadMidiMapValues(int nibbleIndex)
    {
        var offset = DATA_START_OFFSET + (nibbleIndex * 4);
        if (RawSysEx == null || RawSysEx.Length != EXPECTED_LENGTH)
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
        if (RawSysEx == null || RawSysEx.Length != EXPECTED_LENGTH)
            return Result.Fail("SystemDump har ingen gyldig RawSysEx data");

        if (nibbleIndex < 0 || nibbleIndex >= NIBBLE_COUNT)
            return Result.Fail($"Nibble index out of range: {nibbleIndex} (valid range: 0-{NIBBLE_COUNT - 1})");

        if (v1 < 0 || v1 > 127 || v2 < 0 || v2 > 127 || v3 < 0 || v3 > 127)
            return Result.Fail("MIDI map values must be between 0 and 127");

        var offset = DATA_START_OFFSET + (nibbleIndex * 4);
        RawSysEx[offset] = (byte)v3;
        RawSysEx[offset + 1] = (byte)((v2 % 64) * 2);
        RawSysEx[offset + 2] = (byte)((v1 % 32) * 4 + (v2 / 64));
        RawSysEx[offset + 3] = (byte)(v1 / 32);

        return Result.Ok();
    }

    private static int DecodeNibbleValue(byte[] data, int offset)
    {
        if (offset + 3 >= data.Length)
            return 0;

        int b0 = data[offset];
        int b1 = data[offset + 1];
        int b3 = data[offset + 3];

        if (b3 == 0)
        {
            return b0 + (b1 * 128);
        }

        return (b0 - 128) + ((b1 - 127) * 128);
    }

    private static void EncodeNibbleValue(byte[] data, int offset, int value)
    {
        if (offset + 3 >= data.Length)
            return;

        if (value >= 0)
        {
            data[offset] = (byte)(value % 128);
            data[offset + 1] = (byte)(value / 128);
            data[offset + 2] = 0;
            data[offset + 3] = 0;
        }
        else
        {
            data[offset] = (byte)(128 - ((-value) % 128));
            data[offset + 1] = (byte)((value / 128) + 127);
            data[offset + 2] = 127;
            data[offset + 3] = 7;
        }
    }

    private static void UpdateChecksum(byte[] data)
    {
        if (data == null || data.Length != EXPECTED_LENGTH)
            return;

        int sum = 0;
        for (int i = DATA_START_OFFSET; i < CHECKSUM_OFFSET; i++)
        {
            sum += data[i];
        }

        data[CHECKSUM_OFFSET] = (byte)(sum & 0x7F);
    }

    private static bool TryNormalizeSysEx(byte[] sysex, out byte[] normalized, out string errorMessage)
    {
        normalized = sysex;
        errorMessage = string.Empty;

        if (sysex == null)
        {
            errorMessage = "System Dump is null";
            return false;
        }

        if (sysex.Length == EXPECTED_LENGTH)
            return true;

        if (sysex.Length == EXPECTED_LENGTH + 1 && sysex[^1] == 0xF7 && sysex[^2] == 0xF7)
        {
            normalized = sysex[..^1];
            return true;
        }

        errorMessage = $"Invalid System Dump length: expected {EXPECTED_LENGTH} bytes (or {EXPECTED_LENGTH + 1} with double F7), got {sysex.Length}";
        return false;
    }
}
