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
    private byte _ccNumber;

    [ObservableProperty]
    private byte _parameterId;

    public bool IsAssigned => CcNumber != 0xFF && ParameterId != 0xFF;

    public CCMappingEditorViewModel(int index, byte ccNumber, byte parameterId)
    {
        _index = index;
        _ccNumber = ccNumber;
        _parameterId = parameterId;
    }

    partial void OnCcNumberChanged(byte value)
    {
        OnPropertyChanged(nameof(IsAssigned));
    }

    partial void OnParameterIdChanged(byte value)
    {
        OnPropertyChanged(nameof(IsAssigned));
    }
}

/// <summary>
/// ViewModel for MIDI CC Mapping Editor (Module 9).
/// Displays CC to parameter assignments and allows editing.
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
    /// Loads CC mappings from a SystemDump object.
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
                var editorVm = new CCMappingEditorViewModel(i, mapping.CCNumber, mapping.ParameterId);
                
                // Subscribe to property changes to track dirty state
                editorVm.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(CCMappingEditorViewModel.CcNumber) ||
                        e.PropertyName == nameof(CCMappingEditorViewModel.ParameterId))
                    {
                        HasUnsavedChanges = true;
                    }
                };
                
                CcMappings.Add(editorVm);
            }
            
            StatusMessage = $"Loaded {result.Value.Count} CC mappings";
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
                mapping.CcNumber,
                mapping.ParameterId);

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
            StatusMessage = "CC mappings saved successfully";
            HasUnsavedChanges = false;
        }
        else
        {
            StatusMessage = $"Failed to save: {saveResult.Errors.First().Message}";
        }
    }
}
