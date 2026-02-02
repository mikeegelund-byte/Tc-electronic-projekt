using FluentAssertions;
using Moq;
using Nova.Application.UseCases;
using Nova.Midi;
using Nova.Presentation.ViewModels;
using Xunit;

namespace Nova.Presentation.Tests.ViewModels;

public class MainViewModelTests
{
    [Fact]
    public void AvailablePorts_InitiallyEmpty()
    {
        var mockMidi = new Mock<IMidiPort>();
        var mockConnect = new Mock<IConnectUseCase>();
        var mockDownload = new Mock<IDownloadBankUseCase>();
        var mockGetPorts = new Mock<IGetAvailablePortsUseCase>();
        var mockRequestDump = new Mock<IRequestSystemDumpUseCase>();
        mockGetPorts.Setup(x => x.Execute()).Returns(new List<string>());
        
        var vm = new MainViewModel(mockMidi.Object, mockConnect.Object, mockDownload.Object, mockGetPorts.Object, mockRequestDump.Object);
        
        vm.AvailablePorts.Should().BeEmpty();
    }

    [Fact]
    public void IsConnected_InitiallyFalse()
    {
        var mockMidi = new Mock<IMidiPort>();
        var mockConnect = new Mock<IConnectUseCase>();
        var mockDownload = new Mock<IDownloadBankUseCase>();
        var mockGetPorts = new Mock<IGetAvailablePortsUseCase>();
        var mockRequestDump = new Mock<IRequestSystemDumpUseCase>();
        mockGetPorts.Setup(x => x.Execute()).Returns(new List<string>());
        
        var vm = new MainViewModel(mockMidi.Object, mockConnect.Object, mockDownload.Object, mockGetPorts.Object, mockRequestDump.Object);
        
        vm.IsConnected.Should().BeFalse();
    }

    [Fact]
    public void StatusMessage_InitiallyReady()
    {
        var mockMidi = new Mock<IMidiPort>();
        var mockConnect = new Mock<IConnectUseCase>();
        var mockDownload = new Mock<IDownloadBankUseCase>();
        var mockGetPorts = new Mock<IGetAvailablePortsUseCase>();
        var mockRequestDump = new Mock<IRequestSystemDumpUseCase>();
        mockGetPorts.Setup(x => x.Execute()).Returns(new List<string>());
        
        var vm = new MainViewModel(mockMidi.Object, mockConnect.Object, mockDownload.Object, mockGetPorts.Object, mockRequestDump.Object);
        
        vm.StatusMessage.Should().Be("Found 0 MIDI port(s)");
    }
}
