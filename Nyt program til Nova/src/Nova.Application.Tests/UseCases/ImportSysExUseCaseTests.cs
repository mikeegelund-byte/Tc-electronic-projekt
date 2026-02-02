using FluentAssertions;
using Nova.Application.UseCases;
using Nova.Domain.Models;

namespace Nova.Application.Tests.UseCases;

public class ImportSysExUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithPresetFile_ReturnsPreset()
    {
        // Arrange
        var tempFile = Path.Combine(Path.GetTempPath(), $"test-import-preset-{Guid.NewGuid()}.syx");
        var sysex = CreatePresetSysEx(31);
        await File.WriteAllBytesAsync(tempFile, sysex);
        var useCase = new ImportSysExUseCase();

        try
        {
            // Act
            var result = await useCase.ExecuteAsync(tempFile);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<Preset>();
            var preset = (Preset)result.Value;
            preset.Number.Should().Be(31);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ExecuteAsync_WithSystemDumpFile_ReturnsSystemDump()
    {
        // Arrange
        var tempFile = Path.Combine(Path.GetTempPath(), $"test-import-system-{Guid.NewGuid()}.syx");
        var sysex = CreateSystemDumpSysEx();
        await File.WriteAllBytesAsync(tempFile, sysex);
        var useCase = new ImportSysExUseCase();

        try
        {
            // Act
            var result = await useCase.ExecuteAsync(tempFile);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<SystemDump>();
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ExecuteAsync_WithUserBankFile_ReturnsUserBankDump()
    {
        // Arrange
        var tempFile = Path.Combine(Path.GetTempPath(), $"test-import-bank-{Guid.NewGuid()}.syx");
        var sysex = CreateUserBankSysEx(60);
        await File.WriteAllBytesAsync(tempFile, sysex);
        var useCase = new ImportSysExUseCase();

        try
        {
            // Act
            var result = await useCase.ExecuteAsync(tempFile);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<UserBankDump>();
            var bank = (UserBankDump)result.Value;
            bank.Presets.Count(p => p != null).Should().Be(60);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidFile_ReturnsFailure()
    {
        // Arrange
        var tempFile = Path.Combine(Path.GetTempPath(), $"test-import-invalid-{Guid.NewGuid()}.syx");
        await File.WriteAllBytesAsync(tempFile, new byte[] { 0x00, 0x01, 0x02 });
        var useCase = new ImportSysExUseCase();

        try
        {
            // Act
            var result = await useCase.ExecuteAsync(tempFile);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.First().Message.Should().Contain("Unknown or invalid");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    private byte[] CreatePresetSysEx(int presetNumber)
    {
        var sysex = new byte[521];
        sysex[0] = 0xF0;
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F;
        sysex[4] = 0x00;
        sysex[5] = 0x63;
        sysex[6] = 0x20;
        sysex[7] = 0x01;
        sysex[8] = (byte)presetNumber;
        sysex[520] = 0xF7;
        return sysex;
    }

    private byte[] CreateSystemDumpSysEx()
    {
        var sysex = new byte[527];
        sysex[0] = 0xF0;
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F;
        sysex[4] = 0x00;
        sysex[5] = 0x63;
        sysex[6] = 0x20;
        sysex[7] = 0x02;
        sysex[526] = 0xF7;
        return sysex;
    }

    private byte[] CreateUserBankSysEx(int presetCount)
    {
        var sysex = new byte[presetCount * 521];
        for (int i = 0; i < presetCount; i++)
        {
            int offset = i * 521;
            int presetNumber = 31 + i;
            sysex[offset] = 0xF0;
            sysex[offset + 1] = 0x00; sysex[offset + 2] = 0x20; sysex[offset + 3] = 0x1F;
            sysex[offset + 4] = 0x00;
            sysex[offset + 5] = 0x63;
            sysex[offset + 6] = 0x20;
            sysex[offset + 7] = 0x01;
            sysex[offset + 8] = (byte)presetNumber;
            sysex[offset + 520] = 0xF7;
        }
        return sysex;
    }
}
