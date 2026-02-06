using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

/// <summary>
/// Interface for renaming a preset.
/// </summary>
public interface IRenamePresetUseCase
{
    /// <summary>
    /// Renames a preset and saves it back to the same slot.
    /// </summary>
    /// <param name="preset">The preset to rename</param>
    /// <param name="newName">New name (max 24 characters)</param>
    /// <returns>Result with the renamed preset on success, or error on failure</returns>
    Task<Result<Preset>> ExecuteAsync(Preset preset, string newName);
}
