using FluentResults;
using Melanchall.DryWetMidi.Multimedia;
using Nova.Midi;

namespace Nova.Infrastructure.Midi;

public sealed class DryWetMidiPort : IMidiPort, IDisposable
{
    // Placeholder implementation - will be extended in subsequent tasks
    
    public string Name { get; private set; } = string.Empty;
    public bool IsConnected => false;

    public static List<string> GetAvailablePorts()
    {
        var inputs = InputDevice.GetAll().Select(d => d.Name).ToList();
        var outputs = OutputDevice.GetAll().Select(d => d.Name).ToList();
        
        // Return ports that have both input AND output (bidirectional)
        return inputs.Intersect(outputs).ToList();
    }

    public Task<Result> ConnectAsync(string portName)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DisconnectAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result> SendSysExAsync(byte[] sysex)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<byte[]> ReceiveSysExAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        // Cleanup will be implemented in subsequent tasks
    }
}
