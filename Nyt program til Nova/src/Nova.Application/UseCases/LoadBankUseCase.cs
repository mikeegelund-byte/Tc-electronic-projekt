using FluentResults;
using Nova.Domain.Models;
using Nova.Midi;

namespace Nova.Application.UseCases;

/// <summary>
/// Loads a .syx file and uploads the presets to the pedal.
/// The .syx file must contain 60 preset dumps (60 × 521 bytes = 31,260 bytes total).
/// </summary>
public sealed class LoadBankUseCase
{
    private readonly IMidiPort _midiPort;

    public LoadBankUseCase(IMidiPort midiPort)
    {
        _midiPort = midiPort;
    }

    /// <summary>
    /// Loads a .syx file and uploads all presets to the pedal.
    /// </summary>
    /// <param name="filePath">Path to the .syx file</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success or failure reason</returns>
    public async Task<Result> ExecuteAsync(string filePath, CancellationToken cancellationToken)
    {
        // Validate file path
        if (string.IsNullOrWhiteSpace(filePath))
            return Result.Fail("File path cannot be empty");

        if (!File.Exists(filePath))
            return Result.Fail($"File not found: {filePath}");

        // Read .syx file
        byte[] sysexData;
        try
        {
            sysexData = await File.ReadAllBytesAsync(filePath, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return Result.Fail("Load operation was cancelled");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to read file: {ex.Message}");
        }

        // Parse .syx file into UserBankDump
        var bankResult = ParseSysExToBank(sysexData);
        if (bankResult.IsFailed)
            return Result.Fail($"Failed to parse .syx file: {string.Join(", ", bankResult.Errors)}");

        var bank = bankResult.Value;

        // Upload each preset to the pedal
        for (int i = 0; i < 60; i++)
        {
            if (cancellationToken.IsCancellationRequested)
                return Result.Fail($"Upload cancelled after {i} presets");

            var preset = bank.Presets[i];
            if (preset == null)
                return Result.Fail($"Preset at index {i} (number {i + 31}) is null");

            var presetSysExResult = preset.ToSysEx();
            if (presetSysExResult.IsFailed)
                return Result.Fail($"Failed to convert preset {preset.Number}: {string.Join(", ", presetSysExResult.Errors)}");

            var sendResult = await _midiPort.SendSysExAsync(presetSysExResult.Value);
            if (sendResult.IsFailed)
                return Result.Fail($"Failed to send preset {preset.Number}: {string.Join(", ", sendResult.Errors)}");
        }

        return Result.Ok();
    }

    /// <summary>
    /// Parses a .syx binary file into a UserBankDump.
    /// The file must contain exactly 60 presets of 521 bytes each.
    /// </summary>
    private Result<UserBankDump> ParseSysExToBank(byte[] sysexData)
    {
        const int presetSize = 521;
        const int presetCount = 60;
        const int expectedSize = presetSize * presetCount;

        // Validate file size
        if (sysexData.Length != expectedSize)
            return Result.Fail($"Invalid .syx file size: expected {expectedSize} bytes, got {sysexData.Length}");

        // Parse each preset
        var presets = new List<Preset>();
        for (int i = 0; i < presetCount; i++)
        {
            var presetData = new byte[presetSize];
            Array.Copy(sysexData, i * presetSize, presetData, 0, presetSize);

            var presetResult = Preset.FromSysEx(presetData);
            if (presetResult.IsFailed)
                return Result.Fail($"Failed to parse preset at index {i}: {string.Join(", ", presetResult.Errors)}");

            presets.Add(presetResult.Value);
        }

        // Create UserBankDump from presets
        return UserBankDump.FromPresets(presets);
    }
}
