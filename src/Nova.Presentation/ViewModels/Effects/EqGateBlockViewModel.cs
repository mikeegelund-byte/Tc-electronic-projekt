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
    [ObservableProperty] private bool _eqEnabled;
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

    private int _gateDamp;
    public int GateDamp
    {
        get => _gateDamp;
        set
        {
            if (value < 0 || value > 90)
                throw new ArgumentOutOfRangeException(nameof(value), "Gate Damp must be between 0 and 90 dB");
            SetProperty(ref _gateDamp, value);
        }
    }

    private int _gateRelease;
    public int GateRelease
    {
        get => _gateRelease;
        set
        {
            if (value < 0 || value > 200)
                throw new ArgumentOutOfRangeException(nameof(value), "Gate Release must be between 0 and 200 dB/s");
            SetProperty(ref _gateRelease, value);
        }
    }

    private int _eqBand1Width;
    public int EqBand1Width
    {
        get => _eqBand1Width;
        set
        {
            if (value < 5 || value > 12)
                throw new ArgumentOutOfRangeException(nameof(value), "EQ Band 1 Width must be between 5 and 12");
            SetProperty(ref _eqBand1Width, value);
        }
    }

    private int _eqBand2Width;
    public int EqBand2Width
    {
        get => _eqBand2Width;
        set
        {
            if (value < 5 || value > 12)
                throw new ArgumentOutOfRangeException(nameof(value), "EQ Band 2 Width must be between 5 and 12");
            SetProperty(ref _eqBand2Width, value);
        }
    }

    private int _eqBand3Width;
    public int EqBand3Width
    {
        get => _eqBand3Width;
        set
        {
            if (value < 5 || value > 12)
                throw new ArgumentOutOfRangeException(nameof(value), "EQ Band 3 Width must be between 5 and 12");
            SetProperty(ref _eqBand3Width, value);
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
        EqBand1Width = Math.Clamp(preset.EqWidth1, 5, 12);
        EqBand2Freq = preset.EqFreq2;
        EqBand2Gain = Math.Clamp(preset.EqGain2, -12, 12);
        EqBand2Width = Math.Clamp(preset.EqWidth2, 5, 12);
        EqBand3Freq = preset.EqFreq3;
        EqBand3Gain = Math.Clamp(preset.EqGain3, -12, 12);
        EqBand3Width = Math.Clamp(preset.EqWidth3, 5, 12);
        GateThreshold = Math.Clamp(preset.GateThreshold, -60, 0);
        GateDamp = Math.Clamp(preset.GateDamp, 0, 90);
        GateRelease = Math.Clamp(preset.GateRelease, 0, 200);

        GateEnabled = preset.GateEnabled;
        EqEnabled = preset.EqEnabled;
        IsEnabled = GateEnabled || EqEnabled;
    }

    public void WriteToPreset(Preset preset)
    {
        if (preset == null) return;

        preset.GateType = Type switch
        {
            "Soft" => 1,
            _ => 0
        };

        preset.EqFreq1 = EqBand1Freq;
        preset.EqGain1 = EqBand1Gain;
        preset.EqWidth1 = EqBand1Width;
        preset.EqFreq2 = EqBand2Freq;
        preset.EqGain2 = EqBand2Gain;
        preset.EqWidth2 = EqBand2Width;
        preset.EqFreq3 = EqBand3Freq;
        preset.EqGain3 = EqBand3Gain;
        preset.EqWidth3 = EqBand3Width;
        preset.GateThreshold = GateThreshold;
        preset.GateDamp = GateDamp;
        preset.GateRelease = GateRelease;
        preset.GateEnabled = GateEnabled;
        preset.EqEnabled = EqEnabled;
    }

    partial void OnGateEnabledChanged(bool value)
    {
        IsEnabled = value || EqEnabled;
    }

    partial void OnEqEnabledChanged(bool value)
    {
        IsEnabled = value || GateEnabled;
    }
}
