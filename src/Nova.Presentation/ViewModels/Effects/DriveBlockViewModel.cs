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
    [ObservableProperty] private int _typeId = 0;
    [ObservableProperty] private bool _isEnabled;
    [ObservableProperty] private bool _boostEnabled;

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

    private int _tone;
    public int Tone
    {
        get => _tone;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Tone must be between 0 and 100");
            SetProperty(ref _tone, value);
        }
    }

    private int _level;
    public int Level
    {
        get => _level;
        set
        {
            if (value < -100 || value > 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Level must be between -100 and 0 dB");
            SetProperty(ref _level, value);
        }
    }

    private int _boostLevel;
    public int BoostLevel
    {
        get => _boostLevel;
        set
        {
            if (value < 0 || value > 10)
                throw new ArgumentOutOfRangeException(nameof(value), "Boost Level must be between 0 and 10 dB");
            SetProperty(ref _boostLevel, value);
        }
    }

    /// <summary>
    /// Loads Drive parameters from a Preset domain model.
    /// </summary>
    public void LoadFromPreset(Preset? preset)
    {
        if (preset == null) return;

        TypeId = preset.DriveType;
        Type = preset.DriveType switch
        {
            0 => "Overdrive",
            1 => "Distortion",
            _ => "Unknown"
        };
        
        Gain = Math.Clamp(preset.DriveGain, 0, 100);
        Tone = Math.Clamp(preset.DriveTone, 0, 100);
        Level = Math.Clamp(preset.DriveLevel, -100, 0);
        BoostLevel = Math.Clamp(preset.BoostLevel, 0, 10);
        BoostEnabled = preset.BoostEnabled;
        IsEnabled = preset.DriveEnabled;
    }
}
