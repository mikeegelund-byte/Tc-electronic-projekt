using FluentAssertions;
using FluentResults;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Nova.Midi;
using Xunit;

namespace Nova.Application.Tests.UseCases;

public class BankManagerUseCaseTests
{
    private const int PresetSize = 521;
    private const int PresetCount = 60;
    private const int FirstPresetNumber = 31;
    private const int LastPresetNumber = 90;

    #region SaveBankUseCase Tests

    [Fact]
    public async Task SaveBank_WithValidBank_CreatesSysExFile()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            var midi = new Mock<IMidiPort>();
            var presets = CreateValidPresets(PresetCount);
            
            midi.Setup(m => m.ReceiveSysExAsync(It.IsAny<CancellationToken>()))
                .Returns(presets.Select(p => p.RawSysEx).ToAsyncEnumerable());

            var useCase = new SaveBankUseCase(midi.Object);

            // Act
            var result = await useCase.ExecuteAsync(tempFile, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            File.Exists(tempFile).Should().BeTrue();
            
            var fileData = await File.ReadAllBytesAsync(tempFile);
            fileData.Length.Should().Be(PresetSize * PresetCount);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task SaveBank_WithEmptyFilePath_ReturnsFailure()
    {
        // Arrange
        var midi = new Mock<IMidiPort>();
        var useCase = new SaveBankUseCase(midi.Object);

        // Act
        var result = await useCase.ExecuteAsync("", CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("File path cannot be empty"));
    }

    [Fact]
    public async Task SaveBank_WithDownloadFailure_ReturnsFailure()
    {
        // Arrange
        var midi = new Mock<IMidiPort>();
        var tempFile = Path.GetTempFileName();
        
        // Setup to return empty enumerable (no presets)
        midi.Setup(m => m.ReceiveSysExAsync(It.IsAny<CancellationToken>()))
            .Returns(new List<byte[]>().ToAsyncEnumerable());

        var useCase = new SaveBankUseCase(midi.Object);

        // Act
        var result = await useCase.ExecuteAsync(tempFile, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        
        // Clean up
        if (File.Exists(tempFile))
            File.Delete(tempFile);
    }

    #endregion

    #region LoadBankUseCase Tests

    [Fact]
    public async Task LoadBank_WithValidSysExFile_SendsAllPresets()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            var presets = CreateValidPresets(PresetCount);
            var sysexData = CreateSysExFile(presets);
            await File.WriteAllBytesAsync(tempFile, sysexData);

            var midi = new Mock<IMidiPort>();
            var sentPresets = new List<byte[]>();
            
            midi.Setup(m => m.SendSysExAsync(It.IsAny<byte[]>()))
                .Callback<byte[]>(data => sentPresets.Add(data))
                .ReturnsAsync(Result.Ok());

            var useCase = new LoadBankUseCase(midi.Object);

            // Act
            var result = await useCase.ExecuteAsync(tempFile, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            sentPresets.Should().HaveCount(PresetCount);
            midi.Verify(m => m.SendSysExAsync(It.IsAny<byte[]>()), Times.Exactly(PresetCount));
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task LoadBank_WithInvalidFileSize_ReturnsFailure()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            // Write invalid size file
            await File.WriteAllBytesAsync(tempFile, new byte[1000]);

            var midi = new Mock<IMidiPort>();
            var useCase = new LoadBankUseCase(midi.Object);

            // Act
            var result = await useCase.ExecuteAsync(tempFile, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Message.Contains("Invalid .syx file size"));
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public async Task LoadBank_WithNonExistentFile_ReturnsFailure()
    {
        // Arrange
        var midi = new Mock<IMidiPort>();
        var useCase = new LoadBankUseCase(midi.Object);

        // Act
        var result = await useCase.ExecuteAsync("/nonexistent/file.syx", CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("File not found"));
    }

    [Fact]
    public async Task LoadBank_WithEmptyFilePath_ReturnsFailure()
    {
        // Arrange
        var midi = new Mock<IMidiPort>();
        var useCase = new LoadBankUseCase(midi.Object);

        // Act
        var result = await useCase.ExecuteAsync("", CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("File path cannot be empty"));
    }

    [Fact]
    public async Task LoadBank_WhenMidiSendFails_ReturnsFailure()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            var presets = CreateValidPresets(PresetCount);
            var sysexData = CreateSysExFile(presets);
            await File.WriteAllBytesAsync(tempFile, sysexData);

            var midi = new Mock<IMidiPort>();
            midi.Setup(m => m.SendSysExAsync(It.IsAny<byte[]>()))
                .ReturnsAsync(Result.Fail("MIDI send error"));

            var useCase = new LoadBankUseCase(midi.Object);

            // Act
            var result = await useCase.ExecuteAsync(tempFile, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Message.Contains("Failed to send preset"));
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    #endregion

    #region Round-Trip Tests

    [Fact]
    public async Task RoundTrip_SaveAndLoad_PreservesPresetData()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            // Setup SaveBank
            var saveMidi = new Mock<IMidiPort>();
            var originalPresets = CreateValidPresets(PresetCount);
            
            saveMidi.Setup(m => m.ReceiveSysExAsync(It.IsAny<CancellationToken>()))
                .Returns(originalPresets.Select(p => p.RawSysEx).ToAsyncEnumerable());

            var saveUseCase = new SaveBankUseCase(saveMidi.Object);

            // Save to file
            var saveResult = await saveUseCase.ExecuteAsync(tempFile, CancellationToken.None);
            saveResult.IsSuccess.Should().BeTrue();

            // Setup LoadBank
            var loadMidi = new Mock<IMidiPort>();
            var sentPresets = new List<byte[]>();
            
            loadMidi.Setup(m => m.SendSysExAsync(It.IsAny<byte[]>()))
                .Callback<byte[]>(data => sentPresets.Add(data))
                .ReturnsAsync(Result.Ok());

            var loadUseCase = new LoadBankUseCase(loadMidi.Object);

            // Load from file
            var loadResult = await loadUseCase.ExecuteAsync(tempFile, CancellationToken.None);

            // Assert
            loadResult.IsSuccess.Should().BeTrue();
            sentPresets.Should().HaveCount(PresetCount);

            // Verify each preset matches
            for (int i = 0; i < PresetCount; i++)
            {
                sentPresets[i].Should().BeEquivalentTo(originalPresets[i].RawSysEx,
                    $"Preset {i + FirstPresetNumber} should be identical after round-trip");
            }
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    #endregion

    #region Helper Methods

    private List<Preset> CreateValidPresets(int count)
    {
        var presets = new List<Preset>();
        for (int i = 0; i < count; i++)
        {
            var sysex = CreateDummyPreset(FirstPresetNumber + i);
            var result = Preset.FromSysEx(sysex);
            result.IsSuccess.Should().BeTrue($"Test setup failed: could not create preset {FirstPresetNumber + i}");
            presets.Add(result.Value);
        }
        return presets;
    }

    private byte[] CreateDummyPreset(int presetNumber)
    {
        // Create a valid 521-byte preset SysEx message
        var bytes = new byte[PresetSize];
        
        // SysEx header
        bytes[0] = 0xF0;
        bytes[1] = 0x00; bytes[2] = 0x20; bytes[3] = 0x1F; // TC Electronic manufacturer ID
        bytes[4] = 0x00; // Device ID
        bytes[5] = 0x63; // Nova System model ID
        bytes[6] = 0x20; // Dump message
        bytes[7] = 0x01; // Preset data type
        bytes[8] = (byte)presetNumber;
        
        // Preset name (bytes 9-32)
        var name = $"Test Preset {presetNumber}".PadRight(24);
        var nameBytes = System.Text.Encoding.ASCII.GetBytes(name.Substring(0, 24));
        Array.Copy(nameBytes, 0, bytes, 9, 24);
        
        // Fill remaining payload with zeros (valid for most parameters)
        // The Preset.FromSysEx will decode these as default values
        
        // SysEx terminator
        bytes[520] = 0xF7;
        
        return bytes;
    }

    private byte[] CreateSysExFile(List<Preset> presets)
    {
        var sysexData = new byte[PresetSize * presets.Count];
        for (int i = 0; i < presets.Count; i++)
        {
            var presetSysEx = presets[i].RawSysEx;
            Array.Copy(presetSysEx, 0, sysexData, i * PresetSize, PresetSize);
        }
        return sysexData;
    }

    #endregion
}
