using FluentResults;
using System.Threading.Channels;

namespace Nova.Midi.Tests;

/// <summary>
/// Mock MIDI port for unit testing. No real hardware required.
/// </summary>
public class MockMidiPort : IMidiPort
{
    private bool _isConnected;
    private readonly Queue<byte[]> _responseQueue = new();
    private readonly Channel<byte[]> _receiveChannel = Channel.CreateUnbounded<byte[]>();

    public string Name { get; set; } = "Mock Port";
    public bool IsConnected => _isConnected;

    public Task<Result> ConnectAsync(string portName)
    {
        Name = portName;
        _isConnected = true;
        return Task.FromResult(Result.Ok());
    }

    public Task<Result> DisconnectAsync()
    {
        _isConnected = false;
        _receiveChannel.Writer.TryComplete();
        return Task.FromResult(Result.Ok());
    }

    public Task<Result> SendSysExAsync(byte[] sysex)
    {
        if (!IsConnected)
            return Task.FromResult(Result.Fail("Not connected"));

        if (sysex == null || sysex.Length == 0)
            return Task.FromResult(Result.Fail("Empty SysEx"));

        return Task.FromResult(Result.Ok());
    }

    public async IAsyncEnumerable<byte[]> ReceiveSysExAsync(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!IsConnected)
            yield break;

        // First yield any enqueued responses
        while (_responseQueue.TryDequeue(out var response))
        {
            yield return response;
        }

        // Then wait for new data on channel
        await foreach (var data in _receiveChannel.Reader.ReadAllAsync(cancellationToken))
        {
            yield return data;
        }
    }

    /// <summary>
    /// For testing: enqueue response data to be received.
    /// </summary>
    public void EnqueueResponse(byte[] sysex)
    {
        _responseQueue.Enqueue(sysex);
    }

    /// <summary>
    /// For testing: send data as if received from hardware.
    /// </summary>
    public async Task SendResponseAsync(byte[] sysex)
    {
        await _receiveChannel.Writer.WriteAsync(sysex);
    }

    public static IEnumerable<string> GetAvailableOutputPorts()
        => new[] { "Mock Output 1", "Mock Output 2" };

    public static IEnumerable<string> GetAvailableInputPorts()
        => new[] { "Mock Input 1", "Mock Input 2" };
}
