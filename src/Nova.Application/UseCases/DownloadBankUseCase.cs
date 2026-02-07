using FluentResults;
using Nova.Domain.Midi;
using Nova.Domain.Models;
using Nova.Midi;

namespace Nova.Application.UseCases;

public sealed class DownloadBankUseCase : IDownloadBankUseCase
{
    private readonly IMidiPort _midiPort;
    private static readonly TimeSpan StartTimeout = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan IdleTimeout = TimeSpan.FromSeconds(3);
    private const int ExpectedPresetCount = 60;

    public DownloadBankUseCase(IMidiPort midiPort)
    {
        _midiPort = midiPort;
    }

    public async Task<Result<UserBankDump>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var request = SysExBuilder.BuildBankDumpRequest();
        var sendResult = await _midiPort.SendSysExAsync(request);
        var sendErrorMessage = sendResult.IsFailed
            ? sendResult.Errors.First().Message
            : null;

        var presetsByNumber = new Dictionary<int, Preset>();
        using var receiveCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        receiveCts.CancelAfter(StartTimeout);

        try
        {
            await foreach (var sysex in _midiPort.ReceiveSysExAsync(receiveCts.Token))
            {
                // Once any data starts flowing, use a shorter idle timeout between messages.
                receiveCts.CancelAfter(IdleTimeout);

                var presetResult = Preset.FromSysEx(sysex);
                if (presetResult.IsSuccess)
                {
                    presetsByNumber[presetResult.Value.Number] = presetResult.Value;

                    if (presetsByNumber.Count >= ExpectedPresetCount)
                        break;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected for idle timeout or caller cancellation.
        }

        if (presetsByNumber.Count == 0)
        {
            return cancellationToken.IsCancellationRequested
                ? Result.Fail("Download cancelled")
                : Result.Fail(sendErrorMessage != null
                    ? $"Failed to send dump request: {sendErrorMessage}. No presets received."
                    : "No presets received from pedal");
        }

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
