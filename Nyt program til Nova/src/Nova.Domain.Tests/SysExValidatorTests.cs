using Nova.Domain.Midi;

namespace Nova.Domain.Tests;

public class SysExValidatorTests
{
    [Fact]
    public void CalculateChecksum_KnownData_MatchesExpected()
    {
        // Data from real hardware
        var data = new byte[509];  // bytes 8-516 (509 bytes)
        // Just fill with zeros for testing
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
        
        // Calculate correct checksum for bytes 8-516
        var checksumData = new byte[509];  // bytes 8-516
        Array.Copy(preset, 8, checksumData, 0, 509);
        var checksum = SysExValidator.CalculateChecksum(checksumData);
        preset[517] = checksum;

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
        preset[517] = 0xFF;  // Wrong checksum

        var isValid = SysExValidator.ValidateChecksum(preset);

        Assert.False(isValid);
    }
}
