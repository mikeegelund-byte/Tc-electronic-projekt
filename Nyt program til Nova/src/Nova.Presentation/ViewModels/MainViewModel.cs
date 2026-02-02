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
    private readonly IGetAvailablePortsUseCase _getAvailablePortsUseCase;
    private readonly IRequestSystemDumpUseCase _requestSystemDumpUseCase;
    private readonly IExportBankUseCase _exportBankUseCase;
    private readonly IImportSysExUseCase _importSysExUseCase;
    private readonly ISaveSystemDumpUseCase _saveSystemDumpUseCase;
    private readonly IVerifySystemDumpRoundtripUseCase _verifyRoundtripUseCase;
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
        IGetAvailablePortsUseCase getAvailablePortsUseCase,
        IRequestSystemDumpUseCase requestSystemDumpUseCase,
        IExportBankUseCase exportBankUseCase,
        IImportSysExUseCase importSysExUseCase,
        ISaveSystemDumpUseCase saveSystemDumpUseCase,
        IVerifySystemDumpRoundtripUseCase verifyRoundtripUseCase)
    {
        _midiPort = midiPort;
        _connectUseCase = connectUseCase;
        _downloadBankUseCase = downloadBankUseCase;
        _getAvailablePortsUseCase = getAvailablePortsUseCase;
        _requestSystemDumpUseCase = requestSystemDumpUseCase;
        _exportBankUseCase = exportBankUseCase;
        _importSysExUseCase = importSysExUseCase;
        _saveSystemDumpUseCase = saveSystemDumpUseCase;
        _verifyRoundtripUseCase = verifyRoundtripUseCase;
        
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
        var ports = _getAvailablePortsUseCase.Execute();
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
        IsDownloading = true;
        StatusMessage = "Requesting System Settings dump from pedal...";
        
        try
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var result = await _requestSystemDumpUseCase.ExecuteAsync(10000, cts.Token);

            if (result.IsSuccess)
            {
                _currentSystemDump = result.Value;
                StatusMessage = "System settings downloaded successfully";
                SystemSettings.LoadFromDump(result.Value);
            }
            else
            {
                StatusMessage = $"Error: {result.Errors.First().Message}";
            }
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

            // Verify roundtrip
            var verifyResult = await _verifyRoundtripUseCase.ExecuteAsync(modifiedDump, waitMilliseconds: 1000);

            if (verifyResult.IsSuccess)
            {
                StatusMessage = "System settings saved and verified successfully";
                _currentSystemDump = modifiedDump;
                SystemSettings.LoadFromDump(modifiedDump); // Reset dirty tracking
            }
            else
            {
                StatusMessage = $"Verification failed: {verifyResult.Errors.First().Message}";
            }
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
        SystemSettings.RevertChanges();
        StatusMessage = "Changes cancelled";
    }

    private bool CanSaveSystemSettings() => !IsDownloading && SystemSettings.HasUnsavedChanges;

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

    [RelayCommand(CanExecute = nameof(HasBank))]
    private async Task ExportBankAsync()
    {
        if (_currentBank == null) return;

        try
        {
            var dialog = new Avalonia.Platform.Storage.FilePickerSaveOptions
            {
                Title = "Export User Bank",
                SuggestedFileName = $"NovaBank_{DateTime.Now:yyyyMMdd_HHmmss}.syx",
                FileTypeChoices = new[]
                {
                    new Avalonia.Platform.Storage.FilePickerFileType("SysEx Files")
                    {
                        Patterns = new[] { "*.syx" }
                    }
                }
            };

            var topLevel = Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
                ? desktop.MainWindow
                : null;

            if (topLevel == null) return;

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(dialog);
            if (file != null)
            {
                var result = await _exportBankUseCase.ExecuteAsync(_currentBank, file.Path.LocalPath);
                StatusMessage = result.IsSuccess
                    ? $"Bank exported to {System.IO.Path.GetFileName(file.Path.LocalPath)}"
                    : $"Export failed: {result.Errors.First().Message}";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Export error: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task ImportBankAsync()
    {
        try
        {
            var dialog = new Avalonia.Platform.Storage.FilePickerOpenOptions
            {
                Title = "Import SysEx File",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new Avalonia.Platform.Storage.FilePickerFileType("SysEx Files")
                    {
                        Patterns = new[] { "*.syx" }
                    }
                }
            };

            var topLevel = Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
                ? desktop.MainWindow
                : null;

            if (topLevel == null) return;

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(dialog);
            if (files.Count > 0)
            {
                var result = await _importSysExUseCase.ExecuteAsync(files[0].Path.LocalPath);
                
                if (result.IsSuccess)
                {
                    if (result.Value is UserBankDump bank)
                    {
                        _currentBank = bank;
                        PresetList.LoadFromBank(bank);
                        StatusMessage = $"Imported User Bank with {bank.Presets.Count(p => p != null)} presets";
                    }
                    else if (result.Value is Preset preset)
                    {
                        StatusMessage = $"Imported single preset #{preset.Number}";
                    }
                    else if (result.Value is SystemDump)
                    {
                        StatusMessage = "Imported System Dump (not yet applied)";
                    }
                }
                else
                {
                    StatusMessage = $"Import failed: {result.Errors.First().Message}";
                }
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Import error: {ex.Message}";
        }
    }

    private bool HasBank() => _currentBank != null;
}
