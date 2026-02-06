namespace Nova.Domain.Midi;

/// <summary>
/// Validates SysEx messages from Nova System.
/// </summary>
public static class SysExValidator
{
    /// <summary>
    /// Calculates checksum for SysEx preset data.
    /// Checksum = 7 LSBs of sum of parameter bytes (8-516).
    /// </summary>
    /// <param name="parameterData">Bytes 8-516 of preset (509 bytes)</param>
    /// <returns>Checksum value (0-127)</returns>
    public static byte CalculateChecksum(byte[] parameterData)
    {
        if (parameterData == null || parameterData.Length == 0)
            return 0;

        // Sum all parameter bytes
        int sum = 0;
        foreach (var b in parameterData)
        {
            sum += b;
        }

        // Keep only 7 LSBs (0-127)
        return (byte)(sum & 0x7F);
    }

    /// <summary>
    /// Validates SysEx preset message (520 bytes).
    /// </summary>
    /// <param name="sysex">Complete SysEx: F0...F7 (520 bytes)</param>
    /// <returns>True if checksum is valid</returns>
    public static bool ValidateChecksum(byte[] sysex)
    {
        if (sysex == null || sysex.Length != 520)
            return false;

        if (sysex[0] != 0xF0 || sysex[519] != 0xF7)
            return false;

        // Extract parameter bytes (8-516)
        var parameterData = new byte[509];
        Array.Copy(sysex, 8, parameterData, 0, 509);

        var expectedChecksum = CalculateChecksum(parameterData);
        var actualChecksum = sysex[517];

        return expectedChecksum == actualChecksum;
    }
}
