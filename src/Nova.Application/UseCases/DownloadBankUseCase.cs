using FluentResults;
using Nova.Domain.Midi;
using Nova.Domain.Models;
using Nova.Midi;

namespace Nova.Application.UseCases;

public sealed class DownloadBankUseCase : IDownloadBankUseCase
{
    private readonly IMidiPort _midiPort;
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
        if (sendResult.IsFailed)
        {
            return Result.Fail(sendResult.Errors.First().Message);
        }

        var presetsByNumber = new Dictionary<int, Preset>();
        using var idleCts = new CancellationTokenSource(IdleTimeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, idleCts.Token);

        try
        {
            await foreach (var sysex in _midiPort.ReceiveSysExAsync(linkedCts.Token))
            {
                idleCts.CancelAfter(IdleTimeout);

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
                : Result.Fail("No presets received from pedal");
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
