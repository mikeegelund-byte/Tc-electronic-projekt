using System.Collections.Generic;
using Nova.Application.UseCases;

namespace Nova.Infrastructure.Midi;

public class GetAvailablePortsUseCase : IGetAvailablePortsUseCase
{
    public IReadOnlyList<string> Execute()
    {
        return DryWetMidiPort.GetAvailablePorts();
    }
}
