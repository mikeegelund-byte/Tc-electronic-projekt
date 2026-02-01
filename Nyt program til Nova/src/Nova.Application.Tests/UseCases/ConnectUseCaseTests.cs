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
        midi.Setup(m => m.ConnectAsync("Nova System")).ReturnsAsync(Result.Ok());

        var useCase = new ConnectUseCase(midi.Object);

        // Act
        var result = await useCase.ExecuteAsync("Nova System");

        // Assert
        result.IsSuccess.Should().BeTrue();
        midi.Verify(m => m.ConnectAsync("Nova System"), Times.Once);
    }

    [Fact]
    public async Task Execute_WhenPortFails_ReturnsFailure()
    {
        // Arrange
        var midi = new Mock<IMidiPort>();
        midi.Setup(m => m.ConnectAsync("Bad Port")).ReturnsAsync(Result.Fail("Not found"));

        var useCase = new ConnectUseCase(midi.Object);

        // Act
        var result = await useCase.ExecuteAsync("Bad Port");

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Not found");
    }
}
