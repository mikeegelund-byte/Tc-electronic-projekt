using FluentResults;

namespace Nova.Application.UseCases;

public interface IConnectUseCase
{
    Task<Result> ExecuteAsync(string portName);
}
