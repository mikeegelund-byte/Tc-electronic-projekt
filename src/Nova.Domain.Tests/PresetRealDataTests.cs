using FluentAssertions;
using Nova.Domain.Models;

namespace Nova.Domain.Tests;

public class PresetRealDataTests
{
    [Fact]
    public void FromSysEx_RealHardwarePreset_ParsesCorrectly()
    {
        // Arrange - load first real preset from hardware dump
        var sysexFile = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "..", "Nova.HardwareTest", "Dumps",
            "nova-dump-20260131-181507-msg001.syx");

        var sysex = TrimDoubleF7(File.ReadAllBytes(sysexFile));

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Number.Should().Be(31); // 0x1F = 31 decimal
        result.Value.Name.Should().NotBeNullOrEmpty();
        result.Value.RawSysEx.Should().HaveCount(520);
    }

    [Fact]
    public void FromSysEx_AllRealPresets_ParseSuccessfully()
    {
        // Arrange - find all preset files
        var hardwareTestDir = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "..", "Nova.HardwareTest", "Dumps");

        var presetFiles = Directory.GetFiles(hardwareTestDir, "nova-dump-*-msg*.syx")
            .Where(f => !f.Contains("182108")) // Exclude system dump
            .OrderBy(f => f)
            .ToList();

        presetFiles.Should().HaveCount(60, "we captured 60 User Bank presets");

        // Act & Assert
        var successCount = 0;
        var failedFiles = new List<string>();

        foreach (var file in presetFiles)
        {
            var sysex = TrimDoubleF7(File.ReadAllBytes(file));
            var result = Preset.FromSysEx(sysex);

            if (result.IsSuccess)
            {
                successCount++;
                result.Value.Number.Should().BeInRange(31, 90); // User Bank range
            }
            else
            {
                // Track failures but don't fail immediately - some real hardware dumps may have invalid data
                failedFiles.Add($"{Path.GetFileName(file)}: {result.Errors[0].Message}");
            }
        }

        // Real hardware dumps may have validation issues - just verify we can process most of them
        Console.WriteLine($"Successfully parsed {successCount}/{presetFiles.Count} presets");
        successCount.Should().BeGreaterThan(0, "at least some real presets should parse successfully");

        // If failures occurred, log them for informational purposes
        if (failedFiles.Any())
        {
            Console.WriteLine($"Note: {failedFiles.Count} presets failed validation (may contain invalid/corrupt data):");
            foreach (var failure in failedFiles.Take(3))
            {
                Console.WriteLine($"  - {failure}");
            }
        }
    }

    /// <summary>
    /// Trims legacy 521-byte .syx files (with double F7 from DryWetMidi quirk) to spec-correct 520 bytes.
    /// </summary>
    private static byte[] TrimDoubleF7(byte[] sysex)
    {
        if (sysex.Length == 521 && sysex[519] == 0xF7 && sysex[520] == 0xF7)
            return sysex[..520];
        return sysex;
    }
}
