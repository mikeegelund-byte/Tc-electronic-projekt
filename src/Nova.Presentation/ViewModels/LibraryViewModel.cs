using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using Nova.Application.Library;
using Nova.Domain.Models;
using Nova.Presentation.Services;

namespace Nova.Presentation.ViewModels;

public partial class LibraryViewModel : ObservableObject
{
    private readonly ILibraryRepository _libraryRepository;
    private readonly List<LibraryPresetMetadata> _allPresets = new();
    private Preset? _currentPreset;

    [ObservableProperty]
    private ObservableCollection<LibraryPresetItemViewModel> _presets = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoadPresetCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeletePresetCommand))]
    [NotifyCanExecuteChangedFor(nameof(ExportPresetCommand))]
    private LibraryPresetItemViewModel? _selectedPreset;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _tagsInput = string.Empty;

    [ObservableProperty]
    private string _nameInput = string.Empty;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    public event Action<Preset>? PresetLoaded;

    public LibraryViewModel(ILibraryRepository libraryRepository)
    {
        _libraryRepository = libraryRepository;
    }

    public void SetSelectedPreset(Preset? preset)
    {
        _currentPreset = preset;
        SaveCurrentPresetCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private async Task RefreshLibraryAsync()
    {
        StatusMessage = "Loading library...";
        _allPresets.Clear();
        var items = await _libraryRepository.ListAsync(CancellationToken.None);
        _allPresets.AddRange(items);
        ApplyFilter();
        StatusMessage = $"Loaded {Presets.Count} presets";
    }

    [RelayCommand(CanExecute = nameof(CanSaveCurrentPreset))]
    private async Task SaveCurrentPresetAsync()
    {
        if (_currentPreset == null)
        {
            StatusMessage = "No preset selected";
            return;
        }

        var tags = ParseTags(TagsInput);
        var result = await _libraryRepository.SavePresetAsync(_currentPreset, NameInput, tags, CancellationToken.None);
        if (result.IsSuccess)
        {
            StatusMessage = "Preset saved to library";
            await RefreshLibraryAsync();
        }
        else
        {
            StatusMessage = $"Save failed: {result.Errors.First().Message}";
        }
    }

    private bool CanSaveCurrentPreset() => _currentPreset != null;

    [RelayCommand]
    private async Task ImportPresetAsync()
    {
        var filePath = await FileDialogService.PickOpenFileAsync(
            "Import Preset to Library",
            new[] { FileDialogService.SyxFileType });

        if (string.IsNullOrWhiteSpace(filePath)) return;

        try
        {
            var bytes = await File.ReadAllBytesAsync(filePath);
            var presetResult = Preset.FromSysEx(bytes);
            if (presetResult.IsFailed)
            {
                StatusMessage = $"Import failed: {presetResult.Errors.First().Message}";
                return;
            }

            var nameOverride = Path.GetFileNameWithoutExtension(filePath);
            var tags = ParseTags(TagsInput);
            var result = await _libraryRepository.SavePresetAsync(presetResult.Value, nameOverride, tags, CancellationToken.None);
            if (result.IsSuccess)
            {
                StatusMessage = "Preset imported to library";
                await RefreshLibraryAsync();
            }
            else
            {
                StatusMessage = $"Import failed: {result.Errors.First().Message}";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Import failed: {ex.Message}";
        }
    }

    [RelayCommand(CanExecute = nameof(CanLoadPreset))]
    private async Task LoadPresetAsync()
    {
        if (SelectedPreset == null) return;

        var result = await _libraryRepository.LoadPresetAsync(SelectedPreset.Id, CancellationToken.None);
        if (result.IsSuccess)
        {
            StatusMessage = $"Loaded '{SelectedPreset.Name}'";
            PresetLoaded?.Invoke(result.Value);
        }
        else
        {
            StatusMessage = $"Load failed: {result.Errors.First().Message}";
        }
    }

    private bool CanLoadPreset() => SelectedPreset != null;

    [RelayCommand(CanExecute = nameof(CanDeletePreset))]
    private async Task DeletePresetAsync()
    {
        if (SelectedPreset == null) return;

        var result = await _libraryRepository.DeletePresetAsync(SelectedPreset.Id, CancellationToken.None);
        if (result.IsSuccess)
        {
            StatusMessage = "Preset deleted";
            await RefreshLibraryAsync();
        }
        else
        {
            StatusMessage = $"Delete failed: {result.Errors.First().Message}";
        }
    }

    private bool CanDeletePreset() => SelectedPreset != null;

    [RelayCommand(CanExecute = nameof(CanExportPreset))]
    private async Task ExportPresetAsync()
    {
        if (SelectedPreset == null) return;

        var filePath = await FileDialogService.PickSaveFileAsync(
            "Export Library Preset",
            $"{SelectedPreset.Name}.syx",
            new[] { FileDialogService.SyxFileType },
            "syx");

        if (string.IsNullOrWhiteSpace(filePath)) return;

        var result = await _libraryRepository.LoadPresetAsync(SelectedPreset.Id, CancellationToken.None);
        if (result.IsFailed)
        {
            StatusMessage = $"Export failed: {result.Errors.First().Message}";
            return;
        }

        var sysexResult = result.Value.ToSysEx();
        if (sysexResult.IsFailed)
        {
            StatusMessage = $"Export failed: {sysexResult.Errors.First().Message}";
            return;
        }

        await File.WriteAllBytesAsync(filePath, sysexResult.Value);
        StatusMessage = $"Exported to {Path.GetFileName(filePath)}";
    }

    private bool CanExportPreset() => SelectedPreset != null;

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var query = SearchText?.Trim() ?? string.Empty;
        var filtered = _allPresets.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            filtered = filtered.Where(m =>
                m.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                m.Tags.Any(t => t.Contains(query, StringComparison.OrdinalIgnoreCase)));
        }

        Presets.Clear();
        foreach (var item in filtered.OrderByDescending(m => m.UpdatedAtUtc))
        {
            Presets.Add(new LibraryPresetItemViewModel(item));
        }
    }

    private static IReadOnlyList<string> ParseTags(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Array.Empty<string>();

        return input
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(t => t.Length > 0)
            .ToList();
    }
}
