using System.Collections.ObjectModel;
using System.IO;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Nova.Infrastructure.Midi;
using Nova.Midi;
using Nova.Presentation.Services;

namespace Nova.Presentation.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IMidiPort _midiPort;
    private readonly IConnectUseCase _connectUseCase;
    private readonly IDownloadBankUseCase _downloadBankUseCase;
    private readonly ISaveSystemDumpUseCase _saveSystemDumpUseCase;
    private readonly IRequestSystemDumpUseCase _requestSystemDumpUseCase;
    private readonly SaveBankUseCase _saveBankUseCase;
    private readonly LoadBankUseCase _loadBankUseCase;
    private readonly ExportPresetUseCase _exportPresetUseCase;
    private readonly IExportPresetUseCase _exportSyxPresetUseCase;
    private readonly ImportPresetUseCase _importPresetUseCase;
    private readonly ISavePresetUseCase _savePresetUseCase;
    private UserBankDump? _currentBank;
    private SystemDump? _currentSystemDump;

    [ObservableProperty]
    private ObservableCollection<string> _availablePorts = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand))]
    private string? _selectedPort;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectCommand), nameof(DownloadBankCommand))]
    [NotifyCanExecuteChangedFor(nameof(DownloadSystemSettingsCommand), nameof(SaveBankCommand), nameof(LoadBankCommand), nameof(ImportPresetCommand))]
    private bool _isConnected;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DownloadBankCommand), nameof(DownloadSystemSettingsCommand), nameof(SaveBankCommand), nameof(LoadBankCommand), nameof(SaveSystemSettingsCommand), nameof(ImportPresetCommand))]
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

    [ObservableProperty]
    private CCMappingViewModel _ccMapping;

    public MainViewModel(
        IMidiPort midiPort,
        IConnectUseCase connectUseCase,
        IDownloadBankUseCase downloadBankUseCase,
        ISaveSystemDumpUseCase saveSystemDumpUseCase,
        IRequestSystemDumpUseCase requestSystemDumpUseCase,
        SaveBankUseCase saveBankUseCase,
        LoadBankUseCase loadBankUseCase,
        ExportPresetUseCase exportPresetUseCase,
        IExportPresetUseCase exportSyxPresetUseCase,
        ImportPresetUseCase importPresetUseCase,
        ISavePresetUseCase savePresetUseCase,
        CCMappingViewModel ccMappingViewModel)
    {
        _midiPort = midiPort;
        _connectUseCase = connectUseCase;
        _downloadBankUseCase = downloadBankUseCase;
        _saveSystemDumpUseCase = saveSystemDumpUseCase;
        _requestSystemDumpUseCase = requestSystemDumpUseCase;
        _saveBankUseCase = saveBankUseCase;
        _loadBankUseCase = loadBankUseCase;
        _exportPresetUseCase = exportPresetUseCase;
        _exportSyxPresetUseCase = exportSyxPresetUseCase;
        _importPresetUseCase = importPresetUseCase;
        _savePresetUseCase = savePresetUseCase;
        _ccMapping = ccMappingViewModel;
        
        // Auto-refresh ports on startup
        RefreshPorts();
        
        // Subscribe to preset selection changes
        PresetList.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(PresetListViewModel.SelectedPreset))
            {
                OnPresetSelectionChanged();
                ExportPresetCommand.NotifyCanExecuteChanged();
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
        StatusMessage = "Requesting system settings from pedal...";

        try
        {
            var result = await _requestSystemDumpUseCase.ExecuteAsync(timeoutMs: 8000);

            if (result.IsSuccess)
            {
                _currentSystemDump = result.Value;
                SystemSettings.LoadFromDump(result.Value);
                await CcMapping.LoadFromDump(result.Value);
                StatusMessage = "System settings loaded successfully";
                SaveSystemSettingsCommand.NotifyCanExecuteChanged();
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
        if (_currentSystemDump == null)
        {
            StatusMessage = "No system settings loaded";
            return;
        }

        SystemSettings.LoadFromDump(_currentSystemDump);
        StatusMessage = "Changes reverted";
    }

    private bool CanSaveSystemSettings() => IsConnected && !IsDownloading && _currentSystemDump != null;

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

    [RelayCommand(CanExecute = nameof(CanUseDevice))]
    private async Task SaveBankAsync()
    {
        var path = await FileDialogService.PickSaveFileAsync(
            "Save User Bank",
            $"NovaBank_{DateTime.Now:yyyyMMdd_HHmmss}.syx",
            new[] { FileDialogService.SyxFileType, FileDialogService.AllFilesType },
            "syx");

        if (string.IsNullOrWhiteSpace(path))
        {
            UpdateFileStatus("Save bank cancelled");
            return;
        }

        IsDownloading = true;
        StatusMessage = "Waiting for User Bank dump... trigger 'Send Dump' on pedal";
        UpdateFileStatus("Waiting for User Bank dump... trigger 'Send Dump' on pedal", path);
        DownloadProgress = 0;

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            var result = await _saveBankUseCase.ExecuteAsync(path, cts.Token);

            if (result.IsSuccess)
            {
                UpdateFileStatus("Bank saved successfully", result.Value);
                StatusMessage = "Bank saved to file";
            }
            else
            {
                UpdateFileStatus($"Save failed: {result.Errors.First().Message}");
                StatusMessage = $"Error: {result.Errors.First().Message}";
            }
        }
        catch (Exception ex)
        {
            UpdateFileStatus($"Save failed: {ex.Message}");
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsDownloading = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanUseDevice))]
    private async Task LoadBankAsync()
    {
        var path = await FileDialogService.PickOpenFileAsync(
            "Load User Bank",
            new[] { FileDialogService.SyxFileType, FileDialogService.AllFilesType });

        if (string.IsNullOrWhiteSpace(path))
        {
            UpdateFileStatus("Load bank cancelled");
            return;
        }

        IsDownloading = true;
        StatusMessage = "Sending bank to pedal...";
        UpdateFileStatus("Sending bank to pedal...", path);
        DownloadProgress = 0;

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            var progress = new Progress<int>(count =>
            {
                DownloadProgress = (int)Math.Round((count / 60.0) * 100);
            });

            var result = await _loadBankUseCase.ExecuteAsync(path, progress, cts.Token);

            if (result.IsSuccess)
            {
                UpdateFileStatus($"Bank loaded to pedal ({result.Value}/60 presets)", path);
                StatusMessage = "Bank loaded to pedal";
                DownloadProgress = 100;
            }
            else
            {
                UpdateFileStatus($"Load failed: {result.Errors.First().Message}");
                StatusMessage = $"Error: {result.Errors.First().Message}";
                DownloadProgress = 0;
            }
        }
        catch (Exception ex)
        {
            UpdateFileStatus($"Load failed: {ex.Message}");
            StatusMessage = $"Error: {ex.Message}";
            DownloadProgress = 0;
        }
        finally
        {
            IsDownloading = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanExportPreset))]
    private async Task ExportPresetAsync()
    {
        var preset = GetSelectedPreset();
        if (preset == null)
        {
            UpdateFileStatus("Select a preset to export");
            return;
        }

        var suggestedName = $"{SanitizeFileName(preset.Name)}_{preset.Number}.syx";
        var path = await FileDialogService.PickSaveFileAsync(
            "Export Preset",
            suggestedName,
            new[] { FileDialogService.SyxFileType, FileDialogService.TextFileType, FileDialogService.AllFilesType },
            "syx");

        if (string.IsNullOrWhiteSpace(path))
        {
            UpdateFileStatus("Export cancelled");
            return;
        }

        var extension = Path.GetExtension(path).ToLowerInvariant();
        try
        {
            if (extension == ".txt")
            {
                var result = await _exportPresetUseCase.ExecuteAsync(preset, path, CancellationToken.None);
                if (result.IsSuccess)
                {
                    UpdateFileStatus("Preset exported to text file", path);
                }
                else
                {
                    UpdateFileStatus($"Export failed: {result.Errors.First().Message}");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(extension))
                {
                    path = path + ".syx";
                }

                var result = await _exportSyxPresetUseCase.ExecuteAsync(preset, path);
                if (result.IsSuccess)
                {
                    UpdateFileStatus("Preset exported to .syx", path);
                }
                else
                {
                    UpdateFileStatus($"Export failed: {result.Errors.First().Message}");
                }
            }
        }
        catch (Exception ex)
        {
            UpdateFileStatus($"Export failed: {ex.Message}");
        }
    }

    [RelayCommand(CanExecute = nameof(CanUseDevice))]
    private async Task ImportPresetAsync()
    {
        var path = await FileDialogService.PickOpenFileAsync(
            "Import Preset",
            new[] { FileDialogService.SyxFileType, FileDialogService.TextFileType, FileDialogService.AllFilesType });

        if (string.IsNullOrWhiteSpace(path))
        {
            UpdateFileStatus("Import cancelled");
            return;
        }

        Preset? importedPreset = null;
        var extension = Path.GetExtension(path).ToLowerInvariant();

        try
        {
            if (extension == ".txt")
            {
                var result = await _importPresetUseCase.ExecuteAsync(path, CancellationToken.None);
                if (result.IsFailed)
                {
                    UpdateFileStatus($"Import failed: {result.Errors.First().Message}");
                    return;
                }
                importedPreset = result.Value;
            }
            else
            {
                var data = await File.ReadAllBytesAsync(path);
                var result = Preset.FromSysEx(data);
                if (result.IsFailed)
                {
                    UpdateFileStatus($"Import failed: {result.Errors.First().Message}");
                    return;
                }
                importedPreset = result.Value;
            }

            var targetPresetNumber = PresetList.SelectedPreset?.Number ?? importedPreset.Number;
            if (targetPresetNumber < 31 || targetPresetNumber > 90)
            {
                UpdateFileStatus("Select a target preset (31-90) before importing");
                return;
            }

            var presetForNumber = CreatePresetForNumber(importedPreset, targetPresetNumber);
            var saveResult = await _savePresetUseCase.ExecuteAsync(presetForNumber, targetPresetNumber);

            if (saveResult.IsSuccess)
            {
                UpdateFileStatus($"Preset imported to preset #{targetPresetNumber}", path);
                StatusMessage = $"Preset saved to preset #{targetPresetNumber}";
                UpdateCurrentBank(presetForNumber);
            }
            else
            {
                UpdateFileStatus($"Save failed: {saveResult.Errors.First().Message}");
                StatusMessage = $"Error: {saveResult.Errors.First().Message}";
            }
        }
        catch (Exception ex)
        {
            UpdateFileStatus($"Import failed: {ex.Message}");
            StatusMessage = $"Error: {ex.Message}";
        }
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

    private bool CanUseDevice() => IsConnected && !IsDownloading;

    private bool CanExportPreset() => _currentBank != null && PresetList.SelectedPreset != null;

    private Preset? GetSelectedPreset()
    {
        if (_currentBank == null || PresetList.SelectedPreset == null)
            return null;

        return _currentBank.Presets.FirstOrDefault(p => p?.Number == PresetList.SelectedPreset.Number);
    }

    private void UpdateFileStatus(string message, string? path = null)
    {
        FileManager.StatusMessage = message;
        if (!string.IsNullOrWhiteSpace(path))
        {
            FileManager.CurrentFilePath = path;
        }
    }

    private string SanitizeFileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Preset";

        var invalid = Path.GetInvalidFileNameChars();
        var safe = new string(name.Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray());
        return string.IsNullOrWhiteSpace(safe) ? "Preset" : safe.Trim();
    }

    private Preset CreatePresetForNumber(Preset preset, int presetNumber)
    {
        var sysexResult = preset.ToSysEx();
        if (sysexResult.IsFailed)
            return preset;

        var sysex = sysexResult.Value;
        if (sysex.Length > 8)
        {
            sysex[8] = (byte)presetNumber;
        }

        var parsed = Preset.FromSysEx(sysex);
        return parsed.IsSuccess ? parsed.Value : preset;
    }

    private void UpdateCurrentBank(Preset preset)
    {
        if (_currentBank == null)
            return;

        var updated = _currentBank.WithPreset(preset.Number, preset);
        if (updated.IsSuccess)
        {
            _currentBank = updated.Value;
            PresetList.LoadFromBank(_currentBank);
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
