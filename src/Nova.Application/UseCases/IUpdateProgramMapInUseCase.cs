using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

public interface IUpdateProgramMapInUseCase
{
    Task<Result> ExecuteAsync(SystemDump systemDump, int incomingProgram, int? presetNumber);
}
