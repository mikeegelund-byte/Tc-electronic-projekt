using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

public interface IImportPresetUseCase
{
    Task<Result<Preset>> ExecuteAsync(string filePath);
}
