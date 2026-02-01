using Nova.Domain.Midi;

namespace Nova.Domain.Tests;

public class SysExBuilderTests
{
    [Fact]
    public void BuildBankDumpRequest_Returns9Bytes()
    {
        var request = SysExBuilder.BuildBankDumpRequest();

        Assert.Equal(9, request.Length);
        Assert.Equal(0xF0, request[0]);
        Assert.Equal(0xF7, request[8]);
    }

    [Fact]
    public void BuildBankDumpRequest_HasCorrectManufacturerId()
    {
        var request = SysExBuilder.BuildBankDumpRequest();

        Assert.Equal(0x00, request[1]);
        Assert.Equal(0x20, request[2]);
        Assert.Equal(0x1F, request[3]);
    }

    [Fact]
    public void BuildBankDumpRequest_HasCorrectModelId()
    {
        var request = SysExBuilder.BuildBankDumpRequest();

        Assert.Equal(0x63, request[5]);  // Nova System
    }

    [Fact]
    public void BuildBankDumpRequest_HasCorrectMessageId()
    {
        var request = SysExBuilder.BuildBankDumpRequest();

        Assert.Equal(0x45, request[6]);  // Request message ID
    }

    // System Dump Request tests
    [Fact]
    public void BuildSystemDumpRequest_ReturnsCorrectBytes()
    {
        var request = SysExBuilder.BuildSystemDumpRequest();

        Assert.Equal(9, request.Length);
        Assert.Equal(0xF0, request[0]);
        Assert.Equal(0x00, request[1]);
        Assert.Equal(0x20, request[2]);
        Assert.Equal(0x1F, request[3]);
        Assert.Equal(0x00, request[4]); // Default device ID
        Assert.Equal(0x63, request[5]);
        Assert.Equal(0x45, request[6]);
        Assert.Equal(0x02, request[7]); // System dump type
        Assert.Equal(0xF7, request[8]);
    }

    [Theory]
    [InlineData(0x01)]
    [InlineData(0x05)]
    [InlineData(0x7F)]
    public void BuildSystemDumpRequest_WithDeviceId_SetsCorrectly(byte deviceId)
    {
        var request = SysExBuilder.BuildSystemDumpRequest(deviceId);

        Assert.Equal(deviceId, request[4]);
    }
}
