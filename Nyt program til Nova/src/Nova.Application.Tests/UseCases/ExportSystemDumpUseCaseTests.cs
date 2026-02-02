using FluentAssertions;
using Nova.Application.UseCases;
using Nova.Domain.Models;

namespace Nova.Application.Tests.UseCases;

public class ExportSystemDumpUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidSystemDump_WritesFileSuccessfully()
    {
        // Arrange
        var tempFile = Path.Combine(Path.GetTempPath(), $"test-system-{Guid.NewGuid()}.syx");
        var systemDump = CreateValidSystemDump();
        var useCase = new ExportSystemDumpUseCase();

        try
        {
            // Act
            var result = await useCase.ExecuteAsync(systemDump, tempFile);

            // Assert
            result.IsSuccess.Should().BeTrue();
            File.Exists(tempFile).Should().BeTrue();
            
            var bytes = await File.ReadAllBytesAsync(tempFile);
            bytes.Length.Should().Be(527); // System dump size
            bytes[0].Should().Be(0xF0);
            bytes[526].Should().Be(0xF7);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidPath_ReturnsFailure()
    {
        // Arrange
        var invalidPath = "Z:\\NonExistentDrive\\invalid.syx";
        var systemDump = CreateValidSystemDump();
        var useCase = new ExportSystemDumpUseCase();

        // Act
        var result = await useCase.ExecuteAsync(systemDump, invalidPath);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("Failed to write");
    }

    private SystemDump CreateValidSystemDump()
    {
        var sysex = new byte[527];
        sysex[0] = 0xF0;
        sysex[1] = 0x00;
        sysex[2] = 0x20;
        sysex[3] = 0x1F;
        sysex[4] = 0x00;
        sysex[5] = 0x63;
        sysex[6] = 0x20; // Message ID: Dump
        sysex[7] = 0x02; // Data type: System
        sysex[526] = 0xF7;
        return SystemDump.FromSysEx(sysex).Value;
    }
}
