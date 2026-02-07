using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Nova.Presentation.Services;

namespace Nova.Presentation.ViewModels;

/// <summary>
/// ViewModel for file management operations.
/// Handles save/load of bank dumps and individual presets.
/// </summary>
public partial class FileManagerViewModel : ObservableObject
{
    private readonly ISaveBankUseCase _saveBankUseCase;
    private readonly ILoadBankUseCase _loadBankUseCase;
    private readonly IExportPresetUseCase _exportPresetUseCase;
    private readonly IImportPresetUseCase _importPresetUseCase;
    private readonly ISendBankToHardwareUseCase _sendBankUseCase;
    private UserBankDump? _currentBank;
    private Preset? _selectedPreset;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private string _currentFilePath = string.Empty;

    public FileManagerViewModel(
        ISaveBankUseCase saveBankUseCase,
        ILoadBankUseCase loadBankUseCase,
        IExportPresetUseCase exportPresetUseCase,
        IImportPresetUseCase importPresetUseCase,
        ISendBankToHardwareUseCase sendBankUseCase)
    {
        _saveBankUseCase = saveBankUseCase;
        _loadBankUseCase = loadBankUseCase;
        _exportPresetUseCase = exportPresetUseCase;
        _importPresetUseCase = importPresetUseCase;
        _sendBankUseCase = sendBankUseCase;
    }

    public void LoadFromBank(UserBankDump bank)
    {
        _currentBank = bank;
        UploadBankToHardwareCommand.NotifyCanExecuteChanged();
    }

    public void SetSelectedPreset(Preset? preset)
    {
        _selectedPreset = preset;
        ExportPresetCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private async Task SaveBankAsync()
    {
        var filePath = await FileDialogService.PickSaveFileAsync(
            "Save Bank",
            $"NovaBank_{DateTime.Now:yyyyMMdd_HHmmss}.syx",
            new[] { FileDialogService.SyxFileType },
            "syx");

        if (string.IsNullOrWhiteSpace(filePath)) return;

        CurrentFilePath = filePath;
        StatusMessage = "Saving bank (downloading from device)...";

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
        var saveResult = await _saveBankUseCase.ExecuteAsync(filePath, cts.Token);

        StatusMessage = saveResult.IsSuccess
            ? $"Bank saved to {Path.GetFileName(filePath)}"
            : $"Save failed: {saveResult.Errors.First().Message}";
    }

    [RelayCommand]
    private async Task LoadBankAsync()
    {
        var filePath = await FileDialogService.PickOpenFileAsync(
            "Load Bank",
            new[] { FileDialogService.SyxFileType });

        if (string.IsNullOrWhiteSpace(filePath)) return;

        CurrentFilePath = filePath;
        StatusMessage = "Loading bank...";

        var progress = new Progress<int>(p => StatusMessage = $"Loading preset {p}/60...");
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));

        var loadResult = await _loadBankUseCase.ExecuteAsync(filePath, progress, cts.Token);

        if (loadResult.IsSuccess)
        {
            _currentBank = loadResult.Value;
            UploadBankToHardwareCommand.NotifyCanExecuteChanged();
            StatusMessage = $"Bank loaded from {Path.GetFileName(filePath)} - 60 presets sent to pedal";
        }
        else
        {
            StatusMessage = $"Load failed: {loadResult.Errors.First().Message}";
        }
    }

    [RelayCommand(CanExecute = nameof(CanExportPreset))]
    private async Task ExportPresetAsync()
    {
        if (_selectedPreset == null)
        {
            StatusMessage = "No preset selected";
            return;
        }

        var filePath = await FileDialogService.PickSaveFileAsync(
            "Export Preset",
            $"{_selectedPreset.Name}.syx",
            new[] { FileDialogService.SyxFileType },
            "syx");

        if (string.IsNullOrWhiteSpace(filePath)) return;

        CurrentFilePath = filePath;
        StatusMessage = "Exporting preset...";

        var exportResult = await _exportPresetUseCase.ExecuteAsync(_selectedPreset, filePath);

        StatusMessage = exportResult.IsSuccess
            ? $"Preset exported to {Path.GetFileName(filePath)}"
            : $"Export failed: {exportResult.Errors.First().Message}";
    }

    private bool CanExportPreset() => _selectedPreset != null;

    [RelayCommand]
    private async Task ImportPresetAsync()
    {
        var filePath = await FileDialogService.PickOpenFileAsync(
            "Import Preset",
            new[]
            {
                FileDialogService.TextFileType,
                FileDialogService.SyxFileType
            });

        if (string.IsNullOrWhiteSpace(filePath)) return;

        CurrentFilePath = filePath;
        StatusMessage = "Importing preset...";

        var importResult = await _importPresetUseCase.ExecuteAsync(filePath);

        if (importResult.IsSuccess)
        {
            StatusMessage = $"Preset imported: {importResult.Value.Name}";
            _selectedPreset = importResult.Value;
            ExportPresetCommand.NotifyCanExecuteChanged();
        }
        else
        {
            StatusMessage = $"Import failed: {importResult.Errors.First().Message}";
        }
    }

    [RelayCommand(CanExecute = nameof(CanUploadBank))]
    private async Task UploadBankToHardwareAsync()
    {
        if (_currentBank == null)
        {
            StatusMessage = "No bank loaded. Download or load bank first.";
            return;
        }

        StatusMessage = "Uploading 60 presets to pedal...";
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

        var result = await _sendBankUseCase.ExecuteAsync(_currentBank, cts.Token);

        StatusMessage = result.IsSuccess
            ? "All 60 presets uploaded successfully!"
            : $"Upload failed: {result.Errors.First().Message}";
    }

    private bool CanUploadBank() => _currentBank != null;
}
