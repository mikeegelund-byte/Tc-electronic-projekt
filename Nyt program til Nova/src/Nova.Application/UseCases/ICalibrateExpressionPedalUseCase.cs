namespace Nova.Application.UseCases;

/// <summary>
/// Use case for calibrating the expression pedal by learning min/max values.
/// </summary>
public interface ICalibrateExpressionPedalUseCase
{
    /// <summary>
    /// Starts pedal calibration by listening for CC messages over a time window.
    /// User should move the pedal from minimum to maximum during this period.
    /// </summary>
    /// <param name="timeoutMs">Time window in milliseconds (e.g., 5000 for 5 seconds)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result with min/max CC values observed, or failure if port not connected</returns>
    Task<FluentResults.Result<(int min, int max)>> CalibrateAsync(
        int timeoutMs, 
        CancellationToken cancellationToken = default);
}
