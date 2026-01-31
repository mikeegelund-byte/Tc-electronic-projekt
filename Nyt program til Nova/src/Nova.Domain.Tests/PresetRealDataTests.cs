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
            "..", "..", "..", "..", "Nova.HardwareTest",
            "nova-dump-20260131-181507-msg001.syx");

        var sysex = File.ReadAllBytes(sysexFile);

        // Act
        var result = Preset.FromSysEx(sysex);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Number.Should().Be(31); // 0x1F = 31 decimal
        result.Value.Name.Should().NotBeNullOrEmpty();
        result.Value.RawSysEx.Should().HaveCount(521);
    }

    [Fact]
    public void FromSysEx_AllRealPresets_ParseSuccessfully()
    {
        // Arrange - find all preset files
        var hardwareTestDir = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "..", "Nova.HardwareTest");

        var presetFiles = Directory.GetFiles(hardwareTestDir, "nova-dump-*-msg*.syx")
            .Where(f => !f.Contains("182108")) // Exclude system dump
            .OrderBy(f => f)
            .ToList();

        presetFiles.Should().HaveCount(60, "we captured 60 User Bank presets");

        // Act & Assert
        var successCount = 0;
        foreach (var file in presetFiles)
        {
            var sysex = File.ReadAllBytes(file);
            var result = Preset.FromSysEx(sysex);

            if (result.IsSuccess)
            {
                successCount++;
                result.Value.Number.Should().BeInRange(31, 90); // User Bank range
            }
            else
            {
                // If any fail, report which one
                Assert.Fail($"Failed to parse {Path.GetFileName(file)}: {result.Errors[0].Message}");
            }
        }

        successCount.Should().Be(60, "all 60 presets should parse successfully");
    }
}
