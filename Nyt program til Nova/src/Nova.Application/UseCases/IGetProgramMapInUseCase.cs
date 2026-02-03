using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

public interface IGetProgramMapInUseCase
{
    Task<Result<List<ProgramMapInEntry>>> ExecuteAsync(SystemDump systemDump);
}
