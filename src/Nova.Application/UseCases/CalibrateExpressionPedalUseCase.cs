using FluentResults;
using Nova.Midi;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for calibrating the expression pedal by learning min/max values.
/// </summary>
public class CalibrateExpressionPedalUseCase : ICalibrateExpressionPedalUseCase
{
    private readonly IMidiPort _midiPort;

    public CalibrateExpressionPedalUseCase(IMidiPort midiPort)
    {
        _midiPort = midiPort ?? throw new ArgumentNullException(nameof(midiPort));
    }

    /// <summary>
    /// Starts pedal calibration by listening for CC messages over a time window.
    /// </summary>
    public async Task<Result<(int min, int max)>> CalibrateAsync(
        int timeoutMs, 
        CancellationToken cancellationToken = default)
    {
        if (!_midiPort.IsConnected)
        {
            return Result.Fail<(int, int)>("MIDI port is not connected");
        }

        if (timeoutMs <= 0)
        {
            return Result.Fail<(int, int)>("Timeout must be positive");
        }

        var min = 127; // Start with max value
        var max = 0;   // Start with min value
        var received = false;

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeoutMs);

        try
        {
            await foreach (var ccData in _midiPort.ReceiveCCAsync().WithCancellation(cts.Token))
            {
                if (ccData.Length < 3) continue;

                // ccData format: [status, cc_number, value]
                var ccNumber = ccData[1];
                var value = ccData[2];

                // Track min/max of all CC messages (user may use different pedal CC)
                if (value < min) min = value;
                if (value > max) max = value;
                received = true;
            }
        }
        catch (OperationCanceledException)
        {
            // Timeout or cancellation - return what we collected
            if (!received)
            {
                return Result.Fail<(int, int)>("No CC messages received during calibration");
            }
        }

        return Result.Ok((min, max));
    }
}
