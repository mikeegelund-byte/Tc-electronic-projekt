using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

public class UpdateProgramMapInUseCase : IUpdateProgramMapInUseCase
{
    public Task<Result> ExecuteAsync(SystemDump systemDump, int incomingProgram, int? presetNumber)
    {
        if (systemDump == null)
            return Task.FromResult(Result.Fail("System Dump cannot be null"));

        var result = systemDump.UpdateProgramMapIn(incomingProgram, presetNumber);
        return Task.FromResult(result);
    }
}
