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
        var mockConnect = new Mock<ConnectUseCase>(mockMidi.Object);
        var mockDownload = new Mock<DownloadBankUseCase>(mockMidi.Object);
        
        var vm = new MainViewModel(mockMidi.Object, mockConnect.Object, mockDownload.Object);
        
        vm.AvailablePorts.Should().BeEmpty();
    }

    [Fact]
    public void IsConnected_InitiallyFalse()
    {
        var mockMidi = new Mock<IMidiPort>();
        var mockConnect = new Mock<ConnectUseCase>(mockMidi.Object);
        var mockDownload = new Mock<DownloadBankUseCase>(mockMidi.Object);
        
        var vm = new MainViewModel(mockMidi.Object, mockConnect.Object, mockDownload.Object);
        
        vm.IsConnected.Should().BeFalse();
    }

    [Fact]
    public void StatusMessage_InitiallyReady()
    {
        var mockMidi = new Mock<IMidiPort>();
        var mockConnect = new Mock<ConnectUseCase>(mockMidi.Object);
        var mockDownload = new Mock<DownloadBankUseCase>(mockMidi.Object);
        
        var vm = new MainViewModel(mockMidi.Object, mockConnect.Object, mockDownload.Object);
        
        vm.StatusMessage.Should().Be("Ready");
    }
}
