using FluentResults;
using Nova.Domain.Models;
using Nova.Midi;
using Serilog;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for sending a complete User Bank to the hardware via MIDI.
/// </summary>
public sealed class SendBankToHardwareUseCase : ISendBankToHardwareUseCase
{
    private readonly IMidiPort _midiPort;
    private readonly ILogger _logger;

    public SendBankToHardwareUseCase(IMidiPort midiPort, ILogger logger)
    {
        _midiPort = midiPort;
        _logger = logger;
    }

    public async Task<Result> ExecuteAsync(UserBankDump bank, CancellationToken cancellationToken)
    {
        if (!_midiPort.IsConnected)
        {
            return Result.Fail("MIDI port not connected");
        }

        if (bank.Presets.Length != 60)
        {
            return Result.Fail($"Invalid bank size: {bank.Presets.Length} (expected 60)");
        }

        try
        {
            _logger.Information("Sending {Count} presets to hardware", bank.Presets.Length);

            for (var i = 0; i < bank.Presets.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var preset = bank.Presets[i];
                if (preset == null)
                {
                    _logger.Warning("Skipping null preset at index {Index}", i);
                    continue;
                }

                var sysexResult = preset.ToSysEx();
                if (sysexResult.IsFailed)
                {
                    return Result.Fail($"Failed to serialize preset {preset.Number}: {sysexResult.Errors.First().Message}");
                }

                var sendResult = await _midiPort.SendSysExAsync(sysexResult.Value);
                if (sendResult.IsFailed)
                {
                    return Result.Fail($"Failed to send preset {preset.Number}: {sendResult.Errors.First().Message}");
                }

                await Task.Delay(TimeSpan.FromMilliseconds(75), cancellationToken);
            }

            _logger.Information("Successfully sent {Count} presets to hardware", bank.Presets.Length);
            return Result.Ok();
        }
        catch (OperationCanceledException)
        {
            _logger.Warning("Bank upload cancelled");
            return Result.Fail("Upload cancelled");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error sending bank to hardware");
            return Result.Fail($"Unexpected error: {ex.Message}");
        }
    }
}
