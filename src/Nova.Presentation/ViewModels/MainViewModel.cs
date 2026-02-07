using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Nova.Midi;

namespace Nova.Presentation.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IMidiPort _midiPort;
    private readonly IConnectUseCase _connectUseCase;
    private readonly IDownloadBankUseCase _downloadBankUseCase;
    private readonly IRequestSystemDumpUseCase _requestSystemDumpUseCase;
    private readonly ISaveSystemDumpUseCase _saveSystemDumpUseCase;
    private readonly ISendBankToHardwareUseCase _sendBankUseCase;
    private UserBankDump? _currentBank;
    private SystemDump? _currentSystemDump;

    [ObservableProperty]
    private ObservableCollection<string> _availableInputPorts = new();

    [ObservableProperty]
    private ObservableCollection<string> _availableOutputPorts = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    private string? _selectedInputPort;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    private string? _selectedOutputPort;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand), nameof(DownloadBankCommand), nameof(UploadBankCommand), nameof(DownloadSystemSettingsCommand))]
    private bool _isConnected;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DownloadBankCommand), nameof(UploadBankCommand), nameof(DownloadSystemSettingsCommand), nameof(SaveSystemSettingsCommand))]
    private bool _isDownloading;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private int _downloadProgress;

    [ObservableProperty]
    private PresetListViewModel _presetList = new();

    [ObservableProperty]
    private PresetDetailViewModel _presetDetail = null!;

    [ObservableProperty]
    private FileManagerViewModel _fileManager = null!;

    [ObservableProperty]
    private LibraryViewModel _library = null!;

    [ObservableProperty]
    private TunerViewModel _tuner = new();

    [ObservableProperty]
    private SystemSettingsViewModel _systemSettings = new();

    [ObservableProperty]
    private CCMappingViewModel _midiMapping = null!;

    [ObservableProperty]
    private ProgramMapInViewModel _programMapIn = null!;

    [ObservableProperty]
    private ProgramMapOutViewModel _programMapOut = null!;

    public MainViewModel(
        IMidiPort midiPort,
        IConnectUseCase connectUseCase,
        IDownloadBankUseCase downloadBankUseCase,
        IRequestSystemDumpUseCase requestSystemDumpUseCase,
        ISaveSystemDumpUseCase saveSystemDumpUseCase,
        ISavePresetUseCase savePresetUseCase,
        ISaveBankUseCase saveBankUseCase,
        ILoadBankUseCase loadBankUseCase,
        IExportPresetUseCase exportPresetUseCase,
        IImportPresetUseCase importPresetUseCase,
        ISendBankToHardwareUseCase sendBankUseCase,
        Nova.Application.Library.ILibraryRepository libraryRepository,
        CCMappingViewModel midiMappingViewModel,
        ProgramMapInViewModel programMapInViewModel,
        ProgramMapOutViewModel programMapOutViewModel)
    {
        _midiPort = midiPort;
        _connectUseCase = connectUseCase;
        _downloadBankUseCase = downloadBankUseCase;
        _requestSystemDumpUseCase = requestSystemDumpUseCase;
        _saveSystemDumpUseCase = saveSystemDumpUseCase;
        _sendBankUseCase = sendBankUseCase;

        PresetDetail = new PresetDetailViewModel(savePresetUseCase);
        FileManager = new FileManagerViewModel(
            saveBankUseCase,
            loadBankUseCase,
            exportPresetUseCase,
            importPresetUseCase,
            sendBankUseCase);

        Library = new LibraryViewModel(libraryRepository);
        MidiMapping = midiMappingViewModel;
        ProgramMapIn = programMapInViewModel;
        ProgramMapOut = programMapOutViewModel;

        // Auto-refresh ports on startup
        RefreshPorts();

        // Load library on startup
        _ = Library.RefreshLibraryCommand.ExecuteAsync(null);

        SystemSettings.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(SystemSettingsViewModel.HasUnsavedChanges))
            {
                SaveSystemSettingsCommand.NotifyCanExecuteChanged();
            }
        };

        // Subscribe to preset selection changes
        PresetList.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(PresetListViewModel.SelectedPreset))
            {
                OnPresetSelectionChanged();
            }
        };

        Library.PresetLoaded += preset =>
        {
            PresetDetail.LoadFromPreset(preset);
            FileManager.SetSelectedPreset(preset);
        };
    }

    [RelayCommand]
    private void RefreshPorts()
    {
        AvailableInputPorts.Clear();
        AvailableOutputPorts.Clear();

        var inputPorts = _midiPort.GetAvailableInputPorts();
        var outputPorts = _midiPort.GetAvailableOutputPorts();

        foreach (var port in inputPorts)
        {
            AvailableInputPorts.Add(port);
        }

        foreach (var port in outputPorts)
        {
            AvailableOutputPorts.Add(port);
        }

        // Auto-pair MIDI 0/1 if present
        if (string.IsNullOrWhiteSpace(SelectedOutputPort))
        {
            SelectedOutputPort = outputPorts.FirstOrDefault(p => p.Contains("MIDI 0", StringComparison.OrdinalIgnoreCase));
        }

        if (string.IsNullOrWhiteSpace(SelectedInputPort))
        {
            SelectedInputPort = inputPorts.FirstOrDefault(p => p.Contains("MIDI 1", StringComparison.OrdinalIgnoreCase));
        }

        // Clear selection if device disappeared
        if (!string.IsNullOrWhiteSpace(SelectedInputPort) && !AvailableInputPorts.Contains(SelectedInputPort))
        {
            SelectedInputPort = null;
        }

        if (!string.IsNullOrWhiteSpace(SelectedOutputPort) && !AvailableOutputPorts.Contains(SelectedOutputPort))
        {
            SelectedOutputPort = null;
        }

        StatusMessage = $"Found {AvailableInputPorts.Count} MIDI IN / {AvailableOutputPorts.Count} MIDI OUT";
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        var app = global::Avalonia.Application.Current;
        if (app != null)
        {
            app.RequestedThemeVariant = app.RequestedThemeVariant == ThemeVariant.Dark
                ? ThemeVariant.Light
                : ThemeVariant.Dark;
        }
    }

    [RelayCommand(CanExecute = nameof(CanConnect))]
    private async Task ConnectAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedInputPort) || string.IsNullOrWhiteSpace(SelectedOutputPort)) return;

        StatusMessage = $"Connecting (IN: {SelectedInputPort} / OUT: {SelectedOutputPort})...";
        var selection = new MidiPortSelection(SelectedInputPort, SelectedOutputPort);
        var result = await _connectUseCase.ExecuteAsync(selection);

        if (result.IsSuccess)
        {
            IsConnected = true;
            StatusMessage = $"Connected (IN: {SelectedInputPort} / OUT: {SelectedOutputPort})";
        }
        else
        {
            StatusMessage = $"Error: {result.Errors.First().Message}";
        }
    }

    private bool CanConnect()
        => !IsConnected
           && !string.IsNullOrWhiteSpace(SelectedInputPort)
           && !string.IsNullOrWhiteSpace(SelectedOutputPort);

    [RelayCommand(CanExecute = nameof(CanDownload))]
    private async Task DownloadBankAsync()
    {
        IsDownloading = true;
        StatusMessage = "Requesting User Bank dump from pedal...";
        DownloadProgress = 0;

        try
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            var result = await _downloadBankUseCase.ExecuteAsync(cts.Token);

            if (result.IsSuccess)
            {
                var bank = result.Value;
                var receivedCount = bank.Presets.Count(p => p != null);
                StatusMessage = receivedCount == 60
                    ? "Downloaded 60 presets successfully"
                    : $"Downloaded {receivedCount}/60 presets (partial)";
                DownloadProgress = (int)Math.Round(receivedCount / 60.0 * 100);

                // Store the bank for later preset retrieval
                _currentBank = bank;
                UploadBankCommand.NotifyCanExecuteChanged();

                // Load presets into list view
                PresetList.LoadFromBank(bank);

                // Keep FileManager in sync for upload/export
                FileManager.LoadFromBank(bank);
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

    [RelayCommand(CanExecute = nameof(CanUploadBank))]
    private async Task UploadBankAsync()
    {
        if (_currentBank == null)
        {
            StatusMessage = "No bank loaded to upload";
            return;
        }

        IsDownloading = true;
        StatusMessage = "Uploading bank to pedal...";
        DownloadProgress = 0;

        try
        {
            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
            var sendResult = await _sendBankUseCase.ExecuteAsync(_currentBank, cts.Token);

            if (sendResult.IsSuccess)
            {
                StatusMessage = "Bank uploaded successfully (60 presets)";
                DownloadProgress = 100;
            }
            else
            {
                StatusMessage = $"Upload failed: {sendResult.Errors.First().Message}";
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

    private bool CanUploadBank() => IsConnected && !IsDownloading && _currentBank != null;

    [RelayCommand(CanExecute = nameof(CanDownload))]
    private async Task DownloadSystemSettingsAsync()
    {
        IsDownloading = true;
        StatusMessage = "Requesting system dump from pedal...";
        DownloadProgress = 0;

        try
        {
            var result = await _requestSystemDumpUseCase.ExecuteAsync(timeoutMs: 10000);

            if (result.IsSuccess)
            {
                var dump = result.Value;
                _currentSystemDump = dump;
                SystemSettings.LoadFromDump(dump);
                await MidiMapping.LoadFromDump(dump);
                await ProgramMapIn.LoadFromDump(dump);
                await ProgramMapOut.LoadFromDump(dump);
                StatusMessage = "System dump loaded successfully";
                DownloadProgress = 100;
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
        SystemSettings.RevertChanges();
        StatusMessage = "System settings changes cancelled";
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
                FileManager.SetSelectedPreset(fullPreset);
                Library.SetSelectedPreset(fullPreset);
            }
        }
        else
        {
            PresetDetail.LoadFromPreset(null);
            FileManager.SetSelectedPreset(null);
            Library.SetSelectedPreset(null);
        }
    }

    // ============= KEYBOARD SHORTCUTS (v1.0 placeholders) =============

    /// <summary>
    /// Placeholder for Save Preset functionality (planned for v1.1).
    /// </summary>
    [RelayCommand]
    private void SavePreset()
    {
        StatusMessage = "Save Preset (Ctrl+S) - Coming in v1.1";
    }

    /// <summary>
    /// Placeholder for Undo functionality (planned for v1.1).
    /// </summary>
    [RelayCommand]
    private void Undo()
    {
        StatusMessage = "Undo (Ctrl+Z) - Coming in v1.1";
    }

    /// <summary>
    /// Placeholder for Redo functionality (planned for v1.1).
    /// </summary>
    [RelayCommand]
    private void Redo()
    {
        StatusMessage = "Redo (Ctrl+Y) - Coming in v1.1";
    }

    /// <summary>
    /// Placeholder for Copy Preset functionality (planned for v1.1).
    /// </summary>
    [RelayCommand]
    private void CopyPreset()
    {
        StatusMessage = "Copy Preset (Ctrl+C) - Coming in v1.1";
    }
}
