using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Domain.Models;

namespace Nova.Presentation.ViewModels.Effects;

/// <summary>
/// ViewModel for EQ + Noise Gate block.
/// Shows 3-band parametric EQ and gate settings.
/// </summary>
public partial class EqGateBlockViewModel : ObservableObject
{
    [ObservableProperty] private string _type = "Hard";
    [ObservableProperty] private int _eqBand1Freq;
    [ObservableProperty] private int _eqBand2Freq;
    [ObservableProperty] private int _eqBand3Freq;
    [ObservableProperty] private bool _gateEnabled;
    [ObservableProperty] private bool _isEnabled;

    private int _eqBand1Gain;
    public int EqBand1Gain
    {
        get => _eqBand1Gain;
        set
        {
            if (value < -12 || value > 12)
                throw new ArgumentOutOfRangeException(nameof(value), "EQ Band 1 Gain must be between -12 and 12 dB");
            SetProperty(ref _eqBand1Gain, value);
        }
    }

    private int _eqBand2Gain;
    public int EqBand2Gain
    {
        get => _eqBand2Gain;
        set
        {
            if (value < -12 || value > 12)
                throw new ArgumentOutOfRangeException(nameof(value), "EQ Band 2 Gain must be between -12 and 12 dB");
            SetProperty(ref _eqBand2Gain, value);
        }
    }

    private int _eqBand3Gain;
    public int EqBand3Gain
    {
        get => _eqBand3Gain;
        set
        {
            if (value < -12 || value > 12)
                throw new ArgumentOutOfRangeException(nameof(value), "EQ Band 3 Gain must be between -12 and 12 dB");
            SetProperty(ref _eqBand3Gain, value);
        }
    }

    private int _gateThreshold;
    public int GateThreshold
    {
        get => _gateThreshold;
        set
        {
            if (value < -60 || value > 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Gate Threshold must be between -60 and 0 dB");
            SetProperty(ref _gateThreshold, value);
        }
    }

    public void LoadFromPreset(Preset? preset)
    {
        if (preset == null) return;

        Type = preset.GateType switch
        {
            0 => "Hard",
            1 => "Soft",
            _ => "Unknown"
        };

        EqBand1Freq = preset.EqFreq1;
        EqBand1Gain = Math.Clamp(preset.EqGain1, -12, 12);
        EqBand2Freq = preset.EqFreq2;
        EqBand2Gain = Math.Clamp(preset.EqGain2, -12, 12);
        EqBand3Freq = preset.EqFreq3;
        EqBand3Gain = Math.Clamp(preset.EqGain3, -12, 12);
        GateThreshold = Math.Clamp(preset.GateThreshold, -60, 0);

        GateEnabled = true;
        IsEnabled = GateEnabled;
    }
}
