using FluentAssertions;
using FluentResults;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Nova.Midi;
using Serilog;
using Xunit;

namespace Nova.Application.Tests.UseCases;

public class SaveBankUseCaseTests
{
    private readonly Mock<IDownloadBankUseCase> _downloadBankUseCase;
    private readonly Mock<ILogger> _logger;
    private readonly SaveBankUseCase _useCase;
    private readonly string _testFilePath;

    public SaveBankUseCaseTests()
    {
        _downloadBankUseCase = new Mock<IDownloadBankUseCase>();
        _logger = new Mock<ILogger>();
        _useCase = new SaveBankUseCase(_downloadBankUseCase.Object, _logger.Object);
        _testFilePath = Path.Combine(Path.GetTempPath(), $"test_bank_{Guid.NewGuid()}.syx");
    }

    [Fact]
    public async Task SaveBank_WithValidBank_CreatesFile()
    {
        // Arrange
        var presets = new List<Preset>();
        for (int i = 0; i < 60; i++)
        {
            var presetData = CreateDummyPreset(31 + i);
            var preset = Preset.FromSysEx(presetData).Value;
            presets.Add(preset);
        }

        var userBank = UserBankDump.FromPresets(presets).Value;
        _downloadBankUseCase.Setup(d => d.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(userBank));

        // Act
        var result = await _useCase.ExecuteAsync(_testFilePath, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(_testFilePath);
        File.Exists(_testFilePath).Should().BeTrue();

        var fileContent = await File.ReadAllBytesAsync(_testFilePath);
        fileContent.Length.Should().Be(31260); // 60 presets * 521 bytes

        // Cleanup
        File.Delete(_testFilePath);
    }

    [Fact]
    public async Task SaveBank_InvalidPath_ReturnsFailed()
    {
        // Arrange - use an invalid path (directory doesn't exist)
        var invalidPath = "/nonexistent/directory/test.syx";
        
        var presets = new List<Preset>();
        for (int i = 0; i < 60; i++)
        {
            var presetData = CreateDummyPreset(31 + i);
            var preset = Preset.FromSysEx(presetData).Value;
            presets.Add(preset);
        }

        var userBank = UserBankDump.FromPresets(presets).Value;
        _downloadBankUseCase.Setup(d => d.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(userBank));

        // Act
        var result = await _useCase.ExecuteAsync(invalidPath, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SaveBank_FileExists_Overwrites()
    {
        // Arrange
        // Create an existing file
        await File.WriteAllTextAsync(_testFilePath, "old content");
        File.Exists(_testFilePath).Should().BeTrue();

        var presets = new List<Preset>();
        for (int i = 0; i < 60; i++)
        {
            var presetData = CreateDummyPreset(31 + i);
            var preset = Preset.FromSysEx(presetData).Value;
            presets.Add(preset);
        }

        var userBank = UserBankDump.FromPresets(presets).Value;
        _downloadBankUseCase.Setup(d => d.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(userBank));

        // Act
        var result = await _useCase.ExecuteAsync(_testFilePath, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(_testFilePath);
        
        var fileContent = await File.ReadAllBytesAsync(_testFilePath);
        fileContent.Length.Should().Be(31260); // New content, not old

        // Cleanup
        File.Delete(_testFilePath);
    }

    private byte[] CreateDummyPreset(int presetNumber)
    {
        return TestHelpers.CreateValidPresetSysEx(presetNumber, $"Preset {presetNumber}");
    }
}
