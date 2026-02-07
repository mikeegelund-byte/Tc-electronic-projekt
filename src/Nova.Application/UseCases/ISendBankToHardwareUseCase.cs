using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

public interface ISendBankToHardwareUseCase
{
    Task<Result> ExecuteAsync(UserBankDump bank, CancellationToken cancellationToken);
}
