using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

public interface IUpdateProgramMapOutUseCase
{
    Task<Result> ExecuteAsync(SystemDump systemDump, int presetNumber, int outgoingProgram);
}
