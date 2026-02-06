using FluentResults;
using Nova.Domain.Models;
using Serilog;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for copying a preset to a different slot.
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
    /// Copies a preset to a different slot on the hardware.
    /// </summary>
    /// <param name="preset">The preset to copy</param>
    /// <param name="targetSlot">Target slot number (1-60)</param>
    /// <returns>Result indicating success or failure</returns>
    public async Task<Result> ExecuteAsync(Preset preset, int targetSlot)
    {
        try
        {
            // Validate target slot
            if (targetSlot < 1 || targetSlot > 60)
            {
                _logger.Error("Invalid target slot: {TargetSlot}. Must be 1-60", targetSlot);
                return Result.Fail($"Target slot must be between 1 and 60, got {targetSlot}");
            }

            // Check if copying to same slot
            if (preset.Number == targetSlot)
            {
                _logger.Warning("Attempted to copy preset to same slot {SlotNumber}", targetSlot);
                return Result.Fail("Cannot copy preset to the same slot");
            }

            _logger.Information("Copying preset '{PresetName}' from slot {SourceSlot} to slot {TargetSlot}", 
                preset.Name, preset.Number, targetSlot);

            // Use SavePresetUseCase to save the copy - it handles slot number update and SysEx
            var saveResult = await _savePresetUseCase.ExecuteAsync(preset, targetSlot, verify: false);
            
            if (saveResult.IsSuccess)
            {
                _logger.Information("Successfully copied preset '{PresetName}' to slot {TargetSlot}", 
                    preset.Name, targetSlot);
            }

            return saveResult;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error copying preset to slot {TargetSlot}", targetSlot);
            return Result.Fail($"Unexpected error: {ex.Message}");
        }
    }
}
