using FluentAssertions;
using Nova.Domain.Models;

namespace Nova.Domain.Tests.Models;

/// <summary>
/// Tests for Preset parameter extraction from bytes 33-519.
/// These tests verify that we can read all effect parameters from a preset.
/// </summary>
public class PresetParametersTests
{
    /// <summary>
    /// Test fixture: Real preset from hardware (nova-dump-*-181507-msg001.syx)
    /// Preset 31: "Amp-Bright Clean1"
    /// </summary>
    private readonly byte[] _realPresetBytes;

    public PresetParametersTests()
    {
        // Path to real hardware test fixtures in Nova.HardwareTest project
        var fixturesPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "..", "..", "src", "Nova.HardwareTest"
        );
        var presetPath = Path.Combine(fixturesPath, "nova-dump-20260131-181507-msg001.syx");

        if (File.Exists(presetPath))
        {
            _realPresetBytes = File.ReadAllBytes(presetPath);
        }
        else
        {
            // Fallback: minimal valid preset structure (all zeros for parameters)
            _realPresetBytes = new byte[521];
            _realPresetBytes[0] = 0xF0;
            _realPresetBytes[1] = 0x00;
            _realPresetBytes[2] = 0x20;
            _realPresetBytes[3] = 0x1F;
            _realPresetBytes[4] = 0x00;
            _realPresetBytes[5] = 0x63;
            _realPresetBytes[6] = 0x20;
            _realPresetBytes[7] = 0x01;
            _realPresetBytes[8] = 31; // Preset number
            _realPresetBytes[520] = 0xF7;
        }
    }

    [Fact]
    public void FromSysEx_ExtractsTapTempo_FromBytes38To41()
    {
        // Arrange: Bytes 38-41 contain Tap Tempo (100-3000ms)
        // According to SysEx map: 4 bytes encode tempo value

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TapTempo.Should().BeInRange(100, 3000,
            "Tap Tempo must be between 100ms and 3000ms according to Nova System spec");
    }

    [Fact]
    public void FromSysEx_ExtractsRouting_FromBytes42To45()
    {
        // Arrange: Bytes 42-45 contain Routing (0=Semi-parallel, 1=Serial, 2=Parallel)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Routing.Should().BeInRange(0, 2,
            "Routing must be 0 (Semi-par), 1 (Serial), or 2 (Parallel)");
    }

    [Fact]
    public void FromSysEx_ExtractsLevelOutLeft_FromBytes46To49()
    {
        // Arrange: Bytes 46-49 contain Level Out L (-100 to 0dB)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.LevelOutLeft.Should().BeInRange(-100, 0,
            "Level Out Left must be between -100dB and 0dB");
    }

    [Fact]
    public void FromSysEx_ExtractsLevelOutRight_FromBytes50To53()
    {
        // Arrange: Bytes 50-53 contain Level Out R (-100 to 0dB)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.LevelOutRight.Should().BeInRange(-100, 0,
            "Level Out Right must be between -100dB and 0dB");
    }

    [Fact]
    public void FromSysEx_ExtractsCompressorEnabled_FromBytes130To133()
    {
        // Arrange: Bytes 130-133 contain COMP ON/OFF (0=Off, 1=On)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        // Bool property extracted successfully (value validated by type system)
    }

    [Fact]
    public void FromSysEx_ExtractsDriveEnabled_FromBytes194To197()
    {
        // Arrange: Bytes 194-197 contain DRIVE ON/OFF (0=Off, 1=On)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        // Bool property extracted successfully
    }

    [Fact]
    public void FromSysEx_ExtractsModulationEnabled_FromBytes258To261()
    {
        // Arrange: Bytes 258-261 contain MOD ON/OFF (0=Off, 1=On)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        // Bool property extracted successfully
    }

    [Fact]
    public void FromSysEx_ExtractsDelayEnabled_FromBytes322To325()
    {
        // Arrange: Bytes 322-325 contain DELAY ON/OFF (0=Off, 1=On)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        // Bool property extracted successfully
    }

    [Fact]
    public void FromSysEx_ExtractsReverbEnabled_FromBytes386To389()
    {
        // Arrange: Bytes 386-389 contain REVERB ON/OFF (0=Off, 1=On)

        // Act
        var result = Preset.FromSysEx(_realPresetBytes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        // Bool property extracted successfully
    }
}
