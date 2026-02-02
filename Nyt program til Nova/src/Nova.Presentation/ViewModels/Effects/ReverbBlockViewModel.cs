using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Domain.Models;

namespace Nova.Presentation.ViewModels.Effects;

/// <summary>
/// ViewModel for Reverb effect block.
/// Displays Spring, Hall, Room, or Plate reverb with parameters.
/// </summary>
public partial class ReverbBlockViewModel : ObservableObject
{
    [ObservableProperty] private string _type = "Spring";
    [ObservableProperty] private int _decay;
    [ObservableProperty] private int _preDelay;
    [ObservableProperty] private int _level;
    [ObservableProperty] private bool _isEnabled;

    public void LoadFromPreset(Preset? preset)
    {
        if (preset == null) return;

        Type = preset.ReverbType switch
        {
            0 => "Spring",
            1 => "Hall",
            2 => "Room",
            3 => "Plate",
            _ => "Unknown"
        };
        
        Decay = preset.ReverbDecay;
        PreDelay = preset.ReverbPreDelay;
        Level = preset.ReverbLevel;
        IsEnabled = preset.ReverbEnabled;
    }
}
