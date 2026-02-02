using FluentResults;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for exporting a single preset to a .syx file.
/// </summary>
public interface IExportPresetUseCase
{
    /// <summary>
    /// Exports a preset to the specified file path.
    /// </summary>
    /// <param name="presetNumber">Preset number (31-90)</param>
    /// <param name="filePath">Destination file path (.syx)</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> ExecuteAsync(int presetNumber, string filePath);
}
