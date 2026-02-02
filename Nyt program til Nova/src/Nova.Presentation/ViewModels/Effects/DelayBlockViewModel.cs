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
    [ObservableProperty] private int _time;
    [ObservableProperty] private int _feedback;
    [ObservableProperty] private int _mix;
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
        
        Time = preset.DelayTime;
        Feedback = preset.DelayFeedback;
        Mix = preset.DelayMix;
        IsEnabled = preset.DelayEnabled;
    }
}
