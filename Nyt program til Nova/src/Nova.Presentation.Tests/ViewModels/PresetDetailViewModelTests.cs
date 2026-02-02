using FluentAssertions;
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
        var vm = new PresetDetailViewModel();

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
        var vm = new PresetDetailViewModel();
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
        
        // Assert - Global parameters
        vm.TapTempo.Should().Be(preset.TapTempo);
        vm.Routing.Should().Be(preset.Routing);
        vm.LevelOutLeft.Should().Be(preset.LevelOutLeft);
        vm.LevelOutRight.Should().Be(preset.LevelOutRight);
        
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
        vm.Compressor.Drive.Should().Be(preset.CompDrive);
        vm.Compressor.Response.Should().Be(preset.CompResponse);
        vm.Compressor.Level.Should().Be(preset.CompLevel);
        
        // Assert - Drive parameters (subset available in DriveBlockViewModel)
        vm.Drive.Type.Should().Be(preset.DriveType switch
        {
            0 => "Overdrive",
            1 => "Distortion",
            2 => "Fuzz",
            3 => "Line6 Drive",
            4 => "Custom",
            5 => "Tube",
            6 => "Metal",
            _ => "Unknown"
        });
        vm.Drive.Gain.Should().Be(preset.DriveGain);
        vm.Drive.Level.Should().Be(preset.DriveLevel);
        
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
        vm.Modulation.Speed.Should().Be(preset.ModSpeed);
        vm.Modulation.Depth.Should().Be(preset.ModDepth);
        vm.Modulation.Mix.Should().Be(preset.ModMix);
        
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
        vm.EqGate.GateThreshold.Should().Be(-60);
        
        // Assert - EQ parameters (subset available in EqGateBlockViewModel)
        vm.EqGate.EqBand1Freq.Should().Be(100);
        vm.EqGate.EqBand1Gain.Should().Be(0);
        vm.EqGate.EqBand2Freq.Should().Be(1000);
        vm.EqGate.EqBand2Gain.Should().Be(0);
        vm.EqGate.EqBand3Freq.Should().Be(5000);
        vm.EqGate.EqBand3Gain.Should().Be(0);
        
        // Assert - Pitch parameters (subset available in PitchBlockViewModel)
        vm.Pitch.Type.Should().BeInRange(0, 4); // Clamped
        vm.Pitch.Voice1.Should().BeInRange(-100, 100); // Clamped
        vm.Pitch.Voice2.Should().BeInRange(-100, 100); // Clamped
    }

    [Fact]
    public void LoadFromPreset_WithNull_DoesNotThrow()
    {
        // Arrange
        var vm = new PresetDetailViewModel();

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
        var sysex = new byte[521];
        
        // Header
        sysex[0] = 0xF0;  // SysEx start
        sysex[1] = 0x00;  // TC Electronic manufacturer ID
        sysex[2] = 0x20;
        sysex[3] = 0x1F;
        sysex[4] = 0x00;  // Device ID
        sysex[5] = 0x63;  // Nova System model ID
        sysex[6] = 0x20;  // Message ID (Dump)
        sysex[7] = 0x01;  // Data type (Preset)
        sysex[8] = (byte)presetNumber;
        
        // Preset name (bytes 9-32, 24 ASCII chars)
        var nameBytes = System.Text.Encoding.ASCII.GetBytes(presetName.PadRight(24));
        Array.Copy(nameBytes, 0, sysex, 9, 24);
        
        // Set some default parameter values using 4-byte encoding
        // TapTempo = 500ms (bytes 38-41)
        Encode4ByteValue(sysex, 38, 500);
        
        // Routing = 0 (Semi-parallel) (bytes 42-45)
        Encode4ByteValue(sysex, 42, 0);
        
        // LevelOutLeft = -10dB (bytes 46-49)
        EncodeSignedDbValue(sysex, 46, -10);
        
        // LevelOutRight = -10dB (bytes 50-53)
        EncodeSignedDbValue(sysex, 50, -10);
        
        // Enable some effects
        Encode4ByteValue(sysex, 130, 1); // Compressor enabled
        Encode4ByteValue(sysex, 194, 0); // Drive disabled
        Encode4ByteValue(sysex, 258, 1); // Modulation enabled
        Encode4ByteValue(sysex, 322, 1); // Delay enabled
        Encode4ByteValue(sysex, 386, 0); // Reverb disabled
        
        // Footer
        sysex[520] = 0xF7;  // SysEx end
        
        return sysex;
    }

    private static void Encode4ByteValue(byte[] sysex, int offset, int value)
    {
        sysex[offset] = (byte)(value & 0x7F);
        sysex[offset + 1] = (byte)((value >> 7) & 0x7F);
        sysex[offset + 2] = (byte)((value >> 14) & 0x7F);
        sysex[offset + 3] = (byte)((value >> 21) & 0x7F);
    }

    private static void EncodeSignedDbValue(byte[] sysex, int offset, int value)
    {
        const int DB_ENCODING_OFFSET = 16777216; // 2^24 - offset used for encoding signed dB values
        int encoded = value + DB_ENCODING_OFFSET;
        Encode4ByteValue(sysex, offset, encoded);
    }
}
