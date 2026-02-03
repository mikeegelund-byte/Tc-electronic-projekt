using FluentResults;
using Nova.Domain.Models;
using Serilog;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for copying a preset to a different user preset number.
/// </summary>
public sealed class CopyPresetUseCase : ICopyPresetUseCase
{
    private readonly ISavePresetUseCase _savePresetUseCase;
    private readonly ILogger _logger;

    public CopyPresetUseCase(ISavePresetUseCase savePresetUseCase, ILogger logger)
    {
        _savePresetUseCase = savePresetUseCase;
        _logger = logger;
    }

    /// <summary>
    /// Copies a preset to a different user preset number on the hardware.
    /// </summary>
    /// <param name="preset">The preset to copy</param>
    /// <param name="targetPresetNumber">Target preset number (31-90)</param>
    /// <returns>Result indicating success or failure</returns>
    public async Task<Result> ExecuteAsync(Preset preset, int targetPresetNumber)
    {
        try
        {
            // Validate target preset number
            if (targetPresetNumber < 31 || targetPresetNumber > 90)
            {
                _logger.Error("Invalid target preset number: {PresetNumber}. Must be 31-90", targetPresetNumber);
                return Result.Fail($"Target preset number must be between 31 and 90, got {targetPresetNumber}");
            }

            // Check if copying to same preset
            if (preset.Number == targetPresetNumber)
            {
                _logger.Warning("Attempted to copy preset to same preset #{PresetNumber}", targetPresetNumber);
                return Result.Fail("Cannot copy preset to the same preset");
            }

            _logger.Information("Copying preset '{PresetName}' from preset #{SourcePreset} to preset #{TargetPreset}", 
                preset.Name, preset.Number, targetPresetNumber);

            // Use SavePresetUseCase to save the copy - it handles preset number update and SysEx
            var saveResult = await _savePresetUseCase.ExecuteAsync(preset, targetPresetNumber, verify: false);
            
            if (saveResult.IsSuccess)
            {
                _logger.Information("Successfully copied preset '{PresetName}' to preset #{TargetPreset}", 
                    preset.Name, targetPresetNumber);
            }

            return saveResult;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error copying preset to preset #{PresetNumber}", targetPresetNumber);
            return Result.Fail($"Unexpected error: {ex.Message}");
        }
    }
}
