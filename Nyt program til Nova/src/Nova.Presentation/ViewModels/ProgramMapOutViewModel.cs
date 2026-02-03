using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using System.Collections.ObjectModel;

namespace Nova.Presentation.ViewModels;

public partial class ProgramMapOutEntryViewModel : ObservableObject
{
    [ObservableProperty]
    private int _presetNumber;

    [ObservableProperty]
    private string _presetDisplay = string.Empty;

    [ObservableProperty]
    private int _outgoingProgram;

    public ProgramMapOutEntryViewModel(int presetNumber, int outgoingProgram)
    {
        _presetNumber = presetNumber;
        _outgoingProgram = outgoingProgram;
        _presetDisplay = ProgramMapInEntryViewModel.FormatPresetNumber(presetNumber);
    }
}

public partial class ProgramMapOutViewModel : ObservableObject
{
    private readonly IGetProgramMapOutUseCase _getProgramMapOutUseCase;
    private readonly IUpdateProgramMapOutUseCase _updateProgramMapOutUseCase;
    private readonly ISaveSystemDumpUseCase _saveSystemDumpUseCase;
    private SystemDump? _currentSystemDump;

    [ObservableProperty]
    private ObservableCollection<ProgramMapOutEntryViewModel> _entries = new();

    [ObservableProperty]
    private string _statusMessage = "No System Dump loaded";

    [ObservableProperty]
    private bool _hasUnsavedChanges;

    public ProgramMapOutViewModel(
        IGetProgramMapOutUseCase getProgramMapOutUseCase,
        IUpdateProgramMapOutUseCase updateProgramMapOutUseCase,
        ISaveSystemDumpUseCase saveSystemDumpUseCase)
    {
        _getProgramMapOutUseCase = getProgramMapOutUseCase;
        _updateProgramMapOutUseCase = updateProgramMapOutUseCase;
        _saveSystemDumpUseCase = saveSystemDumpUseCase;
    }

    public async Task LoadFromDump(SystemDump dump)
    {
        if (dump == null)
        {
            Entries.Clear();
            _currentSystemDump = null;
            StatusMessage = "No System Dump loaded";
            HasUnsavedChanges = false;
            return;
        }

        _currentSystemDump = dump;
        var result = await _getProgramMapOutUseCase.ExecuteAsync(dump);

        if (result.IsSuccess)
        {
            Entries.Clear();
            foreach (var entry in result.Value)
            {
                var vm = new ProgramMapOutEntryViewModel(entry.PresetNumber, entry.OutgoingProgram);
                vm.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(ProgramMapOutEntryViewModel.OutgoingProgram))
                        HasUnsavedChanges = true;
                };
                Entries.Add(vm);
            }

            StatusMessage = $"Loaded {result.Value.Count} program map entries";
            HasUnsavedChanges = false;
        }
        else
        {
            Entries.Clear();
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

        foreach (var entry in Entries)
        {
            var updateResult = await _updateProgramMapOutUseCase.ExecuteAsync(
                _currentSystemDump,
                entry.PresetNumber,
                entry.OutgoingProgram);

            if (updateResult.IsFailed)
            {
                StatusMessage = $"Failed to update preset {entry.PresetNumber}: {updateResult.Errors.First().Message}";
                return;
            }
        }

        var saveResult = await _saveSystemDumpUseCase.ExecuteAsync(_currentSystemDump);
        if (saveResult.IsSuccess)
        {
            StatusMessage = "Program Map Out saved successfully";
            HasUnsavedChanges = false;
        }
        else
        {
            StatusMessage = $"Failed to save: {saveResult.Errors.First().Message}";
        }
    }
}
