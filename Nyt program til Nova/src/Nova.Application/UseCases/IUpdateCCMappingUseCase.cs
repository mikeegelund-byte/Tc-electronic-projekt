using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for updating a single MIDI CC assignment in a System Dump.
/// </summary>
public interface IUpdateCCMappingUseCase
{
    /// <summary>
    /// Updates a MIDI CC assignment at the specified index.
    /// </summary>
    /// <param name="systemDump">The System Dump to modify</param>
    /// <param name="ccIndex">The assignment index (0-10) to update</param>
    /// <param name="ccNumber">The new CC number (0-127) or null for Off</param>
    /// <returns>Result indicating success or failure with validation messages</returns>
    Task<Result> ExecuteAsync(SystemDump systemDump, int ccIndex, int? ccNumber);
}
