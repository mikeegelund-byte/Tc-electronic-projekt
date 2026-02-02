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
    [ObservableProperty] private int _gain;
    [ObservableProperty] private int _level;
    [ObservableProperty] private bool _isEnabled;

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
        
        Gain = preset.DriveGain;
        Level = preset.DriveLevel;
        IsEnabled = preset.DriveEnabled;
    }
}
