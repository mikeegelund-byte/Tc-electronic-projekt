namespace Nova.Midi.Tests;

public class MidiPortContractTests
{
    [Fact]
    public void IMidiPort_HasConnectAsync()
    {
        // Verify interface exists and has ConnectAsync
        var interfaceType = typeof(IMidiPort);
        var method = interfaceType.GetMethod("ConnectAsync", new[] { typeof(MidiPortSelection) });
        Assert.NotNull(method);
    }
    
    [Fact]
    public void IMidiPort_HasSendSysExAsync()
    {
        var interfaceType = typeof(IMidiPort);
        var method = interfaceType.GetMethod("SendSysExAsync");
        Assert.NotNull(method);
    }
    
    [Fact]
    public void IMidiPort_HasReceiveSysExAsync()
    {
        var interfaceType = typeof(IMidiPort);
        var method = interfaceType.GetMethod("ReceiveSysExAsync");
        Assert.NotNull(method);
    }

    [Fact]
    public void IMidiPort_HasGetAvailableInputPorts()
    {
        var interfaceType = typeof(IMidiPort);
        var method = interfaceType.GetMethod("GetAvailableInputPorts");
        Assert.NotNull(method);
    }

    [Fact]
    public void IMidiPort_HasGetAvailableOutputPorts()
    {
        var interfaceType = typeof(IMidiPort);
        var method = interfaceType.GetMethod("GetAvailableOutputPorts");
        Assert.NotNull(method);
    }
}
