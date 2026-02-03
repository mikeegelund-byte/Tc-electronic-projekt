using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for updating expression pedal mapping and saving to hardware.
/// </summary>
public class UpdatePedalMappingUseCase : IUpdatePedalMappingUseCase
{
    private readonly SystemDump _systemDump;
    private readonly ISaveSystemDumpUseCase _saveUseCase;

    public UpdatePedalMappingUseCase(
        SystemDump systemDump,
        ISaveSystemDumpUseCase saveUseCase)
    {
        _systemDump = systemDump ?? throw new ArgumentNullException(nameof(systemDump));
        _saveUseCase = saveUseCase ?? throw new ArgumentNullException(nameof(saveUseCase));
    }

    /// <summary>
    /// Updates the expression pedal mapping and saves to hardware.
    /// </summary>
    public async Task<Result> UpdateAsync(int parameter, int min, int mid, int max)
    {
        // Update SystemDump in-memory
        var paramResult = _systemDump.UpdatePedalParameter(parameter);
        if (paramResult.IsFailed)
            return paramResult;

        var minResult = _systemDump.UpdatePedalMin(min);
        if (minResult.IsFailed)
            return minResult;

        var midResult = _systemDump.UpdatePedalMid(mid);
        if (midResult.IsFailed)
            return midResult;

        var maxResult = _systemDump.UpdatePedalMax(max);
        if (maxResult.IsFailed)
            return maxResult;

        // Save to hardware
        var saveResult = await _saveUseCase.ExecuteAsync(_systemDump);
        if (saveResult.IsFailed)
            return saveResult;

        return Result.Ok();
    }
}
