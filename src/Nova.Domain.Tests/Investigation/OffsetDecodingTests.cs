using FluentAssertions;
using Nova.Domain.Models;

namespace Nova.Domain.Tests.Investigation;

/// <summary>
/// Investigation: Understand offset encoding for signed dB parameters.
/// </summary>
public class OffsetDecodingTests
{
    private readonly byte[] _realPresetBytes;

    public OffsetDecodingTests()
    {
        // Load real hardware fixture
        var fixturePath = Path.Combine(
            "..", "..", "..", "..", "Nova.HardwareTest", "Dumps",
            "nova-dump-20260131-181507-msg001.syx");
        var fullPath = Path.GetFullPath(fixturePath);
        _realPresetBytes = File.ReadAllBytes(fullPath);
    }

    [Fact]
    public void Investigate_CompLevel_Encoding()
    {
        // CompLevel: -12 to +12 dB (bytes 98-101)
        // Expected: Some value in that range
        byte b0 = _realPresetBytes[98];
        byte b1 = _realPresetBytes[99];
        byte b2 = _realPresetBytes[100];
        byte b3 = _realPresetBytes[101];

        int raw = b0 | (b1 << 7) | (b2 << 14) | (b3 << 21);

        // Output for investigation
        Console.WriteLine($"CompLevel bytes: [{b0}, {b1}, {b2}, {b3}]");
        Console.WriteLine($"Raw decoded: {raw}");
        Console.WriteLine($"Expected range: -12 to +12 dB");
        Console.WriteLine($"Hypothesis: raw - offset = actual");
        Console.WriteLine($"If offset = raw - 0 (assuming neutral), then actual = {raw - raw}");
    }

    [Fact]
    public void Investigate_CompThreshold_Encoding()
    {
        // CompThreshold: -30 to 0 dB (bytes 74-77)
        byte b0 = _realPresetBytes[74];
        byte b1 = _realPresetBytes[75];
        byte b2 = _realPresetBytes[76];
        byte b3 = _realPresetBytes[77];

        int raw = b0 | (b1 << 7) | (b2 << 14) | (b3 << 21);

        Console.WriteLine($"CompThreshold bytes: [{b0}, {b1}, {b2}, {b3}]");
        Console.WriteLine($"Raw decoded: {raw}");
        Console.WriteLine($"Expected range: -30 to 0 dB");
    }

    [Fact]
    public void Investigate_LevelOutLeft_Encoding()
    {
        // LevelOutLeft: -100 to 0 dB (bytes 46-49)
        byte b0 = _realPresetBytes[46];
        byte b1 = _realPresetBytes[47];
        byte b2 = _realPresetBytes[48];
        byte b3 = _realPresetBytes[49];

        int raw = b0 | (b1 << 7) | (b2 << 14) | (b3 << 21);

        Console.WriteLine($"LevelOutLeft bytes: [{b0}, {b1}, {b2}, {b3}]");
        Console.WriteLine($"Raw decoded: {raw}");
        Console.WriteLine($"Expected range: -100 to 0 dB");
    }

    [Fact]
    public void Investigate_DriveLevel_Encoding()
    {
        // DriveLevel: -100 to 0 dB (bytes 190-193)
        byte b0 = _realPresetBytes[190];
        byte b1 = _realPresetBytes[191];
        byte b2 = _realPresetBytes[192];
        byte b3 = _realPresetBytes[193];

        int raw = b0 | (b1 << 7) | (b2 << 14) | (b3 << 21);

        Console.WriteLine($"DriveLevel bytes: [{b0}, {b1}, {b2}, {b3}]");
        Console.WriteLine($"Raw decoded: {raw}");
        Console.WriteLine($"Expected range: -100 to 0 dB");
    }

    [Fact]
    public void Investigate_TapTempo_Encoding_AsBaseline()
    {
        // TapTempo: 100-3000 ms (bytes 38-41) - UNSIGNED, should work correctly
        byte b0 = _realPresetBytes[38];
        byte b1 = _realPresetBytes[39];
        byte b2 = _realPresetBytes[40];
        byte b3 = _realPresetBytes[41];

        int raw = b0 | (b1 << 7) | (b2 << 14) | (b3 << 21);

        Console.WriteLine($"TapTempo bytes: [{b0}, {b1}, {b2}, {b3}]");
        Console.WriteLine($"Raw decoded: {raw}");
        Console.WriteLine($"Expected range: 100 to 3000 ms");
        Console.WriteLine($"This should decode correctly as-is (unsigned)");

        raw.Should().BeInRange(100, 3000, "TapTempo baseline - unsigned value");
    }
}
