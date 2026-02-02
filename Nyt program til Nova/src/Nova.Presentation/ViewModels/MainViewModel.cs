using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Nova.Infrastructure.Midi;
using Nova.Midi;

namespace Nova.Presentation.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IMidiPort _midiPort;
    private readonly IConnectUseCase _connectUseCase;
    private readonly IDownloadBankUseCase _downloadBankUseCase;
    private readonly ISaveSystemDumpUseCase _saveSystemDumpUseCase;
    private UserBankDump? _currentBank;
    private SystemDump? _currentSystemDump;

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

    [ObservableProperty]
    private PresetListViewModel _presetList = new();

    [ObservableProperty]
    private PresetDetailViewModel _presetDetail = new();

    [ObservableProperty]
    private FileManagerViewModel _fileManager = new();

    [ObservableProperty]
    private SystemSettingsViewModel _systemSettings = new();

    public MainViewModel(
        IMidiPort midiPort,
        IConnectUseCase connectUseCase,
        IDownloadBankUseCase downloadBankUseCase,
        ISaveSystemDumpUseCase saveSystemDumpUseCase)
    {
        _midiPort = midiPort;
        _connectUseCase = connectUseCase;
        _downloadBankUseCase = downloadBankUseCase;
        _saveSystemDumpUseCase = saveSystemDumpUseCase;
        
        // Auto-refresh ports on startup
        RefreshPorts();
        
        // Subscribe to preset selection changes
        PresetList.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(PresetListViewModel.SelectedPreset))
            {
                OnPresetSelectionChanged();
            }
        };
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
                
                // Store the bank for later preset retrieval
                _currentBank = bank;
                
                // Load presets into list view
                PresetList.LoadFromBank(bank);
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

    [RelayCommand(CanExecute = nameof(CanDownload))]
    private async Task DownloadSystemSettingsAsync()
    {
        // TODO: Implement when IRequestSystemDumpUseCase is available
        await Task.CompletedTask;
        StatusMessage = "System settings download not yet implemented";
    }

    [RelayCommand(CanExecute = nameof(CanSaveSystemSettings))]
    private async Task SaveSystemSettingsAsync()
    {
        if (_currentSystemDump == null) return;

        IsDownloading = true;
        StatusMessage = "Saving system settings to pedal...";

        try
        {
            // Create modified dump with current VM values
            var modifiedDump = CreateModifiedSystemDump(_currentSystemDump);

            // Save to hardware
            var saveResult = await _saveSystemDumpUseCase.ExecuteAsync(modifiedDump);

            if (saveResult.IsFailed)
            {
                StatusMessage = $"Save failed: {saveResult.Errors.First().Message}";
                return;
            }

            StatusMessage = "System settings saved successfully";
            _currentSystemDump = modifiedDump;
            SystemSettings.LoadFromDump(modifiedDump);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsDownloading = false;
        }
    }

    [RelayCommand]
    private void CancelSystemChanges()
    {
        // TODO: Implement when SystemSettings.RevertChanges is available
        StatusMessage = "Cancel changes not yet implemented";
    }

    private bool CanSaveSystemSettings() => !IsDownloading; // TODO: Add && SystemSettings.HasUnsavedChanges when available

    private SystemDump CreateModifiedSystemDump(SystemDump original)
    {
        // Clone original SysEx bytes
        var modifiedSysEx = new byte[original.RawSysEx.Length];
        Array.Copy(original.RawSysEx, modifiedSysEx, original.RawSysEx.Length);

        // Update bytes with ViewModel values
        modifiedSysEx[4] = (byte)SystemSettings.DeviceId; // Device ID
        modifiedSysEx[8] = (byte)SystemSettings.MidiChannel; // MIDI Channel (0-15)
        modifiedSysEx[20] = (byte)(SystemSettings.MidiClockEnabled ? 0x01 : 0x00); // MIDI Clock
        modifiedSysEx[21] = (byte)(SystemSettings.MidiProgramChangeEnabled ? 0x01 : 0x00); // Program Change

        // Recalculate checksum (last byte before 0xF7)
        // For now, we assume SystemDump.FromSysEx will validate/fix this
        
        return SystemDump.FromSysEx(modifiedSysEx).Value;
    }

    /// <summary>
    /// Handles preset selection changes and loads the full preset details.
    /// </summary>
    private void OnPresetSelectionChanged()
    {
        var selectedPreset = PresetList.SelectedPreset;
        
        if (selectedPreset != null && _currentBank != null)
        {
            // Find the full preset in the bank
            var fullPreset = _currentBank.Presets.FirstOrDefault(p => p?.Number == selectedPreset.Number);
            
            if (fullPreset != null)
            {
                PresetDetail.LoadFromPreset(fullPreset);
            }
        }
        else
        {
            PresetDetail.LoadFromPreset(null);
        }
    }
}
