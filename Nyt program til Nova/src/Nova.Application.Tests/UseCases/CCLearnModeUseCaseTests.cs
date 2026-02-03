using FluentResults;
using Nova.Application.UseCases;
using Nova.Midi.Tests;
using Xunit;

namespace Nova.Application.Tests.UseCases;

public class CCLearnModeUseCaseTests
{
    [Fact]
    public async Task StartLearnAsync_WithIncomingCC_ReturnsCCNumber()
    {
        // Arrange
        var mockPort = new MockMidiPort();
        await mockPort.ConnectAsync("Test Port");
        var useCase = new CCLearnModeUseCase(mockPort);

        // Act: Start learn mode (10 second timeout)
        var learnTask = useCase.StartLearnAsync(10);
        
        // Simulate receiving CC message after 100ms
        await Task.Delay(100);
        await mockPort.SendCCAsync(ccNumber: 7, value: 127);

        var result = await learnTask;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(7, result.Value);
    }

    [Fact]
    public async Task StartLearnAsync_WithTimeout_ReturnsFailure()
    {
        // Arrange
        var mockPort = new MockMidiPort();
        await mockPort.ConnectAsync("Test Port");
        var useCase = new CCLearnModeUseCase(mockPort);

        // Act: Start learn with 1 second timeout, don't send any CC
        var result = await useCase.StartLearnAsync(1);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("No CC message received within 1 seconds", result.Errors.First().Message);
    }

    [Fact]
    public async Task StartLearnAsync_NotConnected_ReturnsFailure()
    {
        // Arrange
        var mockPort = new MockMidiPort();  // Not connected
        var useCase = new CCLearnModeUseCase(mockPort);

        // Act
        var result = await useCase.StartLearnAsync(5);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("MIDI port not connected", result.Errors.First().Message);
    }

    [Fact]
    public async Task StartLearnAsync_WithCancellation_ReturnsFailure()
    {
        // Arrange
        var mockPort = new MockMidiPort();
        await mockPort.ConnectAsync("Test Port");
        var useCase = new CCLearnModeUseCase(mockPort);
        var cts = new CancellationTokenSource();

        // Act: Start learn, then cancel after 100ms
        var learnTask = useCase.StartLearnAsync(10, cts.Token);
        await Task.Delay(100);
        cts.Cancel();

        var result = await learnTask;

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("CC Learn cancelled by user", result.Errors.First().Message);
    }

    [Fact]
    public async Task StartLearnAsync_WithZeroTimeout_ReturnsFailure()
    {
        // Arrange
        var mockPort = new MockMidiPort();
        await mockPort.ConnectAsync("Test Port");
        var useCase = new CCLearnModeUseCase(mockPort);

        // Act
        var result = await useCase.StartLearnAsync(0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Timeout must be greater than 0 seconds", result.Errors.First().Message);
    }

    [Fact]
    public async Task StartLearnAsync_WithMultipleCCMessages_ReturnsFirstCC()
    {
        // Arrange
        var mockPort = new MockMidiPort();
        await mockPort.ConnectAsync("Test Port");
        var useCase = new CCLearnModeUseCase(mockPort);

        // Act: Start learn mode
        var learnTask = useCase.StartLearnAsync(10);
        
        // Send multiple CC messages
        await Task.Delay(50);
        await mockPort.SendCCAsync(ccNumber: 1, value: 64);
        await mockPort.SendCCAsync(ccNumber: 2, value: 127);
        await mockPort.SendCCAsync(ccNumber: 3, value: 0);

        var result = await learnTask;

        // Assert: Should return first CC number received
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value);
    }
}
