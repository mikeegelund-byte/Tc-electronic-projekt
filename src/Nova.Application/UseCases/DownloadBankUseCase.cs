using FluentResults;
using Nova.Domain.Midi;
using Nova.Domain.Models;
using Nova.Midi;
using Serilog;

namespace Nova.Application.UseCases;

public sealed class DownloadBankUseCase : IDownloadBankUseCase
{
    private readonly IMidiPort _midiPort;
    private readonly ILogger _logger;
    private static readonly TimeSpan StartTimeout = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan IdleTimeout = TimeSpan.FromSeconds(3);
    private const int ExpectedPresetCount = 60;

    public DownloadBankUseCase(IMidiPort midiPort, ILogger logger)
    {
        _midiPort = midiPort;
        _logger = logger;
    }

    public async Task<Result<UserBankDump>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var request = SysExBuilder.BuildBankDumpRequest();
        _logger.Debug("Sending bank dump request ({Length} bytes)", request.Length);
        var sendResult = await _midiPort.SendSysExAsync(request);
        var sendErrorMessage = sendResult.IsFailed
            ? sendResult.Errors.First().Message
            : null;

        if (sendResult.IsFailed)
            _logger.Warning("Send bank dump request failed: {Error}", sendErrorMessage);
        else
            _logger.Debug("Bank dump request sent successfully");

        var presetsByNumber = new Dictionary<int, Preset>();
        using var receiveCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        receiveCts.CancelAfter(StartTimeout);

        try
        {
            await foreach (var sysex in _midiPort.ReceiveSysExAsync(receiveCts.Token))
            {
                // Once any data starts flowing, use a shorter idle timeout between messages.
                receiveCts.CancelAfter(IdleTimeout);

                _logger.Debug("Received SysEx: {Length} bytes", sysex.Length);

                var presetResult = Preset.FromSysEx(sysex);
                if (presetResult.IsSuccess)
                {
                    _logger.Debug("Parsed preset #{Number} '{Name}'", presetResult.Value.Number, presetResult.Value.Name);
                    presetsByNumber[presetResult.Value.Number] = presetResult.Value;

                    if (presetsByNumber.Count >= ExpectedPresetCount)
                        break;
                }
                else
                {
                    _logger.Warning("Failed to parse preset SysEx ({Length} bytes): {Errors}",
                        sysex.Length,
                        string.Join("; ", presetResult.Errors.Select(e => e.Message)));
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected for idle timeout or caller cancellation.
            _logger.Debug("Receive loop ended (timeout or cancellation). Got {Count} presets", presetsByNumber.Count);
        }

        if (presetsByNumber.Count == 0)
        {
            _logger.Warning("No presets received. SendFailed={SendFailed}, Cancelled={Cancelled}",
                sendErrorMessage != null, cancellationToken.IsCancellationRequested);
            return cancellationToken.IsCancellationRequested
                ? Result.Fail("Download cancelled")
                : Result.Fail(sendErrorMessage != null
                    ? $"Failed to send dump request: {sendErrorMessage}. No presets received."
                    : "No presets received from pedal");
        }

        _logger.Information("Download complete: {Count}/{Expected} presets received", presetsByNumber.Count, ExpectedPresetCount);

        var bank = UserBankDump.Empty();
        foreach (var preset in presetsByNumber.Values)
        {
            var addResult = bank.WithPreset(preset.Number, preset);
            if (addResult.IsFailed)
                return Result.Fail(addResult.Errors.First().Message);

            bank = addResult.Value;
        }

        return Result.Ok(bank);
    }
}
