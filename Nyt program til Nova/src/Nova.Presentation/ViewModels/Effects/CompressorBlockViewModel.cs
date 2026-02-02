using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Domain.Models;

namespace Nova.Presentation.ViewModels.Effects;

/// <summary>
/// ViewModel for Compressor effect block.
/// Displays Percussive, Sustaining, or Advanced type with parameters.
/// </summary>
public partial class CompressorBlockViewModel : ObservableObject
{
    [ObservableProperty] private string _type = "Percussive";
    [ObservableProperty] private int _drive;
    [ObservableProperty] private int _response;
    [ObservableProperty] private int _level;
    [ObservableProperty] private bool _isEnabled;

    public void LoadFromPreset(Preset? preset)
    {
        if (preset == null) return;

        Type = preset.CompType switch
        {
            0 => "Percussive",
            1 => "Sustaining",
            2 => "Advanced",
            _ => "Unknown"
        };
        
        Drive = preset.CompDrive;
        Response = preset.CompResponse;
        Level = preset.CompLevel;
        IsEnabled = preset.CompressorEnabled;
    }
}
