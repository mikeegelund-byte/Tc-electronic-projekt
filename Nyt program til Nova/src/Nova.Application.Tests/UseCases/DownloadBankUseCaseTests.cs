using FluentAssertions;
using FluentResults;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Nova.Midi;
using Xunit;

namespace Nova.Application.Tests.UseCases;

public class DownloadBankUseCaseTests
{
    [Fact]
    public async Task Execute_WhenAllPresetsReceived_ReturnsUserBankDump()
    {
        // Arrange
        var midi = new Mock<IMidiPort>();
        var presets = new List<byte[]>();

        // Generate 60 valid preset SysEx messages
        for (int i = 0; i < 60; i++)
        {
            presets.Add(CreateDummyPreset(31 + i)); // User Bank presets 31-90
        }

        // Setup mock to stream these presets
        midi.Setup(m => m.ReceiveSysExAsync(It.IsAny<CancellationToken>()))
            .Returns(presets.ToAsyncEnumerable());

        var useCase = new DownloadBankUseCase(midi.Object);

        // Act
        var result = await useCase.ExecuteAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Presets.Should().HaveCount(60);
        result.Value.Presets[0].Should().NotBeNull();
        result.Value.Presets[0]!.Number.Should().Be(31);
    }
    
    private byte[] CreateDummyPreset(int presetNumber)
    {
        // Structure based on 06-sysex-formats.md (520 bytes)
        // F0 00 20 1F 00 63 20 01 [Num] [Name:24] [Tap:4] ... [CS] F7
        var bytes = new byte[520];
        bytes[0] = 0xF0;
        bytes[1] = 0x00; bytes[2] = 0x20; bytes[3] = 0x1F; // Header
        bytes[4] = 0x00; bytes[5] = 0x63;
        bytes[6] = 0x20; bytes[7] = 0x01;
        bytes[8] = (byte)presetNumber;
        
        // Fill remaining payload (0s are valid for most parameters)
        
        // Checksum: Sum bytes 34 to 517
        int sum = 0;
        for (int i = 34; i <= 517; i++) sum += bytes[i];
        
        bytes[518] = (byte)(sum & 0x7F);
        bytes[519] = 0xF7;
        
        return bytes;
    }
}

// Extension to help mocking IAsyncEnumerable
public static class TestExtensions
{
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> source)
    {
        foreach (var item in source)
        {
            await Task.Yield();
            yield return item;
        }
    }
}
