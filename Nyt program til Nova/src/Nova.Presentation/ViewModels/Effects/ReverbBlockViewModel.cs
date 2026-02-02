using System;
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
    
    private int _decay;
    public int Decay
    {
        get => _decay;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Decay must be between 0 and 100%");
            SetProperty(ref _decay, value);
        }
    }
    
    private int _preDelay;
    public int PreDelay
    {
        get => _preDelay;
        set
        {
            if (value < 0 || value > 200)
                throw new ArgumentOutOfRangeException(nameof(value), "PreDelay must be between 0 and 200 ms");
            SetProperty(ref _preDelay, value);
        }
    }
    
    private int _level;
    public int Level
    {
        get => _level;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Level must be between 0 and 100%");
            SetProperty(ref _level, value);
        }
    }
    
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
        
        // Clamp values to validation ranges
        Decay = Math.Clamp(preset.ReverbDecay, 0, 100);
        PreDelay = Math.Clamp(preset.ReverbPreDelay, 0, 200);
        Level = Math.Clamp(preset.ReverbLevel, 0, 100);
        IsEnabled = preset.ReverbEnabled;
    }
}
