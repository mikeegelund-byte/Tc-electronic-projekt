using FluentAssertions;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Nova.Presentation.ViewModels;
using Serilog;
using Xunit;

namespace Nova.Presentation.Tests.ViewModels;

/// <summary>
/// Unit tests for EditablePresetViewModel.
/// Tests observable property behavior, change tracking, and save/revert commands.
/// </summary>
public class EditablePresetViewModelTests
{
    private readonly Mock<IUpdatePresetUseCase> _mockUpdateUseCase;
    private readonly Mock<ILogger> _mockLogger;
    private readonly EditablePresetViewModel _viewModel;

    public EditablePresetViewModelTests()
    {
        _mockUpdateUseCase = new Mock<IUpdatePresetUseCase>();
        _mockLogger = new Mock<ILogger>();
        _viewModel = new EditablePresetViewModel(_mockUpdateUseCase.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_InitializesDefaultValues()
    {
        // Arrange & Act
        var vm = new EditablePresetViewModel(_mockUpdateUseCase.Object);

        // Assert
        vm.HasChanges.Should().BeFalse();
        vm.StatusMessage.Should().Contain("Ready");
        vm.PresetNumber.Should().Be(0);
        vm.TapTempo.Should().Be(120);
        vm.Routing.Should().Be(0);
    }

    [Fact]
    public void LoadPreset_LoadsAllProperties()
    {
        // Arrange
        var testSysEx = new byte[521];
        testSysEx[0] = 0xF0;
        testSysEx[520] = 0xF7;
        testSysEx[1] = 0x00;
        testSysEx[2] = 0x20;
        testSysEx[3] = 0x1F;
        testSysEx[5] = 0x63;
        testSysEx[6] = 0x20;
        testSysEx[7] = 0x01;
        testSysEx[8] = 5;
        
        var nameBytes = System.Text.Encoding.ASCII.GetBytes("TestPreset".PadRight(24));
        Array.Copy(nameBytes, 0, testSysEx, 9, 24);

        var parseResult = Preset.FromSysEx(testSysEx);
        var preset = parseResult.Value;

        // Act
        _viewModel.LoadPreset(preset);

        // Assert
        _viewModel.PresetNumber.Should().Be(preset.Number);
        _viewModel.PresetName.Should().Be(preset.Name);
        _viewModel.HasChanges.Should().BeFalse();
        _viewModel.StatusMessage.Should().Contain("Loaded");
    }

    [Fact]
    public void PropertyChange_SetsHasChanges()
    {
        // Arrange
        var testSysEx = CreateValidSysEx();
        var preset = Preset.FromSysEx(testSysEx).Value;
        _viewModel.LoadPreset(preset);
        _viewModel.HasChanges.Should().BeFalse();

        // Act
        _viewModel.PresetName = "Modified";

        // Assert
        _viewModel.HasChanges.Should().BeTrue();
    }

    [Fact]
    public void TapTempoChange_SetsHasChanges()
    {
        // Arrange
        var testSysEx = CreateValidSysEx();
        var preset = Preset.FromSysEx(testSysEx).Value;
        _viewModel.LoadPreset(preset);
        _viewModel.HasChanges = false;

        // Act
        _viewModel.TapTempo = 200;

        // Assert
        _viewModel.HasChanges.Should().BeTrue();
    }

    [Fact]
    public void RoutingChange_SetsHasChanges()
    {
        // Arrange
        var testSysEx = CreateValidSysEx();
        var preset = Preset.FromSysEx(testSysEx).Value;
        _viewModel.LoadPreset(preset);
        _viewModel.HasChanges = false;

        // Act
        _viewModel.Routing = 2;

        // Assert
        _viewModel.HasChanges.Should().BeTrue();
    }

    [Fact]
    public void CompressorEnabledChange_SetsHasChanges()
    {
        // Arrange
        var testSysEx = CreateValidSysEx();
        var preset = Preset.FromSysEx(testSysEx).Value;
        _viewModel.LoadPreset(preset);
        var originalValue = _viewModel.CompressorEnabled;

        // Act
        _viewModel.CompressorEnabled = !originalValue;

        // Assert
        _viewModel.HasChanges.Should().BeTrue();
    }

    [Fact]
    public void ReverbTypeChange_SetsHasChanges()
    {
        // Arrange
        var testSysEx = CreateValidSysEx();
        var preset = Preset.FromSysEx(testSysEx).Value;
        _viewModel.LoadPreset(preset);
        _viewModel.HasChanges = false;

        // Act
        _viewModel.ReverbType = 2;

        // Assert
        _viewModel.HasChanges.Should().BeTrue();
    }

    [Fact]
    public async Task SaveCommand_WithNoPreset_ShowsError()
    {
        // Arrange & Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        _viewModel.StatusMessage.Should().Contain("Error");
    }

    [Fact]
    public async Task SaveCommand_WithNoChanges_ShowsMessage()
    {
        // Arrange
        var testSysEx = CreateValidSysEx();
        var preset = Preset.FromSysEx(testSysEx).Value;
        _viewModel.LoadPreset(preset);

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        _viewModel.StatusMessage.Should().Contain("No changes");
    }

    [Fact]
    public async Task SaveCommand_WithInvalidName_ShowsError()
    {
        // Arrange
        var testSysEx = CreateValidSysEx();
        var preset = Preset.FromSysEx(testSysEx).Value;
        _viewModel.LoadPreset(preset);
        _viewModel.PresetName = new string('A', 25); // Too long

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        _viewModel.StatusMessage.Should().Contain("Error");
    }

    [Fact]
    public void RevertCommand_ReloadsOriginalPreset()
    {
        // Arrange
        var testSysEx = CreateValidSysEx();
        var preset = Preset.FromSysEx(testSysEx).Value;
        _viewModel.LoadPreset(preset);
        var originalName = _viewModel.PresetName;
        _viewModel.PresetName = "Modified";
        _viewModel.HasChanges.Should().BeTrue();

        // Act
        _viewModel.RevertCommand.Execute(null);

        // Assert
        _viewModel.PresetName.Should().Be(originalName);
        _viewModel.HasChanges.Should().BeFalse();
    }

    [Fact]
    public void LoadPreset_WithNull_ShowsError()
    {
        // Act
        _viewModel.LoadPreset(null!);

        // Assert
        _viewModel.StatusMessage.Should().Contain("Error");
    }

    [Fact]
    public void AllEffectPropertiesLoadCorrectly()
    {
        // Arrange
        var testSysEx = CreateValidSysEx();
        var preset = Preset.FromSysEx(testSysEx).Value;

        // Act
        _viewModel.LoadPreset(preset);

        // Assert
        _viewModel.CompressorEnabled.Should().Be(preset.CompressorEnabled);
        _viewModel.DriveEnabled.Should().Be(preset.DriveEnabled);
        _viewModel.ModulationEnabled.Should().Be(preset.ModulationEnabled);
        _viewModel.DelayEnabled.Should().Be(preset.DelayEnabled);
        _viewModel.ReverbEnabled.Should().Be(preset.ReverbEnabled);
    }

    /// <summary>
    /// Creates a minimal valid SysEx message for testing.
    /// </summary>
    private static byte[] CreateValidSysEx()
    {
        var sysex = new byte[521];
        sysex[0] = 0xF0;  // Start
        sysex[520] = 0xF7;  // End
        sysex[1] = 0x00;  // TC Electronic manufacturer
        sysex[2] = 0x20;
        sysex[3] = 0x1F;
        sysex[5] = 0x63;  // Nova System model
        sysex[6] = 0x20;  // Dump message
        sysex[7] = 0x01;  // Preset data type
        sysex[8] = 5;     // Preset number

        // Add valid preset name
        var nameBytes = System.Text.Encoding.ASCII.GetBytes("TestPreset".PadRight(24));
        Array.Copy(nameBytes, 0, sysex, 9, 24);

        return sysex;
    }
}
