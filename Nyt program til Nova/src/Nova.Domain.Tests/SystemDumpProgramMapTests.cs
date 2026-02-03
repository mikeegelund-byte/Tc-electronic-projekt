using FluentAssertions;
using Nova.Domain.Models;

namespace Nova.Domain.Tests;

public class SystemDumpProgramMapTests
{
    [Fact]
    public void GetProgramMapIn_ParsesValuesFromMidiMapNibble()
    {
        var sysex = CreateValidSystemDump();
        WriteMidiMapNibble(sysex, 64, v1: 10, v2: 20, v3: 0);

        var dump = SystemDump.FromSysEx(sysex).Value;
        var mapIn = dump.GetProgramMapIn();

        mapIn.Should().HaveCount(127);
        mapIn[0].IncomingProgram.Should().Be(1);
        mapIn[0].PresetNumber.Should().Be(10);
        mapIn[1].IncomingProgram.Should().Be(2);
        mapIn[1].PresetNumber.Should().Be(20);
        mapIn[2].IncomingProgram.Should().Be(3);
        mapIn[2].PresetNumber.Should().BeNull();
    }

    [Fact]
    public void UpdateProgramMapIn_ValidValue_UpdatesMapping()
    {
        var sysex = CreateValidSystemDump();
        var dump = SystemDump.FromSysEx(sysex).Value;

        var result = dump.UpdateProgramMapIn(1, 31);

        result.IsSuccess.Should().BeTrue();
        dump.GetProgramMapIn()[0].PresetNumber.Should().Be(31);
    }

    [Fact]
    public void UpdateProgramMapIn_OutOfRange_Fails()
    {
        var sysex = CreateValidSystemDump();
        var dump = SystemDump.FromSysEx(sysex).Value;

        dump.UpdateProgramMapIn(0, 31).IsFailed.Should().BeTrue();
        dump.UpdateProgramMapIn(128, 31).IsFailed.Should().BeTrue();
        dump.UpdateProgramMapIn(1, 91).IsFailed.Should().BeTrue();
    }

    [Fact]
    public void GetProgramMapOut_ParsesValuesFromMidiMapNibble()
    {
        var sysex = CreateValidSystemDump();
        WriteMidiMapNibble(sysex, 107, v1: 5, v2: 6, v3: 7);

        var dump = SystemDump.FromSysEx(sysex).Value;
        var mapOut = dump.GetProgramMapOut();

        mapOut.Should().HaveCount(60);
        mapOut[0].PresetNumber.Should().Be(31);
        mapOut[0].OutgoingProgram.Should().Be(5);
        mapOut[1].PresetNumber.Should().Be(32);
        mapOut[1].OutgoingProgram.Should().Be(6);
        mapOut[2].PresetNumber.Should().Be(33);
        mapOut[2].OutgoingProgram.Should().Be(7);
    }

    [Fact]
    public void UpdateProgramMapOut_ValidValue_UpdatesMapping()
    {
        var sysex = CreateValidSystemDump();
        var dump = SystemDump.FromSysEx(sysex).Value;

        var result = dump.UpdateProgramMapOut(31, 12);

        result.IsSuccess.Should().BeTrue();
        dump.GetProgramMapOut()[0].OutgoingProgram.Should().Be(12);
    }

    [Fact]
    public void UpdateProgramMapOut_OutOfRange_Fails()
    {
        var sysex = CreateValidSystemDump();
        var dump = SystemDump.FromSysEx(sysex).Value;

        dump.UpdateProgramMapOut(30, 10).IsFailed.Should().BeTrue();
        dump.UpdateProgramMapOut(91, 10).IsFailed.Should().BeTrue();
        dump.UpdateProgramMapOut(31, 128).IsFailed.Should().BeTrue();
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
