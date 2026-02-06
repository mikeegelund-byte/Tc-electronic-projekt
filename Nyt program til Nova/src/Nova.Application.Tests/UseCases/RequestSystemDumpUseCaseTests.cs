using FluentResults;
using FluentAssertions;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Nova.Midi;
using Nova.Midi.Tests;

namespace Nova.Application.Tests.UseCases;

public class RequestSystemDumpUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidResponse_ReturnsParsedSystemDump()
    {
        // Arrange
        var mockPort = new MockMidiPort();
        await mockPort.ConnectAsync(new MidiPortSelection("Mock In", "Mock Out"));  // Must connect first
        mockPort.EnqueueResponse(CreateValidSystemDumpBytes());

        var useCase = new RequestSystemDumpUseCase(mockPort);

        // Act
        var result = await useCase.ExecuteAsync(cancellationToken: CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task ExecuteAsync_SendFails_ReturnsFailed()
    {
        // Arrange - create a mock that always fails on send
        var mockPort = new FailingSendMockMidiPort();

        var useCase = new RequestSystemDumpUseCase(mockPort);

        // Act
        var result = await useCase.ExecuteAsync();

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_TimeoutReached_ReturnsFailed()
    {
        // Arrange - mock that never returns data
        var mockPort = new MockMidiPort();
        // Don't enqueue any response - will timeout

        var useCase = new RequestSystemDumpUseCase(mockPort);

        // Act
        var result = await useCase.ExecuteAsync(timeoutMs: 500);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    // Helper to create valid SystemDump SysEx bytes (527 bytes)
    private static byte[] CreateValidSystemDumpBytes()
    {
        var sysex = new byte[527];
        sysex[0] = 0xF0;
        sysex[1] = 0x00; sysex[2] = 0x20; sysex[3] = 0x1F; // TC Electronic
        sysex[4] = 0x00; // Device ID
        sysex[5] = 0x63; // Nova System
        sysex[6] = 0x20; // Dump message type
        sysex[7] = 0x02; // System dump data type
        // Rest can be zeros - validation only checks header
        sysex[526] = 0xF7;
        return sysex;
    }
}

/// <summary>
/// Mock MIDI port that fails on send for testing error handling
/// </summary>
public class FailingSendMockMidiPort : IMidiPort
{
    public string Name => "Failing Mock Port";
    public string? InputPortName => "Mock In";
    public string? OutputPortName => "Mock Out";
    public bool IsConnected => true;

    public Task<Result> ConnectAsync(MidiPortSelection selection) => Task.FromResult(Result.Ok());
    public Task<Result> DisconnectAsync() => Task.FromResult(Result.Ok());
    
    public Task<Result> SendSysExAsync(byte[] sysex)
        => Task.FromResult(Result.Fail("MIDI error"));

    public IAsyncEnumerable<byte[]> ReceiveSysExAsync(CancellationToken cancellationToken = default)
    {
#pragma warning disable CS1998
        return new AsyncEnumerableEmpty();
#pragma warning restore CS1998
    }

    public IAsyncEnumerable<byte[]> ReceiveCCAsync(CancellationToken cancellationToken = default)
    {
#pragma warning disable CS1998
        return new AsyncEnumerableEmpty();
#pragma warning restore CS1998
    }

    public IReadOnlyList<string> GetAvailableInputPorts()
        => new[] { "Mock In" };

    public IReadOnlyList<string> GetAvailableOutputPorts()
        => new[] { "Mock Out" };

    private class AsyncEnumerableEmpty : IAsyncEnumerable<byte[]>
    {
        public IAsyncEnumerator<byte[]> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new AsyncEnumeratorEmpty();
        }

        private class AsyncEnumeratorEmpty : IAsyncEnumerator<byte[]>
        {
            public byte[] Current => null!;
            public ValueTask<bool> MoveNextAsync() => ValueTask.FromResult(false);
            public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        }
    }
}
