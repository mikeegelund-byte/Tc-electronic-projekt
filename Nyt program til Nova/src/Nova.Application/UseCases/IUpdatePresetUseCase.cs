using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

/// <summary>
/// Contract for updating a preset on the Nova System device.
/// </summary>
public interface IUpdatePresetUseCase
{
    /// <summary>
    /// Executes the update preset use case.
    /// </summary>
    /// <param name="modifiedPreset">The preset with modified values to send to the device</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success or failure with error message</returns>
    Task<Result> ExecuteAsync(
        Preset modifiedPreset,
        CancellationToken cancellationToken = default);
}
