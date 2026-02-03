using FluentResults;
using Nova.Domain.Models;
using Serilog;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for renaming a preset.
/// </summary>
public sealed class RenamePresetUseCase : IRenamePresetUseCase
{
    private readonly ISavePresetUseCase _savePresetUseCase;
    private readonly IRequestPresetUseCase _requestPresetUseCase;
    private readonly ILogger _logger;

    public RenamePresetUseCase(
        ISavePresetUseCase savePresetUseCase,
        IRequestPresetUseCase requestPresetUseCase,
        ILogger logger)
    {
        _savePresetUseCase = savePresetUseCase;
        _requestPresetUseCase = requestPresetUseCase;
        _logger = logger;
    }

    /// <summary>
    /// Renames a preset and saves it back to the same preset number.
    /// </summary>
    /// <param name="preset">The preset to rename</param>
    /// <param name="newName">New name (max 24 characters)</param>
    /// <returns>Result with the renamed preset on success, or error on failure</returns>
    public async Task<Result<Preset>> ExecuteAsync(Preset preset, string newName)
    {
        try
        {
            // Validate name
            if (string.IsNullOrWhiteSpace(newName))
            {
                _logger.Error("Preset name cannot be empty");
                return Result.Fail<Preset>("Preset name cannot be empty");
            }

            if (newName.Length > 24)
            {
                _logger.Error("Preset name too long: {Length} chars (max 24)", newName.Length);
                return Result.Fail<Preset>("Preset name must be 24 characters or less");
            }

            _logger.Information("Renaming preset from '{OldName}' to '{NewName}' in preset #{PresetNumber}", 
                preset.Name, newName, preset.Number);

            // Get current preset SysEx and modify name bytes (9-32 = 24 ASCII chars)
            var sysexData = preset.ToSysEx().Value;
            
            // Pad name to 24 chars and encode as ASCII
            var nameBytes = System.Text.Encoding.ASCII.GetBytes(newName.PadRight(24));
            Array.Copy(nameBytes, 0, sysexData, 9, 24);

            // Recalculate checksum (bytes 34-517 & 0x7F)
            int checksum = 0;
            for (int i = 34; i <= 517; i++)
            {
                checksum += sysexData[i];
            }
            sysexData[518] = (byte)(checksum & 0x7F);

            // Parse updated SysEx to create new Preset object
            var renamedPresetResult = Preset.FromSysEx(sysexData);
            if (renamedPresetResult.IsFailed)
            {
                _logger.Error("Failed to parse renamed preset: {Errors}", 
                    string.Join(", ", renamedPresetResult.Errors));
                return Result.Fail<Preset>($"Failed to create renamed preset: {renamedPresetResult.Errors.First().Message}");
            }

            var renamedPreset = renamedPresetResult.Value;

            // Save back to hardware
            var saveResult = await _savePresetUseCase.ExecuteAsync(renamedPreset, preset.Number, verify: false);
            if (saveResult.IsFailed)
            {
                _logger.Error("Failed to save renamed preset: {Errors}", string.Join(", ", saveResult.Errors));
                return Result.Fail<Preset>($"Failed to save: {saveResult.Errors.First().Message}");
            }

            // Request it back to verify rename succeeded
            var verifyResult = await _requestPresetUseCase.ExecuteAsync(preset.Number, timeout: 2000);
            if (verifyResult.IsFailed)
            {
                _logger.Warning("Rename succeeded but verification failed: {Errors}", 
                    string.Join(", ", verifyResult.Errors));
                // Return the renamed preset even if verification failed
                return Result.Ok(renamedPreset);
            }

            _logger.Information("Successfully renamed preset to '{NewName}' in preset #{PresetNumber}", 
                newName, preset.Number);

            return Result.Ok(verifyResult.Value);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error renaming preset");
            return Result.Fail<Preset>($"Unexpected error: {ex.Message}");
        }
    }
}
