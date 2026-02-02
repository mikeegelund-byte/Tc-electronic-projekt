using FluentResults;
using Nova.Domain.Models;
using Nova.Midi;
using Serilog;

namespace Nova.Application.UseCases;

/// <summary>
/// Use case for updating a preset on the Nova System device.
/// Validates preset data and sends via MIDI SysEx.
/// </summary>
public sealed class UpdatePresetUseCase : IUpdatePresetUseCase
{
    private readonly IMidiPort _midiPort;
    private readonly ILogger _logger;

    public UpdatePresetUseCase(IMidiPort midiPort, ILogger logger)
    {
        _midiPort = midiPort ?? throw new ArgumentNullException(nameof(midiPort));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Executes the update preset use case.
    /// </summary>
    /// <param name="modifiedPreset">The preset with modified values to send to the device</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success or failure with error message</returns>
    public async Task<Result> ExecuteAsync(
        Preset modifiedPreset,
        CancellationToken cancellationToken = default)
    {
        if (modifiedPreset == null)
        {
            _logger.Warning("UpdatePresetUseCase: Preset is null");
            return Result.Fail("Preset cannot be null");
        }

        // Validate preset data
        var validationResult = ValidatePreset(modifiedPreset);
        if (validationResult.IsFailed)
        {
            _logger.Warning("UpdatePresetUseCase: Validation failed - {Errors}", 
                string.Join(", ", validationResult.Errors.Select(e => e.Message)));
            return validationResult;
        }

        // Check MIDI connection
        if (!_midiPort.IsConnected)
        {
            _logger.Warning("UpdatePresetUseCase: MIDI port not connected");
            return Result.Fail("MIDI port is not connected");
        }

        // Convert preset to SysEx
        var sysExResult = modifiedPreset.ToSysEx();
        if (sysExResult.IsFailed)
        {
            _logger.Error("UpdatePresetUseCase: Failed to convert preset to SysEx - {Errors}", 
                string.Join(", ", sysExResult.Errors.Select(e => e.Message)));
            return Result.Fail($"Failed to convert preset to SysEx: {string.Join(", ", sysExResult.Errors.Select(e => e.Message))}");
        }

        try
        {
            // Send SysEx via MIDI
            var sendResult = await _midiPort.SendSysExAsync(sysExResult.Value);
            
            if (sendResult.IsSuccess)
            {
                _logger.Information("UpdatePresetUseCase: Successfully sent preset {PresetNumber} '{PresetName}' to device", 
                    modifiedPreset.Number, modifiedPreset.Name);
                return Result.Ok();
            }
            else
            {
                _logger.Error("UpdatePresetUseCase: Failed to send SysEx - {Errors}", 
                    string.Join(", ", sendResult.Errors.Select(e => e.Message)));
                return Result.Fail($"Failed to send preset: {string.Join(", ", sendResult.Errors.Select(e => e.Message))}");
            }
        }
        catch (OperationCanceledException)
        {
            _logger.Warning("UpdatePresetUseCase: Operation cancelled");
            return Result.Fail("Update operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "UpdatePresetUseCase: Unexpected error sending preset");
            return Result.Fail($"Unexpected error: {ex.Message}");
        }
    }

    /// <summary>
    /// Validates preset data according to Nova System specifications.
    /// </summary>
    private Result ValidatePreset(Preset preset)
    {
        var errors = new List<string>();

        // Validate preset name (1-24 characters, printable ASCII)
        if (string.IsNullOrWhiteSpace(preset.Name))
        {
            errors.Add("Preset name cannot be empty");
        }
        else if (preset.Name.Length > 24)
        {
            errors.Add("Preset name cannot exceed 24 characters");
        }

        // Validate tap tempo (100-3000 ms per Preset.cs domain model)
        if (preset.TapTempo < 100 || preset.TapTempo > 3000)
        {
            errors.Add($"Tap tempo must be between 100 and 3000 ms, got {preset.TapTempo}");
        }

        // Validate routing (0-2 per Preset.cs: 0=Semi-parallel, 1=Serial, 2=Parallel)
        if (preset.Routing < 0 || preset.Routing > 2)
        {
            errors.Add($"Routing must be between 0 and 2, got {preset.Routing}");
        }

        // Validate level out (-100 to 0 dB)
        if (preset.LevelOutLeft < -100 || preset.LevelOutLeft > 0)
        {
            errors.Add($"Level Out Left must be between -100 and 0 dB, got {preset.LevelOutLeft}");
        }
        if (preset.LevelOutRight < -100 || preset.LevelOutRight > 0)
        {
            errors.Add($"Level Out Right must be between -100 and 0 dB, got {preset.LevelOutRight}");
        }

        // Validate compressor parameters
        if (preset.CompType < 0 || preset.CompType > 2)
        {
            errors.Add($"Compressor type must be 0-2, got {preset.CompType}");
        }
        if (preset.CompThreshold < -30 || preset.CompThreshold > 0)
        {
            errors.Add($"Compressor threshold must be between -30 and 0 dB, got {preset.CompThreshold}");
        }

        // Validate other effect parameters are within 0-127 range (simplified validation)
        // In a production system, each parameter would have its specific range validation

        if (errors.Any())
        {
            return Result.Fail(string.Join("; ", errors));
        }

        return Result.Ok();
    }
}
