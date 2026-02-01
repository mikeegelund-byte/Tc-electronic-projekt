using FluentResults;

namespace Nova.Domain.Midi;

/// <summary>
/// Represents a MIDI Control Change (CC) message.
/// Format: [Status Byte: 0xB0-0xBF] [Controller: 0-127] [Value: 0-127]
/// </summary>
public sealed class MidiCCMessage
{
    /// <summary>
    /// MIDI channel (0-15)
    /// </summary>
    public byte Channel { get; private init; }
    
    /// <summary>
    /// Controller number (0-127)
    /// </summary>
    public byte Controller { get; private init; }
    
    /// <summary>
    /// Controller value (0-127)
    /// </summary>
    public byte Value { get; private init; }
    
    private MidiCCMessage() { }
    
    /// <summary>
    /// Creates a new MIDI CC message with validation.
    /// </summary>
    /// <param name="channel">MIDI channel (0-15)</param>
    /// <param name="controller">Controller number (0-127)</param>
    /// <param name="value">Controller value (0-127)</param>
    /// <returns>Result containing the message or validation errors</returns>
    public static Result<MidiCCMessage> Create(byte channel, byte controller, byte value)
    {
        // Validate channel (0-15)
        if (channel > 15)
        {
            return Result.Fail<MidiCCMessage>($"Invalid MIDI channel: {channel}. Must be 0-15.");
        }
        
        // Validate controller (0-127)
        if (controller > 127)
        {
            return Result.Fail<MidiCCMessage>($"Invalid controller number: {controller}. Must be 0-127.");
        }
        
        // Validate value (0-127)
        if (value > 127)
        {
            return Result.Fail<MidiCCMessage>($"Invalid controller value: {value}. Must be 0-127.");
        }
        
        return Result.Ok(new MidiCCMessage
        {
            Channel = channel,
            Controller = controller,
            Value = value
        });
    }
    
    /// <summary>
    /// Converts the CC message to a 3-byte MIDI message array.
    /// Format: [0xB0 + channel, controller, value]
    /// </summary>
    /// <returns>3-byte array containing the MIDI CC message</returns>
    public byte[] ToMidiBytes()
    {
        byte statusByte = (byte)(0xB0 | Channel);
        return new[] { statusByte, Controller, Value };
    }
    
    /// <summary>
    /// Parses a 3-byte MIDI CC message.
    /// </summary>
    /// <param name="data">3-byte MIDI message array</param>
    /// <returns>Result containing the parsed message or parsing errors</returns>
    public static Result<MidiCCMessage> FromMidiBytes(byte[] data)
    {
        // Validate data length
        if (data == null)
        {
            return Result.Fail<MidiCCMessage>("MIDI data is null.");
        }
        
        if (data.Length != 3)
        {
            return Result.Fail<MidiCCMessage>($"Invalid MIDI CC message length: {data.Length}. Expected 3 bytes.");
        }
        
        byte statusByte = data[0];
        byte controller = data[1];
        byte value = data[2];
        
        // Validate status byte (0xB0-0xBF for CC messages)
        if ((statusByte & 0xF0) != 0xB0)
        {
            return Result.Fail<MidiCCMessage>($"Invalid status byte: 0x{statusByte:X2}. Expected 0xB0-0xBF for CC message.");
        }
        
        // Extract channel from status byte
        byte channel = (byte)(statusByte & 0x0F);
        
        // Validate controller and value (should be 0-127, meaning bit 7 should be 0)
        if ((controller & 0x80) != 0)
        {
            return Result.Fail<MidiCCMessage>($"Invalid controller number: {controller}. Must be 0-127.");
        }
        
        if ((value & 0x80) != 0)
        {
            return Result.Fail<MidiCCMessage>($"Invalid controller value: {value}. Must be 0-127.");
        }
        
        return Create(channel, controller, value);
    }
}
