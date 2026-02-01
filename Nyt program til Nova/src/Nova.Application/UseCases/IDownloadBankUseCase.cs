using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

public interface IDownloadBankUseCase
{
    Task<Result<UserBankDump>> ExecuteAsync(CancellationToken cancellationToken);
}
