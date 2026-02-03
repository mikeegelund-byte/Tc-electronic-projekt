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
    /// Gets whether the port is currently connected.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Connects to the MIDI port.
    /// </summary>
    /// <param name="portName">Name of the port to connect to (e.g., "Nova System")</param>
    /// <returns>Success or failure reason</returns>
    Task<Result> ConnectAsync(string portName);

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
