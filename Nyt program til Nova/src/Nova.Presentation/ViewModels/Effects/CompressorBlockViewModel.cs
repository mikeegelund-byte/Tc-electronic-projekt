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
    [ObservableProperty] private bool _isEnabled;

    private int _drive;
    public int Drive
    {
        get => _drive;
        set
        {
            if (value < 1 || value > 20)
                throw new ArgumentOutOfRangeException(nameof(value), "Drive must be between 1 and 20");
            SetProperty(ref _drive, value);
        }
    }

    private int _response;
    public int Response
    {
        get => _response;
        set
        {
            if (value < 1 || value > 10)
                throw new ArgumentOutOfRangeException(nameof(value), "Response must be between 1 and 10");
            SetProperty(ref _response, value);
        }
    }

    private int _level;
    public int Level
    {
        get => _level;
        set
        {
            if (value < -12 || value > 12)
                throw new ArgumentOutOfRangeException(nameof(value), "Level must be between -12 and 12 dB");
            SetProperty(ref _level, value);
        }
    }

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
        
        Drive = Math.Clamp(preset.CompDrive, 1, 20);
        Response = Math.Clamp(preset.CompResponse, 1, 10);
        Level = Math.Clamp(preset.CompLevel, -12, 12);
        IsEnabled = preset.CompressorEnabled;
    }
}
