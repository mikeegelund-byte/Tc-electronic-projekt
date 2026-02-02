using FluentAssertions;
using Nova.Application.UseCases;
using Nova.Domain.Models;

namespace Nova.Application.Tests.UseCases;

public class ExportPresetUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidPreset_WritesFileSuccessfully()
    {
        // Arrange
        var tempFile = Path.Combine(Path.GetTempPath(), $"test-preset-{Guid.NewGuid()}.syx");
        var preset = CreateValidPreset(presetNumber: 31);
        var bank = UserBankDump.Empty().WithPreset(31, preset).Value;
        var useCase = new ExportPresetUseCase(bank);

        try
        {
            // Act
            var result = await useCase.ExecuteAsync(31, tempFile);

            // Assert
            result.IsSuccess.Should().BeTrue();
            File.Exists(tempFile).Should().BeTrue();
            var bytes = await File.ReadAllBytesAsync(tempFile);
            bytes.Length.Should().Be(521);
            bytes[0].Should().Be(0xF0); // SysEx start
            bytes[520].Should().Be(0xF7); // SysEx end
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentPreset_ReturnsFailure()
    {
        // Arrange
        var tempFile = Path.Combine(Path.GetTempPath(), $"test-preset-{Guid.NewGuid()}.syx");
        var bank = UserBankDump.Empty(); // No presets
        var useCase = new ExportPresetUseCase(bank);

        // Act
        var result = await useCase.ExecuteAsync(31, tempFile);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("Preset 31 not found");
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidPresetNumber_ReturnsFailure()
    {
        // Arrange
        var tempFile = Path.Combine(Path.GetTempPath(), $"test-preset-{Guid.NewGuid()}.syx");
        var bank = UserBankDump.Empty();
        var useCase = new ExportPresetUseCase(bank);

        // Act
        var result = await useCase.ExecuteAsync(99, tempFile);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("must be between 31 and 90");
    }

    private Preset CreateValidPreset(int presetNumber)
    {
        var sysex = new byte[521];
        sysex[0] = 0xF0; // Start
        sysex[1] = 0x00; // Manufacturer
        sysex[2] = 0x20;
        sysex[3] = 0x1F;
        sysex[4] = 0x00; // Device ID
        sysex[5] = 0x63; // Nova System ID
        sysex[6] = 0x20; // Message ID: Dump
        sysex[7] = 0x01; // Data type: Preset
        sysex[8] = (byte)presetNumber;
        
        // Name "Test" in bytes 10-25
        sysex[10] = 0x54; // 'T'
        sysex[11] = 0x65; // 'e'
        sysex[12] = 0x73; // 's'
        sysex[13] = 0x74; // 't'
        
        sysex[520] = 0xF7; // End
        
        return Preset.FromSysEx(sysex).Value;
    }
}
