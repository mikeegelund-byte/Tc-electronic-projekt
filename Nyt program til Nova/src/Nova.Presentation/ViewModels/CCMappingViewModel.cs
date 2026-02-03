using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using System.Collections.ObjectModel;

namespace Nova.Presentation.ViewModels;

/// <summary>
/// ViewModel for MIDI CC Mapping Editor (Module 9).
/// Displays CC to parameter assignments and allows editing.
/// </summary>
public partial class CCMappingViewModel : ObservableObject
{
    private readonly IGetCCMappingsUseCase _getCCMappingsUseCase;

    [ObservableProperty]
    private ObservableCollection<CCMapping> _ccMappings = new();

    [ObservableProperty]
    private string _statusMessage = "No System Dump loaded";

    public CCMappingViewModel(IGetCCMappingsUseCase getCCMappingsUseCase)
    {
        _getCCMappingsUseCase = getCCMappingsUseCase;
    }

    /// <summary>
    /// Loads CC mappings from a SystemDump object.
    /// </summary>
    public async Task LoadFromDump(SystemDump dump)
    {
        if (dump == null)
        {
            CcMappings.Clear();
            StatusMessage = "No System Dump loaded";
            return;
        }

        var result = await _getCCMappingsUseCase.ExecuteAsync(dump);

        if (result.IsSuccess)
        {
            CcMappings = new ObservableCollection<CCMapping>(result.Value);
            StatusMessage = $"Loaded {result.Value.Count} CC mappings";
        }
        else
        {
            CcMappings.Clear();
            StatusMessage = result.Errors.First().Message;
        }
    }
}
