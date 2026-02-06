using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for retrieving MIDI CC (Control Change) to parameter mappings from a System Dump.
/// </summary>
public interface IGetCCMappingsUseCase
{
    /// <summary>
    /// Gets all 64 CC mappings from the provided System Dump.
    /// </summary>
    /// <param name="systemDump">The System Dump to extract CC mappings from.</param>
    /// <returns>A Result containing a list of CCMapping objects, or a failure message if System Dump is invalid.</returns>
    Task<Result<List<CCMapping>>> ExecuteAsync(SystemDump systemDump);
}
