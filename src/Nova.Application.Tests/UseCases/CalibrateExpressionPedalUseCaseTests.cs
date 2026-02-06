using FluentResults;
using Nova.Application.UseCases;
using Nova.Midi;
using Nova.Midi.Tests;
using Xunit;

namespace Nova.Application.Tests.UseCases;

public class CalibrateExpressionPedalUseCaseTests
{
    [Fact]
    public async Task CalibrateAsync_WithMultipleCCMessages_ReturnsMinMax()
    {
        // Arrange
        var mockPort = new MockMidiPort();
        await mockPort.ConnectAsync(new MidiPortSelection("Test In", "Test Out"));
        var useCase = new CalibrateExpressionPedalUseCase(mockPort);

        // Simulate pedal sweep: 20, 50, 80, 100, 40
        _ = Task.Run(async () =>
        {
            await Task.Delay(10);
            await mockPort.SendCCAsync(7, 20);  // Min seen: 20
            await Task.Delay(10);
            await mockPort.SendCCAsync(7, 50);
            await Task.Delay(10);
            await mockPort.SendCCAsync(7, 80);
            await Task.Delay(10);
            await mockPort.SendCCAsync(7, 100); // Max seen: 100
            await Task.Delay(10);
            await mockPort.SendCCAsync(7, 40);
        });

        // Act
        var result = await useCase.CalibrateAsync(1000, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var (min, max) = result.Value;
        Assert.Equal(20, min);
        Assert.Equal(100, max);
    }

    [Fact]
    public async Task CalibrateAsync_WithFullRange_Returns0And127()
    {
        // Arrange
        var mockPort = new MockMidiPort();
        await mockPort.ConnectAsync(new MidiPortSelection("Test In", "Test Out"));
        var useCase = new CalibrateExpressionPedalUseCase(mockPort);

        // Simulate full pedal range
        _ = Task.Run(async () =>
        {
            await Task.Delay(10);
            await mockPort.SendCCAsync(7, 0);
            await Task.Delay(10);
            await mockPort.SendCCAsync(7, 127);
        });

        // Act
        var result = await useCase.CalibrateAsync(1000, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var (min, max) = result.Value;
        Assert.Equal(0, min);
        Assert.Equal(127, max);
    }

    [Fact]
    public async Task CalibrateAsync_NotConnected_Fails()
    {
        // Arrange
        var mockPort = new MockMidiPort();
        // Don't connect
        var useCase = new CalibrateExpressionPedalUseCase(mockPort);

        // Act
        var result = await useCase.CalibrateAsync(1000, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("not connected", result.Errors[0].Message);
    }

    [Fact]
    public async Task CalibrateAsync_ZeroTimeout_Fails()
    {
        // Arrange
        var mockPort = new MockMidiPort();
        await mockPort.ConnectAsync(new MidiPortSelection("Test In", "Test Out"));
        var useCase = new CalibrateExpressionPedalUseCase(mockPort);

        // Act
        var result = await useCase.CalibrateAsync(0, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("positive", result.Errors[0].Message);
    }

    [Fact]
    public async Task CalibrateAsync_NoCCReceived_Fails()
    {
        // Arrange
        var mockPort = new MockMidiPort();
        await mockPort.ConnectAsync(new MidiPortSelection("Test In", "Test Out"));
        var useCase = new CalibrateExpressionPedalUseCase(mockPort);

        // Don't send any CC messages

        // Act
        var result = await useCase.CalibrateAsync(100, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("No CC messages received", result.Errors[0].Message);
    }
}
