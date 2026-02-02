using FluentResults;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for detecting the type of a .syx file by analyzing its contents.
/// </summary>
public interface IDetectSysExTypeUseCase
{
    /// <summary>
    /// Detects the SysEx type by reading the file and analyzing its structure.
    /// </summary>
    /// <param name="filePath">Path to the .syx file</param>
    /// <returns>Result with detected SysExType or error</returns>
    Task<Result<SysExType>> ExecuteAsync(string filePath);
}
