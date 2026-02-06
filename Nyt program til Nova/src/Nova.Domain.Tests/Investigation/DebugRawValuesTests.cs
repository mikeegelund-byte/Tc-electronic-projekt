using FluentAssertions;
using Nova.Domain.Models;

namespace Nova.Domain.Tests.Investigation;

/// <summary>
/// Debug: Find out actual raw values for failing parameters
/// NOTE: These tests are for investigation only and may fail with validation errors
/// </summary>
[Trait("Category", "Investigation")]
public class DebugRawValuesTests
{
    private readonly byte[] _realPresetBytes;

    public DebugRawValuesTests()
    {
        var fixturePath = Path.Combine(
            "..", "..", "..", "..", "Nova.HardwareTest", "Dumps",
            "nova-dump-20260131-181507-msg001.syx");
        var fullPath = Path.GetFullPath(fixturePath);
        _realPresetBytes = File.ReadAllBytes(fullPath);
    }

    private int DecodeRaw(int offset)
    {
        byte b0 = _realPresetBytes[offset];
        byte b1 = _realPresetBytes[offset + 1];
        byte b2 = _realPresetBytes[offset + 2];
        byte b3 = _realPresetBytes[offset + 3];
        return b0 | (b1 << 7) | (b2 << 14) | (b3 << 21);
    }

    [Fact]
    public void Debug_BoostLevel()
    {
        int raw = DecodeRaw(122);
        // BoostLevel: -12 to +12 dB

        // Test both strategies:
        int withLargeOffset = raw - 16777216;
        int withSimpleOffset = raw + (-12);

        // Output via assertion failure to see values
        raw.Should().Be(999999, $"Raw: {raw}, Large: {withLargeOffset}, Simple: {withSimpleOffset}");
    }

    [Fact]
    public void Debug_CompThreshold()
    {
        int raw = DecodeRaw(74);
        // CompThreshold: -30 to 0 dB

        int withLargeOffset = raw - 16777216;
        int withSimpleOffset = raw + (-30);

        raw.Should().Be(999999, $"Raw: {raw}, Large: {withLargeOffset}, Simple: {withSimpleOffset}");
    }

    [Fact]
    public void Debug_GateThreshold()
    {
        int raw = DecodeRaw(394);
        // GateThreshold: -90 to 0 dB

        int withLargeOffset = raw - 16777216;
        int withSimpleOffset = raw + (-90);

        raw.Should().Be(999999, $"Raw: {raw}, Large: {withLargeOffset}, Simple: {withSimpleOffset}");
    }

    [Fact]
    public void Debug_ReverbLevel()
    {
        int raw = DecodeRaw(366);
        // ReverbLevel: -100 to 0 dB

        int withLargeOffset = raw - 16777216;
        int withSimpleOffset = raw + (-100);

        raw.Should().Be(999999, $"Raw: {raw}, Large: {withLargeOffset}, Simple: {withSimpleOffset}");
    }

    [Fact]
    public void Debug_ModFeedback()
    {
        int raw = DecodeRaw(218);
        // ModFeedback: -100 to +100%

        int withLargeOffset = raw - 16777216;
        int withSimpleOffset = raw + (-100);

        raw.Should().Be(999999, $"Raw: {raw}, Large: {withLargeOffset}, Simple: {withSimpleOffset}");
    }
}
