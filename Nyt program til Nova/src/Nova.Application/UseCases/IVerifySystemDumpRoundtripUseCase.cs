using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for verifying that a saved SystemDump was correctly written to hardware.
/// Performs: Save → Wait → Request → Compare
/// </summary>
public interface IVerifySystemDumpRoundtripUseCase
{
    /// <summary>
    /// Saves the system dump, waits, requests it back, and compares.
    /// </summary>
    /// <param name="systemDump">The system dump to save and verify</param>
    /// <param name="waitMilliseconds">Time to wait after save before requesting (default 1000ms)</param>
    /// <returns>Result with verification success or failure details</returns>
    Task<Result> ExecuteAsync(SystemDump systemDump, int waitMilliseconds = 1000);
}
