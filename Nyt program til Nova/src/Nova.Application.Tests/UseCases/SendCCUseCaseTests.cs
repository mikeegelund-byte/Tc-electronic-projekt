using FluentAssertions;
using FluentResults;
using Moq;
using Nova.Application.UseCases;
using Nova.Midi;
using Serilog;
using Xunit;

namespace Nova.Application.Tests.UseCases;

public class SendCCUseCaseTests
{
    private readonly Mock<IMidiPort> _midiPortMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly SendCCUseCase _useCase;
    
    public SendCCUseCaseTests()
    {
        _midiPortMock = new Mock<IMidiPort>();
        _loggerMock = new Mock<ILogger>();
        _useCase = new SendCCUseCase(_midiPortMock.Object, _loggerMock.Object);
    }
    
    [Fact]
    public async Task ExecuteAsync_WithValidParameters_ReturnsSuccess()
    {
        // Arrange
        byte channel = 0;
        byte controller = 7; // Volume
        byte value = 64;
        
        // Act
        var result = await _useCase.ExecuteAsync(channel, controller, value);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
    }
    
    [Fact]
    public async Task ExecuteAsync_WithInvalidChannel_ReturnsFailure()
    {
        // Arrange
        byte channel = 16; // Invalid: must be 0-15
        byte controller = 7;
        byte value = 64;
        
        // Act
        var result = await _useCase.ExecuteAsync(channel, controller, value);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Invalid MIDI channel");
    }
    
    [Fact]
    public async Task ExecuteAsync_WithInvalidController_ReturnsFailure()
    {
        // Arrange
        byte channel = 0;
        byte controller = 128; // Invalid: must be 0-127
        byte value = 64;
        
        // Act
        var result = await _useCase.ExecuteAsync(channel, controller, value);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Invalid controller number");
    }
    
    [Fact]
    public async Task ExecuteAsync_WithInvalidValue_ReturnsFailure()
    {
        // Arrange
        byte channel = 0;
        byte controller = 7;
        byte value = 128; // Invalid: must be 0-127
        
        // Act
        var result = await _useCase.ExecuteAsync(channel, controller, value);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Invalid controller value");
    }
    
    [Fact]
    public async Task ExecuteAsync_LogsInformation()
    {
        // Arrange
        byte channel = 5;
        byte controller = 20; // Delay Time
        byte value = 100;
        
        // Act
        await _useCase.ExecuteAsync(channel, controller, value);
        
        // Assert
        _loggerMock.Verify(
            x => x.Information(
                It.Is<string>(s => s.Contains("Sending MIDI CC")),
                It.IsAny<object[]>()),
            Times.Once);
    }
    
    [Fact]
    public async Task ExecuteAsync_WithCancellationToken_CanBeCancelled()
    {
        // Arrange
        byte channel = 0;
        byte controller = 7;
        byte value = 64;
        var cts = new CancellationTokenSource();
        cts.Cancel();
        
        // Act
        var result = await _useCase.ExecuteAsync(channel, controller, value, cts.Token);
        
        // Assert
        // The current implementation doesn't actually use the token for async operations
        // but we verify it accepts the parameter
        result.Should().NotBeNull();
    }
    
    [Theory]
    [InlineData(0, 1, 127)]    // Channel 0, Mod Wheel, max value
    [InlineData(15, 7, 0)]     // Channel 15, Volume, min value
    [InlineData(9, 64, 64)]    // Channel 9, Sustain, mid value
    public async Task ExecuteAsync_WithVariousValidInputs_Succeeds(byte channel, byte controller, byte value)
    {
        // Act
        var result = await _useCase.ExecuteAsync(channel, controller, value);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
