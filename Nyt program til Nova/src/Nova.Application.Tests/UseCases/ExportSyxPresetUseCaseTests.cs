using FluentAssertions;
using FluentResults;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Serilog;
using Xunit;

namespace Nova.Application.Tests.UseCases;

public sealed class ExportSyxPresetUseCaseTests : IDisposable
{
    private readonly Mock<ILogger> _mockLogger;
    private readonly ExportSyxPresetUseCase _useCase;
    private readonly string _tempDir;

    public ExportSyxPresetUseCaseTests()
    {
        _mockLogger = new Mock<ILogger>();
        _useCase = new ExportSyxPresetUseCase(_mockLogger.Object);
        _tempDir = Path.Combine(Path.GetTempPath(), $"nova_test_{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDir);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidPreset_ExportsToFile()
    {
        // Arrange
        var preset = CreateValidPreset("Test Preset");
        var filePath = Path.Combine(_tempDir, "test_export.syx");

        // Act
        var result = await _useCase.ExecuteAsync(preset, filePath);

        // Assert
        result.IsSuccess.Should().BeTrue();
        File.Exists(filePath).Should().BeTrue();
        var fileBytes = File.ReadAllBytes(filePath);
        fileBytes.Should().HaveCount(521);  // SysEx preset size
        fileBytes[0].Should().Be(0xF0);     // SysEx start
        fileBytes[520].Should().Be(0xF7);   // SysEx end
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyFilePath_ReturnsFailed()
    {
        // Arrange
        var preset = CreateValidPreset("Test");

        // Act
        var result = await _useCase.ExecuteAsync(preset, "");

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("cannot be empty");
    }

    [Fact]
    public async Task ExecuteAsync_WithNullFilePath_ReturnsFailed()
    {
        // Arrange
        var preset = CreateValidPreset("Test");

        // Act
        var result = await _useCase.ExecuteAsync(preset, null!);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle()
            .Which.Message.Should().Contain("cannot be empty");
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidDirectory_CreatesDirectory()
    {
        // Arrange
        var preset = CreateValidPreset("Test");
        var nonExistentDir = Path.Combine(_tempDir, "subdir", "nested");
        var filePath = Path.Combine(nonExistentDir, "export.syx");

        // Act
        var result = await _useCase.ExecuteAsync(preset, filePath);

        // Assert
        result.IsSuccess.Should().BeTrue();
        Directory.Exists(nonExistentDir).Should().BeTrue();
        File.Exists(filePath).Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_ExportsMultiplePresets_EachHasCorrectData()
    {
        // Arrange
        var preset1 = CreateValidPreset("Preset 1");
        var preset2 = CreateValidPreset("Preset 2");
        var file1 = Path.Combine(_tempDir, "preset1.syx");
        var file2 = Path.Combine(_tempDir, "preset2.syx");

        // Act
        var result1 = await _useCase.ExecuteAsync(preset1, file1);
        var result2 = await _useCase.ExecuteAsync(preset2, file2);

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeTrue();
        File.Exists(file1).Should().BeTrue();
        File.Exists(file2).Should().BeTrue();
        
        // Both files should be valid SysEx
        var bytes1 = File.ReadAllBytes(file1);
        var bytes2 = File.ReadAllBytes(file2);
        bytes1.Should().HaveCount(521);
        bytes2.Should().HaveCount(521);
    }

    private static Preset CreateValidPreset(string name)
    {
        return TestHelpers.CreateValidPreset(31, name);
    }

    public void Dispose()
    {
        // Cleanup temp directory
        if (Directory.Exists(_tempDir))
        {
            try { Directory.Delete(_tempDir, true); }
            catch { /* Ignore cleanup errors */ }
        }
    }
}

