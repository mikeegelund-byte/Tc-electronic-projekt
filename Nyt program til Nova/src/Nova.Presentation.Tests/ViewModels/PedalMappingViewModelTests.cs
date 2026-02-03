using Nova.Domain.Models;
using Nova.Presentation.ViewModels;
using Xunit;

namespace Nova.Presentation.Tests.ViewModels;

public class PedalMappingViewModelTests
{
    [Fact]
    public void LoadFromDump_SetsParameterFromSystemDump()
    {
        // Arrange
        var sysex = CreateValidSystemDump();
        WriteNibble(sysex, 6, 42);
        var dumpResult = SystemDump.FromSysEx(sysex);
        var dump = dumpResult.Value;
        var viewModel = new PedalMappingViewModel();

        // Act
        viewModel.LoadFromDump(dump);

        // Assert
        Assert.Equal(42, viewModel.Parameter);
    }

    [Fact]
    public void LoadFromDump_SetsMinFromSystemDump()
    {
        // Arrange
        var sysex = CreateValidSystemDump();
        WriteNibble(sysex, 3, 10);
        var dumpResult = SystemDump.FromSysEx(sysex);
        var dump = dumpResult.Value;
        var viewModel = new PedalMappingViewModel();

        // Act
        viewModel.LoadFromDump(dump);

        // Assert
        Assert.Equal(10, viewModel.Min);
    }

    [Fact]
    public void LoadFromDump_SetsMidFromSystemDump()
    {
        // Arrange
        var sysex = CreateValidSystemDump();
        WriteNibble(sysex, 4, 50);
        var dumpResult = SystemDump.FromSysEx(sysex);
        var dump = dumpResult.Value;
        var viewModel = new PedalMappingViewModel();

        // Act
        viewModel.LoadFromDump(dump);

        // Assert
        Assert.Equal(50, viewModel.Mid);
    }

    [Fact]
    public void LoadFromDump_SetsMaxFromSystemDump()
    {
        // Arrange
        var sysex = CreateValidSystemDump();
        WriteNibble(sysex, 5, 100);
        var dumpResult = SystemDump.FromSysEx(sysex);
        var dump = dumpResult.Value;
        var viewModel = new PedalMappingViewModel();

        // Act
        viewModel.LoadFromDump(dump);

        // Assert
        Assert.Equal(100, viewModel.Max);
    }

    [Fact]
    public void LoadFromDump_WithTypicalConfiguration_LoadsAllValues()
    {
        // Arrange
        var sysex = CreateValidSystemDump();
        WriteNibble(sysex, 6, 7);   // Parameter 7
        WriteNibble(sysex, 3, 0);   // Min 0%
        WriteNibble(sysex, 4, 50);  // Mid 50%
        WriteNibble(sysex, 5, 100); // Max 100%
        var dumpResult = SystemDump.FromSysEx(sysex);
        var dump = dumpResult.Value;
        var viewModel = new PedalMappingViewModel();

        // Act
        viewModel.LoadFromDump(dump);

        // Assert
        Assert.Equal(7, viewModel.Parameter);
        Assert.Equal(0, viewModel.Min);
        Assert.Equal(50, viewModel.Mid);
        Assert.Equal(100, viewModel.Max);
    }

    private static byte[] CreateValidSystemDump()
    {
        var sysex = new byte[526];
        sysex[0] = 0xF0;                // Start
        sysex[1] = 0x00;                // Manufacturer
        sysex[2] = 0x20;
        sysex[3] = 0x1F;
        sysex[4] = 0x00;                // Device ID
        sysex[5] = 0x63;                // Model ID (Nova System)
        sysex[6] = 0x20;                // Dump command
        sysex[7] = 0x02;                // System dump type
        sysex[525] = 0xF7;              // End

        // Default pedal values
        WriteNibble(sysex, 6, 0);
        WriteNibble(sysex, 3, 0);
        WriteNibble(sysex, 4, 50);
        WriteNibble(sysex, 5, 100);

        return sysex;
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
