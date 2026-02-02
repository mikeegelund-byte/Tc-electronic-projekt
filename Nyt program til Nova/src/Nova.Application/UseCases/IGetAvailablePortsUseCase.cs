using System.Collections.Generic;

namespace Nova.Application.UseCases;

public interface IGetAvailablePortsUseCase
{
    IReadOnlyList<string> Execute();
}
