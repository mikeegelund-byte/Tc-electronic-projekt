using FluentResults;
using Nova.Domain.Models;
using Serilog;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for deleting a preset by clearing its user preset number.
/// </summary>
public sealed class DeletePresetUseCase : IDeletePresetUseCase
{
    private readonly ISavePresetUseCase _savePresetUseCase;
    private readonly ILogger _logger;

    public DeletePresetUseCase(
        ISavePresetUseCase savePresetUseCase,
        ILogger logger)
    {
        _savePresetUseCase = savePresetUseCase;
        _logger = logger;
    }

    /// <summary>
    /// Deletes a preset by sending an init preset with default parameters to clear the user preset number.
    /// </summary>
    /// <param name="presetNumber">Preset number (31-90)</param>
    /// <returns>Result indicating success or failure</returns>
    public async Task<Result> ExecuteAsync(int presetNumber)
    {
        try
        {
            // Validate preset number
            if (presetNumber < 31 || presetNumber > 90)
            {
                _logger.Error("Invalid preset number for delete: {PresetNumber}", presetNumber);
                return Result.Fail($"Preset number must be between 31 and 90 (got {presetNumber})");
            }

            _logger.Information("Deleting preset #{PresetNumber}", presetNumber);

            // Create init preset SysEx with default parameters
            var initPresetSysEx = CreateInitPresetSysEx(presetNumber);

            // Parse to Preset object
            var presetResult = Preset.FromSysEx(initPresetSysEx);
            if (presetResult.IsFailed)
            {
                _logger.Error("Failed to create init preset: {Errors}", 
                    string.Join(", ", presetResult.Errors));
                return Result.Fail($"Failed to create init preset: {presetResult.Errors.First().Message}");
            }

            // Send init preset to hardware to clear the preset
            var saveResult = await _savePresetUseCase.ExecuteAsync(presetResult.Value, presetNumber, verify: false);
            if (saveResult.IsFailed)
            {
                _logger.Error("Failed to delete preset #{PresetNumber}: {Errors}", 
                    presetNumber, string.Join(", ", saveResult.Errors));
                return Result.Fail($"Failed to delete preset: {saveResult.Errors.First().Message}");
            }

            _logger.Information("Successfully deleted preset #{PresetNumber}", presetNumber);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error deleting preset #{PresetNumber}", presetNumber);
            return Result.Fail($"Unexpected error: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a minimal init preset SysEx with all parameters at default values (0).
    /// </summary>
    private static byte[] CreateInitPresetSysEx(int presetNumber)
    {
        var sysex = new byte[520];
        
        // SysEx header
        sysex[0] = 0xF0; // SysEx start
        sysex[1] = 0x00; // TC Electronic ID
        sysex[2] = 0x20;
        sysex[3] = 0x1F;
        sysex[4] = 0x00; // Device ID
        sysex[5] = 0x63; // Nova System
        sysex[6] = 0x20; // Preset dump
        sysex[7] = 0x01; // Data type: Preset
        sysex[8] = (byte)presetNumber; // Preset number (31-90)
        
        // Reserved byte (void)
        sysex[9] = 0x00;
        
        // Preset name at bytes 9-32 (24 chars): "Init 31", "Init 32", etc.
        var name = $"Init {presetNumber:D2}".PadRight(24);
        var nameBytes = System.Text.Encoding.ASCII.GetBytes(name);
        Array.Copy(nameBytes, 0, sysex, 9, 24);
        
        // Parameters bytes 34-517: all zero (default values)
        // Already initialized to 0 by default
        
        // Calculate checksum (sum of bytes 34-517 & 0x7F)
        int checksum = 0;
        for (int i = 34; i <= 517; i++)
        {
            checksum += sysex[i];
        }
        sysex[518] = (byte)(checksum & 0x7F);
        
        sysex[519] = 0xF7; // SysEx end
        
        return sysex;
    }
}
