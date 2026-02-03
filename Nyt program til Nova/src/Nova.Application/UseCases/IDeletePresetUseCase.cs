using FluentResults;

namespace Nova.Application.UseCases;

/// <summary>
/// Interface for deleting a preset by clearing the user preset number.
/// </summary>
public interface IDeletePresetUseCase
{
    /// <summary>
    /// Deletes a preset by clearing its user preset number (sending an init preset with default parameters).
    /// </summary>
    /// <param name="presetNumber">Preset number (31-90)</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> ExecuteAsync(int presetNumber);
}
