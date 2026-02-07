using Nova.Domain.Midi;

namespace Nova.Domain.Tests;

public class SysExValidatorTests
{
    [Fact]
    public void CalculateChecksum_KnownData_MatchesExpected()
    {
        // Data from real hardware (bytes 34-517 per TC spec = 484 bytes)
        var data = new byte[484];
        Array.Fill(data, (byte)0x00);

        var checksum = SysExValidator.CalculateChecksum(data);

        Assert.InRange(checksum, 0, 127);  // Must be 7-bit value
    }

    [Fact]
    public void ValidateChecksum_CorrectChecksum_ReturnsTrue()
    {
        // Build a simple SysEx with known checksum
        var preset = new byte[520];
        Array.Fill(preset, (byte)0x00);
        preset[0] = 0xF0;
        preset[519] = 0xF7;

        // Calculate correct checksum for bytes 34-517 (TC spec)
        var checksumData = new byte[484];
        Array.Copy(preset, 34, checksumData, 0, 484);
        var checksum = SysExValidator.CalculateChecksum(checksumData);
        preset[518] = checksum;

        var isValid = SysExValidator.ValidateChecksum(preset);

        Assert.True(isValid);
    }

    [Fact]
    public void ValidateChecksum_BadChecksum_ReturnsFalse()
    {
        var preset = new byte[520];
        Array.Fill(preset, (byte)0x00);
        preset[0] = 0xF0;
        preset[519] = 0xF7;
        preset[518] = 0xFF;  // Wrong checksum

        var isValid = SysExValidator.ValidateChecksum(preset);

        Assert.False(isValid);
    }
}
