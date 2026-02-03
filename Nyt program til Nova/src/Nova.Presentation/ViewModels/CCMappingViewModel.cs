using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using System.Collections.ObjectModel;

namespace Nova.Presentation.ViewModels;

/// <summary>
/// Editable wrapper for CCMapping to support DataGrid editing.
/// </summary>
public partial class CCMappingEditorViewModel : ObservableObject
{
    [ObservableProperty]
    private int _index;

    [ObservableProperty]
    private string _assignment = string.Empty;

    [ObservableProperty]
    private int? _ccNumber;

    public bool IsAssigned => CcNumber.HasValue;

    public CCMappingEditorViewModel(int index, string assignment, int? ccNumber)
    {
        _index = index;
        _assignment = assignment;
        _ccNumber = ccNumber;
    }

    partial void OnCcNumberChanged(int? value)
    {
        OnPropertyChanged(nameof(IsAssigned));
    }
}

/// <summary>
/// ViewModel for MIDI CC Assignment Editor.
/// Displays fixed assignment slots and allows editing.
/// </summary>
public partial class CCMappingViewModel : ObservableObject
{
    private readonly IGetCCMappingsUseCase _getCCMappingsUseCase;
    private readonly IUpdateCCMappingUseCase _updateCCMappingUseCase;
    private readonly ISaveSystemDumpUseCase _saveSystemDumpUseCase;
    private SystemDump? _currentSystemDump;

    [ObservableProperty]
    private ObservableCollection<CCMappingEditorViewModel> _ccMappings = new();

    [ObservableProperty]
    private string _statusMessage = "No System Dump loaded";

    [ObservableProperty]
    private bool _hasUnsavedChanges;

    [ObservableProperty]
    private PedalMappingViewModel _pedalMapping;

    public CCMappingViewModel(
        IGetCCMappingsUseCase getCCMappingsUseCase,
        IUpdateCCMappingUseCase updateCCMappingUseCase,
        ISaveSystemDumpUseCase saveSystemDumpUseCase)
    {
        _getCCMappingsUseCase = getCCMappingsUseCase;
        _updateCCMappingUseCase = updateCCMappingUseCase;
        _saveSystemDumpUseCase = saveSystemDumpUseCase;
        _pedalMapping = new PedalMappingViewModel();
    }

    /// <summary>
    /// Loads CC assignments from a SystemDump object.
    /// </summary>
    public async Task LoadFromDump(SystemDump dump)
    {
        if (dump == null)
        {
            CcMappings.Clear();
            _currentSystemDump = null;
            StatusMessage = "No System Dump loaded";
            HasUnsavedChanges = false;
            return;
        }

        _currentSystemDump = dump;

        // Load pedal mapping
        PedalMapping.LoadFromDump(dump);

        var result = await _getCCMappingsUseCase.ExecuteAsync(dump);

        if (result.IsSuccess)
        {
            CcMappings.Clear();
            for (int i = 0; i < result.Value.Count; i++)
            {
                var mapping = result.Value[i];
                var editorVm = new CCMappingEditorViewModel(i, mapping.Assignment, mapping.CCNumber);

                // Subscribe to property changes to track dirty state
                editorVm.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(CCMappingEditorViewModel.CcNumber))
                    {
                        HasUnsavedChanges = true;
                    }
                };

                CcMappings.Add(editorVm);
            }

            StatusMessage = $"Loaded {result.Value.Count} CC assignments";
            HasUnsavedChanges = false;
        }
        else
        {
            CcMappings.Clear();
            StatusMessage = result.Errors.First().Message;
        }
    }

    [RelayCommand]
    private async Task SaveChangesAsync()
    {
        if (_currentSystemDump == null)
        {
            StatusMessage = "No System Dump loaded";
            return;
        }

        // Apply all changes to SystemDump
        foreach (var mapping in CcMappings)
        {
            var updateResult = await _updateCCMappingUseCase.ExecuteAsync(
                _currentSystemDump,
                mapping.Index,
                mapping.CcNumber);

            if (updateResult.IsFailed)
            {
                StatusMessage = $"Failed to update mapping {mapping.Index}: {updateResult.Errors.First().Message}";
                return;
            }
        }

        // Save to MIDI device
        var saveResult = await _saveSystemDumpUseCase.ExecuteAsync(_currentSystemDump);

        if (saveResult.IsSuccess)
        {
            StatusMessage = "CC assignments saved successfully";
            HasUnsavedChanges = false;
        }
        else
        {
            StatusMessage = $"Failed to save: {saveResult.Errors.First().Message}";
        }
    }
}
