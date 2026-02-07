using FluentResults;
using Nova.Domain.Models;
using Serilog;

namespace Nova.Application.UseCases;

/// <summary>
/// Imports a TC Electronic Nova System preset from a human-readable .txt file.
/// </summary>
public sealed class ImportPresetUseCase : IImportPresetUseCase
{
    private readonly ILogger _logger;

    public ImportPresetUseCase(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Imports a preset from a .txt file with human-readable key-value pairs.
    /// </summary>
    /// <param name="filePath">The source file path</param>
    /// <returns>Result with Preset on success or error message on failure</returns>
    public async Task<Result<Preset>> ExecuteAsync(string filePath)
    {
        return await ExecuteAsync(filePath, CancellationToken.None);
    }

    /// <summary>
    /// Imports a preset from a .txt file with human-readable key-value pairs.
    /// </summary>
    /// <param name="filePath">The source file path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result with Preset on success or error message on failure</returns>
    public async Task<Result<Preset>> ExecuteAsync(
        string filePath,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("Importing preset from {FilePath}", filePath);

            // Validate file path
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return Result.Fail("File path cannot be empty");
            }

            if (!File.Exists(filePath))
            {
                return Result.Fail($"File not found: {filePath}");
            }

            // Read the file
            var content = await File.ReadAllTextAsync(filePath, System.Text.Encoding.UTF8, cancellationToken);

            // Parse the content
            var parseResult = ParseTextContent(content);
            if (parseResult.IsFailed)
            {
                return Result.Fail(parseResult.Errors);
            }

            var parameters = parseResult.Value;

            // Build SysEx from parameters
            var sysexResult = BuildSysExFromParameters(parameters);
            if (sysexResult.IsFailed)
            {
                return Result.Fail(sysexResult.Errors);
            }

            // Parse the SysEx into a Preset
            var presetResult = Preset.FromSysEx(sysexResult.Value);
            if (presetResult.IsFailed)
            {
                return Result.Fail(presetResult.Errors);
            }

            _logger.Information("Successfully imported preset {PresetNumber} '{PresetName}' from {FilePath}",
                presetResult.Value.Number, presetResult.Value.Name, filePath);

            return Result.Ok(presetResult.Value);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.Error(ex, "Access denied when reading from {FilePath}", filePath);
            return Result.Fail($"Access denied: {ex.Message}");
        }
        catch (IOException ex)
        {
            _logger.Error(ex, "IO error when reading from {FilePath}", filePath);
            return Result.Fail($"IO error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error importing preset from {FilePath}", filePath);
            return Result.Fail($"Import failed: {ex.Message}");
        }
    }

    private Result<Dictionary<string, string>> ParseTextContent(string content)
    {
        var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            // Skip comments and empty lines
            if (line.TrimStart().StartsWith("#") || string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            // Parse key-value pairs (format: "Key: Value")
            var parts = line.Split(':', 2);
            if (parts.Length == 2)
            {
                var key = parts[0].Trim();
                var value = parts[1].Trim();
                parameters[key] = value;
            }
        }

        // Validate that required fields are present
        if (!parameters.ContainsKey("Preset Number"))
        {
            return Result.Fail("Missing required field: Preset Number");
        }

        if (!parameters.ContainsKey("Preset Name"))
        {
            return Result.Fail("Missing required field: Preset Name");
        }

        return Result.Ok(parameters);
    }

    private Result<byte[]> BuildSysExFromParameters(Dictionary<string, string> parameters)
    {
        try
        {
            // Create a 521-byte SysEx message
            var sysex = new byte[521];

            // Header (F0 00 20 1F 00 63 20 01)
            sysex[0] = 0xF0;
            sysex[1] = 0x00;
            sysex[2] = 0x20;
            sysex[3] = 0x1F;
            sysex[4] = 0x00;
            sysex[5] = 0x63;
            sysex[6] = 0x20;
            sysex[7] = 0x01;

            // Preset Number (byte 8)
            sysex[8] = (byte)ParseInt(parameters, "Preset Number");

            // Preset Name (bytes 10-33 = 24 ASCII chars; byte 9 reserved)
            var name = parameters["Preset Name"];
            if (name.Length > 24)
            {
                name = name.Substring(0, 24);
            }
            var nameBytes = System.Text.Encoding.ASCII.GetBytes(name.PadRight(24));
            Array.Copy(nameBytes, 0, sysex, 10, 24);

            // Global parameters
            Encode4ByteValue(sysex, 38, ParseInt(parameters, "Tap Tempo"));
            Encode4ByteValue(sysex, 42, ParseInt(parameters, "Routing"));
            EncodeSignedDbValue(sysex, 46, ParseInt(parameters, "Level Out Left"), -100, 0);
            EncodeSignedDbValue(sysex, 50, ParseInt(parameters, "Level Out Right"), -100, 0);
            Encode4ByteValue(sysex, 54, ParseInt(parameters, "Map Parameter"));
            Encode4ByteValue(sysex, 58, ParseInt(parameters, "Map Min"));
            Encode4ByteValue(sysex, 62, ParseInt(parameters, "Map Mid"));
            Encode4ByteValue(sysex, 66, ParseInt(parameters, "Map Max"));

            // Compressor
            Encode4ByteValue(sysex, 70, ParseInt(parameters, "Comp Type"));
            EncodeSignedDbValue(sysex, 74, ParseInt(parameters, "Comp Threshold"), -30, 0);
            Encode4ByteValue(sysex, 78, ParseInt(parameters, "Comp Ratio"));
            Encode4ByteValue(sysex, 82, ParseInt(parameters, "Comp Attack"));
            Encode4ByteValue(sysex, 86, ParseInt(parameters, "Comp Release"));
            Encode4ByteValue(sysex, 90, ParseInt(parameters, "Comp Response"));
            Encode4ByteValue(sysex, 94, ParseInt(parameters, "Comp Drive"));
            EncodeSignedDbValue(sysex, 98, ParseInt(parameters, "Comp Level"), -12, 12);

            // Drive
            Encode4ByteValue(sysex, 134, ParseInt(parameters, "Drive Type"));
            Encode4ByteValue(sysex, 138, ParseInt(parameters, "Drive Gain"));
            Encode4ByteValue(sysex, 142, ParseInt(parameters, "Drive Tone"));
            EncodeSignedDbValue(sysex, 190, ParseInt(parameters, "Drive Level"), -100, 0);

            // Boost
            Encode4ByteValue(sysex, 182, ParseInt(parameters, "Boost Level"));
            Encode4ByteValue(sysex, 186, ParseBool(parameters, "Boost Enabled") ? 1 : 0);

            // Effect switches
            Encode4ByteValue(sysex, 130, ParseBool(parameters, "Compressor Enabled") ? 1 : 0);

            // Modulation
            Encode4ByteValue(sysex, 198, ParseInt(parameters, "Mod Type"));
            Encode4ByteValue(sysex, 202, ParseInt(parameters, "Mod Speed"));
            Encode4ByteValue(sysex, 206, ParseInt(parameters, "Mod Depth"));
            Encode4ByteValue(sysex, 210, ParseInt(parameters, "Mod Tempo"));
            Encode4ByteValue(sysex, 214, ParseInt(parameters, "Mod Hi Cut"));
            EncodeSignedDbValue(sysex, 218, ParseInt(parameters, "Mod Feedback"), -100, 100);
            Encode4ByteValue(sysex, 222, ParseInt(parameters, "Mod Delay Or Range"));
            Encode4ByteValue(sysex, 238, ParseInt(parameters, "Mod Width"));
            Encode4ByteValue(sysex, 250, ParseInt(parameters, "Mod Mix"));

            // Effect switches continued
            Encode4ByteValue(sysex, 194, ParseBool(parameters, "Drive Enabled") ? 1 : 0);
            Encode4ByteValue(sysex, 258, ParseBool(parameters, "Modulation Enabled") ? 1 : 0);

            // Delay
            Encode4ByteValue(sysex, 262, ParseInt(parameters, "Delay Type"));
            Encode4ByteValue(sysex, 266, ParseInt(parameters, "Delay Time"));
            Encode4ByteValue(sysex, 270, ParseInt(parameters, "Delay Time 2"));
            Encode4ByteValue(sysex, 274, ParseInt(parameters, "Delay Tempo"));
            Encode4ByteValue(sysex, 278, ParseInt(parameters, "Delay Tempo2 Or Width"));
            Encode4ByteValue(sysex, 282, ParseInt(parameters, "Delay Feedback"));
            Encode4ByteValue(sysex, 286, ParseInt(parameters, "Delay Clip Or Feedback2"));
            Encode4ByteValue(sysex, 290, ParseInt(parameters, "Delay Hi Cut"));
            Encode4ByteValue(sysex, 294, ParseInt(parameters, "Delay Lo Cut"));
            EncodeSignedDbValue(sysex, 298, ParseInt(parameters, "Delay Offset Or Pan1"), -200, 200);
            EncodeSignedDbValue(sysex, 302, ParseInt(parameters, "Delay Sense Or Pan2"), -50, 50);
            Encode4ByteValue(sysex, 306, ParseInt(parameters, "Delay Damp"));
            Encode4ByteValue(sysex, 310, ParseInt(parameters, "Delay Release"));
            Encode4ByteValue(sysex, 314, ParseInt(parameters, "Delay Mix"));

            // Effect switch
            Encode4ByteValue(sysex, 322, ParseBool(parameters, "Delay Enabled") ? 1 : 0);

            // Reverb
            Encode4ByteValue(sysex, 326, ParseInt(parameters, "Reverb Type"));
            Encode4ByteValue(sysex, 330, ParseInt(parameters, "Reverb Decay"));
            Encode4ByteValue(sysex, 334, ParseInt(parameters, "Reverb Pre Delay"));
            Encode4ByteValue(sysex, 338, ParseInt(parameters, "Reverb Shape"));
            Encode4ByteValue(sysex, 342, ParseInt(parameters, "Reverb Size"));
            Encode4ByteValue(sysex, 346, ParseInt(parameters, "Reverb Hi Color"));
            EncodeSignedDbValue(sysex, 350, ParseInt(parameters, "Reverb Hi Level"), -25, 25);
            Encode4ByteValue(sysex, 354, ParseInt(parameters, "Reverb Lo Color"));
            EncodeSignedDbValue(sysex, 358, ParseInt(parameters, "Reverb Lo Level"), -25, 25);
            EncodeSignedDbValue(sysex, 362, ParseInt(parameters, "Reverb Room Level"), -100, 0);
            EncodeSignedDbValue(sysex, 366, ParseInt(parameters, "Reverb Level"), -100, 0);
            EncodeSignedDbValue(sysex, 370, ParseInt(parameters, "Reverb Diffuse"), -25, 25);
            Encode4ByteValue(sysex, 374, ParseInt(parameters, "Reverb Mix"));

            // Effect switch
            Encode4ByteValue(sysex, 386, ParseBool(parameters, "Reverb Enabled") ? 1 : 0);

            // Gate
            Encode4ByteValue(sysex, 390, ParseInt(parameters, "Gate Type"));
            EncodeSignedDbValue(sysex, 394, ParseInt(parameters, "Gate Threshold"), -60, 0);
            Encode4ByteValue(sysex, 398, ParseInt(parameters, "Gate Damp"));
            Encode4ByteValue(sysex, 402, ParseInt(parameters, "Gate Release"));

            // EQ
            Encode4ByteValue(sysex, 406, ParseBool(parameters, "EQ Enabled") ? 1 : 0);
            Encode4ByteValue(sysex, 410, ParseInt(parameters, "EQ Freq 1"));
            EncodeSignedDbValue(sysex, 414, ParseInt(parameters, "EQ Gain 1"), -12, 12);
            Encode4ByteValue(sysex, 418, ParseInt(parameters, "EQ Width 1"));
            Encode4ByteValue(sysex, 422, ParseInt(parameters, "EQ Freq 2"));
            EncodeSignedDbValue(sysex, 426, ParseInt(parameters, "EQ Gain 2"), -12, 12);
            Encode4ByteValue(sysex, 430, ParseInt(parameters, "EQ Width 2"));
            Encode4ByteValue(sysex, 434, ParseInt(parameters, "EQ Freq 3"));
            EncodeSignedDbValue(sysex, 438, ParseInt(parameters, "EQ Gain 3"), -12, 12);
            Encode4ByteValue(sysex, 442, ParseInt(parameters, "EQ Width 3"));
            Encode4ByteValue(sysex, 450, ParseBool(parameters, "Gate Enabled") ? 1 : 0);

            // Pitch
            int pitchType = ParseInt(parameters, "Pitch Type");
            Encode4ByteValue(sysex, 454, pitchType);
            EncodeSignedDbValue(sysex, 458, ParseInt(parameters, "Pitch Voice 1"), -100, 100);
            EncodeSignedDbValue(sysex, 462, ParseInt(parameters, "Pitch Voice 2"), -100, 100);
            EncodeSignedDbValue(sysex, 466, ParseInt(parameters, "Pitch Pan 1"), -50, 50);
            EncodeSignedDbValue(sysex, 470, ParseInt(parameters, "Pitch Pan 2"), -50, 50);
            Encode4ByteValue(sysex, 474, ParseInt(parameters, "Pitch Delay 1"));
            Encode4ByteValue(sysex, 478, ParseInt(parameters, "Pitch Delay 2"));
            Encode4ByteValue(sysex, 482, ParseInt(parameters, "Pitch Feedback1 Or Key"));
            Encode4ByteValue(sysex, 486, ParseInt(parameters, "Pitch Feedback2 Or Scale"));
            EncodeSignedDbValue(sysex, 490, ParseInt(parameters, "Pitch Level 1"), -100, 0);
            if (pitchType == 1 || pitchType == 2)
            {
                Encode4ByteValue(sysex, 494, ParseInt(parameters, "Pitch Direction"));
            }
            else
            {
                EncodeSignedDbValue(sysex, 494, ParseInt(parameters, "Pitch Level 2"), -100, 0);
            }
            Encode4ByteValue(sysex, 498, ParseInt(parameters, "Pitch Range"));
            Encode4ByteValue(sysex, 502, ParseInt(parameters, "Pitch Mix"));
            Encode4ByteValue(sysex, 514, ParseBool(parameters, "Pitch Enabled") ? 1 : 0);

            // End marker
            sysex[520] = 0xF7;

            return Result.Ok(sysex);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to build SysEx: {ex.Message}");
        }
    }

    private int ParseInt(Dictionary<string, string> parameters, string key)
    {
        if (!parameters.TryGetValue(key, out var value))
        {
            return 0; // Default value if missing
        }

        if (int.TryParse(value, out var result))
        {
            return result;
        }

        throw new FormatException($"Invalid integer value for '{key}': {value}");
    }

    private bool ParseBool(Dictionary<string, string> parameters, string key)
    {
        if (!parameters.TryGetValue(key, out var value))
        {
            return false; // Default value if missing
        }

        if (bool.TryParse(value, out var result))
        {
            return result;
        }

        // Handle "True"/"False" case-insensitive
        return value.Equals("True", StringComparison.OrdinalIgnoreCase);
    }

    private void Encode4ByteValue(byte[] sysex, int offset, int value)
    {
        // Nova System uses 4-byte little-endian encoding
        // Each byte is 7-bit (0-127), combined into 28-bit value
        sysex[offset] = (byte)(value & 0x7F);
        sysex[offset + 1] = (byte)((value >> 7) & 0x7F);
        sysex[offset + 2] = (byte)((value >> 14) & 0x7F);
        sysex[offset + 3] = (byte)((value >> 21) & 0x7F);
    }

    private void EncodeSignedDbValue(byte[] sysex, int offset, int value, int minimumValue, int maximumValue)
    {
        const int LARGE_OFFSET = 16777216; // 2^24

        // Check if this is a symmetric range
        bool isSymmetric = minimumValue + maximumValue == 0;

        int encodedValue;
        if (isSymmetric)
        {
            // Symmetric ranges use large offset encoding
            encodedValue = value + LARGE_OFFSET;
        }
        else
        {
            // Asymmetric ranges use simple offset or large offset
            // Based on analysis, most use large offset
            encodedValue = value + LARGE_OFFSET;
        }

        Encode4ByteValue(sysex, offset, encodedValue);
    }
}
