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
    private bool _handlersSubscribed = false;

    public string Name { get; private set; } = string.Empty;
    public string? InputPortName { get; private set; }
    public string? OutputPortName { get; private set; }
    public bool IsConnected => _inputDevice != null && _outputDevice != null;

    public IReadOnlyList<string> GetAvailableInputPorts()
        => InputDevice.GetAll()
            .Select(d => d.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

    public IReadOnlyList<string> GetAvailableOutputPorts()
        => OutputDevice.GetAll()
            .Select(d => d.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

    public Task<Result> ConnectAsync(MidiPortSelection selection)
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

            if (!selection.IsValid(out var selectionError))
            {
                return Task.FromResult(Result.Fail(selectionError));
            }

            var inputName = selection.InputPortName;
            var outputName = selection.OutputPortName;

            var input = FindInputDevice(inputDevices, inputName);
            var output = FindOutputDevice(outputDevices, outputName);

            if (input == null || output == null)
            {
                // Clean up any partial resources
                input?.Dispose();
                output?.Dispose();

                var missing = new List<string>();
                if (input == null) missing.Add($"IN '{inputName}'");
                if (output == null) missing.Add($"OUT '{outputName}'");

                return Task.FromResult(Result.Fail($"MIDI port(s) not found: {string.Join(", ", missing)}"));
            }

            _inputDevice = input;
            _outputDevice = output;
            
            _inputDevice.StartEventsListening();
            InputPortName = inputName;
            OutputPortName = outputName;
            Name = $"IN: {InputPortName} / OUT: {OutputPortName}";

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
            InputPortName = null;
            OutputPortName = null;
            
            return Task.FromResult(Result.Fail($"Failed to connect: {ex.Message}"));
        }
    }

    public Task<Result> DisconnectAsync()
    {
        try
        {
            // Fjern event handler FØR dispose
            if (_inputDevice != null && _handlersSubscribed)
            {
                _inputDevice.EventReceived -= OnEventReceived;
                _handlersSubscribed = false;
            }

            // Luk channels for at afslutte async enumerations
            _sysExChannel?.Writer.Complete();
            _ccChannel?.Writer.Complete();

            _inputDevice?.StopEventsListening();
            _inputDevice?.Dispose();
            _outputDevice?.Dispose();

            _inputDevice = null;
            _outputDevice = null;
            _sysExChannel = null;
            _ccChannel = null;
            Name = string.Empty;
            InputPortName = null;
            OutputPortName = null;

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

        // Initialiser channel én gang
        if (_sysExChannel == null)
        {
            _sysExChannel = Channel.CreateUnbounded<byte[]>();
        }

        // Subscribe event handler én gang
        if (!_handlersSubscribed)
        {
            _inputDevice.EventReceived += OnEventReceived;
            _handlersSubscribed = true;
        }

        return ReadSysExAsync(cancellationToken);
    }

    private void OnEventReceived(object? sender, MidiEventReceivedEventArgs e)
    {
        if (e.Event is NormalSysExEvent sysEx)
        {
            if (_sysExChannel?.Writer != null)
            {
                // DryWetMIDI returns data WITHOUT F0/F7, we add them back
                var data = new byte[sysEx.Data.Length + 2];
                data[0] = 0xF0;
                Array.Copy(sysEx.Data, 0, data, 1, sysEx.Data.Length);
                data[^1] = 0xF7;

                _sysExChannel.Writer.TryWrite(data);
            }
        }
        else if (e.Event is ControlChangeEvent ccEvent)
        {
            if (_ccChannel?.Writer != null)
            {
                // MIDI CC format: [Status: 0xB0 + channel] [CC#: 0-127] [Value: 0-127]
                var ccMessage = new byte[3];
                ccMessage[0] = (byte)(0xB0 + ccEvent.Channel);  // Status byte with channel
                ccMessage[1] = (byte)ccEvent.ControlNumber;    // CC number
                ccMessage[2] = (byte)ccEvent.ControlValue;     // CC value

                _ccChannel.Writer.TryWrite(ccMessage);
            }
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

        // Initialiser channel én gang
        if (_ccChannel == null)
        {
            _ccChannel = Channel.CreateUnbounded<byte[]>();
        }

        // Subscribe event handler én gang
        if (!_handlersSubscribed)
        {
            _inputDevice.EventReceived += OnEventReceived;
            _handlersSubscribed = true;
        }

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

    private static InputDevice? FindInputDevice(IEnumerable<InputDevice> devices, string name)
    {
        return devices.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            ?? devices.FirstOrDefault(d => d.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0);
    }

    private static OutputDevice? FindOutputDevice(IEnumerable<OutputDevice> devices, string name)
    {
        return devices.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            ?? devices.FirstOrDefault(d => d.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0);
    }
}
