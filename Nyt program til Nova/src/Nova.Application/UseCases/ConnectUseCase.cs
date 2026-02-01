using FluentResults;
using Nova.Midi;

namespace Nova.Application.UseCases;

public sealed class ConnectUseCase
{
    private readonly IMidiPort _midiPort;

    public ConnectUseCase(IMidiPort midiPort)
    {
        _midiPort = midiPort;
    }

    public Task<Result> ExecuteAsync(string portName)
    {
        return _midiPort.ConnectAsync(portName);
    }
}
