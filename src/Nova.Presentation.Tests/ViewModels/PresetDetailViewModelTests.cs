using FluentAssertions;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Nova.Presentation.ViewModels;
using Xunit;

namespace Nova.Presentation.Tests.ViewModels;

public class PresetDetailViewModelTests
{
    [Fact]
    public void InitialState_AllPropertiesAreDefault()
    {
        // Arrange & Act
        var vm = CreateViewModel();

        // Assert
        vm.PresetNumber.Should().Be(0);
        vm.PresetName.Should().BeEmpty();
        vm.TapTempo.Should().Be(0);
        vm.Routing.Should().Be(0);
        vm.LevelOutLeft.Should().Be(0);
        vm.LevelOutRight.Should().Be(0);
        
        // Verify effect switches are false by default
        vm.Compressor.IsEnabled.Should().BeFalse();
        vm.Drive.IsEnabled.Should().BeFalse();
        vm.Modulation.IsEnabled.Should().BeFalse();
        vm.Delay.IsEnabled.Should().BeFalse();
        vm.Reverb.IsEnabled.Should().BeFalse();
    }

    [Fact]
    public void LoadFromPreset_WithValidPreset_PopulatesAllProperties()
    {
        // Arrange
        var vm = CreateViewModel();
        var sysex = CreateValidPresetSysEx(
            presetNumber: 31,
            presetName: "Test Preset"
        );
        var preset = Preset.FromSysEx(sysex).Value;

        // Act
        vm.LoadFromPreset(preset);

        // Assert - Basic properties
        vm.PresetNumber.Should().Be(31);
        vm.PresetName.Should().Be("Test Preset");
        
        // Assert - Global parameters (should be clamped to valid ranges)
        vm.TapTempo.Should().Be(Math.Clamp(preset.TapTempo, 0, 255));
        vm.Routing.Should().Be(Math.Clamp(preset.Routing, 0, 7));
        vm.LevelOutLeft.Should().Be(Math.Clamp(preset.LevelOutLeft, -20, 20));
        vm.LevelOutRight.Should().Be(Math.Clamp(preset.LevelOutRight, -20, 20));
        
        // Assert - Effect switches
        vm.Compressor.IsEnabled.Should().Be(preset.CompressorEnabled);
        vm.Drive.IsEnabled.Should().Be(preset.DriveEnabled);
        vm.Modulation.IsEnabled.Should().Be(preset.ModulationEnabled);
        vm.Delay.IsEnabled.Should().Be(preset.DelayEnabled);
        vm.Reverb.IsEnabled.Should().Be(preset.ReverbEnabled);
        
        // Assert - Compressor parameters (subset available in CompressorBlockViewModel)
        vm.Compressor.Type.Should().Be(preset.CompType switch
        {
            0 => "Percussive",
            1 => "Sustaining",
            2 => "Advanced",
            _ => "Unknown"
        });
        vm.Compressor.Drive.Should().Be(Math.Clamp(preset.CompDrive, 1, 20));
        vm.Compressor.Response.Should().Be(Math.Clamp(preset.CompResponse, 1, 10));
        vm.Compressor.Level.Should().Be(Math.Clamp(preset.CompLevel, -12, 12));
        
        // Assert - Drive parameters (subset available in DriveBlockViewModel)
        vm.Drive.Type.Should().Be(preset.DriveType switch
        {
            0 => "Overdrive",
            1 => "Distortion",
            _ => "Unknown"
        });
        vm.Drive.Gain.Should().Be(Math.Clamp(preset.DriveGain, 0, 100));
        vm.Drive.Level.Should().Be(Math.Clamp(preset.DriveLevel, -100, 0));
        
        // Assert - Modulation parameters (subset available in ModulationBlockViewModel)
        vm.Modulation.Type.Should().Be(preset.ModType switch
        {
            0 => "Chorus",
            1 => "Flanger",
            2 => "Vibrato",
            3 => "Phaser",
            4 => "Tremolo",
            5 => "Panner",
            _ => "Unknown"
        });
        vm.Modulation.Speed.Should().Be(Math.Clamp(preset.ModSpeed, 0, 81));
        vm.Modulation.Depth.Should().Be(Math.Clamp(preset.ModDepth, 0, 100));
        vm.Modulation.Mix.Should().Be(Math.Clamp(preset.ModMix, 0, 100));
        
        // Assert - Delay parameters (subset available in DelayBlockViewModel)
        vm.Delay.Type.Should().Be(preset.DelayType switch
        {
            0 => "Clean",
            1 => "Analog",
            2 => "Tape",
            3 => "Dynamic",
            4 => "Dual",
            5 => "Ping-Pong",
            _ => "Unknown"
        });
        vm.Delay.Time.Should().BeInRange(0, 2000); // Clamped
        vm.Delay.Feedback.Should().BeInRange(0, 100); // Clamped
        vm.Delay.Mix.Should().BeInRange(0, 100); // Clamped
        
        // Assert - Reverb parameters (subset available in ReverbBlockViewModel)
        vm.Reverb.Type.Should().Be(preset.ReverbType switch
        {
            0 => "Spring",
            1 => "Hall",
            2 => "Room",
            3 => "Plate",
            _ => "Unknown"
        });
        vm.Reverb.Decay.Should().BeInRange(0, 100); // Clamped
        vm.Reverb.PreDelay.Should().BeInRange(0, 200); // Clamped
        vm.Reverb.Level.Should().BeInRange(0, 100); // Clamped
        
        // Assert - Gate parameters (subset available in EqGateBlockViewModel)
        vm.EqGate.GateThreshold.Should().Be(Math.Clamp(preset.GateThreshold, -60, 0));
        
        // Assert - EQ parameters (subset available in EqGateBlockViewModel)
        vm.EqGate.EqBand1Freq.Should().Be(preset.EqFreq1);
        vm.EqGate.EqBand1Gain.Should().Be(Math.Clamp(preset.EqGain1, -12, 12));
        vm.EqGate.EqBand2Freq.Should().Be(preset.EqFreq2);
        vm.EqGate.EqBand2Gain.Should().Be(Math.Clamp(preset.EqGain2, -12, 12));
        vm.EqGate.EqBand3Freq.Should().Be(preset.EqFreq3);
        vm.EqGate.EqBand3Gain.Should().Be(Math.Clamp(preset.EqGain3, -12, 12));
        
        // Assert - Pitch parameters (subset available in PitchBlockViewModel)
        vm.Pitch.Type.Should().BeInRange(0, 4); // Clamped
        vm.Pitch.Voice1.Should().BeInRange(-100, 100); // Clamped
        vm.Pitch.Voice2.Should().BeInRange(-100, 100); // Clamped
    }

    [Fact]
    public void LoadFromPreset_WithNull_DoesNotThrow()
    {
        // Arrange
        var vm = CreateViewModel();

        // Act
        var act = () => vm.LoadFromPreset(null);

        // Assert
        act.Should().NotThrow();
        vm.PresetNumber.Should().Be(0);
        vm.PresetName.Should().BeEmpty();
    }

    /// <summary>
    /// Helper method to create a valid 521-byte Nova System preset SysEx message
    /// </summary>
    private static byte[] CreateValidPresetSysEx(int presetNumber, string presetName)
    {
        return TestHelpers.CreateValidPresetSysEx(presetNumber, presetName);
    }

    private static PresetDetailViewModel CreateViewModel()
    {
        var savePresetUseCase = new Mock<ISavePresetUseCase>();
        return new PresetDetailViewModel(savePresetUseCase.Object);
    }
}
