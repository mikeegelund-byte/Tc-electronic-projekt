using FluentAssertions;
using FluentResults;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Nova.Midi;
using Serilog;
using Xunit;

namespace Nova.Application.Tests.UseCases;

public class SendBankToHardwareUseCaseTests
{
    private readonly Mock<IMidiPort> _midiPort;
    private readonly Mock<ILogger> _logger;
    private readonly SendBankToHardwareUseCase _useCase;

    public SendBankToHardwareUseCaseTests()
    {
        _midiPort = new Mock<IMidiPort>();
        _logger = new Mock<ILogger>();
        _useCase = new SendBankToHardwareUseCase(_midiPort.Object, _logger.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidBank_SendsAllPresets()
    {
        // Arrange
        var bank = CreateValidBank();
        _midiPort.Setup(m => m.IsConnected).Returns(true);
        _midiPort.Setup(m => m.SendSysExAsync(It.IsAny<byte[]>()))
            .ReturnsAsync(Result.Ok());

        // Act
        var result = await _useCase.ExecuteAsync(bank, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _midiPort.Verify(m => m.SendSysExAsync(It.IsAny<byte[]>()), Times.Exactly(60));
    }

    [Fact]
    public async Task ExecuteAsync_WhenNotConnected_ReturnsError()
    {
        // Arrange
        var bank = CreateValidBank();
        _midiPort.Setup(m => m.IsConnected).Returns(false);

        // Act
        var result = await _useCase.ExecuteAsync(bank, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Contain("not connected");
    }

    private static UserBankDump CreateValidBank()
    {
        var presets = new List<Preset>();
        for (int i = 0; i < 60; i++)
        {
            presets.Add(TestHelpers.CreateValidPreset(31 + i, $"Preset {i + 1}"));
        }

        return UserBankDump.FromPresets(presets).Value;
    }
}
