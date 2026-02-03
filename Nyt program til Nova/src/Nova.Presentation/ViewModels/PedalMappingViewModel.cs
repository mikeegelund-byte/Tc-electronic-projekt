using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Domain.Models;

namespace Nova.Presentation.ViewModels;

public partial class PedalMappingViewModel : ObservableObject
{
    [ObservableProperty]
    private int _parameter;

    [ObservableProperty]
    private int _min;

    [ObservableProperty]
    private int _mid;

    [ObservableProperty]
    private int _max;

    public void LoadFromDump(SystemDump dump)
    {
        Parameter = dump.GetPedalParameter();
        Min = dump.GetPedalMin();
        Mid = dump.GetPedalMid();
        Max = dump.GetPedalMax();
    }
}
