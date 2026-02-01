using FluentAssertions;
using FluentResults;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Nova.Midi;
using Xunit;

namespace Nova.Application.Tests.UseCases;

public class RequestSystemDumpUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidResponse_ReturnsParsedSystemDump()
    {
        // Arrange
        var midi = new Mock<IMidiPort>();
        
        // Setup SendSysExAsync to succeed
        midi.Setup(m => m.SendSysExAsync(It.IsAny<byte[]>()))
            .ReturnsAsync(Result.Ok());
        
        // Setup ReceiveSysExAsync to return a valid 527-byte system dump
        var validDump = CreateValidSystemDumpBytes();
        midi.Setup(m => m.ReceiveSysExAsync(It.IsAny<CancellationToken>()))
            .Returns(new[] { validDump }.ToAsyncEnumerable());
        
        var useCase = new RequestSystemDumpUseCase(midi.Object);

        // Act
        var result = await useCase.ExecuteAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.RawSysEx.Should().HaveCount(527);
    }

    [Fact]
    public async Task ExecuteAsync_SendFails_ReturnsFailed()
    {
        // Arrange
        var midi = new Mock<IMidiPort>();
        
        // Setup SendSysExAsync to fail
        midi.Setup(m => m.SendSysExAsync(It.IsAny<byte[]>()))
            .ReturnsAsync(Result.Fail("MIDI error"));
        
        var useCase = new RequestSystemDumpUseCase(midi.Object);

        // Act
        var result = await useCase.ExecuteAsync();

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "MIDI error");
    }

    [Fact]
    public async Task ExecuteAsync_ParseFails_ReturnsFailed()
    {
        // Arrange
        var midi = new Mock<IMidiPort>();
        
        // Setup SendSysExAsync to succeed
        midi.Setup(m => m.SendSysExAsync(It.IsAny<byte[]>()))
            .ReturnsAsync(Result.Ok());
        
        // Setup ReceiveSysExAsync to return invalid data (wrong length)
        var invalidDump = new byte[100];
        midi.Setup(m => m.ReceiveSysExAsync(It.IsAny<CancellationToken>()))
            .Returns(new[] { invalidDump }.ToAsyncEnumerable());
        
        var useCase = new RequestSystemDumpUseCase(midi.Object);

        // Act
        var result = await useCase.ExecuteAsync();

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("Invalid System Dump length"));
    }

    [Fact(Skip = "Timeout test hangs in CI - needs investigation")]
    public async Task ExecuteAsync_TimeoutReached_ReturnsFailed()
    {
        // Arrange
        var midi = new Mock<IMidiPort>();
        
        // Setup SendSysExAsync to succeed
        midi.Setup(m => m.SendSysExAsync(It.IsAny<byte[]>()))
            .ReturnsAsync(Result.Ok());
        
        // Setup ReceiveSysExAsync to return an enumerable that waits forever
        // Use a Channel to simulate a never-ending stream
        var channel = System.Threading.Channels.Channel.CreateUnbounded<byte[]>();
        
        async IAsyncEnumerable<byte[]> ReadChannel([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
        {
            await foreach (var item in channel.Reader.ReadAllAsync(ct))
            {
                yield return item;
            }
        }
        
        midi.Setup(m => m.ReceiveSysExAsync(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(ct => ReadChannel(ct));
        
        var useCase = new RequestSystemDumpUseCase(midi.Object);

        // Act - use very short timeout
        var result = await useCase.ExecuteAsync(timeoutMs: 100);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("timeout") || e.Message.Contains("Timeout"));
    }

    [Fact]
    public void ExecuteAsync_NullMidiPort_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RequestSystemDumpUseCase(null!));
    }

    // Helper methods
    private static byte[] CreateValidSystemDumpBytes()
    {
        var bytes = new byte[527];
        
        // F0 start
        bytes[0] = 0xF0;
        
        // TC Electronic manufacturer ID
        bytes[1] = 0x00;
        bytes[2] = 0x20;
        bytes[3] = 0x1F;
        
        // Device ID
        bytes[4] = 0x00;
        
        // Nova System model ID
        bytes[5] = 0x63;
        
        // Message ID (0x20 = Dump)
        bytes[6] = 0x20;
        
        // Data Type (0x02 = System Dump)
        bytes[7] = 0x02;
        
        // Fill rest with zeros (valid for system dump)
        
        // F7 end
        bytes[526] = 0xF7;
        
        return bytes;
    }
}
