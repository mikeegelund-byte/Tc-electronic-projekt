using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

public class UpdateProgramMapOutUseCase : IUpdateProgramMapOutUseCase
{
    public Task<Result> ExecuteAsync(SystemDump systemDump, int presetNumber, int outgoingProgram)
    {
        if (systemDump == null)
            return Task.FromResult(Result.Fail("System Dump cannot be null"));

        var result = systemDump.UpdateProgramMapOut(presetNumber, outgoingProgram);
        return Task.FromResult(result);
    }
}
