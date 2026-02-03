using FluentAssertions;
using FluentResults;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Serilog;
using Xunit;

namespace Nova.Application.Tests.UseCases;

public class ExportPresetUseCaseTests
{
    private readonly Mock<ILogger> _mockLogger;
    private readonly ExportPresetUseCase _useCase;

    public ExportPresetUseCaseTests()
    {
        _mockLogger = new Mock<ILogger>();
        _useCase = new ExportPresetUseCase(_mockLogger.Object);
    }

    [Fact]
    public async Task Export_ValidPreset_CreatesFile()
    {
        // Arrange
        var preset = CreateTestPreset(42, "Test Preset");
        var filePath = Path.Combine(Path.GetTempPath(), $"test_preset_{Guid.NewGuid()}.txt");

        try
        {
            // Act
            var result = await _useCase.ExecuteAsync(preset, filePath, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(filePath);
            File.Exists(filePath).Should().BeTrue();

            // Verify file content contains key fields
            var content = await File.ReadAllTextAsync(filePath);
            content.Should().Contain("Preset Number: 42");
            content.Should().Contain("Preset Name: Test Preset");
            content.Should().Contain("Tap Tempo:");
            content.Should().Contain("Compressor Enabled:");
        }
        finally
        {
            // Cleanup
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public async Task Export_InvalidPath_ReturnsFailed()
    {
        // Arrange
        var preset = CreateTestPreset(42, "Test Preset");
        var invalidPath = ""; // Empty path

        // Act
        var result = await _useCase.ExecuteAsync(preset, invalidPath, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("File path cannot be empty"));
    }

    [Fact]
    public async Task Export_NullPreset_ReturnsFailed()
    {
        // Arrange
        Preset? preset = null;
        var filePath = Path.Combine(Path.GetTempPath(), $"test_preset_{Guid.NewGuid()}.txt");

        // Act
        var result = await _useCase.ExecuteAsync(preset!, filePath, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("Preset cannot be null"));
    }

    [Fact]
    public async Task Export_AllParameters_IncludedInFile()
    {
        // Arrange
        var preset = CreateTestPreset(99, "Full Test");
        var filePath = Path.Combine(Path.GetTempPath(), $"test_preset_{Guid.NewGuid()}.txt");

        try
        {
            // Act
            var result = await _useCase.ExecuteAsync(preset, filePath, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            
            var content = await File.ReadAllTextAsync(filePath);
            
            // Check all major parameter sections are present
            content.Should().Contain("# === BASIC INFO ===");
            content.Should().Contain("# === GLOBAL PARAMETERS ===");
            content.Should().Contain("# === EFFECT SWITCHES ===");
            content.Should().Contain("# === COMPRESSOR ===");
            content.Should().Contain("# === DRIVE ===");
            content.Should().Contain("# === BOOST ===");
            content.Should().Contain("# === MODULATION ===");
            content.Should().Contain("# === DELAY ===");
            content.Should().Contain("# === REVERB ===");
            content.Should().Contain("# === GATE ===");
            content.Should().Contain("# === EQ ===");
            content.Should().Contain("# === PITCH ===");

            // Verify specific parameters from each section
            content.Should().Contain("Tap Tempo:");
            content.Should().Contain("Routing:");
            content.Should().Contain("Comp Type:");
            content.Should().Contain("Drive Gain:");
            content.Should().Contain("Boost Level:");
            content.Should().Contain("Mod Type:");
            content.Should().Contain("Delay Type:");
            content.Should().Contain("Reverb Type:");
            content.Should().Contain("Gate Type:");
            content.Should().Contain("EQ Freq 1:");
            content.Should().Contain("Pitch Type:");
        }
        finally
        {
            // Cleanup
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public async Task Export_CreatesDirectoryIfNotExists()
    {
        // Arrange
        var preset = CreateTestPreset(42, "Test Preset");
        var tempDir = Path.Combine(Path.GetTempPath(), $"nova_test_{Guid.NewGuid()}");
        var filePath = Path.Combine(tempDir, "preset.txt");

        try
        {
            // Act
            var result = await _useCase.ExecuteAsync(preset, filePath, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            Directory.Exists(tempDir).Should().BeTrue();
            File.Exists(filePath).Should().BeTrue();
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    private Preset CreateTestPreset(int number, string name)
    {
        // Create a minimal valid SysEx message for testing
        var sysex = new byte[520];
        
        // Header
        sysex[0] = 0xF0;
        sysex[1] = 0x00;
        sysex[2] = 0x20;
        sysex[3] = 0x1F;
        sysex[4] = 0x00;
        sysex[5] = 0x63;
        sysex[6] = 0x20;
        sysex[7] = 0x01;
        
        // Preset number
        sysex[8] = (byte)number;
        
        // Preset name (24 ASCII chars)
        var nameBytes = System.Text.Encoding.ASCII.GetBytes(name.PadRight(24));
        Array.Copy(nameBytes, 0, sysex, 9, 24);
        
        // End marker
        sysex[519] = 0xF7;
        
        var presetResult = Preset.FromSysEx(sysex);
        return presetResult.Value;
    }
}
