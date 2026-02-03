using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case interface for requesting a preset from the hardware.
/// </summary>
public interface IRequestPresetUseCase
{
    /// <summary>
    /// Requests a specific preset from the hardware device.
    /// </summary>
    /// <param name="presetNumber">User preset number (31-90)</param>
    /// <param name="timeout">Timeout in milliseconds (default: 2000ms)</param>
    /// <returns>Result containing the preset on success, or error on failure</returns>
    Task<Result<Preset>> ExecuteAsync(int presetNumber, int timeout = 2000);
}
