using FluentAssertions;
using Nova.Domain.Models;

namespace Nova.Domain.Tests;

public class SystemDumpRealDataTests
{
    [Fact]
    public void FromSysEx_RealHardwareSystemDump_ParsesCorrectly()
    {
        // Arrange - Load real System Dump file
        var baseDir = GetHardwareTestDir();
        var systemDumpFile = Directory.GetFiles(baseDir, "*-182108-*.syx").Single();
        var sysex = File.ReadAllBytes(systemDumpFile);

        // Act
        var result = SystemDump.FromSysEx(sysex);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.RawSysEx.Should().HaveCount(527);

        // Verify key structure bytes
        result.Value.RawSysEx[0].Should().Be(0xF0, "SysEx start");
        result.Value.RawSysEx[1].Should().Be(0x00, "TC Electronic ID byte 1");
        result.Value.RawSysEx[2].Should().Be(0x20, "TC Electronic ID byte 2");
        result.Value.RawSysEx[3].Should().Be(0x1F, "TC Electronic ID byte 3");
        result.Value.RawSysEx[5].Should().Be(0x63, "Nova System model ID");
        result.Value.RawSysEx[6].Should().Be(0x20, "Dump message ID");
        result.Value.RawSysEx[7].Should().Be(0x02, "System Dump data type");
        result.Value.RawSysEx[526].Should().Be(0xF7, "SysEx end");
    }

    private static string GetHardwareTestDir()
    {
        var baseDir = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "..", "Nova.HardwareTest");
        return Path.GetFullPath(baseDir);
    }
}
