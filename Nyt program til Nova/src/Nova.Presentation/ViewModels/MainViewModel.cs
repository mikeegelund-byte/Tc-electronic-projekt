using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nova.Infrastructure.Midi;
using Nova.Midi;
using ConnectUseCase = Nova.Application.UseCases.ConnectUseCase;
using DownloadBankUseCase = Nova.Application.UseCases.DownloadBankUseCase;

namespace Nova.Presentation.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IMidiPort _midiPort;
    private readonly ConnectUseCase _connectUseCase;
    private readonly DownloadBankUseCase _downloadBankUseCase;

    [ObservableProperty]
    private ObservableCollection<string> _availablePorts = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    private string? _selectedPort;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand), nameof(DownloadBankCommand))]
    private bool _isConnected;

    [ObservableProperty]
    private bool _isDownloading;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private int _downloadProgress;

    public MainViewModel(
        IMidiPort midiPort,
        ConnectUseCase connectUseCase,
        DownloadBankUseCase downloadBankUseCase)
    {
        _midiPort = midiPort;
        _connectUseCase = connectUseCase;
        _downloadBankUseCase = downloadBankUseCase;
        
        // Auto-refresh ports on startup
        RefreshPorts();
    }

    [RelayCommand]
    private void RefreshPorts()
    {
        AvailablePorts.Clear();
        var ports = DryWetMidiPort.GetAvailablePorts();
        foreach (var port in ports)
        {
            AvailablePorts.Add(port);
        }
        StatusMessage = $"Found {AvailablePorts.Count} MIDI port(s)";
    }

    [RelayCommand(CanExecute = nameof(CanConnect))]
    private async Task ConnectAsync()
    {
        if (string.IsNullOrEmpty(SelectedPort)) return;

        StatusMessage = $"Connecting to {SelectedPort}...";
        var result = await _connectUseCase.ExecuteAsync(SelectedPort);

        if (result.IsSuccess)
        {
            IsConnected = true;
            StatusMessage = $"Connected to {SelectedPort}";
        }
        else
        {
            StatusMessage = $"Error: {result.Errors.First().Message}";
        }
    }

    private bool CanConnect() => !string.IsNullOrEmpty(SelectedPort) && !IsConnected;

    [RelayCommand(CanExecute = nameof(CanDownload))]
    private async Task DownloadBankAsync()
    {
        IsDownloading = true;
        StatusMessage = "Waiting for User Bank dump from pedal...";
        DownloadProgress = 0;

        try
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            var result = await _downloadBankUseCase.ExecuteAsync(cts.Token);

            if (result.IsSuccess)
            {
                var bank = result.Value;
                StatusMessage = $"Downloaded {bank.Presets.Length} presets successfully";
                DownloadProgress = 100;
                // TODO: Store bank and update preset list (Phase 2)
            }
            else
            {
                StatusMessage = $"Error: {result.Errors.First().Message}";
                DownloadProgress = 0;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            DownloadProgress = 0;
        }
        finally
        {
            IsDownloading = false;
        }
    }

    private bool CanDownload() => IsConnected && !IsDownloading;
}
