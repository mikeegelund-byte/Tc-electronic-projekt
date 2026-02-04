using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for updating a single CC mapping in a System Dump
/// </summary>
public interface IUpdateCCMappingUseCase
{
    /// <summary>
    /// Updates a CC mapping at the specified index
    /// </summary>
    /// <param name="systemDump">The System Dump to modify</param>
    /// <param name="ccIndex">The CC index (0-63) to update</param>
    /// <param name="ccNumber">The new CC number (0-127)</param>
    /// <param name="parameterId">The new parameter ID</param>
    /// <returns>Result indicating success or failure with validation messages</returns>
    Task<Result> ExecuteAsync(SystemDump systemDump, int ccIndex, byte ccNumber, byte parameterId);
}
