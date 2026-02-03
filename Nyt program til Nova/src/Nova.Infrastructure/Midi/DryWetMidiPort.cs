using System.Runtime.CompilerServices;
using System.Threading.Channels;
using FluentResults;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Nova.Midi;

namespace Nova.Infrastructure.Midi;

public sealed class DryWetMidiPort : IMidiPort, IDisposable
{
    private InputDevice? _inputDevice;
    private OutputDevice? _outputDevice;
    private Channel<byte[]>? _sysExChannel;
    private Channel<byte[]>? _ccChannel;

    public string Name { get; private set; } = string.Empty;
    public bool IsConnected => _inputDevice != null && _outputDevice != null;

    public static List<string> GetAvailablePorts()
    {
        var inputs = InputDevice.GetAll().Select(d => d.Name).ToList();
        var outputs = OutputDevice.GetAll().Select(d => d.Name).ToList();
        
        // Return ports that have both input AND output (bidirectional)
        return inputs.Intersect(outputs).ToList();
    }

    public Task<Result> ConnectAsync(string portName)
    {
        try
        {
            // Dispose existing connections if any
            if (_inputDevice != null || _outputDevice != null)
            {
                DisconnectAsync().GetAwaiter().GetResult();
            }

            var inputDevices = InputDevice.GetAll();
            var outputDevices = OutputDevice.GetAll();

            var input = inputDevices.FirstOrDefault(d => d.Name == portName);
            var output = outputDevices.FirstOrDefault(d => d.Name == portName);

            if (input == null || output == null)
            {
                // Clean up any partial resources
                input?.Dispose();
                output?.Dispose();
                return Task.FromResult(Result.Fail($"MIDI port '{portName}' not found"));
            }

            _inputDevice = input;
            _outputDevice = output;
            
            _inputDevice.StartEventsListening();
            Name = portName;

            return Task.FromResult(Result.Ok());
        }
        catch (Exception ex)
        {
            // Clean up on error
            _inputDevice?.Dispose();
            _outputDevice?.Dispose();
            _inputDevice = null;
            _outputDevice = null;
            Name = string.Empty;
            
            return Task.FromResult(Result.Fail($"Failed to connect: {ex.Message}"));
        }
    }

    public Task<Result> DisconnectAsync()
    {
        try
        {
            _inputDevice?.StopEventsListening();
            _inputDevice?.Dispose();
            _outputDevice?.Dispose();
            
            _inputDevice = null;
            _outputDevice = null;
            Name = string.Empty;

            return Task.FromResult(Result.Ok());
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result.Fail($"Failed to disconnect: {ex.Message}"));
        }
    }

    public Task<Result> SendSysExAsync(byte[] sysex)
    {
        if (_outputDevice == null)
            return Task.FromResult(Result.Fail("Not connected"));

        try
        {
            // DryWetMIDI expects data WITHOUT F0/F7 framing
            var dataWithoutFrame = sysex.Length > 2 && sysex[0] == 0xF0 && sysex[^1] == 0xF7
                ? sysex[1..^1]
                : sysex;

            var sysExEvent = new NormalSysExEvent(dataWithoutFrame);
            _outputDevice.SendEvent(sysExEvent);

            return Task.FromResult(Result.Ok());
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result.Fail($"Failed to send SysEx: {ex.Message}"));
        }
    }

    public IAsyncEnumerable<byte[]> ReceiveSysExAsync(CancellationToken cancellationToken = default)
    {
        if (_inputDevice == null)
            throw new InvalidOperationException("Not connected");

        _sysExChannel = Channel.CreateUnbounded<byte[]>();
        _inputDevice.EventReceived += OnEventReceived;

        return ReadSysExAsync(cancellationToken);
    }

    private void OnEventReceived(object? sender, MidiEventReceivedEventArgs e)
    {
        if (e.Event is NormalSysExEvent sysEx)
        {
            // DryWetMIDI should return data WITHOUT F0/F7, but some devices include trailing F7.
            var payload = sysEx.Data;
            if (payload.Length > 0 && payload[0] == 0xF0)
            {
                payload = payload[1..];
            }

            if (payload.Length > 0 && payload[^1] == 0xF7)
            {
                payload = payload[..^1];
            }

            var data = new byte[payload.Length + 2];
            data[0] = 0xF0;
            Array.Copy(payload, 0, data, 1, payload.Length);
            data[^1] = 0xF7;

            _sysExChannel?.Writer.TryWrite(data);
        }
        else if (e.Event is ControlChangeEvent ccEvent)
        {
            // MIDI CC format: [Status: 0xB0 + channel] [CC#: 0-127] [Value: 0-127]
            var ccMessage = new byte[3];
            ccMessage[0] = (byte)(0xB0 + ccEvent.Channel);  // Status byte with channel
            ccMessage[1] = (byte)ccEvent.ControlNumber;    // CC number
            ccMessage[2] = (byte)ccEvent.ControlValue;     // CC value

            _ccChannel?.Writer.TryWrite(ccMessage);
        }
    }

    private async IAsyncEnumerable<byte[]> ReadSysExAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (_sysExChannel == null) yield break;

        await foreach (var sysex in _sysExChannel.Reader.ReadAllAsync(cancellationToken))
        {
            yield return sysex;
        }
    }

    public IAsyncEnumerable<byte[]> ReceiveCCAsync(CancellationToken cancellationToken = default)
    {
        if (_inputDevice == null)
            throw new InvalidOperationException("Not connected");

        _ccChannel = Channel.CreateUnbounded<byte[]>();
        _inputDevice.EventReceived += OnEventReceived;

        return ReadCCAsync(cancellationToken);
    }

    private async IAsyncEnumerable<byte[]> ReadCCAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (_ccChannel == null) yield break;

        await foreach (var ccMessage in _ccChannel.Reader.ReadAllAsync(cancellationToken))
        {
            yield return ccMessage;
        }
    }

    public void Dispose()
    {
        DisconnectAsync().GetAwaiter().GetResult();
    }
}
