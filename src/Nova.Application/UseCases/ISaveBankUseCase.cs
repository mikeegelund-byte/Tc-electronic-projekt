using FluentResults;

namespace Nova.Application.UseCases;

public interface ISaveBankUseCase
{
    Task<Result<string>> ExecuteAsync(string filePath, CancellationToken cancellationToken);
}
