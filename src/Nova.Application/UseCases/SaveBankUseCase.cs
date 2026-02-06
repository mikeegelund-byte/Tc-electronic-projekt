using FluentResults;
using Nova.Domain.Models;
using Nova.Midi;
using Serilog;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for saving the current User Bank to a .syx file.
/// Downloads the bank from the device and writes it to disk in SysEx format.
/// </summary>
public sealed class SaveBankUseCase
{
    private readonly IDownloadBankUseCase _downloadBankUseCase;
    private readonly ILogger _logger;

    public SaveBankUseCase(IDownloadBankUseCase downloadBankUseCase, ILogger logger)
    {
        _downloadBankUseCase = downloadBankUseCase;
        _logger = logger;
    }

    /// <summary>
    /// Downloads the current User Bank from the device and saves it to a .syx file.
    /// </summary>
    /// <param name="filePath">Path where the .syx file should be saved</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result with the file path on success, or error on failure</returns>
    public async Task<Result<string>> ExecuteAsync(
        string filePath,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("Starting bank save to {FilePath}", filePath);

            // Step 1: Download current bank from device
            var downloadResult = await _downloadBankUseCase.ExecuteAsync(cancellationToken);

            if (downloadResult.IsFailed)
            {
                _logger.Error("Failed to download bank: {Errors}", string.Join(", ", downloadResult.Errors));
                return Result.Fail<string>($"Failed to download bank: {string.Join(", ", downloadResult.Errors)}");
            }

            var bank = downloadResult.Value;

            // Step 2: Convert UserBankDump to .syx binary format
            var sysexData = new List<byte>();
            foreach (var preset in bank.Presets)
            {
                if (preset == null)
                {
                    _logger.Error("Bank contains null preset");
                    return Result.Fail<string>("Bank contains incomplete preset data");
                }

                var presetSysExResult = preset.ToSysEx();
                if (presetSysExResult.IsFailed)
                {
                    _logger.Error("Failed to serialize preset {Number}: {Errors}", 
                        preset.Number, string.Join(", ", presetSysExResult.Errors));
                    return Result.Fail<string>($"Failed to serialize preset {preset.Number}");
                }

                sysexData.AddRange(presetSysExResult.Value);
            }

            // Step 3: Write to file with error handling
            await File.WriteAllBytesAsync(filePath, sysexData.ToArray(), cancellationToken);

            _logger.Information("Successfully saved bank to {FilePath} ({Size} bytes)", 
                filePath, sysexData.Count);

            // Step 4: Return success
            return Result.Ok(filePath);
        }
        catch (OperationCanceledException)
        {
            _logger.Warning("Bank save operation was cancelled");
            return Result.Fail<string>("Operation cancelled");
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.Error(ex, "Access denied when writing to {FilePath}", filePath);
            return Result.Fail<string>($"Access denied: {ex.Message}");
        }
        catch (DirectoryNotFoundException ex)
        {
            _logger.Error(ex, "Directory not found for {FilePath}", filePath);
            return Result.Fail<string>($"Directory not found: {ex.Message}");
        }
        catch (IOException ex)
        {
            _logger.Error(ex, "IO error when writing to {FilePath}", filePath);
            return Result.Fail<string>($"File write error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error saving bank to {FilePath}", filePath);
            return Result.Fail<string>($"Unexpected error: {ex.Message}");
        }
    }
}
