using FluentResults;
using Nova.Domain.Models;
using Nova.Midi;

namespace Nova.Application.UseCases;

/// <summary>
/// Downloads the current pedal bank and saves it as a .syx file.
/// The .syx file contains 60 preset dumps concatenated (60 × 521 bytes = 31,260 bytes total).
/// </summary>
public sealed class SaveBankUseCase
{
    private readonly DownloadBankUseCase _downloadBankUseCase;

    public SaveBankUseCase(IMidiPort midiPort)
    {
        _downloadBankUseCase = new DownloadBankUseCase(midiPort);
    }

    /// <summary>
    /// Downloads the current bank from the pedal and saves it to a .syx file.
    /// </summary>
    /// <param name="filePath">Path to save the .syx file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success or failure reason</returns>
    public async Task<Result> ExecuteAsync(string filePath, CancellationToken cancellationToken)
    {
        // Validate file path
        if (string.IsNullOrWhiteSpace(filePath))
            return Result.Fail("File path cannot be empty");

        // Download bank from pedal
        var downloadResult = await _downloadBankUseCase.ExecuteAsync(cancellationToken);
        if (downloadResult.IsFailed)
            return Result.Fail($"Failed to download bank: {string.Join(", ", downloadResult.Errors)}");

        var bank = downloadResult.Value;

        // Convert bank to .syx format
        var sysexDataResult = ConvertBankToSysEx(bank);
        if (sysexDataResult.IsFailed)
            return Result.Fail($"Failed to convert bank to SysEx: {string.Join(", ", sysexDataResult.Errors)}");

        // Save to file
        try
        {
            await File.WriteAllBytesAsync(filePath, sysexDataResult.Value, cancellationToken);
            return Result.Ok();
        }
        catch (OperationCanceledException)
        {
            return Result.Fail("Save operation was cancelled");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to write file: {ex.Message}");
        }
    }

    /// <summary>
    /// Converts a UserBankDump to .syx binary format.
    /// Each preset is 521 bytes, concatenated for 60 presets.
    /// </summary>
    private Result<byte[]> ConvertBankToSysEx(UserBankDump bank)
    {
        const int presetSize = 521;
        const int presetCount = 60;
        const int firstPresetNumber = 31;
        var sysexData = new byte[presetSize * presetCount];

        for (int i = 0; i < presetCount; i++)
        {
            var preset = bank.Presets[i];
            if (preset == null)
                return Result.Fail($"Preset at index {i} (number {i + firstPresetNumber}) is null");

            var presetSysExResult = preset.ToSysEx();
            if (presetSysExResult.IsFailed)
                return Result.Fail($"Failed to convert preset {preset.Number}: {string.Join(", ", presetSysExResult.Errors)}");

            var presetSysEx = presetSysExResult.Value;
            if (presetSysEx.Length != presetSize)
                return Result.Fail($"Preset {preset.Number} has invalid size: {presetSysEx.Length} bytes (expected {presetSize})");

            Array.Copy(presetSysEx, 0, sysexData, i * presetSize, presetSize);
        }

        return Result.Ok(sysexData);
    }
}
