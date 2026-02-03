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
        var parameter = 42;
        BitConverter.GetBytes(parameter).CopyTo(sysex, 54); // PEDAL_PARAMETER_OFFSET = 54
        var dumpResult = SystemDump.FromSysEx(sysex);
        var dump = dumpResult.Value;
        var viewModel = new PedalMappingViewModel();

        // Act
        viewModel.LoadFromDump(dump);

        // Assert
        Assert.Equal(parameter, viewModel.Parameter);
    }

    [Fact]
    public void LoadFromDump_SetsMinFromSystemDump()
    {
        // Arrange
        var sysex = CreateValidSystemDump();
        var min = 10;
        BitConverter.GetBytes(min).CopyTo(sysex, 58); // PEDAL_MIN_OFFSET = 58
        var dumpResult = SystemDump.FromSysEx(sysex);
        var dump = dumpResult.Value;
        var viewModel = new PedalMappingViewModel();

        // Act
        viewModel.LoadFromDump(dump);

        // Assert
        Assert.Equal(min, viewModel.Min);
    }

    [Fact]
    public void LoadFromDump_SetsMidFromSystemDump()
    {
        // Arrange
        var sysex = CreateValidSystemDump();
        var mid = 50;
        BitConverter.GetBytes(mid).CopyTo(sysex, 62); // PEDAL_MID_OFFSET = 62
        var dumpResult = SystemDump.FromSysEx(sysex);
        var dump = dumpResult.Value;
        var viewModel = new PedalMappingViewModel();

        // Act
        viewModel.LoadFromDump(dump);

        // Assert
        Assert.Equal(mid, viewModel.Mid);
    }

    [Fact]
    public void LoadFromDump_SetsMaxFromSystemDump()
    {
        // Arrange
        var sysex = CreateValidSystemDump();
        var max = 100;
        BitConverter.GetBytes(max).CopyTo(sysex, 66); // PEDAL_MAX_OFFSET = 66
        var dumpResult = SystemDump.FromSysEx(sysex);
        var dump = dumpResult.Value;
        var viewModel = new PedalMappingViewModel();

        // Act
        viewModel.LoadFromDump(dump);

        // Assert
        Assert.Equal(max, viewModel.Max);
    }

    [Fact]
    public void LoadFromDump_WithTypicalConfiguration_LoadsAllValues()
    {
        // Arrange
        var sysex = CreateValidSystemDump();
        BitConverter.GetBytes(7).CopyTo(sysex, 54);   // Parameter 7
        BitConverter.GetBytes(0).CopyTo(sysex, 58);   // Min 0%
        BitConverter.GetBytes(50).CopyTo(sysex, 62);  // Mid 50%
        BitConverter.GetBytes(100).CopyTo(sysex, 66); // Max 100%
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
        var sysex = new byte[527];
        sysex[0] = 0xF0;                // Start
        sysex[1] = 0x00;                // Manufacturer
        sysex[2] = 0x20;
        sysex[3] = 0x1F;
        sysex[4] = 0x00;                // Device ID
        sysex[5] = 0x63;                // Model ID (Nova System)
        sysex[6] = 0x20;                // Dump command
        sysex[7] = 0x02;                // System dump type
        sysex[526] = 0xF7;              // End

        // Set default pedal values
        BitConverter.GetBytes(0).CopyTo(sysex, 54);   // Parameter
        BitConverter.GetBytes(0).CopyTo(sysex, 58);   // Min
        BitConverter.GetBytes(50).CopyTo(sysex, 62);  // Mid
        BitConverter.GetBytes(100).CopyTo(sysex, 66); // Max

        return sysex;
    }
}
