using FluentAssertions;
using Nova.Domain.Midi;
using Xunit;

namespace Nova.Domain.Tests;

public class MidiCCMapTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(64)]
    [InlineData(127)]
    public void IsValidCC_WithValidCCNumbers_ReturnsTrue(byte ccNumber)
    {
        // Act
        var result = MidiCCMap.IsValidCC(ccNumber);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Theory]
    [InlineData(128)]
    [InlineData(200)]
    [InlineData(255)]
    public void IsValidCC_WithInvalidCCNumbers_ReturnsFalse(byte ccNumber)
    {
        // Act
        var result = MidiCCMap.IsValidCC(ccNumber);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Theory]
    [InlineData(MidiCCMap.ModulationWheel, "Modulation Wheel")]
    [InlineData(MidiCCMap.Volume, "Volume")]
    [InlineData(MidiCCMap.Pan, "Pan")]
    [InlineData(MidiCCMap.Expression, "Expression")]
    [InlineData(MidiCCMap.CompressorThreshold, "Compressor Threshold")]
    [InlineData(MidiCCMap.DriveGain, "Drive Gain")]
    [InlineData(MidiCCMap.DelayTime, "Delay Time")]
    [InlineData(MidiCCMap.ReverbTime, "Reverb Time")]
    [InlineData(MidiCCMap.DriveOnOff, "Drive On/Off")]
    [InlineData(MidiCCMap.ReverbOnOff, "Reverb On/Off")]
    public void GetCCName_WithKnownCCNumbers_ReturnsCorrectName(byte ccNumber, string expectedName)
    {
        // Act
        var name = MidiCCMap.GetCCName(ccNumber);
        
        // Assert
        name.Should().Be(expectedName);
    }
    
    [Fact]
    public void GetCCName_WithUnknownCCNumber_ReturnsUnknownFormat()
    {
        // Arrange
        byte unknownCC = 99;
        
        // Act
        var name = MidiCCMap.GetCCName(unknownCC);
        
        // Assert
        name.Should().Be("Unknown CC 99");
    }
    
    [Fact]
    public void MidiCCMap_AllConstants_AreValidCCNumbers()
    {
        // Arrange & Act
        var constants = new[]
        {
            MidiCCMap.Volume,
            MidiCCMap.Pan,
            MidiCCMap.Expression,
            MidiCCMap.ModulationWheel,
            MidiCCMap.EffectDepth,
            MidiCCMap.ChorusDepth,
            MidiCCMap.ReverbDepth,
            MidiCCMap.CompressorThreshold,
            MidiCCMap.DriveGain,
            MidiCCMap.ModulationRate,
            MidiCCMap.ModulationDepth,
            MidiCCMap.DelayTime,
            MidiCCMap.DelayFeedback,
            MidiCCMap.ReverbTime,
            MidiCCMap.TapTempo,
            MidiCCMap.DriveOnOff,
            MidiCCMap.CompOnOff,
            MidiCCMap.NoiseGateOnOff,
            MidiCCMap.EqOnOff,
            MidiCCMap.BoostOnOff,
            MidiCCMap.ModOnOff,
            MidiCCMap.PitchOnOff,
            MidiCCMap.DelayOnOff,
            MidiCCMap.ReverbOnOff
        };
        
        // Assert
        foreach (var cc in constants)
        {
            MidiCCMap.IsValidCC(cc).Should().BeTrue($"CC constant {cc} should be valid");
        }
    }
}
