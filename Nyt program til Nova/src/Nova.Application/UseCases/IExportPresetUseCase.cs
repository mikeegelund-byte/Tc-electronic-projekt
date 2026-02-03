using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

/// <summary>
/// Interface for exporting a preset to a .syx file.
/// </summary>
public interface IExportPresetUseCase
{
    /// <summary>
    /// Exports a preset to a .syx file.
    /// </summary>
    /// <param name="preset">The preset to export</param>
    /// <param name="filePath">Full path where to save the .syx file</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> ExecuteAsync(Preset preset, string filePath);
}
