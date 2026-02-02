using FluentAssertions;
using Nova.Application.UseCases;
using Nova.Domain.Models;

namespace Nova.Application.Tests.UseCases;

public class ExportBankUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidBank_WritesAllPresetsSuccessfully()
    {
        // Arrange
        var tempFile = Path.Combine(Path.GetTempPath(), $"test-bank-{Guid.NewGuid()}.syx");
        var bank = CreateBankWith60Presets();
        var useCase = new ExportBankUseCase();

        try
        {
            // Act
            var result = await useCase.ExecuteAsync(bank, tempFile);

            // Assert
            result.IsSuccess.Should().BeTrue();
            File.Exists(tempFile).Should().BeTrue();
            
            var bytes = await File.ReadAllBytesAsync(tempFile);
            // 60 presets * 521 bytes each = 31,260 bytes
            bytes.Length.Should().Be(60 * 521);
            
            // Verify first preset starts correctly
            bytes[0].Should().Be(0xF0);
            bytes[520].Should().Be(0xF7);
            
            // Verify second preset starts at byte 521
            bytes[521].Should().Be(0xF0);
            bytes[1041].Should().Be(0xF7);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyBank_ReturnsFailure()
    {
        // Arrange
        var tempFile = Path.Combine(Path.GetTempPath(), $"test-bank-{Guid.NewGuid()}.syx");
        var bank = UserBankDump.Empty(); // No presets
        var useCase = new ExportBankUseCase();

        // Act
        var result = await useCase.ExecuteAsync(bank, tempFile);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("No presets found");
    }

    [Fact]
    public async Task ExecuteAsync_WithPartialBank_ExportsOnlyExistingPresets()
    {
        // Arrange
        var tempFile = Path.Combine(Path.GetTempPath(), $"test-bank-{Guid.NewGuid()}.syx");
        var preset1 = CreateValidPreset(31);
        var preset2 = CreateValidPreset(32);
        var bank = UserBankDump.Empty()
            .WithPreset(31, preset1).Value
            .WithPreset(32, preset2).Value;
        var useCase = new ExportBankUseCase();

        try
        {
            // Act
            var result = await useCase.ExecuteAsync(bank, tempFile);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var bytes = await File.ReadAllBytesAsync(tempFile);
            // Only 2 presets: 2 * 521 = 1042 bytes
            bytes.Length.Should().Be(2 * 521);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    private UserBankDump CreateBankWith60Presets()
    {
        var bank = UserBankDump.Empty();
        for (int i = 31; i <= 90; i++)
        {
            var preset = CreateValidPreset(i);
            bank = bank.WithPreset(i, preset).Value;
        }
        return bank;
    }

    private Preset CreateValidPreset(int presetNumber)
    {
        var sysex = new byte[521];
        sysex[0] = 0xF0;
        sysex[1] = 0x00;
        sysex[2] = 0x20;
        sysex[3] = 0x1F;
        sysex[4] = 0x00;
        sysex[5] = 0x63;
        sysex[6] = 0x20;
        sysex[7] = 0x01;
        sysex[8] = (byte)presetNumber;
        sysex[10] = 0x50; // 'P'
        sysex[520] = 0xF7;
        return Preset.FromSysEx(sysex).Value;
    }
}
