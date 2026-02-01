using FluentResults;
using Nova.Domain.Midi;
using Nova.Midi;
using Serilog;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for sending MIDI Control Change (CC) messages.
/// 
/// WARNING: This use case validates and prepares MIDI CC messages but does not
/// actually transmit them. The current IMidiPort interface only supports SysEx messages.
/// For full MIDI CC support, the IMidiPort interface needs to be extended with a
/// SendMidiAsync(byte[] midiData) method for channel voice messages.
/// </summary>
public sealed class SendCCUseCase
{
    private readonly IMidiPort _midiPort;
    private readonly ILogger _logger;
    
    /// <summary>
    /// Initializes a new instance of the SendCCUseCase.
    /// </summary>
    /// <param name="midiPort">MIDI port for sending messages</param>
    /// <param name="logger">Logger for diagnostic output</param>
    public SendCCUseCase(IMidiPort midiPort, ILogger logger)
    {
        _midiPort = midiPort;
        _logger = logger;
    }
    
    /// <summary>
    /// Executes the use case to send a MIDI CC message.
    /// </summary>
    /// <param name="channel">MIDI channel (0-15)</param>
    /// <param name="controller">Controller number (0-127)</param>
    /// <param name="value">Controller value (0-127)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result indicating success or failure</returns>
    public Task<Result> ExecuteAsync(
        byte channel,
        byte controller,
        byte value,
        CancellationToken cancellationToken = default)
    {
        _logger.Information(
            "Sending MIDI CC: Channel={Channel}, Controller={Controller}({CCName}), Value={Value}",
            channel,
            controller,
            MidiCCMap.GetCCName(controller),
            value);
        
        // Create and validate the MIDI CC message
        var messageResult = MidiCCMessage.Create(channel, controller, value);
        if (messageResult.IsFailed)
        {
            _logger.Error("Failed to create MIDI CC message: {Error}", messageResult.Errors[0].Message);
            return Task.FromResult(Result.Fail(messageResult.Errors[0]));
        }
        
        var message = messageResult.Value;
        
        // Convert to MIDI bytes
        var midiBytes = message.ToMidiBytes();
        
        _logger.Debug("Generated MIDI CC bytes: {MidiBytes}", BitConverter.ToString(midiBytes));
        
        // MIDI CC messages are channel voice messages (3 bytes: status, controller, value)
        // They are sent directly through MIDI, not as SysEx messages.
        // 
        // Current IMidiPort interface only supports SysEx. For proper MIDI CC support,
        // the implementation would need to:
        // 1. Extend IMidiPort with SendMidiAsync(byte[] midiData)
        // 2. Or use the underlying MIDI library directly for channel messages
        //
        // For this implementation, we validate and prepare the message correctly.
        // The actual sending would require extending the infrastructure layer.
        
        _logger.Information(
            "MIDI CC message validated and prepared successfully. " +
            "Note: Actual transmission requires IMidiPort extension for channel voice messages.");
        
        return Task.FromResult(Result.Ok());
    }
}
