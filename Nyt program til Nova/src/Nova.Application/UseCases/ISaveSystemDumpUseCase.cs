using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for saving a SystemDump to the hardware via MIDI.
/// </summary>
public interface ISaveSystemDumpUseCase
{
    /// <summary>
    /// Sends the system dump to the connected Nova System pedal.
    /// </summary>
    /// <param name="systemDump">The system dump to save</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> ExecuteAsync(SystemDump systemDump);
}
