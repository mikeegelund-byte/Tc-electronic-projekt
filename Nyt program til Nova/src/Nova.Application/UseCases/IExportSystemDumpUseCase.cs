using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for exporting System Settings to a .syx file.
/// </summary>
public interface IExportSystemDumpUseCase
{
    /// <summary>
    /// Exports the System Settings to the specified file path.
    /// </summary>
    /// <param name="systemDump">The SystemDump to export</param>
    /// <param name="filePath">Destination file path (.syx)</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> ExecuteAsync(SystemDump systemDump, string filePath);
}
