using FluentAssertions;
using Moq;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Nova.Midi;
using Nova.Presentation.ViewModels;
using Xunit;

namespace Nova.Presentation.Tests.ViewModels;

public class MainViewModelTests
{
    private Mock<IMidiPort> _mockMidi = null!;
    private Mock<IConnectUseCase> _mockConnect = null!;
    private Mock<IDownloadBankUseCase> _mockDownload = null!;
    private Mock<IGetAvailablePortsUseCase> _mockGetPorts = null!;
    private Mock<IRequestSystemDumpUseCase> _mockRequestDump = null!;
    private Mock<IExportBankUseCase> _mockExportBank = null!;
    private Mock<IImportSysExUseCase> _mockImportSysEx = null!;
    private Mock<ISaveSystemDumpUseCase> _mockSaveSystemDump = null!;
    private Mock<IVerifySystemDumpRoundtripUseCase> _mockVerifyRoundtrip = null!;

    public MainViewModelTests()
    {
        _mockMidi = new Mock<IMidiPort>();
        _mockConnect = new Mock<IConnectUseCase>();
        _mockDownload = new Mock<IDownloadBankUseCase>();
        _mockGetPorts = new Mock<IGetAvailablePortsUseCase>();
        _mockRequestDump = new Mock<IRequestSystemDumpUseCase>();
        _mockExportBank = new Mock<IExportBankUseCase>();
        _mockImportSysEx = new Mock<IImportSysExUseCase>();
        _mockSaveSystemDump = new Mock<ISaveSystemDumpUseCase>();
        _mockVerifyRoundtrip = new Mock<IVerifySystemDumpRoundtripUseCase>();
        
        _mockGetPorts.Setup(x => x.Execute()).Returns(new List<string>());
    }

    private MainViewModel CreateViewModel() => new MainViewModel(
        _mockMidi.Object,
        _mockConnect.Object,
        _mockDownload.Object,
        _mockGetPorts.Object,
        _mockRequestDump.Object,
        _mockExportBank.Object,
        _mockImportSysEx.Object,
        _mockSaveSystemDump.Object,
        _mockVerifyRoundtrip.Object);

    [Fact]
    public void AvailablePorts_InitiallyEmpty()
    {
        var vm = CreateViewModel();
        var mockMidi = new Mock<IMidiPort>();
        var mockConnect = new Mock<IConnectUseCase>();
        var mockDownload = new Mock<IDownloadBankUseCase>();
        
        var vm = new MainViewModel(mockMidi.Object, mockConnect.Object, mockDownload.Object);
        
        vm.AvailablePorts.Should().BeEmpty();
    }

    [Fact]
    public void IsConnected_InitiallyFalse()
    {
        var vm = CreateViewModel();
        var mockMidi = new Mock<IMidiPort>();
        var mockConnect = new Mock<IConnectUseCase>();
        var mockDownload = new Mock<IDownloadBankUseCase>();
        
        var vm = new MainViewModel(mockMidi.Object, mockConnect.Object, mockDownload.Object);
        
        vm.IsConnected.Should().BeFalse();
    }

    [Fact]
    public void StatusMessage_InitiallyReady()
    {
        var vm = CreateViewModel();
        var mockMidi = new Mock<IMidiPort>();
        var mockConnect = new Mock<IConnectUseCase>();
        var mockDownload = new Mock<IDownloadBankUseCase>();
        
        var vm = new MainViewModel(mockMidi.Object, mockConnect.Object, mockDownload.Object);
        
        vm.StatusMessage.Should().Be("Ready");
    }

    [Fact]
    public async Task SaveSystemSettingsCommand_WithNoSystemDump_DoesNothing()
    {
        // Arrange
        var vm = CreateViewModel();

        // Act
        await vm.SaveSystemSettingsCommand.ExecuteAsync(null);

        // Assert
        _mockSaveSystemDump.Verify(x => x.ExecuteAsync(It.IsAny<SystemDump>()), Times.Never);
    }

    [Fact]
    public async Task SaveSystemSettingsCommand_WithValidDump_SavesAndVerifies()
    {
        // Arrange
        var sysex = CreateValidSystemDumpSysEx();
        var dump = SystemDump.FromSysEx(sysex).Value;
        
        _mockRequestDump.Setup(x => x.ExecuteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(FluentResults.Result.Ok(dump));
        
        _mockSaveSystemDump.Setup(x => x.ExecuteAsync(It.IsAny<SystemDump>()))
                          .ReturnsAsync(FluentResults.Result.Ok());
        
        _mockVerifyRoundtrip.Setup(x => x.ExecuteAsync(It.IsAny<SystemDump>(), It.IsAny<int>()))
                           .ReturnsAsync(FluentResults.Result.Ok());

        var vm = CreateViewModel();
        
        // First download system settings to populate _currentSystemDump
        await vm.DownloadSystemSettingsCommand.ExecuteAsync(null);
        
        // Make a change to enable save
        vm.SystemSettings.MidiChannel = 5;

        // Act
        await vm.SaveSystemSettingsCommand.ExecuteAsync(null);

        // Assert
        _mockSaveSystemDump.Verify(x => x.ExecuteAsync(It.IsAny<SystemDump>()), Times.Once);
        _mockVerifyRoundtrip.Verify(x => x.ExecuteAsync(It.IsAny<SystemDump>(), 1000), Times.Once);
    }

    [Fact]
    public void CancelSystemChangesCommand_RevertsChanges()
    {
        // Arrange
        var sysex = CreateValidSystemDumpSysEx();
        var dump = SystemDump.FromSysEx(sysex).Value;
        var vm = CreateViewModel();
        
        vm.SystemSettings.LoadFromDump(dump);
        var originalChannel = vm.SystemSettings.MidiChannel;
        vm.SystemSettings.MidiChannel = 10;

        // Act
        vm.CancelSystemChangesCommand.Execute(null);

        // Assert
        vm.SystemSettings.MidiChannel.Should().Be(originalChannel);
        vm.SystemSettings.HasUnsavedChanges.Should().BeFalse();
    }

    private static byte[] CreateValidSystemDumpSysEx()
    {
        var sysex = new byte[527];
        sysex[0] = 0xF0; // Start
        sysex[1] = 0x00; // Manufacturer (TC Electronic)
        sysex[2] = 0x20;
        sysex[3] = 0x1F;
        sysex[4] = 0x00; // Device ID
        sysex[5] = 0x63; // Product: NOVA SYSTEM
        sysex[6] = 0x20; // Message ID: Dump
        sysex[7] = 0x02; // Data Type: System Dump
        sysex[8] = 0x00; // MIDI Channel (0-15)
        
        // Fill rest with zeros
        for (int i = 9; i < 526; i++)
            sysex[i] = 0x00;
        
        sysex[526] = 0xF7; // End
        return sysex;
    }
}