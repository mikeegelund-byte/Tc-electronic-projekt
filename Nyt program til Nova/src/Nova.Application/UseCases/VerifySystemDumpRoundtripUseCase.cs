using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for verifying that a saved SystemDump was correctly written to hardware.
/// Performs: Save → Wait → Request → Compare
/// </summary>
public class VerifySystemDumpRoundtripUseCase : IVerifySystemDumpRoundtripUseCase
{
    private readonly ISaveSystemDumpUseCase _saveUseCase;
    private readonly IRequestSystemDumpUseCase _requestUseCase;

    public VerifySystemDumpRoundtripUseCase(
        ISaveSystemDumpUseCase saveUseCase,
        IRequestSystemDumpUseCase requestUseCase)
    {
        _saveUseCase = saveUseCase;
        _requestUseCase = requestUseCase;
    }

    /// <summary>
    /// Saves the system dump, waits, requests it back, and compares.
    /// </summary>
    /// <param name="systemDump">The system dump to save and verify</param>
    /// <param name="waitMilliseconds">Time to wait after save before requesting (default 1000ms)</param>
    /// <returns>Result with verification success or failure details</returns>
    public async Task<Result> ExecuteAsync(SystemDump systemDump, int waitMilliseconds = 1000)
    {
        // Step 1: Save to hardware
        var saveResult = await _saveUseCase.ExecuteAsync(systemDump);
        if (saveResult.IsFailed)
            return Result.Fail($"Save failed: {saveResult.Errors.First().Message}");

        // Step 2: Wait for hardware to process
        await Task.Delay(waitMilliseconds);

        // Step 3: Request back from hardware
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var requestResult = await _requestUseCase.ExecuteAsync(10000, cts.Token);
        if (requestResult.IsFailed)
            return Result.Fail($"Request failed: {requestResult.Errors.First().Message}");

        // Step 4: Compare original vs retrieved
        var originalSysEx = systemDump.ToSysEx().Value;
        var retrievedSysEx = requestResult.Value.ToSysEx().Value;

        if (!originalSysEx.SequenceEqual(retrievedSysEx))
        {
            return Result.Fail("Roundtrip verification failed: Data mismatch between saved and retrieved SystemDump");
        }

        return Result.Ok();
    }
}
