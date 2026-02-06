namespace Nova.Presentation.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nova.Domain.Models;

/// <summary>
/// ViewModel for the preset list view.
/// Displays all 60 presets from the User Bank dump.
/// </summary>
public partial class PresetListViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<PresetSummaryViewModel> _presets = new();

    [ObservableProperty]
    private PresetSummaryViewModel? _selectedPreset;

    [ObservableProperty]
    private bool _hasPresets;

    /// <summary>
    /// Loads presets from a UserBankDump and populates the list.
    /// Sorts by preset number (31-90) for consistent display order.
    /// </summary>
    public void LoadFromBank(UserBankDump bank)
    {
        Presets.Clear();
        
        var sorted = bank.Presets
            .Where(p => p != null)
            .OrderBy(p => p!.Number)
            .Select(p => PresetSummaryViewModel.FromPreset(p!));

        foreach (var preset in sorted)
        {
            Presets.Add(preset);
        }

        HasPresets = Presets.Count > 0;
    }

    /// <summary>
    /// Clears all presets from the list.
    /// </summary>
    public void Clear()
    {
        Presets.Clear();
        SelectedPreset = null;
        HasPresets = false;
    }

    /// <summary>
    /// Copies the selected preset to a new slot.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecutePresetAction))]
    private void CopyPreset()
    {
        if (SelectedPreset is null)
            return;
        
        // TODO: Show dialog to select target slot
        // Will be wired to ICopyPresetUseCase
    }

    /// <summary>
    /// Renames the selected preset.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecutePresetAction))]
    private void RenamePreset()
    {
        if (SelectedPreset is null)
            return;
        
        // TODO: Show dialog to enter new name
        // Will be wired to IRenamePresetUseCase
    }

    /// <summary>
    /// Deletes the selected preset.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecutePresetAction))]
    private void DeletePreset()
    {
        if (SelectedPreset is null)
            return;
        
        // TODO: Show confirmation dialog
        // Will be wired to IDeletePresetUseCase
    }

    private bool CanExecutePresetAction()
    {
        return SelectedPreset != null;
    }
}
