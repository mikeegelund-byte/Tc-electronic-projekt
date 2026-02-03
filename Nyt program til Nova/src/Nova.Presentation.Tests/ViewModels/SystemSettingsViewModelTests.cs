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
        vm.DeviceId.Should().Be(12);
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

    private static SystemDump CreateValidSystemDump()
    {
        var sysex = new byte[526];
        sysex[0] = 0xF0;
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F;
        sysex[4] = 0x00;
        sysex[5] = 0x63;
        sysex[6] = 0x20;
        sysex[7] = 0x02;

        // MIDI settings via nibble encoding
        WriteNibble(sysex, 19, 5);  // MIDI channel
        WriteNibble(sysex, 20, 1);  // Program Change In enabled
        WriteNibble(sysex, 21, 1);  // Program Change Out enabled
        WriteNibble(sysex, 22, 1);  // MIDI clock enabled
        WriteNibble(sysex, 23, 12); // SysEx ID

        // Fake version bytes (legacy behavior)
        sysex[22] = 1;
        sysex[23] = 2;

        sysex[525] = 0xF7;

        var result = SystemDump.FromSysEx(sysex);
        return result.Value;
    }

    private static void WriteNibble(byte[] data, int nibbleIndex, int value)
    {
        var offset = 8 + (nibbleIndex * 4);
        if (value >= 0)
        {
            data[offset] = (byte)(value % 128);
            data[offset + 1] = (byte)(value / 128);
            data[offset + 2] = 0;
            data[offset + 3] = 0;
        }
        else
        {
            data[offset] = (byte)(128 - ((-value) % 128));
            data[offset + 1] = (byte)((value / 128) + 127);
            data[offset + 2] = 127;
            data[offset + 3] = 7;
        }
    }
}
