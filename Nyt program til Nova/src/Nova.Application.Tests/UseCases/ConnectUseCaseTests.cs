using FluentAssertions;
using FluentResults;
using Moq;
using Nova.Application.UseCases;
using Nova.Midi;
using Xunit;

namespace Nova.Application.Tests.UseCases;

public class ConnectUseCaseTests
{
    [Fact]
    public async Task Execute_WhenPortConnects_ReturnsSuccess()
    {
        // Arrange
        var midi = new Mock<IMidiPort>();
        var selection = new MidiPortSelection("Nova System MIDI 1", "Nova System MIDI 0");
        midi.Setup(m => m.ConnectAsync(It.Is<MidiPortSelection>(s =>
                s.InputPortName == selection.InputPortName && s.OutputPortName == selection.OutputPortName)))
            .ReturnsAsync(Result.Ok());

        var useCase = new ConnectUseCase(midi.Object);

        // Act
        var result = await useCase.ExecuteAsync(selection);

        // Assert
        result.IsSuccess.Should().BeTrue();
        midi.Verify(m => m.ConnectAsync(It.IsAny<MidiPortSelection>()), Times.Once);
    }

    [Fact]
    public async Task Execute_WhenPortFails_ReturnsFailure()
    {
        // Arrange
        var midi = new Mock<IMidiPort>();
        var selection = new MidiPortSelection("Bad In", "Bad Out");
        midi.Setup(m => m.ConnectAsync(It.IsAny<MidiPortSelection>()))
            .ReturnsAsync(Result.Fail("Not found"));

        var useCase = new ConnectUseCase(midi.Object);

        // Act
        var result = await useCase.ExecuteAsync(selection);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Not found");
    }
}
