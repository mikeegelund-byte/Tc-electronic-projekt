using FluentResults;
using Nova.Midi;

namespace Nova.Application.UseCases;

public sealed class ConnectUseCase : IConnectUseCase
{
    private readonly IMidiPort _midiPort;

    public ConnectUseCase(IMidiPort midiPort)
    {
        _midiPort = midiPort;
    }

    public Task<Result> ExecuteAsync(MidiPortSelection selection)
    {
        return _midiPort.ConnectAsync(selection);
    }
}
