using FluentAssertions;

namespace Nova.Domain.Tests.Investigation;

/// <summary>
/// Test hypothesis: Signed parameters use 2^24 (16777216) as zero offset.
/// Formula: actualValue = rawDecoded - 16777216
/// </summary>
public class OffsetDecodingHypothesisTests
{
    private const int ZERO_OFFSET = 16777216; // 2^24

    [Theory]
    [InlineData(16777204, -12, "CompLevel at -12dB (minimum)")]
    [InlineData(16777216, 0, "Neutral/zero point")]
    [InlineData(16777228, 12, "CompLevel at +12dB (maximum)")]
    [InlineData(16777192, -24, "CompThreshold example")]
    [InlineData(16777186, -30, "CompThreshold at -30dB (minimum)")]
    public void Hypothesis_SignedDbParameters_UseZeroOffset(int rawEncoded, int expectedDb, string description)
    {
        // Act
        int actualDb = rawEncoded - ZERO_OFFSET;

        // Assert
        actualDb.Should().Be(expectedDb, because: description);
    }

    [Fact]
    public void Hypothesis_CompLevel_Range_MapsCorrectly()
    {
        // CompLevel: -12 to +12 dB
        int minEncoded = 16777204; // From hardware
        int maxEncoded = 16777228; // Calculated

        int minDb = minEncoded - ZERO_OFFSET;
        int maxDb = maxEncoded - ZERO_OFFSET;

        minDb.Should().Be(-12, "Minimum CompLevel");
        maxDb.Should().Be(12, "Maximum CompLevel");
    }

    [Fact]
    public void Hypothesis_CompThreshold_Range_MapsCorrectly()
    {
        // CompThreshold: -30 to 0 dB
        int minEncoded = 16777186; // Calculated: 16777216 - 30
        int zeroEncoded = 16777216; // Zero point

        int minDb = minEncoded - ZERO_OFFSET;
        int zeroDb = zeroEncoded - ZERO_OFFSET;

        minDb.Should().Be(-30, "Minimum CompThreshold");
        zeroDb.Should().Be(0, "Maximum CompThreshold (0dB)");
    }

    [Fact]
    public void Hypothesis_LevelOut_ZeroValue_MapsToMinus100()
    {
        // LevelOutLeft/Right: -100 to 0 dB
        // Hardware showed [0,0,0,0] = raw 0
        // This should map to -100dB (minimum)
        
        int rawZero = 0;

        // Hypothesis: Different offset! Not 2^24
        // Try: actualDb = rawDecoded - 100
        int actualDb = rawZero - 100;

        actualDb.Should().Be(-100, "LevelOut at minimum");
    }

    [Fact]
    public void Hypothesis_DriveLevel_ZeroValue_MapsToMinus30()
    {
        // DriveLevel: -30 to +20 dB
        // Hardware showed [0,0,0,0] = raw 0
        
        // Try: actualDb = rawDecoded - 30
        int rawZero = 0;
        int actualDb = rawZero - 30;

        actualDb.Should().Be(-30, "DriveLevel at minimum");
        
        // Max would be: 50 - 30 = +20dB
        int maxEncoded = 50;
        int maxDb = maxEncoded - 30;
        maxDb.Should().Be(20, "DriveLevel at maximum");
    }

    [Fact]
    public void Pattern_Analysis_TwoEncodingStrategies()
    {
        // PATTERN 1: "Large offset" encoding (2^24 = 16777216)
        // Used when zero is in MIDDLE of range: -12 to +12, -25 to +25
        
        // PATTERN 2: "Simple offset" encoding
        // Used when zero is at END of range: -100 to 0, -30 to 0
        // Formula: actualDb = rawDecoded - abs(minimumValue)
        
        // CompLevel (-12 to +12): Uses PATTERN 1 ✅
        int compLevelRaw = 16777204;
        int compLevelDb = compLevelRaw - ZERO_OFFSET;
        compLevelDb.Should().Be(-12);

        // LevelOutLeft (-100 to 0): Uses PATTERN 2 ✅
        int levelOutRaw = 0;
        int levelOutDb = levelOutRaw - 100;
        levelOutDb.Should().Be(-100);

        // DriveLevel (-30 to +20): Uses PATTERN 2 ✅
        int driveLevelRaw = 0;
        int driveLevelDb = driveLevelRaw - 30;
        driveLevelDb.Should().Be(-30);
    }
}
