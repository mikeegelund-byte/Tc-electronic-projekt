using FluentResults;

namespace Nova.Application.UseCases;

/// <summary>
/// Interface for deleting a preset by clearing the slot.
/// </summary>
public interface IDeletePresetUseCase
{
    /// <summary>
    /// Deletes a preset by clearing its slot (sending an init preset with default parameters).
    /// </summary>
    /// <param name="slotNumber">Slot number (1-60)</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> ExecuteAsync(int slotNumber);
}
