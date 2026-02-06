using FluentResults;

namespace Nova.Midi;

/// <summary>
/// Abstraction for MIDI port I/O operations.
/// Implemented by both real MIDI and mock implementations.
/// </summary>
public interface IMidiPort
{
    /// <summary>
    /// Gets the name of this MIDI port.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the connected MIDI input port name (from device OUT to app IN).
    /// </summary>
    string? InputPortName { get; }

    /// <summary>
    /// Gets the connected MIDI output port name (from app OUT to device IN).
    /// </summary>
    string? OutputPortName { get; }

    /// <summary>
    /// Gets whether the port is currently connected.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Connects to the MIDI input/output ports.
    /// </summary>
    /// <param name="selection">Input/Output port selection</param>
    /// <returns>Success or failure reason</returns>
    Task<Result> ConnectAsync(MidiPortSelection selection);

    /// <summary>
    /// Lists available MIDI input ports.
    /// </summary>
    IReadOnlyList<string> GetAvailableInputPorts();

    /// <summary>
    /// Lists available MIDI output ports.
    /// </summary>
    IReadOnlyList<string> GetAvailableOutputPorts();

    /// <summary>
    /// Disconnects from the MIDI port.
    /// </summary>
    Task<Result> DisconnectAsync();

    /// <summary>
    /// Sends a SysEx message to the MIDI port.
    /// </summary>
    /// <param name="sysex">Complete SysEx data (F0...F7)</param>
    /// <returns>Success or failure reason</returns>
    Task<Result> SendSysExAsync(byte[] sysex);

    /// <summary>
    /// Receives SysEx messages from the MIDI port.
    /// Yields complete SysEx messages (F0...F7).
    /// 
    /// Use with foreach or async enumeration:
    ///   await foreach (var sysex in port.ReceiveSysExAsync())
    ///   {
    ///       ProcessSysEx(sysex);
    ///   }
    /// </summary>
    /// <param name="cancellationToken">Token to cancel reception</param>
    /// <returns>Async enumerable of SysEx messages</returns>
    IAsyncEnumerable<byte[]> ReceiveSysExAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Receives MIDI Control Change messages from the MIDI port.
    /// Yields complete CC messages (3 bytes: status, CC number, value).
    /// 
    /// Use with foreach or async enumeration:
    ///   await foreach (var ccMessage in port.ReceiveCCAsync())
    ///   {
    ///       byte ccNumber = ccMessage[1];
    ///       byte value = ccMessage[2];
    ///   }
    /// </summary>
    /// <param name="cancellationToken">Token to cancel reception</param>
    /// <returns>Async enumerable of CC messages</returns>
    IAsyncEnumerable<byte[]> ReceiveCCAsync(CancellationToken cancellationToken = default);
}
