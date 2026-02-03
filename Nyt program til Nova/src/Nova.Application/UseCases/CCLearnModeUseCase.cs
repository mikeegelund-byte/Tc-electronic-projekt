using FluentResults;
using Nova.Midi;

namespace Nova.Application.UseCases;

/// <summary>
/// Implements CC Learn Mode functionality.
/// Listens for the first incoming MIDI CC message within a timeout period.
/// </summary>
public class CCLearnModeUseCase : ICCLearnModeUseCase
{
    private readonly IMidiPort _midiPort;

    public CCLearnModeUseCase(IMidiPort midiPort)
    {
        _midiPort = midiPort ?? throw new ArgumentNullException(nameof(midiPort));
    }

    /// <summary>
    /// Starts CC Learn Mode - listens for incoming MIDI CC messages.
    /// Returns the first CC number received within the timeout period.
    /// </summary>
    public async Task<Result<byte>> StartLearnAsync(int timeoutSeconds, CancellationToken cancellationToken = default)
    {
        if (timeoutSeconds <= 0)
            return Result.Fail<byte>("Timeout must be greater than 0 seconds");

        // Check if MIDI port is connected
        if (!_midiPort.IsConnected)
            return Result.Fail<byte>("MIDI port not connected. Please connect first.");

        try
        {
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            // Listen for incoming MIDI messages
            await foreach (var message in _midiPort.ReceiveCCAsync(linkedCts.Token))
            {
                // Extract CC number from first data byte
                // MIDI CC format: [Status: 0xB0-0xBF] [CC#: 0-127] [Value: 0-127]
                if (message.Length >= 2)
                {
                    byte ccNumber = message[1];
                    return Result.Ok(ccNumber);
                }
            }

            // If we exit the loop without receiving a message, it means timeout
            return Result.Fail<byte>($"No CC message received within {timeoutSeconds} seconds");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return Result.Fail<byte>("CC Learn cancelled by user");
        }
        catch (OperationCanceledException)
        {
            return Result.Fail<byte>($"No CC message received within {timeoutSeconds} seconds");
        }
        catch (Exception ex)
        {
            return Result.Fail<byte>($"Error during CC Learn: {ex.Message}");
        }
    }
}
