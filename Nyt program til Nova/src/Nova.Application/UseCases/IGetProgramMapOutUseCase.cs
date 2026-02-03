using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

public interface IGetProgramMapOutUseCase
{
    Task<Result<List<ProgramMapOutEntry>>> ExecuteAsync(SystemDump systemDump);
}
