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
    [ObservableProperty] private int _speed;
    [ObservableProperty] private int _depth;
    [ObservableProperty] private int _mix;
    [ObservableProperty] private bool _isEnabled;

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
        
        Speed = preset.ModSpeed;
        Depth = preset.ModDepth;
        Mix = preset.ModMix;
        IsEnabled = preset.ModulationEnabled;
    }
}
