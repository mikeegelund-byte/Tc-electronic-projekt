using FluentAssertions;
using Nova.Domain.Models;
using Nova.Presentation.ViewModels;

namespace Nova.Presentation.Tests.ViewModels;

public class SystemSettingsViewModelTests
{
    [Fact]
    public void LoadFromDump_WithValidSystemDump_SetsAllProperties()
    {
        // Arrange
        var dump = CreateValidSystemDump();
        var vm = new SystemSettingsViewModel();

        // Act
        vm.LoadFromDump(dump);

        // Assert
        vm.MidiChannel.Should().Be(5);
        vm.DeviceId.Should().Be(0);
        vm.MidiClockEnabled.Should().BeTrue();
        vm.MidiProgramChangeEnabled.Should().BeTrue();
        vm.Version.Should().NotBeEmpty();
    }

    [Fact]
    public void LoadFromDump_WithNullDump_DoesNotThrow()
    {
        // Arrange
        var vm = new SystemSettingsViewModel();

        // Act
        var action = () => vm.LoadFromDump(null!);

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void Initial_Properties_HaveDefaultValues()
    {
        // Arrange
        var vm = new SystemSettingsViewModel();

        // Assert
        vm.MidiChannel.Should().Be(0);
        vm.DeviceId.Should().Be(0);
        vm.MidiClockEnabled.Should().BeFalse();
        vm.MidiProgramChangeEnabled.Should().BeFalse();
        vm.Version.Should().Be(string.Empty);
    }

    [Fact]
    public void HasUnsavedChanges_InitiallyFalse()
    {
        // Arrange
        var vm = new SystemSettingsViewModel();

        // Assert
        vm.HasUnsavedChanges.Should().BeFalse();
    }

    [Fact]
    public void HasUnsavedChanges_FalseAfterLoadingDump()
    {
        // Arrange
        var dump = CreateValidSystemDump();
        var vm = new SystemSettingsViewModel();

        // Act
        vm.LoadFromDump(dump);

        // Assert
        vm.HasUnsavedChanges.Should().BeFalse();
    }

    [Fact]
    public void HasUnsavedChanges_TrueWhenMidiChannelChanged()
    {
        // Arrange
        var dump = CreateValidSystemDump();
        var vm = new SystemSettingsViewModel();
        vm.LoadFromDump(dump);

        // Act
        vm.MidiChannel = 10;

        // Assert
        vm.HasUnsavedChanges.Should().BeTrue();
    }

    [Fact]
    public void HasUnsavedChanges_TrueWhenDeviceIdChanged()
    {
        // Arrange
        var dump = CreateValidSystemDump();
        var vm = new SystemSettingsViewModel();
        vm.LoadFromDump(dump);

        // Act
        vm.DeviceId = 1;

        // Assert
        vm.HasUnsavedChanges.Should().BeTrue();
    }

    [Fact]
    public void HasUnsavedChanges_TrueWhenMidiClockEnabledChanged()
    {
        // Arrange
        var dump = CreateValidSystemDump();
        var vm = new SystemSettingsViewModel();
        vm.LoadFromDump(dump);

        // Act
        vm.MidiClockEnabled = false;

        // Assert
        vm.HasUnsavedChanges.Should().BeTrue();
    }

    [Fact]
    public void HasUnsavedChanges_TrueWhenMidiProgramChangeEnabledChanged()
    {
        // Arrange
        var dump = CreateValidSystemDump();
        var vm = new SystemSettingsViewModel();
        vm.LoadFromDump(dump);

        // Act
        vm.MidiProgramChangeEnabled = false;

        // Assert
        vm.HasUnsavedChanges.Should().BeTrue();
    }

    [Fact]
    public void RevertChanges_RestoresOriginalValues()
    {
        // Arrange
        var dump = CreateValidSystemDump();
        var vm = new SystemSettingsViewModel();
        vm.LoadFromDump(dump);
        var originalMidiChannel = vm.MidiChannel;
        var originalDeviceId = vm.DeviceId;
        var originalMidiClock = vm.MidiClockEnabled;
        var originalProgChange = vm.MidiProgramChangeEnabled;

        // Act - Make changes
        vm.MidiChannel = 10;
        vm.DeviceId = 5;
        vm.MidiClockEnabled = false;
        vm.MidiProgramChangeEnabled = false;

        // Assert - Changes detected
        vm.HasUnsavedChanges.Should().BeTrue();

        // Act - Revert
        vm.RevertChanges();

        // Assert - Values restored
        vm.MidiChannel.Should().Be(originalMidiChannel);
        vm.DeviceId.Should().Be(originalDeviceId);
        vm.MidiClockEnabled.Should().Be(originalMidiClock);
        vm.MidiProgramChangeEnabled.Should().Be(originalProgChange);
        vm.HasUnsavedChanges.Should().BeFalse();
    }

    private static SystemDump CreateValidSystemDump()
    {
        // Create minimal valid SystemDump for testing
        var sysex = new byte[527];
        sysex[0] = 0xF0;
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F;
        sysex[4] = 0x00; // Device ID
        sysex[5] = 0x63; // Nova System
        sysex[6] = 0x20; // Dump message
        sysex[7] = 0x02; // System dump type
        
        // Set MIDI channel (offset 8 = channel 5)
        sysex[8] = 5;
        
        // Set MIDI clock enabled (offset 20)
        sysex[20] = 0x01;
        
        // Set MIDI program change enabled (offset 21)
        sysex[21] = 0x01;
        
        // Set version (offset 22-23)
        sysex[22] = 1;
        sysex[23] = 2;
        
        sysex[526] = 0xF7;
        
        var result = SystemDump.FromSysEx(sysex);
        return result.Value;
    }
}
