using FluentAssertions;
using Nova.Domain.Models;
using Nova.Presentation.ViewModels.Effects;

namespace Nova.Presentation.Tests.ViewModels.Effects;

public class DriveBlockViewModelTests
{
    [Fact]
    public void LoadFromPreset_WithOverdriveType_SetsTypeToOverdrive()
    {
        // Arrange
        var preset = CreatePresetWithDrive(driveType: 0); // Overdrive
        var vm = new DriveBlockViewModel();

        // Act
        vm.LoadFromPreset(preset);

        // Assert
        vm.Type.Should().Be("Overdrive");
        vm.IsEnabled.Should().BeTrue();
    }

    [Fact]
    public void LoadFromPreset_WithDistortionType_SetsTypeToDistortion()
    {
        // Arrange
        var preset = CreatePresetWithDrive(driveType: 1); // Distortion
        var vm = new DriveBlockViewModel();

        // Act
        vm.LoadFromPreset(preset);

        // Assert
        vm.Type.Should().Be("Distortion");
    }

    [Fact]
    public void LoadFromPreset_LoadsAllParameters()
    {
        // Arrange
        var preset = CreatePresetWithDrive(driveType: 0, gain: 75, level: -10);
        var vm = new DriveBlockViewModel();

        // Act
        vm.LoadFromPreset(preset);

        // Assert
        vm.Gain.Should().Be(75);
        vm.Level.Should().Be(-10);
    }

    [Fact]
    public void LoadFromPreset_WithNullPreset_DoesNotThrow()
    {
        // Arrange
        var vm = new DriveBlockViewModel();

        // Act
        var action = () => vm.LoadFromPreset(null!);

        // Assert
        action.Should().NotThrow();
    }

    private static Preset CreatePresetWithDrive(int driveType, int gain = 50, int level = 0)
    {
        var sysex = new byte[521];
        
        // Standard header
        sysex[0] = 0xF0; // Start
        sysex[1] = 0x00; // Manufacturer (TC Electronic)
        sysex[2] = 0x20;
        sysex[3] = 0x1F;
        sysex[4] = 0x00; // Device ID
        sysex[5] = 0x63; // Product: NOVA SYSTEM
        sysex[6] = 0x20; // Message ID: Dump
        sysex[7] = 0x01; // Data Type: Single Preset
        
        // Preset number (byte 8)
        sysex[8] = 0x00;
        
        // Preset name (bytes 9-24: "Test Preset     ")
        var nameBytes = System.Text.Encoding.ASCII.GetBytes("Test Preset         ".PadRight(24));
        Array.Copy(nameBytes, 0, sysex, 9, 24);

        // Set minimum valid parameter values (validated ranges)
        Encode4ByteValue(sysex, 38, 500);   // TapTempo: 500ms (100-3000)
        Encode4ByteValue(sysex, 86, 15);    // CompRelease: 15 (13-23)
        Encode4ByteValue(sysex, 330, 50);   // ReverbDecay: 50 (1-200)
        Encode4ByteValue(sysex, 418, 8);    // EqWidth1: 8 (5-12)
        Encode4ByteValue(sysex, 430, 8);    // EqWidth2: 8 (5-12)
        Encode4ByteValue(sysex, 442, 8);    // EqWidth3: 8 (5-12)

        // Drive parameters using 4-byte encoding
        Encode4ByteValue(sysex, 102, driveType); // DriveType
        Encode4ByteValue(sysex, 106, gain); // DriveGain
        
        // Drive Level -30 to +20dB - needs signed encoding
        const int LARGE_OFFSET = 16777216; // 2^24
        int levelEncoded = level + LARGE_OFFSET;
        Encode4ByteValue(sysex, 110, levelEncoded); // DriveLevel
        
        Encode4ByteValue(sysex, 194, 1); // DriveEnabled
        
        // Fill remaining bytes with zeros
        for (int i = 33; i < 520; i++)
        {
            if (sysex[i] == 0) // Don't overwrite already set bytes
                sysex[i] = 0x00;
        }
        
        sysex[520] = 0xF7; // End
        
        return Preset.FromSysEx(sysex).Value;
    }

    /// <summary>
    /// Encodes a 4-byte value into Nova System SysEx format (little-endian, 7-bit per byte).
    /// </summary>
    private static void Encode4ByteValue(byte[] sysex, int offset, int value)
    {
        sysex[offset] = (byte)(value & 0x7F);         // LSB (bits 0-6)
        sysex[offset + 1] = (byte)((value >> 7) & 0x7F);  // Bits 7-13
        sysex[offset + 2] = (byte)((value >> 14) & 0x7F); // Bits 14-20
        sysex[offset + 3] = (byte)((value >> 21) & 0x7F); // MSB (bits 21-27)
    }
}
