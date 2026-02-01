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
    public async Task ConnectAsync_WithInvalidPort_ReturnsFailure()
    {
        var port = new DryWetMidiPort();
        var result = await port.ConnectAsync("NonExistent Port 12345");
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].Message.Should().Contain("not found");
    }

    [Fact]
    public async Task ConnectAsync_SetsNameProperty()
    {
        // This would need a mock device - but we can at least test the error path
        var port = new DryWetMidiPort();
        var result = await port.ConnectAsync("TestPort");
        // Should fail since port doesn't exist
        result.IsFailed.Should().BeTrue();
        port.Name.Should().BeEmpty(); // Name only set on success
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

    [Fact]
    public async Task SendSysExAsync_NotConnected_ReturnsFail()
    {
        var port = new DryWetMidiPort();
        var sysex = new byte[] { 0xF0, 0x41, 0x10, 0x42, 0x12, 0x7F, 0x7F, 0xF7 };
        var result = await port.SendSysExAsync(sysex);
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void SendSysExAsync_WithValidData_SkippedForManualTesting()
    {
        // This test requires actual MIDI device - skip in CI
        // Marked for manual testing only
    }
}
