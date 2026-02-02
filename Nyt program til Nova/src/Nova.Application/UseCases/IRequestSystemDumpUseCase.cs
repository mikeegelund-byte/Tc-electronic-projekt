using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

public interface IRequestSystemDumpUseCase
{
    Task<Result<SystemDump>> ExecuteAsync(int timeoutMs = 5000, CancellationToken cancellationToken = default);
}
