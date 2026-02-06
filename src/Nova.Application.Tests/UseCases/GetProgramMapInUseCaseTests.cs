using FluentAssertions;
using Nova.Application.UseCases;
using Nova.Domain.Models;

namespace Nova.Application.Tests.UseCases;

public class GetProgramMapInUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithNullDump_ReturnsFailure()
    {
        var useCase = new GetProgramMapInUseCase();
        var result = await useCase.ExecuteAsync(null!);

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_WithValidDump_Returns127Entries()
    {
        var useCase = new GetProgramMapInUseCase();
        var dump = SystemDump.FromSysEx(CreateValidSystemDump()).Value;

        var result = await useCase.ExecuteAsync(dump);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(127);
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

