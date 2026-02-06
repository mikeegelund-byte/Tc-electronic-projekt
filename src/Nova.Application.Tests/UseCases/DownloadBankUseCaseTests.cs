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
        return TestHelpers.CreateValidPresetSysEx(presetNumber, $"Preset {presetNumber}");
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
