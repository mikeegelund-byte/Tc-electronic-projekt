using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

public interface ILoadBankUseCase
{
    Task<Result<UserBankDump>> ExecuteAsync(string filePath, IProgress<int>? progress, CancellationToken cancellationToken);
}
