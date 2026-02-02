using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Domain.Models;
using Nova.Presentation.ViewModels.Effects;

namespace Nova.Presentation.ViewModels;

/// <summary>
/// ViewModel for detailed preset view showing all 7 effect blocks.
/// Composes individual effect block ViewModels.
/// </summary>
public partial class PresetDetailViewModel : ObservableObject
{
    [ObservableProperty] private string _presetName = "";
    [ObservableProperty] private string _position = "";
    
    public DriveBlockViewModel Drive { get; } = new();
    public CompressorBlockViewModel Compressor { get; } = new();
    public EqGateBlockViewModel EqGate { get; } = new();
    public ModulationBlockViewModel Modulation { get; } = new();
    public PitchBlockViewModel Pitch { get; } = new();
    public DelayBlockViewModel Delay { get; } = new();
    public ReverbBlockViewModel Reverb { get; } = new();

    /// <summary>
    /// Loads all effect block parameters from a Preset domain model.
    /// </summary>
    public void LoadFromPreset(Preset? preset)
    {
        if (preset == null)
        {
            PresetName = "";
            Position = "";
            return;
        }

        PresetName = preset.Name;
        Position = $"#{preset.Number}";
        
        // Load all effect blocks
        Drive.LoadFromPreset(preset);
        Compressor.LoadFromPreset(preset);
        EqGate.LoadFromPreset(preset);
        Modulation.LoadFromPreset(preset);
        Pitch.LoadFromPreset(preset);
        Delay.LoadFromPreset(preset);
        Reverb.LoadFromPreset(preset);
    }
}
