using FluentAssertions;
using FluentResults;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Serilog;
using Xunit;

namespace Nova.Application.Tests.UseCases;

public class ImportPresetUseCaseTests
{
    private readonly Mock<ILogger> _mockLogger;
    private readonly ImportPresetUseCase _useCase;

    public ImportPresetUseCaseTests()
    {
        _mockLogger = new Mock<ILogger>();
        _useCase = new ImportPresetUseCase(_mockLogger.Object);
    }

    [Fact]
    public async Task Import_ValidFile_ReturnsPreset()
    {
        // Arrange
        var filePath = Path.Combine(Path.GetTempPath(), $"test_import_{Guid.NewGuid()}.txt");
        var testContent = CreateValidPresetText(42, "Test Import");
        
        try
        {
            await File.WriteAllTextAsync(filePath, testContent);

            // Act
            var result = await _useCase.ExecuteAsync(filePath, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Number.Should().Be(42);
            result.Value.Name.Should().Contain("Test Import");
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public async Task Import_MissingField_ReturnsFailed()
    {
        // Arrange
        var filePath = Path.Combine(Path.GetTempPath(), $"test_import_{Guid.NewGuid()}.txt");
        var incompleteContent = @"# TC Electronic Nova System Preset
# Missing required fields
Tap Tempo: 500
";
        
        try
        {
            await File.WriteAllTextAsync(filePath, incompleteContent);

            // Act
            var result = await _useCase.ExecuteAsync(filePath, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Message.Contains("Missing required field"));
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public async Task Import_InvalidValue_ReturnsFailed()
    {
        // Arrange
        var filePath = Path.Combine(Path.GetTempPath(), $"test_import_{Guid.NewGuid()}.txt");
        var invalidContent = @"# TC Electronic Nova System Preset
Preset Number: not_a_number
Preset Name: Test
Tap Tempo: 500
";
        
        try
        {
            await File.WriteAllTextAsync(filePath, invalidContent);

            // Act
            var result = await _useCase.ExecuteAsync(filePath, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Fact]
    public async Task Import_FileNotFound_ReturnsFailed()
    {
        // Arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), $"nonexistent_{Guid.NewGuid()}.txt");

        // Act
        var result = await _useCase.ExecuteAsync(nonExistentPath, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("File not found"));
    }

    [Fact]
    public async Task Import_RoundTrip_PreservesAllData()
    {
        // Arrange
        var exportPath = Path.Combine(Path.GetTempPath(), $"test_export_{Guid.NewGuid()}.txt");
        var importPath = exportPath;
        
        // Create original preset with specific values
        var originalPreset = CreateTestPreset(77, "RoundTrip Test");
        var exportUseCase = new ExportPresetUseCase(_mockLogger.Object);

        try
        {
            // Export
            var exportResult = await exportUseCase.ExecuteAsync(originalPreset, exportPath, CancellationToken.None);
            exportResult.IsSuccess.Should().BeTrue();

            // Import
            var importResult = await _useCase.ExecuteAsync(importPath, CancellationToken.None);
            importResult.IsSuccess.Should().BeTrue();

            // Assert - verify key fields match
            var importedPreset = importResult.Value;
            importedPreset.Number.Should().Be(originalPreset.Number);
            importedPreset.Name.Trim().Should().Be(originalPreset.Name.Trim());
            
            // Verify various parameter types are preserved
            importedPreset.TapTempo.Should().Be(originalPreset.TapTempo);
            importedPreset.Routing.Should().Be(originalPreset.Routing);
            importedPreset.CompressorEnabled.Should().Be(originalPreset.CompressorEnabled);
            importedPreset.DriveEnabled.Should().Be(originalPreset.DriveEnabled);
            importedPreset.CompType.Should().Be(originalPreset.CompType);
            importedPreset.ModType.Should().Be(originalPreset.ModType);
            importedPreset.DelayType.Should().Be(originalPreset.DelayType);
            importedPreset.ReverbType.Should().Be(originalPreset.ReverbType);
            importedPreset.GateType.Should().Be(originalPreset.GateType);
            importedPreset.PitchType.Should().Be(originalPreset.PitchType);
        }
        finally
        {
            if (File.Exists(exportPath))
            {
                File.Delete(exportPath);
            }
        }
    }

    [Fact]
    public async Task Import_EmptyFilePath_ReturnsFailed()
    {
        // Arrange
        var emptyPath = "";

        // Act
        var result = await _useCase.ExecuteAsync(emptyPath, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("File path cannot be empty"));
    }

    [Fact]
    public async Task Import_WithComments_IgnoresComments()
    {
        // Arrange
        var filePath = Path.Combine(Path.GetTempPath(), $"test_import_{Guid.NewGuid()}.txt");
        var contentWithComments = @"# This is a comment
# Another comment
Preset Number: 50
Preset Name: Comment Test
# More comments
Tap Tempo: 600
# === SECTION HEADER ===
Routing: 1
";
        
        try
        {
            await File.WriteAllTextAsync(filePath, contentWithComments);

            // Act
            var result = await _useCase.ExecuteAsync(filePath, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Number.Should().Be(50);
            result.Value.Name.Should().Contain("Comment Test");
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    private string CreateValidPresetText(int number, string name)
    {
        // Note: Using fixed date in test data - not a real export timestamp
        return $@"# TC Electronic Nova System Preset
# Exported: 2024-01-01 12:00:00 UTC

# === BASIC INFO ===
Preset Number: {number}
Preset Name: {name}

# === GLOBAL PARAMETERS ===
Tap Tempo: 500
Routing: 0
Level Out Left: -10
Level Out Right: -10

# === EFFECT SWITCHES ===
Compressor Enabled: False
Drive Enabled: True
Modulation Enabled: False
Delay Enabled: True
Reverb Enabled: True

# === COMPRESSOR ===
Comp Type: 0
Comp Threshold: -20
Comp Ratio: 4
Comp Attack: 5
Comp Release: 15
Comp Response: 5
Comp Drive: 10
Comp Level: 0

# === DRIVE ===
Drive Type: 1
Drive Gain: 50
Drive Level: -10

# === BOOST ===
Boost Type: 0
Boost Gain: 15
Boost Level: 5

# === MODULATION ===
Mod Type: 0
Mod Speed: 100
Mod Depth: 50
Mod Tempo: 8
Mod Hi Cut: 100
Mod Feedback: 0
Mod Delay Or Range: 10
Mod Mix: 30

# === DELAY ===
Delay Type: 0
Delay Time: 500
Delay Time 2: 600
Delay Tempo: 8
Delay Tempo2 Or Width: 50
Delay Feedback: 50
Delay Clip Or Feedback2: 10
Delay Hi Cut: 100
Delay Lo Cut: 20
Delay Mix: 40

# === REVERB ===
Reverb Type: 1
Reverb Decay: 50
Reverb Pre Delay: 30
Reverb Shape: 1
Reverb Size: 3
Reverb Hi Color: 2
Reverb Hi Level: 0
Reverb Lo Color: 2
Reverb Lo Level: 0
Reverb Room Level: -20
Reverb Level: -10
Reverb Diffuse: 0
Reverb Mix: 35

# === GATE ===
Gate Type: 0
Gate Threshold: -40
Gate Damp: 30
Gate Release: 50

# === EQ ===
EQ Freq 1: 50
EQ Gain 1: 0
EQ Width 1: 10
EQ Freq 2: 100
EQ Gain 2: 3
EQ Width 2: 10
EQ Freq 3: 150
EQ Gain 3: -2
EQ Width 3: 10

# === PITCH ===
Pitch Type: 0
Pitch Voice 1: 0
Pitch Voice 2: 0
Pitch Pan 1: 0
Pitch Pan 2: 0
Pitch Delay 1: 0
Pitch Delay 2: 0
Pitch Feedback1 Or Key: 0
Pitch Feedback2 Or Scale: 0
Pitch Level 1: -10
Pitch Level 2: -10
";
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
