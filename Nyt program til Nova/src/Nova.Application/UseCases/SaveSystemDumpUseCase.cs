using FluentResults;
using Nova.Domain.Models;
using Nova.Midi;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for saving a SystemDump to the hardware via MIDI.
/// </summary>
public class SaveSystemDumpUseCase : ISaveSystemDumpUseCase
{
    private readonly IMidiPort _midiPort;

    public SaveSystemDumpUseCase(IMidiPort midiPort)
    {
        _midiPort = midiPort;
    }

    /// <summary>
    /// Sends the system dump to the connected Nova System pedal.
    /// </summary>
    /// <param name="systemDump">The system dump to save</param>
    /// <returns>Result indicating success or failure</returns>
    public async Task<Result> ExecuteAsync(SystemDump systemDump)
    {
        // Serialize to SysEx
        var sysexResult = systemDump.ToSysEx();
        if (sysexResult.IsFailed)
            return Result.Fail(sysexResult.Errors);

        // Send to hardware
        var sendResult = await _midiPort.SendSysExAsync(sysexResult.Value);
        if (sendResult.IsFailed)
            return Result.Fail($"Failed to send System Dump to hardware: {sendResult.Errors.First().Message}");

        return Result.Ok();
    }
}
