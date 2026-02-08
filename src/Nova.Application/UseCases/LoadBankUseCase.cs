using FluentResults;
using Nova.Domain.Models;
using Nova.Midi;
using Serilog;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for loading a User Bank from a .syx file to the device.
/// Reads the file and sends each preset to the device via MIDI.
/// </summary>
public sealed class LoadBankUseCase : ILoadBankUseCase
{
    private readonly IMidiPort _midiPort;
    private readonly ILogger _logger;

    public LoadBankUseCase(IMidiPort midiPort, ILogger logger)
    {
        _midiPort = midiPort;
        _logger = logger;
    }

    /// <summary>
    /// Loads a User Bank from a .syx file and sends all presets to the device.
    /// </summary>
    /// <param name="filePath">Path to the .syx file to load</param>
    /// <param name="progress">Optional progress reporter (0-60 presets)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result with the loaded UserBankDump on success, or error on failure</returns>
    public async Task<Result<UserBankDump>> ExecuteAsync(
        string filePath,
        IProgress<int>? progress,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("Starting bank load from {FilePath}", filePath);

            // Step 1: Read .syx file
            if (!File.Exists(filePath))
            {
                _logger.Error("File not found: {FilePath}", filePath);
                return Result.Fail<UserBankDump>($"File not found: {filePath}");
            }

            byte[] fileData = await File.ReadAllBytesAsync(filePath, cancellationToken);

            // Step 2: Validate file size — accept both 520-byte (spec) and 521-byte (legacy) formats
            const int specPresetSize = 520;
            const int legacyPresetSize = 521;
            int presetSize;

            if (fileData.Length == 60 * specPresetSize)
            {
                presetSize = specPresetSize;
            }
            else if (fileData.Length == 60 * legacyPresetSize)
            {
                _logger.Information("Loading legacy 521-byte format bank file ({Actual} bytes)", fileData.Length);
                presetSize = legacyPresetSize;
            }
            else
            {
                _logger.Error("Invalid file size: expected {Expected} or {Legacy} bytes, got {Actual}",
                    60 * specPresetSize, 60 * legacyPresetSize, fileData.Length);
                return Result.Fail<UserBankDump>($"Invalid file size: expected {60 * specPresetSize} or {60 * legacyPresetSize} bytes, got {fileData.Length}");
            }

            // Step 3: Parse and send each preset
            int presetsLoaded = 0;
            var presets = new List<Preset>();
            for (int i = 0; i < 60; i++)
            {
                // Extract preset (Preset.FromSysEx handles 521→520 trimming automatically)
                var presetData = new byte[presetSize];
                Array.Copy(fileData, i * presetSize, presetData, 0, presetSize);

                // Parse preset to validate it
                var presetResult = Preset.FromSysEx(presetData);
                if (presetResult.IsFailed)
                {
                    var errorMsg = string.Join(", ", presetResult.Errors.Select(e => e.Message));
                    _logger.Error("Failed to parse preset {Index}: {Errors}", i, errorMsg);
                    return Result.Fail<UserBankDump>($"Failed to parse preset {i}: {errorMsg}");
                }

                // Step 4: Send preset via MIDI
                var sendResult = await _midiPort.SendSysExAsync(presetData);
                if (sendResult.IsFailed)
                {
                    var errorMsg = string.Join(", ", sendResult.Errors.Select(e => e.Message));
                    _logger.Error("Failed to send preset {Index} (number {Number}): {Errors}",
                        i, presetResult.Value.Number, errorMsg);
                    return Result.Fail<UserBankDump>($"Failed to send preset {i}: {errorMsg}");
                }

                presetsLoaded++;
                presets.Add(presetResult.Value);

                // Step 5: Report progress
                progress?.Report(presetsLoaded);

                _logger.Debug("Sent preset {Index} (number {Number})", i, presetResult.Value.Number);
            }

            _logger.Information("Successfully loaded {Count} presets from {FilePath}",
                presetsLoaded, filePath);

            var bankResult = UserBankDump.FromPresets(presets);
            if (bankResult.IsFailed)
            {
                return Result.Fail<UserBankDump>(bankResult.Errors);
            }

            return Result.Ok(bankResult.Value);
        }
        catch (OperationCanceledException)
        {
            _logger.Warning("Bank load operation was cancelled");
            return Result.Fail<UserBankDump>("Operation cancelled");
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.Error(ex, "Access denied when reading from {FilePath}", filePath);
            return Result.Fail<UserBankDump>($"Access denied: {ex.Message}");
        }
        catch (IOException ex)
        {
            _logger.Error(ex, "IO error when reading from {FilePath}", filePath);
            return Result.Fail<UserBankDump>($"File read error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error loading bank from {FilePath}", filePath);
            return Result.Fail<UserBankDump>($"Unexpected error: {ex.Message}");
        }
    }
}
