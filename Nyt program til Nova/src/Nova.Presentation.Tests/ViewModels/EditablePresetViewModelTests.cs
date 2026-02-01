using FluentAssertions;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Nova.Presentation.ViewModels;
using Xunit;

namespace Nova.Presentation.Tests.ViewModels;

public class EditablePresetViewModelTests
{
    private readonly byte[] _validSysEx;

    public EditablePresetViewModelTests()
    {
        // Create a valid 521-byte SysEx message for testing
        _validSysEx = new byte[521];
        _validSysEx[0] = 0xF0;  // SysEx start
        _validSysEx[1] = 0x00;  // TC Electronic manufacturer ID
        _validSysEx[2] = 0x20;
        _validSysEx[3] = 0x1F;
        _validSysEx[5] = 0x63;  // Nova System model ID
        _validSysEx[6] = 0x20;  // Dump message
        _validSysEx[7] = 0x01;  // Preset data type
        _validSysEx[8] = 0x05;  // Preset number
        
        // Preset name at bytes 9-32 (24 characters)
        var name = "Test Preset         ";
        for (int i = 0; i < 24 && i < name.Length; i++)
            _validSysEx[9 + i] = (byte)name[i];
        
        _validSysEx[520] = 0xF7;  // SysEx end
        
        // Set TapTempo to 150 (bytes 38-41) using 4-byte little-endian encoding
        _validSysEx[38] = 0x16;  // LSB: 150 & 0x7F = 0x16
        _validSysEx[39] = 0x01;  // 150 >> 7 = 0x01
        _validSysEx[40] = 0x00;
        _validSysEx[41] = 0x00;  // MSB
    }

    [Fact]
    public void LoadFromPreset_LoadsAllProperties()
    {
        // Arrange
        var viewModel = new EditablePresetViewModel();
        var preset = Preset.FromSysEx(_validSysEx).Value;

        // Act
        viewModel.LoadFromPreset(preset);

        // Assert
        viewModel.PresetNumber.Should().Be(5);
        viewModel.PresetName.Should().Be(preset.Name);  // Use actual trimmed name from preset
        viewModel.TapTempo.Should().Be(150);
    }

    [Fact]
    public void LoadFromPreset_WithNullPreset_ThrowsArgumentNullException()
    {
        // Arrange
        var viewModel = new EditablePresetViewModel();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => viewModel.LoadFromPreset(null!));
    }

    [Fact]
    public void HasChanges_InitiallyFalse()
    {
        // Arrange
        var viewModel = new EditablePresetViewModel();
        var preset = Preset.FromSysEx(_validSysEx).Value;
        viewModel.LoadFromPreset(preset);

        // Act & Assert
        viewModel.HasChanges.Should().BeFalse();
    }

    [Fact]
    public void HasChanges_TrueAfterModifyingProperty()
    {
        // Arrange
        var viewModel = new EditablePresetViewModel();
        var preset = Preset.FromSysEx(_validSysEx).Value;
        viewModel.LoadFromPreset(preset);

        // Act
        viewModel.PresetName = "Modified Name";

        // Assert
        viewModel.HasChanges.Should().BeTrue();
    }

    [Fact]
    public void RevertChanges_RestoresOriginalValues()
    {
        // Arrange
        var viewModel = new EditablePresetViewModel();
        var preset = Preset.FromSysEx(_validSysEx).Value;
        viewModel.LoadFromPreset(preset);
        var originalName = viewModel.PresetName;
        viewModel.PresetName = "Modified Name";

        // Act
        viewModel.RevertChangesCommand.Execute(null);

        // Assert
        viewModel.PresetName.Should().Be(originalName);
        viewModel.HasChanges.Should().BeFalse();
    }

    [Fact]
    public void ToPreset_WithEmptyName_ReturnsFailure()
    {
        // Arrange
        var viewModel = new EditablePresetViewModel();
        var preset = Preset.FromSysEx(_validSysEx).Value;
        viewModel.LoadFromPreset(preset);
        viewModel.PresetName = "";

        // Act
        var result = viewModel.ToPreset();

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Preset name cannot be empty");
        viewModel.ValidationError.Should().Contain("Preset name cannot be empty");
    }

    [Fact]
    public void ToPreset_WithNameTooLong_ReturnsFailure()
    {
        // Arrange
        var viewModel = new EditablePresetViewModel();
        var preset = Preset.FromSysEx(_validSysEx).Value;
        viewModel.LoadFromPreset(preset);
        viewModel.PresetName = "This name is way too long for a preset and exceeds 24 characters";

        // Act
        var result = viewModel.ToPreset();

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("cannot exceed 24 characters");
    }

    [Fact]
    public void ToPreset_WithInvalidTapTempo_ReturnsFailure()
    {
        // Arrange
        var viewModel = new EditablePresetViewModel();
        var preset = Preset.FromSysEx(_validSysEx).Value;
        viewModel.LoadFromPreset(preset);
        viewModel.TapTempo = 50;  // Too low (should be 100-3000)

        // Act
        var result = viewModel.ToPreset();

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Tap tempo must be between");
    }

    [Fact]
    public void ToPreset_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var viewModel = new EditablePresetViewModel();
        var preset = Preset.FromSysEx(_validSysEx).Value;
        viewModel.LoadFromPreset(preset);

        // Act
        var result = viewModel.ToPreset();

        // Assert
        result.IsSuccess.Should().BeTrue();
        viewModel.ValidationError.Should().BeEmpty();
    }

    [Fact]
    public void HasChanges_TracksMultipleProperties()
    {
        // Arrange
        var viewModel = new EditablePresetViewModel();
        var preset = Preset.FromSysEx(_validSysEx).Value;
        viewModel.LoadFromPreset(preset);

        // Act
        viewModel.TapTempo = 200;
        viewModel.Routing = 1;
        viewModel.CompressorEnabled = !viewModel.CompressorEnabled;

        // Assert
        viewModel.HasChanges.Should().BeTrue();
    }
}
