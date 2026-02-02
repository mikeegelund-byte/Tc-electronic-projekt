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
    [ObservableProperty] private int _presetNumber;
    [ObservableProperty] private int _tapTempo;
    [ObservableProperty] private int _routing;
    [ObservableProperty] private int _levelOutLeft;
    [ObservableProperty] private int _levelOutRight;
    
    public bool HasPreset => !string.IsNullOrEmpty(PresetName);
    
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
            PresetNumber = 0;
            TapTempo = 0;
            Routing = 0;
            LevelOutLeft = 0;
            LevelOutRight = 0;
            return;
        }

        PresetName = preset.Name;
        Position = $"#{preset.Number}";
        PresetNumber = preset.Number;
        TapTempo = preset.TapTempo;
        Routing = preset.Routing;
        LevelOutLeft = preset.LevelOutLeft;
        LevelOutRight = preset.LevelOutRight;
        
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
