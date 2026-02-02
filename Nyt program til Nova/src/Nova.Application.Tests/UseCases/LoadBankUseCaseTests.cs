using FluentAssertions;
using FluentResults;
using Moq;
using Nova.Application.UseCases;
using Nova.Midi;
using Serilog;
using Xunit;

namespace Nova.Application.Tests.UseCases;

public class LoadBankUseCaseTests
{
    private readonly Mock<IMidiPort> _midiPort;
    private readonly Mock<ILogger> _logger;
    private readonly LoadBankUseCase _useCase;
    private readonly string _testFilePath;

    public LoadBankUseCaseTests()
    {
        _midiPort = new Mock<IMidiPort>();
        _logger = new Mock<ILogger>();
        _useCase = new LoadBankUseCase(_midiPort.Object, _logger.Object);
        _testFilePath = Path.Combine(Path.GetTempPath(), $"test_bank_{Guid.NewGuid()}.syx");
    }

    [Fact]
    public async Task LoadBank_ValidFile_SendsAllPresets()
    {
        // Arrange
        var fileData = new List<byte>();
        for (int i = 0; i < 60; i++)
        {
            fileData.AddRange(CreateDummyPreset(31 + i));
        }

        await File.WriteAllBytesAsync(_testFilePath, fileData.ToArray());

        _midiPort.Setup(m => m.SendSysExAsync(It.IsAny<byte[]>()))
            .ReturnsAsync(Result.Ok());

        var progressReports = new List<int>();
        var progress = new Progress<int>(p => progressReports.Add(p));

        // Act
        var result = await _useCase.ExecuteAsync(_testFilePath, progress, CancellationToken.None);

        // Give Progress<T> time to complete callbacks (it posts to SynchronizationContext)
        await Task.Delay(100);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(60);
        
        // Verify all 60 presets were sent
        _midiPort.Verify(m => m.SendSysExAsync(It.IsAny<byte[]>()), Times.Exactly(60));
        
        // Verify progress was reported (at least some reports, and final should be 60)
        progressReports.Should().NotBeEmpty();
        progressReports.Last().Should().Be(60);

        // Cleanup
        File.Delete(_testFilePath);
    }

    [Fact]
    public async Task LoadBank_InvalidFile_ReturnsFailed()
    {
        // Arrange - use a non-existent file
        var nonExistentFile = "/nonexistent/file.syx";

        // Act
        var result = await _useCase.ExecuteAsync(nonExistentFile, null, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors[0].Message.Should().Contain("File not found");
    }

    [Fact]
    public async Task LoadBank_MidiError_ReturnsFailed()
    {
        // Arrange
        var fileData = new List<byte>();
        for (int i = 0; i < 60; i++)
        {
            fileData.AddRange(CreateDummyPreset(31 + i));
        }

        await File.WriteAllBytesAsync(_testFilePath, fileData.ToArray());

        // Setup MIDI port to fail on sending
        _midiPort.Setup(m => m.SendSysExAsync(It.IsAny<byte[]>()))
            .ReturnsAsync(Result.Fail("MIDI send failed"));

        // Act
        var result = await _useCase.ExecuteAsync(_testFilePath, null, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors[0].Message.Should().Contain("Failed to send preset");

        // Cleanup
        File.Delete(_testFilePath);
    }

    [Fact]
    public async Task LoadBank_InvalidFileSize_ReturnsFailed()
    {
        // Arrange - create file with wrong size
        var invalidData = new byte[100]; // Wrong size
        await File.WriteAllBytesAsync(_testFilePath, invalidData);

        // Act
        var result = await _useCase.ExecuteAsync(_testFilePath, null, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors[0].Message.Should().Contain("Invalid file size");

        // Cleanup
        File.Delete(_testFilePath);
    }

    private byte[] CreateDummyPreset(int presetNumber)
    {
        // Create a valid 521-byte preset SysEx message
        var bytes = new byte[521];
        bytes[0] = 0xF0;  // SysEx start
        bytes[1] = 0x00;  // TC Electronic manufacturer ID
        bytes[2] = 0x20;
        bytes[3] = 0x1F;
        bytes[4] = 0x00;  // Device ID
        bytes[5] = 0x63;  // Nova System model ID
        bytes[6] = 0x20;  // Dump message
        bytes[7] = 0x01;  // Preset data type
        bytes[8] = (byte)presetNumber;  // Preset number
        
        // Name (bytes 9-32): 24 ASCII characters
        var name = $"Preset {presetNumber}".PadRight(24);
        var nameBytes = System.Text.Encoding.ASCII.GetBytes(name);
        Array.Copy(nameBytes, 0, bytes, 9, Math.Min(24, nameBytes.Length));
        
        // Fill remaining bytes with valid data (zeros are valid for most parameters)
        
        bytes[520] = 0xF7;  // SysEx end
        
        return bytes;
    }
}
