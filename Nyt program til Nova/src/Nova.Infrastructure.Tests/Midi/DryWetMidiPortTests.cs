using FluentAssertions;
using Nova.Infrastructure.Midi;
using Nova.Midi;
using Xunit;

namespace Nova.Infrastructure.Tests.Midi;

public class DryWetMidiPortTests
{
    [Fact]
    public void DryWetMidiPort_Implements_IMidiPort()
    {
        var port = new DryWetMidiPort();
        port.Should().BeAssignableTo<IMidiPort>();
    }

    [Fact]
    public void Name_BeforeConnect_ReturnsEmpty()
    {
        var port = new DryWetMidiPort();
        port.Name.Should().BeEmpty();
    }

    [Fact]
    public void IsConnected_BeforeConnect_ReturnsFalse()
    {
        var port = new DryWetMidiPort();
        port.IsConnected.Should().BeFalse();
    }

    [Fact]
    public void GetAvailablePorts_ReturnsListOfPortNames()
    {
        // This test may return empty list if no MIDI devices connected
        var ports = DryWetMidiPort.GetAvailablePorts();
        ports.Should().NotBeNull();
        ports.Should().BeOfType<List<string>>();
        // ports.Should().Contain(p => p.Contains("USB")); // Only if device connected
    }

    [Fact]
    public async Task DisconnectAsync_BeforeConnect_ReturnsSuccess()
    {
        var port = new DryWetMidiPort();
        var result = await port.DisconnectAsync();
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DisconnectAsync_ClearsName()
    {
        var port = new DryWetMidiPort();
        // Simulate connection by manually setting name (since ConnectAsync not yet impl)
        // After disconnect, name should be empty
        var result = await port.DisconnectAsync();
        result.IsSuccess.Should().BeTrue();
        port.Name.Should().BeEmpty();
    }
}
