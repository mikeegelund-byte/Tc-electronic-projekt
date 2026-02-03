using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

public class GetProgramMapInUseCase : IGetProgramMapInUseCase
{
    public Task<Result<List<ProgramMapInEntry>>> ExecuteAsync(SystemDump systemDump)
    {
        if (systemDump == null)
            return Task.FromResult(Result.Fail<List<ProgramMapInEntry>>("System Dump cannot be null"));

        return Task.FromResult(Result.Ok(systemDump.GetProgramMapIn()));
    }
}
