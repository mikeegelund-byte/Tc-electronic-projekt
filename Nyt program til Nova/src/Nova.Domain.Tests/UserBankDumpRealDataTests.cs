using FluentAssertions;
using Nova.Domain.Models;

namespace Nova.Domain.Tests;

public class UserBankDumpRealDataTests
{
    [Fact]
    public void FromPresets_60RealHardwarePresets_CreatesCompleteBank()
    {
        // Arrange - Load all 60 real SysEx files
        var baseDir = GetHardwareTestDir();
        var allFiles = Directory.GetFiles(baseDir, "nova-dump-*-msg*.syx")
            .Where(f => !f.Contains("182108")) // Exclude System Dump
            .OrderBy(f => f)
            .ToList();

        allFiles.Should().HaveCount(60, "Should have exactly 60 User Bank presets");

        var presets = new List<Preset>();
        foreach (var path in allFiles)
        {
            var sysex = File.ReadAllBytes(path);
            var preset = Preset.FromSysEx(sysex);
            preset.IsSuccess.Should().BeTrue($"Preset from {Path.GetFileName(path)} should parse");
            presets.Add(preset.Value);
        }

        // Act - Create bank from all presets
        var result = UserBankDump.FromPresets(presets);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Presets.Should().HaveCount(60);
        result.Value.Presets.Should().AllSatisfy(p => p.Should().NotBeNull());

        // Verify all preset numbers are 31-90
        for (int i = 0; i < 60; i++)
        {
            result.Value.Presets[i]!.Number.Should().Be(31 + i);
        }
    }

    [Fact]
    public void WithPreset_ReplaceExistingPreset_CreatesNewBank()
    {
        // Arrange - Load first two presets
        var baseDir = GetHardwareTestDir();
        var allFiles = Directory.GetFiles(baseDir, "nova-dump-*-msg*.syx")
            .Where(f => !f.Contains("182108")) // Exclude System Dump
            .OrderBy(f => f)
            .Take(2)
            .ToList();

        var sysex1 = File.ReadAllBytes(allFiles[0]);
        var preset1 = Preset.FromSysEx(sysex1).Value;

        var bank = UserBankDump.Empty();
        bank = bank.WithPreset(31, preset1).Value;

        var sysex2 = File.ReadAllBytes(allFiles[1]);
        var preset2 = Preset.FromSysEx(sysex2).Value;

        // Act - Replace preset 31 with different data
        var originalBank = bank;
        var newBank = bank.WithPreset(32, preset2).Value;

        // Assert - Original unchanged (immutability)
        originalBank.Presets[1].Should().BeNull();
        newBank.Presets[1].Should().NotBeNull();
        newBank.Presets[1]!.Number.Should().Be(32);

        // First preset still intact in both banks
        originalBank.Presets[0].Should().BeSameAs(preset1);
        newBank.Presets[0].Should().BeSameAs(preset1);
    }

    private static string GetHardwareTestDir()
    {
        var baseDir = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "..", "Nova.HardwareTest");
        return Path.GetFullPath(baseDir);
    }
}
