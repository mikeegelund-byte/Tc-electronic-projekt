using FluentAssertions;
using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Nova.Midi;
using Nova.Presentation.ViewModels;
using Xunit;

namespace Nova.Presentation.Tests.ViewModels;

public class EditablePresetViewModelTests
{
    private readonly UpdatePresetUseCase _updateUseCase;
    private readonly Mock<IMidiPort> _mockMidiPort;
    private readonly Mock<ILogger<EditablePresetViewModel>> _mockLogger;

    public EditablePresetViewModelTests()
    {
        _mockMidiPort = new Mock<IMidiPort>();
        _mockLogger = new Mock<ILogger<EditablePresetViewModel>>();
        _updateUseCase = new UpdatePresetUseCase();
    }

    [Fact]
    public void LoadPreset_WithValidPreset_SetsProperties()
    {
        // Arrange
        var preset = CreateValidPreset(31, "Original Name");
        var vm = CreateViewModel();

        // Act
        vm.LoadPreset(preset);

        // Assert
        vm.PresetNumber.Should().Be(31);
        vm.PresetName.Should().Be("Original Name");
        vm.HasChanges.Should().BeFalse();
    }

    [Fact]
    public void LoadPreset_WithNullPreset_ThrowsArgumentNullException()
    {
        // Arrange
        var vm = CreateViewModel();

        // Act
        var action = () => vm.LoadPreset(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void HasChanges_WhenNameModified_ReturnsTrue()
    {
        // Arrange
        var preset = CreateValidPreset(31, "Original Name");
        var vm = CreateViewModel();
        vm.LoadPreset(preset);

        // Act
        vm.PresetName = "Modified Name";

        // Assert
        vm.HasChanges.Should().BeTrue();
    }

    [Fact]
    public void HasChanges_WhenNoChanges_ReturnsFalse()
    {
        // Arrange
        var preset = CreateValidPreset(31, "Original Name");
        var vm = CreateViewModel();
        vm.LoadPreset(preset);

        // Act & Assert
        vm.HasChanges.Should().BeFalse();
    }

    [Fact]
    public void PresetName_WithEmptyValue_HasValidationError()
    {
        // Arrange
        var preset = CreateValidPreset(31, "Original Name");
        var vm = CreateViewModel();
        vm.LoadPreset(preset);

        // Act
        vm.PresetName = "";

        // Assert
        vm.HasErrors.Should().BeTrue();
        vm.GetErrors(nameof(vm.PresetName)).Should().NotBeEmpty();
    }

    [Fact]
    public void PresetName_WithTooLongValue_HasValidationError()
    {
        // Arrange
        var preset = CreateValidPreset(31, "Original Name");
        var vm = CreateViewModel();
        vm.LoadPreset(preset);

        // Act
        vm.PresetName = new string('A', 25); // 25 chars, max is 24

        // Assert
        vm.HasErrors.Should().BeTrue();
        vm.GetErrors(nameof(vm.PresetName)).Should().NotBeEmpty();
    }

    [Fact]
    public void PresetName_WithValidValue_HasNoValidationError()
    {
        // Arrange
        var preset = CreateValidPreset(31, "Original Name");
        var vm = CreateViewModel();
        vm.LoadPreset(preset);

        // Act
        vm.PresetName = "Valid Name";

        // Assert
        vm.HasErrors.Should().BeFalse();
    }

    [Fact]
    public void Volume_WithInvalidRange_HasValidationError()
    {
        // Arrange
        var preset = CreateValidPreset(31, "Test");
        var vm = CreateViewModel();
        vm.LoadPreset(preset);

        // Act
        vm.Volume = 101; // Out of range (0-100)

        // Assert
        vm.HasErrors.Should().BeTrue();
        vm.GetErrors(nameof(vm.Volume)).Should().NotBeEmpty();
    }

    [Fact]
    public void Pan_WithInvalidRange_HasValidationError()
    {
        // Arrange
        var preset = CreateValidPreset(31, "Test");
        var vm = CreateViewModel();
        vm.LoadPreset(preset);

        // Act
        vm.Pan = 51; // Out of range (-50 to 50)

        // Assert
        vm.HasErrors.Should().BeTrue();
        vm.GetErrors(nameof(vm.Pan)).Should().NotBeEmpty();
    }

    [Fact]
    public async Task SaveCommand_WithValidChanges_CallsUpdateUseCase()
    {
        // Arrange
        var preset = CreateValidPreset(31, "Original Name");
        var vm = CreateViewModel();
        vm.LoadPreset(preset);
        vm.PresetName = "Modified Name";

        _mockMidiPort
            .Setup(p => p.SendSysExAsync(It.IsAny<byte[]>()))
            .ReturnsAsync(Result.Ok());

        // Act
        await vm.SaveCommand.ExecuteAsync(null);

        // Assert
        _mockMidiPort.Verify(
            p => p.SendSysExAsync(It.IsAny<byte[]>()),
            Times.Once);
        vm.StatusMessage.Should().Contain("success");
    }

    [Fact]
    public async Task SaveCommand_WhenUpdateFails_ShowsErrorMessage()
    {
        // Arrange
        var preset = CreateValidPreset(31, "Original Name");
        var vm = CreateViewModel();
        vm.LoadPreset(preset);
        vm.PresetName = "Modified Name";

        _mockMidiPort
            .Setup(p => p.SendSysExAsync(It.IsAny<byte[]>()))
            .ReturnsAsync(Result.Fail("MIDI error"));

        // Act
        await vm.SaveCommand.ExecuteAsync(null);

        // Assert
        vm.StatusMessage.Should().Contain("failed");
        vm.StatusMessage.Should().Contain("MIDI error");
    }

    [Fact]
    public void SaveCommand_CanExecute_WhenHasChanges()
    {
        // Arrange
        var preset = CreateValidPreset(31, "Original Name");
        var vm = CreateViewModel();
        vm.LoadPreset(preset);

        // Act
        vm.PresetName = "Modified Name";

        // Assert
        vm.SaveCommand.CanExecute(null).Should().BeTrue();
    }

    [Fact]
    public void SaveCommand_CannotExecute_WhenNoChanges()
    {
        // Arrange
        var preset = CreateValidPreset(31, "Original Name");
        var vm = CreateViewModel();
        vm.LoadPreset(preset);

        // Act & Assert
        vm.SaveCommand.CanExecute(null).Should().BeFalse();
    }

    [Fact]
    public void CancelCommand_RevertsChanges()
    {
        // Arrange
        var preset = CreateValidPreset(31, "Original Name");
        var vm = CreateViewModel();
        vm.LoadPreset(preset);
        vm.PresetName = "Modified Name";

        // Act
        vm.CancelCommand.Execute(null);

        // Assert
        vm.PresetName.Should().Be("Original Name");
        vm.HasChanges.Should().BeFalse();
        vm.StatusMessage.Should().Contain("revert");
    }

    private EditablePresetViewModel CreateViewModel()
    {
        return new EditablePresetViewModel(
            _updateUseCase,
            _mockMidiPort.Object,
            _mockLogger.Object);
    }

    private Preset CreateValidPreset(int presetNumber, string name)
    {
        // Create a valid 521-byte preset SysEx
        var sysex = new byte[521];
        sysex[0] = 0xF0;
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F; // TC Electronic ID
        sysex[4] = 0x00; // Device ID
        sysex[5] = 0x63; // Nova System model ID
        sysex[6] = 0x20; // Dump message
        sysex[7] = 0x01; // Preset data type
        sysex[8] = (byte)presetNumber;

        // Set preset name (bytes 9-32, 24 chars)
        var nameBytes = System.Text.Encoding.ASCII.GetBytes(name.PadRight(24));
        Array.Copy(nameBytes, 0, sysex, 9, Math.Min(nameBytes.Length, 24));

        sysex[520] = 0xF7; // End of SysEx

        var presetResult = Preset.FromSysEx(sysex);
        return presetResult.Value;
    }
}
