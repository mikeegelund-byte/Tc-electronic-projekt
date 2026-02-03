using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

public class GetProgramMapOutUseCase : IGetProgramMapOutUseCase
{
    public Task<Result<List<ProgramMapOutEntry>>> ExecuteAsync(SystemDump systemDump)
    {
        if (systemDump == null)
            return Task.FromResult(Result.Fail<List<ProgramMapOutEntry>>("System Dump cannot be null"));

        return Task.FromResult(Result.Ok(systemDump.GetProgramMapOut()));
    }
}
