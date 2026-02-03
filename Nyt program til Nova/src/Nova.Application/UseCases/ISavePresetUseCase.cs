using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

/// <summary>
/// Interface for saving a preset to a specific user preset number on the hardware.
/// </summary>
public interface ISavePresetUseCase
{
    /// <summary>
    /// Saves the preset to the specified user preset number on the connected Nova System pedal.
    /// </summary>
    /// <param name="preset">The preset to save</param>
    /// <param name="presetNumber">Target preset number (31-90)</param>
    /// <param name="verify">If true, requests the preset back after saving to verify it was saved correctly (default: false)</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> ExecuteAsync(Preset preset, int presetNumber, bool verify = false);
}
