using FluentAssertions;
using Nova.Domain.Models;
using Nova.Presentation.ViewModels;
using Xunit;

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
        vm.MidiChannel.Should().Be(dump.MidiChannel);
        vm.DeviceId.Should().Be(dump.DeviceId);
        vm.MidiClockEnabled.Should().Be(dump.IsMidiClockEnabled);
        vm.MidiProgramChangeEnabled.Should().Be(dump.IsMidiProgramChangeEnabled);
        vm.Version.Should().Be(dump.GetVersionString());
    }

    [Fact]
    public void LoadFromDump_SetsVersionString()
    {
        // Arrange
        var dump = CreateValidSystemDump();
        var vm = new SystemSettingsViewModel();

        // Act
        vm.LoadFromDump(dump);

        // Assert
        vm.Version.Should().NotBeNullOrEmpty();
        vm.Version.Should().Be(dump.GetVersionString());
    }

    [Fact]
    public void MidiChannel_WithinValidRange()
    {
        // Arrange
        var dump = CreateValidSystemDump();
        var vm = new SystemSettingsViewModel();

        // Act
        vm.LoadFromDump(dump);

        // Assert
        vm.MidiChannel.Should().BeInRange(0, 15);
    }

    [Fact]
    public void DeviceId_WithinValidRange()
    {
        // Arrange
        var dump = CreateValidSystemDump();
        var vm = new SystemSettingsViewModel();

        // Act
        vm.LoadFromDump(dump);

        // Assert
        vm.DeviceId.Should().BeInRange(0, 127);
    }

    [Fact]
    public void InitialState_HasEmptyVersion()
    {
        // Arrange & Act
        var vm = new SystemSettingsViewModel();

        // Assert
        vm.Version.Should().Be(string.Empty);
    }

    private static SystemDump CreateValidSystemDump()
    {
        var sysex = new byte[527];
        sysex[0] = 0xF0;
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F; // TC Electronic
        sysex[4] = 0x00; // Device ID
        sysex[5] = 0x63; // Nova System Model
        sysex[6] = 0x20; // Message ID: Dump
        sysex[7] = 0x02; // Data Type: System Dump
        sysex[526] = 0xF7;

        return SystemDump.FromSysEx(sysex).Value;
    }
}
