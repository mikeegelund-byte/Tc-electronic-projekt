using FluentAssertions;
using Nova.Domain.Models;
using Nova.Presentation.ViewModels;
using Xunit;

namespace Nova.Presentation.Tests.ViewModels;

/// <summary>
/// Tests for PresetDetailViewModel composition ViewModel.
/// Validates that LoadFromPreset() delegates to all 7 effect block ViewModels.
/// </summary>
public class PresetDetailViewModelTests
{
    [Fact]
    public void Constructor_InitializesAllEffectBlocks()
    {
        // Arrange & Act
        var vm = new PresetDetailViewModel();

        // Assert - All effect blocks should be initialized
        vm.Drive.Should().NotBeNull();
        vm.Compressor.Should().NotBeNull();
        vm.EqGate.Should().NotBeNull();
        vm.Modulation.Should().NotBeNull();
        vm.Pitch.Should().NotBeNull();
        vm.Delay.Should().NotBeNull();
        vm.Reverb.Should().NotBeNull();
    }

    [Fact]
    public void InitialState_HasEmptyPresetName()
    {
        // Arrange & Act
        var vm = new PresetDetailViewModel();

        // Assert
        vm.PresetName.Should().BeEmpty();
        vm.Position.Should().BeEmpty();
    }

    [Fact]
    public void LoadFromPreset_WithValidPreset_PopulatesBasicProperties()
    {
        // Arrange
        var vm = new PresetDetailViewModel();
        var sysex = CreateValidPresetSysEx(
            presetNumber: 31,
            presetName: "Test Preset"
        );
        var preset = Preset.FromSysEx(sysex).Value;

        // Act
        vm.LoadFromPreset(preset);

        // Assert
        vm.PresetName.Should().Be("Test Preset");
        vm.Position.Should().Be("#31");
    }

    [Fact]
    public void LoadFromPreset_WithValidPreset_LoadsDriveBlock()
    {
        // Arrange
        var vm = new PresetDetailViewModel();
        var sysex = CreateValidPresetSysEx(
            presetNumber: 1,
            presetName: "Drive Test"
        );
        var preset = Preset.FromSysEx(sysex).Value;

        // Act
        vm.LoadFromPreset(preset);

        // Assert - Drive block should be populated
        vm.Drive.Type.Should().NotBeEmpty();
        vm.Drive.Gain.Should().Be(preset.DriveGain);
        vm.Drive.Level.Should().Be(preset.DriveLevel);
        vm.Drive.IsEnabled.Should().Be(preset.DriveEnabled);
    }

    [Fact]
    public void LoadFromPreset_WithValidPreset_LoadsCompressorBlock()
    {
        // Arrange
        var vm = new PresetDetailViewModel();
        var sysex = CreateValidPresetSysEx(
            presetNumber: 1,
            presetName: "Compressor Test"
        );
        var preset = Preset.FromSysEx(sysex).Value;

        // Act
        vm.LoadFromPreset(preset);

        // Assert - Compressor block should be populated
        vm.Compressor.Type.Should().NotBeEmpty();
        vm.Compressor.Drive.Should().Be(preset.CompDrive);
        vm.Compressor.Response.Should().Be(preset.CompResponse);
        vm.Compressor.Level.Should().Be(preset.CompLevel);
        vm.Compressor.IsEnabled.Should().Be(preset.CompressorEnabled);
    }

    [Fact]
    public void LoadFromPreset_WithValidPreset_LoadsModulationBlock()
    {
        // Arrange
        var vm = new PresetDetailViewModel();
        var sysex = CreateValidPresetSysEx(
            presetNumber: 1,
            presetName: "Modulation Test"
        );
        var preset = Preset.FromSysEx(sysex).Value;

        // Act
        vm.LoadFromPreset(preset);

        // Assert - Modulation block should be populated
        vm.Modulation.Type.Should().NotBeEmpty();
        vm.Modulation.Speed.Should().Be(preset.ModSpeed);
        vm.Modulation.Depth.Should().Be(preset.ModDepth);
        vm.Modulation.Mix.Should().Be(preset.ModMix);
        vm.Modulation.IsEnabled.Should().Be(preset.ModulationEnabled);
    }

    [Fact]
    public void LoadFromPreset_WithValidPreset_LoadsDelayBlock()
    {
        // Arrange
        var vm = new PresetDetailViewModel();
        var sysex = CreateValidPresetSysEx(
            presetNumber: 1,
            presetName: "Delay Test"
        );
        var preset = Preset.FromSysEx(sysex).Value;

        // Act
        vm.LoadFromPreset(preset);

        // Assert - Delay block should be populated
        vm.Delay.Type.Should().NotBeEmpty();
        vm.Delay.Time.Should().Be(preset.DelayTime);
        vm.Delay.Feedback.Should().Be(preset.DelayFeedback);
        vm.Delay.Mix.Should().Be(preset.DelayMix);
        vm.Delay.IsEnabled.Should().Be(preset.DelayEnabled);
    }

    [Fact]
    public void LoadFromPreset_WithValidPreset_LoadsReverbBlock()
    {
        // Arrange
        var vm = new PresetDetailViewModel();
        var sysex = CreateValidPresetSysEx(
            presetNumber: 1,
            presetName: "Reverb Test"
        );
        var preset = Preset.FromSysEx(sysex).Value;

        // Act
        vm.LoadFromPreset(preset);

        // Assert - Reverb block should be populated
        vm.Reverb.Type.Should().NotBeEmpty();
        vm.Reverb.Decay.Should().Be(preset.ReverbDecay);
        vm.Reverb.PreDelay.Should().Be(preset.ReverbPreDelay);
        vm.Reverb.Level.Should().Be(preset.ReverbLevel);
        vm.Reverb.IsEnabled.Should().Be(preset.ReverbEnabled);
    }

    [Fact]
    public void LoadFromPreset_WithNull_ClearsPresetName()
    {
        // Arrange
        var vm = new PresetDetailViewModel();
        var sysex = CreateValidPresetSysEx(1, "Test");
        var preset = Preset.FromSysEx(sysex).Value;
        vm.LoadFromPreset(preset);

        // Act
        vm.LoadFromPreset(null);

        // Assert
        vm.PresetName.Should().BeEmpty();
        vm.Position.Should().BeEmpty();
    }

    private static byte[] CreateValidPresetSysEx(int presetNumber, string presetName)
    {
        var sysex = new byte[521];
        sysex[0] = 0xF0;  // SysEx start
        sysex[1] = 0x00;  // Manufacturer ID (TC Electronic)
        sysex[2] = 0x20;
        sysex[3] = 0x1F;
        sysex[4] = 0x00;  // Device ID
        sysex[5] = 0x63;  // Model ID (Nova System)
        sysex[6] = 0x20;  // Message ID (Dump)
        sysex[7] = 0x01;  // Data Type (Single Preset)
        sysex[8] = (byte)presetNumber;  // Preset number

        // Preset name (24 bytes starting at offset 9)
        var nameBytes = System.Text.Encoding.ASCII.GetBytes(presetName.PadRight(24, ' '));
        Array.Copy(nameBytes, 0, sysex, 9, Math.Min(24, nameBytes.Length));

        sysex[520] = 0xF7;  // SysEx end
        return sysex;
    }
}
