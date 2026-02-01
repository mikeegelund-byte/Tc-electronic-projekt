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
            // DryWetMIDI returns data WITHOUT F0/F7, we add them back
            var data = new byte[sysEx.Data.Length + 2];
            data[0] = 0xF0;
            Array.Copy(sysEx.Data, 0, data, 1, sysEx.Data.Length);
            data[^1] = 0xF7;

            _sysExChannel?.Writer.TryWrite(data);
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

    public void Dispose()
    {
        DisconnectAsync().GetAwaiter().GetResult();
    }
}
