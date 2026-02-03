using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

/// <summary>
/// Interface for copying a preset to a different slot.
/// </summary>
public interface ICopyPresetUseCase
{
    /// <summary>
    /// Copies a preset to a different slot on the hardware.
    /// </summary>
    /// <param name="preset">The preset to copy</param>
    /// <param name="targetSlot">Target slot number (1-60)</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> ExecuteAsync(Preset preset, int targetSlot);
}
