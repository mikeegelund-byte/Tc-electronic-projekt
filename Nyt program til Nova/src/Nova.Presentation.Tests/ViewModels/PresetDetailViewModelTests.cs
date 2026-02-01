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
        vm.CompressorEnabled.Should().BeFalse();
        vm.DriveEnabled.Should().BeFalse();
        vm.ModulationEnabled.Should().BeFalse();
        vm.DelayEnabled.Should().BeFalse();
        vm.ReverbEnabled.Should().BeFalse();
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
        vm.CompressorEnabled.Should().Be(preset.CompressorEnabled);
        vm.DriveEnabled.Should().Be(preset.DriveEnabled);
        vm.ModulationEnabled.Should().Be(preset.ModulationEnabled);
        vm.DelayEnabled.Should().Be(preset.DelayEnabled);
        vm.ReverbEnabled.Should().Be(preset.ReverbEnabled);
        
        // Assert - Compressor parameters
        vm.CompType.Should().Be(preset.CompType);
        vm.CompThreshold.Should().Be(preset.CompThreshold);
        vm.CompRatio.Should().Be(preset.CompRatio);
        vm.CompAttack.Should().Be(preset.CompAttack);
        vm.CompRelease.Should().Be(preset.CompRelease);
        vm.CompResponse.Should().Be(preset.CompResponse);
        vm.CompDrive.Should().Be(preset.CompDrive);
        vm.CompLevel.Should().Be(preset.CompLevel);
        
        // Assert - Drive parameters
        vm.DriveType.Should().Be(preset.DriveType);
        vm.DriveGain.Should().Be(preset.DriveGain);
        vm.DriveLevel.Should().Be(preset.DriveLevel);
        
        // Assert - Boost parameters
        vm.BoostType.Should().Be(preset.BoostType);
        vm.BoostGain.Should().Be(preset.BoostGain);
        vm.BoostLevel.Should().Be(preset.BoostLevel);
        
        // Assert - Modulation parameters
        vm.ModType.Should().Be(preset.ModType);
        vm.ModSpeed.Should().Be(preset.ModSpeed);
        vm.ModDepth.Should().Be(preset.ModDepth);
        vm.ModTempo.Should().Be(preset.ModTempo);
        vm.ModHiCut.Should().Be(preset.ModHiCut);
        vm.ModFeedback.Should().Be(preset.ModFeedback);
        vm.ModDelayOrRange.Should().Be(preset.ModDelayOrRange);
        vm.ModMix.Should().Be(preset.ModMix);
        
        // Assert - Delay parameters
        vm.DelayType.Should().Be(preset.DelayType);
        vm.DelayTime.Should().Be(preset.DelayTime);
        vm.DelayTime2.Should().Be(preset.DelayTime2);
        vm.DelayTempo.Should().Be(preset.DelayTempo);
        vm.DelayTempo2OrWidth.Should().Be(preset.DelayTempo2OrWidth);
        vm.DelayFeedback.Should().Be(preset.DelayFeedback);
        vm.DelayClipOrFeedback2.Should().Be(preset.DelayClipOrFeedback2);
        vm.DelayHiCut.Should().Be(preset.DelayHiCut);
        vm.DelayLoCut.Should().Be(preset.DelayLoCut);
        vm.DelayMix.Should().Be(preset.DelayMix);
        
        // Assert - Reverb parameters
        vm.ReverbType.Should().Be(preset.ReverbType);
        vm.ReverbDecay.Should().Be(preset.ReverbDecay);
        vm.ReverbPreDelay.Should().Be(preset.ReverbPreDelay);
        vm.ReverbShape.Should().Be(preset.ReverbShape);
        vm.ReverbSize.Should().Be(preset.ReverbSize);
        vm.ReverbHiColor.Should().Be(preset.ReverbHiColor);
        vm.ReverbHiLevel.Should().Be(preset.ReverbHiLevel);
        vm.ReverbLoColor.Should().Be(preset.ReverbLoColor);
        vm.ReverbLoLevel.Should().Be(preset.ReverbLoLevel);
        vm.ReverbRoomLevel.Should().Be(preset.ReverbRoomLevel);
        vm.ReverbLevel.Should().Be(preset.ReverbLevel);
        vm.ReverbDiffuse.Should().Be(preset.ReverbDiffuse);
        vm.ReverbMix.Should().Be(preset.ReverbMix);
        
        // Assert - Gate parameters
        vm.GateType.Should().Be(preset.GateType);
        vm.GateThreshold.Should().Be(preset.GateThreshold);
        vm.GateDamp.Should().Be(preset.GateDamp);
        vm.GateRelease.Should().Be(preset.GateRelease);
        
        // Assert - EQ parameters
        vm.EqFreq1.Should().Be(preset.EqFreq1);
        vm.EqGain1.Should().Be(preset.EqGain1);
        vm.EqWidth1.Should().Be(preset.EqWidth1);
        vm.EqFreq2.Should().Be(preset.EqFreq2);
        vm.EqGain2.Should().Be(preset.EqGain2);
        vm.EqWidth2.Should().Be(preset.EqWidth2);
        vm.EqFreq3.Should().Be(preset.EqFreq3);
        vm.EqGain3.Should().Be(preset.EqGain3);
        vm.EqWidth3.Should().Be(preset.EqWidth3);
        
        // Assert - Pitch parameters
        vm.PitchType.Should().Be(preset.PitchType);
        vm.PitchVoice1.Should().Be(preset.PitchVoice1);
        vm.PitchVoice2.Should().Be(preset.PitchVoice2);
        vm.PitchPan1.Should().Be(preset.PitchPan1);
        vm.PitchPan2.Should().Be(preset.PitchPan2);
        vm.PitchDelay1.Should().Be(preset.PitchDelay1);
        vm.PitchDelay2.Should().Be(preset.PitchDelay2);
        vm.PitchFeedback1OrKey.Should().Be(preset.PitchFeedback1OrKey);
        vm.PitchFeedback2OrScale.Should().Be(preset.PitchFeedback2OrScale);
        vm.PitchLevel1.Should().Be(preset.PitchLevel1);
        vm.PitchLevel2.Should().Be(preset.PitchLevel2);
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
