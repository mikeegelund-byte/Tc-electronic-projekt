using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Domain.Models;

namespace Nova.Presentation.ViewModels.Effects;

/// <summary>
/// ViewModel for Pitch effect block (placeholder - Nova System has Octaver, Detune, Whammy, IPS, Talker).
/// </summary>
public partial class PitchBlockViewModel : ObservableObject
{
    [ObservableProperty] private string _type = "Octaver";
    [ObservableProperty] private bool _isEnabled;

    public void LoadFromPreset(Preset? preset)
    {
        if (preset == null) return;

        // Note: Pitch properties may need to be added to Preset model
        // For now, show placeholder
        Type = "Pitch Effect";
        IsEnabled = false; // preset.PitchEnabled;
    }
}
