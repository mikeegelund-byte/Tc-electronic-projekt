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
}
