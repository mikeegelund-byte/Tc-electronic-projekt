using FluentResults;
using Nova.Domain.Models;
using Serilog;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for exporting a preset to a .syx (SysEx) file.
/// </summary>
public sealed class ExportSyxPresetUseCase : IExportPresetUseCase
{
    private readonly ILogger _logger;

    public ExportSyxPresetUseCase(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Exports a preset to a .syx (SysEx) file.
    /// </summary>
    /// <param name="preset">The preset to export</param>
    /// <param name="filePath">Full path where to save the .syx file</param>
    /// <returns>Result indicating success or failure</returns>
    public async Task<Result> ExecuteAsync(Preset preset, string filePath)
    {
        try
        {
            // Validate file path
            if (string.IsNullOrWhiteSpace(filePath))
            {
                _logger.Error("File path cannot be empty");
                return Result.Fail("File path cannot be empty");
            }

            // Create directory if it doesn't exist
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                _logger.Debug("Created directory: {Directory}", directory);
            }

            // Get preset as SysEx
            var sysexResult = preset.ToSysEx();
            if (sysexResult.IsFailed)
            {
                _logger.Error("Failed to convert preset to SysEx: {Errors}", 
                    string.Join(", ", sysexResult.Errors));
                return Result.Fail($"Failed to convert preset: {sysexResult.Errors.First().Message}");
            }

            var sysexData = sysexResult.Value;

            // Write to file
            await File.WriteAllBytesAsync(filePath, sysexData);

            _logger.Information("Successfully exported preset '{PresetName}' to {FilePath}", 
                preset.Name, filePath);
            return Result.Ok();
        }
        catch (IOException ex)
        {
            _logger.Error(ex, "IO error exporting preset to {FilePath}", filePath);
            return Result.Fail($"IO error: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.Error(ex, "Access denied exporting preset to {FilePath}", filePath);
            return Result.Fail($"Access denied: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error exporting preset to {FilePath}", filePath);
            return Result.Fail($"Unexpected error: {ex.Message}");
        }
    }
}
