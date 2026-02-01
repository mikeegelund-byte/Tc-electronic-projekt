using FluentResults;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Nova.Midi;

namespace Nova.Infrastructure.Midi;

public sealed class DryWetMidiPort : IMidiPort, IDisposable
{
    private InputDevice? _inputDevice;
    private OutputDevice? _outputDevice;

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
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        DisconnectAsync().GetAwaiter().GetResult();
    }
}
