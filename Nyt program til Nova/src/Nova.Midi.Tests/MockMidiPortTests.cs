using System.Threading.Channels;
using Nova.Midi;

namespace Nova.Midi.Tests;

public class MockMidiPortTests
{
    [Fact]
    public async Task MockMidiPort_Connect_SetsIsConnected()
    {
        var port = new MockMidiPort();
        Assert.False(port.IsConnected);

        var result = await port.ConnectAsync(new MidiPortSelection("Test In", "Test Out"));

        Assert.True(result.IsSuccess);
        Assert.True(port.IsConnected);
    }

    [Fact]
    public async Task MockMidiPort_SendSysEx_Succeeds()
    {
        var port = new MockMidiPort();
        await port.ConnectAsync(new MidiPortSelection("Test In", "Test Out"));

        var sysex = new byte[] { 0xF0, 0x00, 0x20, 0x1F, 0x00, 0x63, 0x45, 0x03, 0xF7 };
        var result = await port.SendSysExAsync(sysex);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task MockMidiPort_ReceiveSysEx_YieldsEnqueuedData()
    {
        var port = new MockMidiPort();
        await port.ConnectAsync(new MidiPortSelection("Test In", "Test Out"));

        var expectedSysex = new byte[] { 0xF0, 0x00, 0x20, 0x1F, 0x00, 0x63, 0x20, 0x03,
            0x00, 0x00, 0x3C, 0xF7 };
        port.EnqueueResponse(expectedSysex);

        var received = new List<byte[]>();
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

        await foreach (var data in port.ReceiveSysExAsync(cts.Token))
        {
            received.Add(data);
            break;  // Just get one for testing
        }

        Assert.Single(received);
        Assert.Equal(expectedSysex, received[0]);
    }
}
