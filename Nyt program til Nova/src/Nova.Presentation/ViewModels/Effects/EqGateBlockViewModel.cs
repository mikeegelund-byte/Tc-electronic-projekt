using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Domain.Models;

namespace Nova.Presentation.ViewModels.Effects;

/// <summary>
/// ViewModel for EQ + Noise Gate block.
/// Shows 3-band parametric EQ and gate settings.
/// </summary>
public partial class EqGateBlockViewModel : ObservableObject
{
    [ObservableProperty] private int _eqBand1Freq;
    [ObservableProperty] private int _eqBand1Gain;
    [ObservableProperty] private int _eqBand2Freq;
    [ObservableProperty] private int _eqBand2Gain;
    [ObservableProperty] private int _eqBand3Freq;
    [ObservableProperty] private int _eqBand3Gain;
    [ObservableProperty] private int _gateThreshold;
    [ObservableProperty] private bool _gateEnabled;

    public void LoadFromPreset(Preset? preset)
    {
        if (preset == null) return;

        // Note: EQ/Gate properties may need to be added to Preset model
        // For now, show placeholder
        EqBand1Freq = 100;
        EqBand1Gain = 0;
        EqBand2Freq = 1000;
        EqBand2Gain = 0;
        EqBand3Freq = 5000;
        EqBand3Gain = 0;
        GateThreshold = -60;
        GateEnabled = false;
    }
}
