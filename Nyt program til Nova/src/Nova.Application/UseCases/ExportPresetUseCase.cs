using FluentResults;
using Nova.Domain.Models;
using Serilog;

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
/// Exports a TC Electronic Nova System preset to a human-readable .txt file.
/// </summary>
public sealed class ExportPresetUseCase
{
    private readonly ILogger _logger;

    public ExportPresetUseCase(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Exports a preset to a .txt file with human-readable key-value pairs.
    /// </summary>
    /// <param name="preset">The preset to export</param>
    /// <param name="filePath">The destination file path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result with file path on success or error message on failure</returns>
    public async Task<Result<string>> ExecuteAsync(
        Preset preset,
        string filePath,
        CancellationToken cancellationToken)
    {
        // Validate preset
        if (preset == null)
        {
            return Result.Fail("Preset cannot be null");
        }

        // Validate file path
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return Result.Fail("File path cannot be empty");
        }

        try
        {
            _logger.Information("Exporting preset {PresetNumber} '{PresetName}' to {FilePath}", 
                preset.Number, preset.Name, filePath);

            // Ensure directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Build the .txt content
            var content = BuildTextContent(preset);

            // Write to file with UTF-8 encoding
            await File.WriteAllTextAsync(filePath, content, System.Text.Encoding.UTF8, cancellationToken);

            _logger.Information("Successfully exported preset to {FilePath}", filePath);
            return Result.Ok(filePath);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.Error(ex, "Access denied when writing to {FilePath}", filePath);
            return Result.Fail($"Access denied: {ex.Message}");
        }
        catch (IOException ex)
        {
            _logger.Error(ex, "IO error when writing to {FilePath}", filePath);
            return Result.Fail($"IO error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error exporting preset to {FilePath}", filePath);
            return Result.Fail($"Export failed: {ex.Message}");
        }
    }

    private string BuildTextContent(Preset preset)
    {
        var lines = new List<string>
        {
            "# TC Electronic Nova System Preset",
            $"# Exported: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC",
            "",
            "# === BASIC INFO ===",
            $"Preset Number: {preset.Number}",
            $"Preset Name: {preset.Name}",
            "",
            "# === GLOBAL PARAMETERS ===",
            $"Tap Tempo: {preset.TapTempo}",
            $"Routing: {preset.Routing}",
            $"Level Out Left: {preset.LevelOutLeft}",
            $"Level Out Right: {preset.LevelOutRight}",
            "",
            "# === EFFECT SWITCHES ===",
            $"Compressor Enabled: {preset.CompressorEnabled}",
            $"Drive Enabled: {preset.DriveEnabled}",
            $"Modulation Enabled: {preset.ModulationEnabled}",
            $"Delay Enabled: {preset.DelayEnabled}",
            $"Reverb Enabled: {preset.ReverbEnabled}",
            "",
            "# === COMPRESSOR ===",
            $"Comp Type: {preset.CompType}",
            $"Comp Threshold: {preset.CompThreshold}",
            $"Comp Ratio: {preset.CompRatio}",
            $"Comp Attack: {preset.CompAttack}",
            $"Comp Release: {preset.CompRelease}",
            $"Comp Response: {preset.CompResponse}",
            $"Comp Drive: {preset.CompDrive}",
            $"Comp Level: {preset.CompLevel}",
            "",
            "# === DRIVE ===",
            $"Drive Type: {preset.DriveType}",
            $"Drive Gain: {preset.DriveGain}",
            $"Drive Level: {preset.DriveLevel}",
            "",
            "# === BOOST ===",
            $"Boost Type: {preset.BoostType}",
            $"Boost Gain: {preset.BoostGain}",
            $"Boost Level: {preset.BoostLevel}",
            "",
            "# === MODULATION ===",
            $"Mod Type: {preset.ModType}",
            $"Mod Speed: {preset.ModSpeed}",
            $"Mod Depth: {preset.ModDepth}",
            $"Mod Tempo: {preset.ModTempo}",
            $"Mod Hi Cut: {preset.ModHiCut}",
            $"Mod Feedback: {preset.ModFeedback}",
            $"Mod Delay Or Range: {preset.ModDelayOrRange}",
            $"Mod Mix: {preset.ModMix}",
            "",
            "# === DELAY ===",
            $"Delay Type: {preset.DelayType}",
            $"Delay Time: {preset.DelayTime}",
            $"Delay Time 2: {preset.DelayTime2}",
            $"Delay Tempo: {preset.DelayTempo}",
            $"Delay Tempo2 Or Width: {preset.DelayTempo2OrWidth}",
            $"Delay Feedback: {preset.DelayFeedback}",
            $"Delay Clip Or Feedback2: {preset.DelayClipOrFeedback2}",
            $"Delay Hi Cut: {preset.DelayHiCut}",
            $"Delay Lo Cut: {preset.DelayLoCut}",
            $"Delay Mix: {preset.DelayMix}",
            "",
            "# === REVERB ===",
            $"Reverb Type: {preset.ReverbType}",
            $"Reverb Decay: {preset.ReverbDecay}",
            $"Reverb Pre Delay: {preset.ReverbPreDelay}",
            $"Reverb Shape: {preset.ReverbShape}",
            $"Reverb Size: {preset.ReverbSize}",
            $"Reverb Hi Color: {preset.ReverbHiColor}",
            $"Reverb Hi Level: {preset.ReverbHiLevel}",
            $"Reverb Lo Color: {preset.ReverbLoColor}",
            $"Reverb Lo Level: {preset.ReverbLoLevel}",
            $"Reverb Room Level: {preset.ReverbRoomLevel}",
            $"Reverb Level: {preset.ReverbLevel}",
            $"Reverb Diffuse: {preset.ReverbDiffuse}",
            $"Reverb Mix: {preset.ReverbMix}",
            "",
            "# === GATE ===",
            $"Gate Type: {preset.GateType}",
            $"Gate Threshold: {preset.GateThreshold}",
            $"Gate Damp: {preset.GateDamp}",
            $"Gate Release: {preset.GateRelease}",
            "",
            "# === EQ ===",
            $"EQ Freq 1: {preset.EqFreq1}",
            $"EQ Gain 1: {preset.EqGain1}",
            $"EQ Width 1: {preset.EqWidth1}",
            $"EQ Freq 2: {preset.EqFreq2}",
            $"EQ Gain 2: {preset.EqGain2}",
            $"EQ Width 2: {preset.EqWidth2}",
            $"EQ Freq 3: {preset.EqFreq3}",
            $"EQ Gain 3: {preset.EqGain3}",
            $"EQ Width 3: {preset.EqWidth3}",
            "",
            "# === PITCH ===",
            $"Pitch Type: {preset.PitchType}",
            $"Pitch Voice 1: {preset.PitchVoice1}",
            $"Pitch Voice 2: {preset.PitchVoice2}",
            $"Pitch Pan 1: {preset.PitchPan1}",
            $"Pitch Pan 2: {preset.PitchPan2}",
            $"Pitch Delay 1: {preset.PitchDelay1}",
            $"Pitch Delay 2: {preset.PitchDelay2}",
            $"Pitch Feedback1 Or Key: {preset.PitchFeedback1OrKey}",
            $"Pitch Feedback2 Or Scale: {preset.PitchFeedback2OrScale}",
            $"Pitch Level 1: {preset.PitchLevel1}",
            $"Pitch Level 2: {preset.PitchLevel2}"
        };

        return string.Join(Environment.NewLine, lines);
    }
}
