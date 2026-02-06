using FluentResults;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for "CC Learn Mode" - listens for incoming MIDI CC messages
/// and automatically assigns them to parameters in the System Dump.
/// </summary>
public interface ICCLearnModeUseCase
{
    /// <summary>
    /// Starts CC Learn Mode with a timeout.
    /// Returns the learned CC number and parameter assignment.
    /// </summary>
    /// <param name="timeoutSeconds">Maximum time to wait for incoming CC</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result with learned CC number or timeout/error</returns>
    Task<Result<byte>> StartLearnAsync(int timeoutSeconds, CancellationToken cancellationToken = default);
}
