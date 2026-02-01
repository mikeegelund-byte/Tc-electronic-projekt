namespace Nova.Domain.Midi;

/// <summary>
/// Builds SysEx messages for Nova System communication.
/// </summary>
public static class SysExBuilder
{
    // MIDI SysEx identifiers
    private const byte SYSEX_START = 0xF0;
    private const byte SYSEX_END = 0xF7;

    // TC Electronic IDs
    private const byte TC_ID_1 = 0x00;
    private const byte TC_ID_2 = 0x20;
    private const byte TC_ID_3 = 0x1F;

    // Nova System
    private const byte DEVICE_ID = 0x00;  // 0 = any device
    private const byte MODEL_ID = 0x63;   // Nova System

    // Message IDs
    private const byte REQUEST_MESSAGE_ID = 0x45;
    private const byte RESPONSE_MESSAGE_ID = 0x20;
    private const byte USER_BANK_DUMP = 0x03;

    /// <summary>
    /// Builds request for User Bank Dump (all 60 presets).
    /// </summary>
    /// <returns>9-byte SysEx request: F0 00 20 1F 00 63 45 03 F7</returns>
    public static byte[] BuildBankDumpRequest()
    {
        return new byte[]
        {
            SYSEX_START,
            TC_ID_1, TC_ID_2, TC_ID_3,
            DEVICE_ID,
            MODEL_ID,
            REQUEST_MESSAGE_ID,
            USER_BANK_DUMP,
            SYSEX_END
        };
    }

    /// <summary>
    /// Builds request for System Dump (global settings, 527 bytes).
    /// </summary>
    /// <param name="deviceId">Device ID (default 0x00 = any device)</param>
    /// <returns>9-byte SysEx request: F0 00 20 1F 00 63 45 02 F7</returns>
    public static byte[] BuildSystemDumpRequest(byte deviceId = 0x00)
    {
        return new byte[]
        {
            SYSEX_START,
            TC_ID_1, TC_ID_2, TC_ID_3,
            deviceId,
            MODEL_ID,
            REQUEST_MESSAGE_ID,
            0x02,  // System dump data type
            SYSEX_END
        };
    }
}
