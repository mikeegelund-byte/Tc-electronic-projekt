using FluentAssertions;
using Nova.Domain.Midi;
using Xunit;

namespace Nova.Domain.Tests;

public class MidiCCMessageTests
{
    [Theory]
    [InlineData(0, 7, 64)]    // Channel 0, Volume CC, mid value
    [InlineData(15, 1, 127)]  // Channel 15, Mod Wheel, max value
    [InlineData(9, 64, 0)]    // Channel 9, Damper Pedal, min value
    public void Create_WithValidParameters_ReturnsSuccess(byte channel, byte controller, byte value)
    {
        // Act
        var result = MidiCCMessage.Create(channel, controller, value);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Channel.Should().Be(channel);
        result.Value.Controller.Should().Be(controller);
        result.Value.Value.Should().Be(value);
    }
    
    [Fact]
    public void Create_WithInvalidChannel_ReturnsFailure()
    {
        // Act
        var result = MidiCCMessage.Create(16, 7, 64);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Invalid MIDI channel: 16");
    }
    
    [Fact]
    public void Create_WithInvalidController_ReturnsFailure()
    {
        // Act
        var result = MidiCCMessage.Create(0, 128, 64);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Invalid controller number: 128");
    }
    
    [Fact]
    public void Create_WithInvalidValue_ReturnsFailure()
    {
        // Act
        var result = MidiCCMessage.Create(0, 7, 128);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Invalid controller value: 128");
    }
    
    [Theory]
    [InlineData(0, 7, 64, new byte[] { 0xB0, 0x07, 0x40 })]
    [InlineData(15, 1, 127, new byte[] { 0xBF, 0x01, 0x7F })]
    [InlineData(9, 64, 0, new byte[] { 0xB9, 0x40, 0x00 })]
    public void ToMidiBytes_ReturnsCorrectByteArray(byte channel, byte controller, byte value, byte[] expected)
    {
        // Arrange
        var message = MidiCCMessage.Create(channel, controller, value).Value;
        
        // Act
        var bytes = message.ToMidiBytes();
        
        // Assert
        bytes.Should().Equal(expected);
    }
    
    [Theory]
    [InlineData(new byte[] { 0xB0, 0x07, 0x40 }, 0, 7, 64)]
    [InlineData(new byte[] { 0xBF, 0x01, 0x7F }, 15, 1, 127)]
    [InlineData(new byte[] { 0xB9, 0x40, 0x00 }, 9, 64, 0)]
    public void FromMidiBytes_WithValidData_ReturnsSuccess(byte[] data, byte expectedChannel, byte expectedController, byte expectedValue)
    {
        // Act
        var result = MidiCCMessage.FromMidiBytes(data);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Channel.Should().Be(expectedChannel);
        result.Value.Controller.Should().Be(expectedController);
        result.Value.Value.Should().Be(expectedValue);
    }
    
    [Fact]
    public void FromMidiBytes_WithNullData_ReturnsFailure()
    {
        // Act
        var result = MidiCCMessage.FromMidiBytes(null!);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("MIDI data is null");
    }
    
    [Fact]
    public void FromMidiBytes_WithInvalidLength_ReturnsFailure()
    {
        // Arrange
        var data = new byte[] { 0xB0, 0x07 }; // Only 2 bytes
        
        // Act
        var result = MidiCCMessage.FromMidiBytes(data);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Invalid MIDI CC message length: 2");
    }
    
    [Fact]
    public void FromMidiBytes_WithInvalidStatusByte_ReturnsFailure()
    {
        // Arrange
        var data = new byte[] { 0x90, 0x07, 0x40 }; // Note On message, not CC
        
        // Act
        var result = MidiCCMessage.FromMidiBytes(data);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Invalid status byte");
    }
    
    [Fact]
    public void FromMidiBytes_WithInvalidControllerByte_ReturnsFailure()
    {
        // Arrange
        var data = new byte[] { 0xB0, 0x80, 0x40 }; // Controller > 127 (bit 7 set)
        
        // Act
        var result = MidiCCMessage.FromMidiBytes(data);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Invalid controller number");
    }
    
    [Fact]
    public void FromMidiBytes_WithInvalidValueByte_ReturnsFailure()
    {
        // Arrange
        var data = new byte[] { 0xB0, 0x07, 0x80 }; // Value > 127 (bit 7 set)
        
        // Act
        var result = MidiCCMessage.FromMidiBytes(data);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Invalid controller value");
    }
    
    [Fact]
    public void ToMidiBytes_AndFromMidiBytes_RoundTrip_PreservesData()
    {
        // Arrange
        var original = MidiCCMessage.Create(5, 20, 100).Value;
        
        // Act
        var bytes = original.ToMidiBytes();
        var result = MidiCCMessage.FromMidiBytes(bytes);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Channel.Should().Be(original.Channel);
        result.Value.Controller.Should().Be(original.Controller);
        result.Value.Value.Should().Be(original.Value);
    }
}
