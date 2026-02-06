using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Domain.Models;

namespace Nova.Presentation.ViewModels.Effects;

/// <summary>
/// ViewModel for Modulation effect block.
/// Displays Chorus, Flanger, Vibrato, Phaser, Tremolo, or Panner with parameters.
/// </summary>
public partial class ModulationBlockViewModel : ObservableObject
{
    [ObservableProperty] private string _type = "Chorus";
    [ObservableProperty] private bool _isEnabled;

    private int _speed;
    public int Speed
    {
        get => _speed;
        set
        {
            if (value < 0 || value > 81)
                throw new ArgumentOutOfRangeException(nameof(value), "Speed must be between 0 and 81");
            SetProperty(ref _speed, value);
        }
    }

    private int _depth;
    public int Depth
    {
        get => _depth;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Depth must be between 0 and 100%");
            SetProperty(ref _depth, value);
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

    public void LoadFromPreset(Preset? preset)
    {
        if (preset == null) return;

        Type = preset.ModType switch
        {
            0 => "Chorus",
            1 => "Flanger",
            2 => "Vibrato",
            3 => "Phaser",
            4 => "Tremolo",
            5 => "Panner",
            _ => "Unknown"
        };
        
        Speed = Math.Clamp(preset.ModSpeed, 0, 81);
        Depth = Math.Clamp(preset.ModDepth, 0, 100);
        Mix = Math.Clamp(preset.ModMix, 0, 100);
        IsEnabled = preset.ModulationEnabled;
    }
}
