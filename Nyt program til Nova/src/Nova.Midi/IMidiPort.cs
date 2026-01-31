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
    /// Enumerates available MIDI output ports on the system.
    /// </summary>
    /// <returns>Port names (e.g., "Nova System", "In From MIDI Port 1")</returns>
    static abstract IEnumerable<string> GetAvailableOutputPorts();

    /// <summary>
    /// Enumerates available MIDI input ports on the system.
    /// </summary>
    /// <returns>Port names</returns>
    static abstract IEnumerable<string> GetAvailableInputPorts();
}
