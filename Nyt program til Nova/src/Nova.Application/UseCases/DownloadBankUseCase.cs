using FluentResults;
using Nova.Domain.Models;
using Nova.Midi;

namespace Nova.Application.UseCases;

public sealed class DownloadBankUseCase
{
    private readonly IMidiPort _midiPort;

    public DownloadBankUseCase(IMidiPort midiPort)
    {
        _midiPort = midiPort;
    }

    public async Task<Result<UserBankDump>> ExecuteAsync(CancellationToken cancellationToken)
    {
        // Ideally send request here, but we listen passively or actively
        // _midiPort.SendSysExAsync(SysExBuilder.BuildBankDumpRequest());

        var receivedPresets = new List<Preset>();

        try 
        {
            await foreach (var sysex in _midiPort.ReceiveSysExAsync(cancellationToken))
            {
                var presetResult = Preset.FromSysEx(sysex);
                if (presetResult.IsSuccess)
                {
                    // Check if we already have this preset number? 
                    // For now simple collection
                    receivedPresets.Add(presetResult.Value);

                    if (receivedPresets.Count == 60)
                        break;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // If cancelled, we might return partial or fail
            return Result.Fail("Download cancelled");
        }

        return UserBankDump.FromPresets(receivedPresets);
    }
}
