using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Domain.Models;

namespace Nova.Presentation.ViewModels;

/// <summary>
/// ViewModel for displaying detailed information about a selected preset.
/// Shows preset metadata and key effect parameters.
/// </summary>
public partial class PresetDetailViewModel : ObservableObject
{
    [ObservableProperty]
    private string _presetName = "No preset selected";

    [ObservableProperty]
    private string _position = "";

    [ObservableProperty]
    private int _presetNumber;

    [ObservableProperty]
    private bool _hasPreset;

    // Global Parameters
    [ObservableProperty]
    private int _tapTempo;

    [ObservableProperty]
    private string _routing = "";

    [ObservableProperty]
    private int _levelOutLeft;

    [ObservableProperty]
    private int _levelOutRight;

    // Effect Block Enabled States
    [ObservableProperty]
    private bool _compressorEnabled;

    [ObservableProperty]
    private bool _driveEnabled;

    [ObservableProperty]
    private bool _modulationEnabled;

    [ObservableProperty]
    private bool _delayEnabled;

    [ObservableProperty]
    private bool _reverbEnabled;

    // Compressor Parameters
    [ObservableProperty]
    private string _compType = "";

    [ObservableProperty]
    private int _compThreshold;

    [ObservableProperty]
    private int _compRatio;

    // Drive Parameters
    [ObservableProperty]
    private string _driveType = "";

    [ObservableProperty]
    private int _driveGain;

    [ObservableProperty]
    private int _driveLevel;

    // Modulation Parameters
    [ObservableProperty]
    private string _modType = "";

    [ObservableProperty]
    private int _modSpeed;

    [ObservableProperty]
    private int _modDepth;

    [ObservableProperty]
    private int _modMix;

    // Delay Parameters
    [ObservableProperty]
    private string _delayType = "";

    [ObservableProperty]
    private int _delayTime;

    [ObservableProperty]
    private int _delayFeedback;

    [ObservableProperty]
    private int _delayMix;

    // Reverb Parameters
    [ObservableProperty]
    private string _reverbType = "";

    [ObservableProperty]
    private int _reverbDecay;

    [ObservableProperty]
    private int _reverbMix;

    /// <summary>
    /// Loads preset details from a Preset domain model.
    /// </summary>
    public void LoadFromPreset(Preset preset)
    {
        PresetName = preset.Name;
        PresetNumber = preset.Number;
        
        // Calculate position (bank group and slot)
        var bankGroup = (preset.Number - 31) / 3;
        var slot = ((preset.Number - 31) % 3) + 1;
        Position = $"{bankGroup:D2}-{slot}";

        // Global Parameters
        TapTempo = preset.TapTempo;
        Routing = preset.Routing switch
        {
            0 => "Semi-Parallel",
            1 => "Serial",
            2 => "Parallel",
            _ => "Unknown"
        };
        LevelOutLeft = preset.LevelOutLeft;
        LevelOutRight = preset.LevelOutRight;

        // Effect Block States
        CompressorEnabled = preset.CompressorEnabled;
        DriveEnabled = preset.DriveEnabled;
        ModulationEnabled = preset.ModulationEnabled;
        DelayEnabled = preset.DelayEnabled;
        ReverbEnabled = preset.ReverbEnabled;

        // Compressor
        CompType = preset.CompType switch
        {
            0 => "Percussion",
            1 => "Sustain",
            2 => "Advanced",
            _ => "Unknown"
        };
        CompThreshold = preset.CompThreshold;
        CompRatio = preset.CompRatio;

        // Drive
        DriveType = preset.DriveType switch
        {
            0 => "Overdrive",
            1 => "Distortion",
            2 => "Fuzz",
            3 => "Line6Drive",
            4 => "Custom",
            5 => "Tube",
            6 => "Metal",
            _ => "Unknown"
        };
        DriveGain = preset.DriveGain;
        DriveLevel = preset.DriveLevel;

        // Modulation
        ModType = preset.ModType switch
        {
            0 => "Chorus",
            1 => "Flanger",
            2 => "Vibrato",
            3 => "Phaser",
            4 => "Tremolo",
            5 => "Panner",
            _ => "Unknown"
        };
        ModSpeed = preset.ModSpeed;
        ModDepth = preset.ModDepth;
        ModMix = preset.ModMix;

        // Delay
        DelayType = preset.DelayType switch
        {
            0 => "Clean",
            1 => "Analog",
            2 => "Tape",
            3 => "Dynamic",
            4 => "Dual",
            5 => "Ping-Pong",
            _ => "Unknown"
        };
        DelayTime = preset.DelayTime;
        DelayFeedback = preset.DelayFeedback;
        DelayMix = preset.DelayMix;

        // Reverb
        ReverbType = preset.ReverbType switch
        {
            0 => "Spring",
            1 => "Hall",
            2 => "Room",
            3 => "Plate",
            _ => "Unknown"
        };
        ReverbDecay = preset.ReverbDecay;
        ReverbMix = preset.ReverbMix;

        HasPreset = true;
    }

    /// <summary>
    /// Clears the preset detail view.
    /// </summary>
    public void Clear()
    {
        PresetName = "No preset selected";
        Position = "";
        PresetNumber = 0;
        HasPreset = false;
    }
}
