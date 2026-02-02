using FluentAssertions;
using Nova.Application.UseCases;

namespace Nova.Application.Tests.UseCases;

public class DetectSysExTypeUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithPresetFile_ReturnsPresetType()
    {
        // Arrange
        var tempFile = Path.Combine(Path.GetTempPath(), $"test-preset-{Guid.NewGuid()}.syx");
        var sysex = CreatePresetSysEx();
        await File.WriteAllBytesAsync(tempFile, sysex);
        var useCase = new DetectSysExTypeUseCase();

        try
        {
            // Act
            var result = await useCase.ExecuteAsync(tempFile);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(SysExType.Preset);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ExecuteAsync_WithSystemDumpFile_ReturnsSystemDumpType()
    {
        // Arrange
        var tempFile = Path.Combine(Path.GetTempPath(), $"test-system-{Guid.NewGuid()}.syx");
        var sysex = CreateSystemDumpSysEx();
        await File.WriteAllBytesAsync(tempFile, sysex);
        var useCase = new DetectSysExTypeUseCase();

        try
        {
            // Act
            var result = await useCase.ExecuteAsync(tempFile);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(SysExType.SystemDump);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ExecuteAsync_WithUserBankFile_ReturnsUserBankType()
    {
        // Arrange
        var tempFile = Path.Combine(Path.GetTempPath(), $"test-bank-{Guid.NewGuid()}.syx");
        var sysex = CreateUserBankSysEx(60); // 60 presets
        await File.WriteAllBytesAsync(tempFile, sysex);
        var useCase = new DetectSysExTypeUseCase();

        try
        {
            // Act
            var result = await useCase.ExecuteAsync(tempFile);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(SysExType.UserBank);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidFile_ReturnsUnknown()
    {
        // Arrange
        var tempFile = Path.Combine(Path.GetTempPath(), $"test-invalid-{Guid.NewGuid()}.syx");
        await File.WriteAllBytesAsync(tempFile, new byte[] { 0x00, 0x01, 0x02 });
        var useCase = new DetectSysExTypeUseCase();

        try
        {
            // Act
            var result = await useCase.ExecuteAsync(tempFile);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(SysExType.Unknown);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentFile_ReturnsFailure()
    {
        // Arrange
        var nonExistentFile = "C:\\NonExistent\\file.syx";
        var useCase = new DetectSysExTypeUseCase();

        // Act
        var result = await useCase.ExecuteAsync(nonExistentFile);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    private byte[] CreatePresetSysEx()
    {
        var sysex = new byte[521];
        sysex[0] = 0xF0;
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F;
        sysex[5] = 0x63;
        sysex[6] = 0x20; // Dump
        sysex[7] = 0x01; // Preset
        sysex[520] = 0xF7;
        return sysex;
    }

    private byte[] CreateSystemDumpSysEx()
    {
        var sysex = new byte[527];
        sysex[0] = 0xF0;
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F;
        sysex[5] = 0x63;
        sysex[6] = 0x20; // Dump
        sysex[7] = 0x02; // System
        sysex[526] = 0xF7;
        return sysex;
    }

    private byte[] CreateUserBankSysEx(int presetCount)
    {
        var sysex = new byte[presetCount * 521];
        for (int i = 0; i < presetCount; i++)
        {
            int offset = i * 521;
            sysex[offset] = 0xF0;
            sysex[offset + 1] = 0x00; sysex[offset + 2] = 0x20; sysex[offset + 3] = 0x1F;
            sysex[offset + 5] = 0x63;
            sysex[offset + 6] = 0x20;
            sysex[offset + 7] = 0x01;
            sysex[offset + 520] = 0xF7;
        }
        return sysex;
    }
}
