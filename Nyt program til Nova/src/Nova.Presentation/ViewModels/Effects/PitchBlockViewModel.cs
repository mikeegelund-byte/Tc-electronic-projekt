using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Domain.Models;

namespace Nova.Presentation.ViewModels.Effects;

/// <summary>
/// ViewModel for Pitch effect block.
/// Supports Pitch Shifter, Octaver, Whammy, Detune, and Intelligent Pitch modes.
/// </summary>
public partial class PitchBlockViewModel : ObservableObject
{
    private int _type;
    public int Type
    {
        get => _type;
        set
        {
            if (value < 0 || value > 4)
                throw new ArgumentOutOfRangeException(nameof(value), "Type must be between 0 and 4");
            SetProperty(ref _type, value);
        }
    }
    
    private int _voice1;
    public int Voice1
    {
        get => _voice1;
        set
        {
            if (value < -100 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Voice1 must be between -100 and +100 cents");
            SetProperty(ref _voice1, value);
        }
    }
    
    private int _voice2;
    public int Voice2
    {
        get => _voice2;
        set
        {
            if (value < -100 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Voice2 must be between -100 and +100 cents");
            SetProperty(ref _voice2, value);
        }
    }
    
    private int _mix;
    public int Mix
    {
        get => _mix;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Mix must be between 0 and 100%");
            SetProperty(ref _mix, value);
        }
    }
    
    [ObservableProperty] private bool _isEnabled;

    public void LoadFromPreset(Preset? preset)
    {
        if (preset == null) return;

        // Clamp values to validation ranges
        Type = Math.Clamp(preset.PitchType, 0, 4);
        Voice1 = Math.Clamp(preset.PitchVoice1, -100, 100);
        Voice2 = Math.Clamp(preset.PitchVoice2, -100, 100);
        // Use PitchLevel1 as Mix since there's no PitchMix property
        Mix = Math.Clamp(preset.PitchLevel1 + 100, 0, 100); // Convert -100 to 0dB → 0-100%
        // Note: Preset doesn't have PitchEnabled, using placeholder
        IsEnabled = false;
    }
}
