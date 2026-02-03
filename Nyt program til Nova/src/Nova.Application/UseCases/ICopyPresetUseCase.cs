using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

/// <summary>
/// Interface for copying a preset to a different user preset number.
/// </summary>
public interface ICopyPresetUseCase
{
    /// <summary>
    /// Copies a preset to a different user preset number on the hardware.
    /// </summary>
    /// <param name="preset">The preset to copy</param>
    /// <param name="targetPresetNumber">Target preset number (31-90)</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> ExecuteAsync(Preset preset, int targetPresetNumber);
}
