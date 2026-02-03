using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using System.Collections.ObjectModel;

namespace Nova.Presentation.ViewModels;

public partial class ProgramMapInEntryViewModel : ObservableObject
{
    [ObservableProperty]
    private int _incomingProgram;

    [ObservableProperty]
    private int? _presetNumber;

    [ObservableProperty]
    private string _presetDisplay = "None";

    public bool IsAssigned => PresetNumber.HasValue;

    public ProgramMapInEntryViewModel(int incomingProgram, int? presetNumber)
    {
        _incomingProgram = incomingProgram;
        _presetNumber = presetNumber;
        _presetDisplay = FormatPresetNumber(presetNumber);
    }

    partial void OnPresetNumberChanged(int? value)
    {
        PresetDisplay = FormatPresetNumber(value);
        OnPropertyChanged(nameof(IsAssigned));
    }

    internal static string FormatPresetNumber(int? presetNumber)
    {
        if (!presetNumber.HasValue)
            return "None";

        var value = presetNumber.Value;
        if (value <= 0 || value > 90)
            return "Invalid";

        if (value <= 30)
        {
            var bank = (value - 1) / 3;
            var slot = ((value - 1) % 3) + 1;
            return $"F{bank}-{slot}";
        }

        var userBank = (value - 31) / 3;
        var userSlot = ((value - 31) % 3) + 1;
        return $"{userBank:D2}-{userSlot}";
    }
}

public partial class ProgramMapInViewModel : ObservableObject
{
    private readonly IGetProgramMapInUseCase _getProgramMapInUseCase;
    private readonly IUpdateProgramMapInUseCase _updateProgramMapInUseCase;
    private readonly ISaveSystemDumpUseCase _saveSystemDumpUseCase;
    private SystemDump? _currentSystemDump;

    [ObservableProperty]
    private ObservableCollection<ProgramMapInEntryViewModel> _entries = new();

    [ObservableProperty]
    private string _statusMessage = "No System Dump loaded";

    [ObservableProperty]
    private bool _hasUnsavedChanges;

    public ProgramMapInViewModel(
        IGetProgramMapInUseCase getProgramMapInUseCase,
        IUpdateProgramMapInUseCase updateProgramMapInUseCase,
        ISaveSystemDumpUseCase saveSystemDumpUseCase)
    {
        _getProgramMapInUseCase = getProgramMapInUseCase;
        _updateProgramMapInUseCase = updateProgramMapInUseCase;
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
        var result = await _getProgramMapInUseCase.ExecuteAsync(dump);

        if (result.IsSuccess)
        {
            Entries.Clear();
            foreach (var entry in result.Value)
            {
                var vm = new ProgramMapInEntryViewModel(entry.IncomingProgram, entry.PresetNumber);
                vm.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(ProgramMapInEntryViewModel.PresetNumber))
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
            var updateResult = await _updateProgramMapInUseCase.ExecuteAsync(
                _currentSystemDump,
                entry.IncomingProgram,
                entry.PresetNumber);

            if (updateResult.IsFailed)
            {
                StatusMessage = $"Failed to update program {entry.IncomingProgram}: {updateResult.Errors.First().Message}";
                return;
            }
        }

        var saveResult = await _saveSystemDumpUseCase.ExecuteAsync(_currentSystemDump);
        if (saveResult.IsSuccess)
        {
            StatusMessage = "Program Map In saved successfully";
            HasUnsavedChanges = false;
        }
        else
        {
            StatusMessage = $"Failed to save: {saveResult.Errors.First().Message}";
        }
    }
}
