using FluentResults;
using Nova.Domain.Midi;
using Nova.Domain.Models;
using Nova.Midi;
using Serilog;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for requesting system dump from Nova System pedal.
/// Follows the ConnectUseCase pattern with ExecuteAsync and FluentResults.
/// </summary>
public sealed class RequestSystemDumpUseCase
{
    private readonly IMidiPort _midiPort;

    public RequestSystemDumpUseCase(IMidiPort midiPort)
    {
        _midiPort = midiPort ?? throw new ArgumentNullException(nameof(midiPort));
    }

    /// <summary>
    /// Executes the system dump request.
    /// </summary>
    /// <param name="timeoutMs">Timeout in milliseconds for receiving the response (default: 5000ms)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result containing SystemDump or error</returns>
    public async Task<Result<SystemDump>> ExecuteAsync(
        int timeoutMs = 5000,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. Build request
            var request = SysExBuilder.BuildSystemDumpRequest();
            Log.Information("Requesting system dump from Nova System pedal");

            // 2. Send request
            var sendResult = await _midiPort.SendSysExAsync(request);
            if (sendResult.IsFailed)
            {
                Log.Error("Failed to send system dump request: {Error}", sendResult.Errors.First().Message);
                return Result.Fail<SystemDump>(sendResult.Errors.First().Message);
            }

            // 3. Wait for response (using timeout via CancellationTokenSource)
            using var timeoutCts = new CancellationTokenSource(timeoutMs);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            byte[]? receivedData = null;
            try
            {
                await foreach (var sysex in _midiPort.ReceiveSysExAsync(linkedCts.Token))
                {
                    receivedData = sysex;
                    break; // We only need the first response
                }
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
            {
                Log.Error("Timeout waiting for system dump response after {TimeoutMs}ms", timeoutMs);
                return Result.Fail($"Timeout waiting for system dump response after {timeoutMs}ms");
            }

            if (receivedData == null)
            {
                Log.Error("No system dump response received");
                return Result.Fail("No system dump response received");
            }

            // 4. Parse response
            var parseResult = SystemDump.FromSysEx(receivedData);

            // 5. Log and return
            if (parseResult.IsSuccess)
            {
                Log.Information("System dump received successfully");
                return parseResult;
            }
            else
            {
                Log.Error("Failed to parse system dump: {Error}", parseResult.Errors.First().Message);
                return parseResult;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error while requesting system dump");
            return Result.Fail($"Unexpected error: {ex.Message}");
        }
    }
}
