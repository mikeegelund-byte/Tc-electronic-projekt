using FluentResults;
using Nova.Domain.Models;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for exporting a single preset to a .syx file.
/// </summary>
public class ExportPresetUseCase : IExportPresetUseCase
{
    private readonly UserBankDump _bank;

    public ExportPresetUseCase(UserBankDump bank)
    {
        _bank = bank;
    }

    public async Task<Result> ExecuteAsync(int presetNumber, string filePath)
    {
        // Validate preset number
        if (presetNumber < 31 || presetNumber > 90)
            return Result.Fail($"Preset number must be between 31 and 90, got {presetNumber}");

        // Find preset in bank
        var preset = _bank.Presets[presetNumber - 31];
        if (preset == null)
            return Result.Fail($"Preset {presetNumber} not found in bank");

        try
        {
            // Get SysEx bytes from preset
            var sysex = preset.RawSysEx;
            
            // Write to file
            await File.WriteAllBytesAsync(filePath, sysex);
            
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to write file: {ex.Message}");
        }
    }
}
