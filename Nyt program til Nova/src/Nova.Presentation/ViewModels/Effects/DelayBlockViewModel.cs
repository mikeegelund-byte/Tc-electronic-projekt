using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Domain.Models;

namespace Nova.Presentation.ViewModels.Effects;

/// <summary>
/// ViewModel for Delay effect block.
/// Displays Clean, Analog, Tape, Dynamic, Dual, or Ping-Pong delay with parameters.
/// </summary>
public partial class DelayBlockViewModel : ObservableObject
{
    [ObservableProperty] private string _type = "Clean";
    
    private int _time;
    public int Time
    {
        get => _time;
        set
        {
            if (value < 0 || value > 2000)
                throw new ArgumentOutOfRangeException(nameof(value), "Time must be between 0 and 2000 ms");
            SetProperty(ref _time, value);
        }
    }
    
    private int _feedback;
    public int Feedback
    {
        get => _feedback;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Feedback must be between 0 and 100%");
            SetProperty(ref _feedback, value);
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

        Type = preset.DelayType switch
        {
            0 => "Clean",
            1 => "Analog",
            2 => "Tape",
            3 => "Dynamic",
            4 => "Dual",
            5 => "Ping-Pong",
            _ => "Unknown"
        };
        
        // Clamp values to validation ranges
        Time = Math.Clamp(preset.DelayTime, 0, 2000);
        Feedback = Math.Clamp(preset.DelayFeedback, 0, 100);
        Mix = Math.Clamp(preset.DelayMix, 0, 100);
        IsEnabled = preset.DelayEnabled;
    }
}
