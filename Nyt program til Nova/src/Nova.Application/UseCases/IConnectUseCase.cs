using FluentResults;
using Nova.Midi;

namespace Nova.Application.UseCases;

public interface IConnectUseCase
{
    Task<Result> ExecuteAsync(MidiPortSelection selection);
}
