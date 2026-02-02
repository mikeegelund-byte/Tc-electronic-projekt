using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for exporting the entire User Bank (60 presets) to a .syx file.
/// </summary>
public interface IExportBankUseCase
{
    /// <summary>
    /// Exports the User Bank to the specified file path.
    /// The file will contain all 60 presets concatenated in sequence.
    /// </summary>
    /// <param name="bank">The UserBankDump to export</param>
    /// <param name="filePath">Destination file path (.syx)</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> ExecuteAsync(UserBankDump bank, string filePath);
}
