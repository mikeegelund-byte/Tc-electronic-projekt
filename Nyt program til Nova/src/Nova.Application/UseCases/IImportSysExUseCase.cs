using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for importing a .syx file and parsing it into appropriate domain models.
/// Supports Preset, UserBank, and SystemDump files.
/// </summary>
public interface IImportSysExUseCase
{
    /// <summary>
    /// Imports a .syx file and returns the parsed data.
    /// The return type depends on the detected file type.
    /// </summary>
    /// <param name="filePath">Path to the .syx file</param>
    /// <returns>Result with imported data (Preset, UserBankDump, or SystemDump) or error</returns>
    Task<Result<object>> ExecuteAsync(string filePath);
}
