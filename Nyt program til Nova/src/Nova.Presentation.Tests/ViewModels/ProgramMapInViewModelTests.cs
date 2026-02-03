using FluentResults;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Nova.Presentation.ViewModels;
using Xunit;

namespace Nova.Presentation.Tests.ViewModels;

public class ProgramMapInViewModelTests
{
    [Fact]
    public async Task LoadFromDump_LoadsEntries()
    {
        var sysex = CreateValidSystemDump();
        WriteMidiMapNibble(sysex, 64, v1: 10, v2: 0, v3: 0);
        var dump = SystemDump.FromSysEx(sysex).Value;

        var vm = new ProgramMapInViewModel(
            new GetProgramMapInUseCase(),
            new UpdateProgramMapInUseCase(),
            CreateSaveUseCase());

        await vm.LoadFromDump(dump);

        Assert.Equal(127, vm.Entries.Count);
        Assert.Equal(1, vm.Entries[0].IncomingProgram);
        Assert.Equal(10, vm.Entries[0].PresetNumber);
    }

    private static ISaveSystemDumpUseCase CreateSaveUseCase()
    {
        var mock = new Mock<ISaveSystemDumpUseCase>();
        mock.Setup(x => x.ExecuteAsync(It.IsAny<SystemDump>()))
            .ReturnsAsync(Result.Ok());
        return mock.Object;
    }

    private static byte[] CreateValidSystemDump()
    {
        var sysex = new byte[526];
        sysex[0] = 0xF0;
        sysex[1] = 0x00;
        sysex[2] = 0x20;
        sysex[3] = 0x1F;
        sysex[4] = 0x00;
        sysex[5] = 0x63;
        sysex[6] = 0x20;
        sysex[7] = 0x02;
        sysex[525] = 0xF7;
        return sysex;
    }

    private static void WriteMidiMapNibble(byte[] data, int nibbleIndex, int v1, int v2, int v3)
    {
        var offset = 8 + (nibbleIndex * 4);
        data[offset] = (byte)v3;
        data[offset + 1] = (byte)((v2 % 64) * 2);
        data[offset + 2] = (byte)((v1 % 32) * 4 + (v2 / 64));
        data[offset + 3] = (byte)(v1 / 32);
    }
}
