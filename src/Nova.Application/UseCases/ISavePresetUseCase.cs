using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

/// <summary>
/// Interface for saving a preset to a specific slot on the hardware.
/// </summary>
public interface ISavePresetUseCase
{
    /// <summary>
    /// Saves the preset to the specified slot on the connected Nova System pedal.
    /// </summary>
    /// <param name="preset">The preset to save</param>
    /// <param name="slotNumber">Target slot number (1-60)</param>
    /// <param name="verify">If true, requests the preset back after saving to verify it was saved correctly (default: false)</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> ExecuteAsync(Preset preset, int slotNumber, bool verify = false);
}
