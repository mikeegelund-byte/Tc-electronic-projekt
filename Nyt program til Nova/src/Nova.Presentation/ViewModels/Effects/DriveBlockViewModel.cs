using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Domain.Models;

namespace Nova.Presentation.ViewModels.Effects;

/// <summary>
/// ViewModel for Drive effect block (NDT™ analog section).
/// Displays Overdrive or Distortion type with parameters.
/// </summary>
public partial class DriveBlockViewModel : ObservableObject
{
    [ObservableProperty] private string _type = "Overdrive";
    [ObservableProperty] private bool _isEnabled;

    private int _gain;
    public int Gain
    {
        get => _gain;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Gain must be between 0 and 100");
            SetProperty(ref _gain, value);
        }
    }

    private int _level;
    public int Level
    {
        get => _level;
        set
        {
            if (value < -30 || value > 20)
                throw new ArgumentOutOfRangeException(nameof(value), "Level must be between -30 and 20 dB");
            SetProperty(ref _level, value);
        }
    }

    /// <summary>
    /// Loads Drive parameters from a Preset domain model.
    /// </summary>
    public void LoadFromPreset(Preset? preset)
    {
        if (preset == null) return;

        Type = preset.DriveType switch
        {
            0 => "Overdrive",
            1 => "Distortion",
            2 => "Fuzz",
            3 => "Line6 Drive",
            4 => "Custom",
            5 => "Tube",
            6 => "Metal",
            _ => "Unknown"
        };
        
        Gain = Math.Clamp(preset.DriveGain, 0, 100);
        Level = Math.Clamp(preset.DriveLevel, -30, 20);
        IsEnabled = preset.DriveEnabled;
    }
}
