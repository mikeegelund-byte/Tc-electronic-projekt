using FluentAssertions;
using Nova.Application.UseCases;
using Nova.Domain.Models;

namespace Nova.Application.Tests.UseCases;

public class UpdateProgramMapInUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithNullDump_ReturnsFailure()
    {
        var useCase = new UpdateProgramMapInUseCase();
        var result = await useCase.ExecuteAsync(null!, 1, 31);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_WithValidInput_UpdatesMapping()
    {
        var useCase = new UpdateProgramMapInUseCase();
        var dump = SystemDump.FromSysEx(CreateValidSystemDump()).Value;

        var result = await useCase.ExecuteAsync(dump, 1, 31);

        result.IsSuccess.Should().BeTrue();
        dump.GetProgramMapIn()[0].PresetNumber.Should().Be(31);
    }

    private static byte[] CreateValidSystemDump()
    {
        var sysex = new byte[527];
        sysex[0] = 0xF0;
        sysex[1] = 0x00;
        sysex[2] = 0x20;
        sysex[3] = 0x1F;
        sysex[4] = 0x00;
        sysex[5] = 0x63;
        sysex[6] = 0x20;
        sysex[7] = 0x02;
        sysex[526] = 0xF7;
        return sysex;
    }
}

