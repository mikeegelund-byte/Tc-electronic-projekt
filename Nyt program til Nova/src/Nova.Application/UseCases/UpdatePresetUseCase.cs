using FluentResults;
using Microsoft.Extensions.Logging;
using Nova.Domain.Models;
using Nova.Midi;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for updating a preset on the Nova System pedal.
/// Sends a modified Preset back to the device via MIDI SysEx.
/// </summary>
public sealed class UpdatePresetUseCase
{
    private readonly ILogger<UpdatePresetUseCase>? _logger;

    public UpdatePresetUseCase(ILogger<UpdatePresetUseCase>? logger = null)
    {
        _logger = logger;
    }

    /// <summary>
    /// Sends a modified preset to the Nova System pedal.
    /// </summary>
    /// <param name="preset">The preset to send</param>
    /// <param name="port">MIDI port to send through</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success or error result</returns>
    public async Task<Result> ExecuteAsync(
        Preset preset,
        IMidiPort port,
        CancellationToken cancellationToken)
    {
        try
        {
            // Check for cancellation before starting
            if (cancellationToken.IsCancellationRequested)
            {
                _logger?.LogWarning("Update preset operation was cancelled before execution");
                return Result.Fail("Update preset operation was cancelled");
            }

            // Validate preset integrity
            var validationResult = ValidatePreset(preset);
            if (validationResult.IsFailed)
            {
                _logger?.LogError("Preset validation failed: {Errors}", 
                    string.Join(", ", validationResult.Errors.Select(e => e.Message)));
                return validationResult;
            }

            // Convert preset to SysEx
            var sysexResult = preset.ToSysEx();
            if (sysexResult.IsFailed)
            {
                _logger?.LogError("Failed to convert preset to SysEx: {Errors}", 
                    string.Join(", ", sysexResult.Errors.Select(e => e.Message)));
                return Result.Fail(sysexResult.Errors);
            }

            _logger?.LogInformation("Sending preset {PresetNumber} ({PresetName}) to pedal", 
                preset.Number, preset.Name);

            // Send SysEx to MIDI port
            var sendResult = await port.SendSysExAsync(sysexResult.Value);
            
            if (sendResult.IsFailed)
            {
                _logger?.LogError("Failed to send preset via MIDI: {Errors}", 
                    string.Join(", ", sendResult.Errors.Select(e => e.Message)));
                return sendResult;
            }

            _logger?.LogInformation("Successfully sent preset {PresetNumber} to pedal", preset.Number);
            return Result.Ok();
        }
        catch (OperationCanceledException)
        {
            _logger?.LogWarning("Update preset operation was cancelled");
            return Result.Fail("Update preset operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Unexpected error during preset update");
            return Result.Fail($"Unexpected error: {ex.Message}");
        }
    }

    /// <summary>
    /// Validates that the preset has valid data for sending.
    /// </summary>
    private Result ValidatePreset(Preset preset)
    {
        if (preset == null)
        {
            return Result.Fail("Preset cannot be null");
        }

        if (preset.RawSysEx == null || preset.RawSysEx.Length == 0)
        {
            return Result.Fail("Preset does not have valid RawSysEx data");
        }

        if (preset.RawSysEx.Length != 521)
        {
            return Result.Fail($"Invalid preset SysEx length: expected 521 bytes, got {preset.RawSysEx.Length}");
        }

        // Validate SysEx framing
        if (preset.RawSysEx[0] != 0xF0)
        {
            return Result.Fail("Preset SysEx must start with 0xF0");
        }

        if (preset.RawSysEx[520] != 0xF7)
        {
            return Result.Fail("Preset SysEx must end with 0xF7");
        }

        return Result.Ok();
    }
}
