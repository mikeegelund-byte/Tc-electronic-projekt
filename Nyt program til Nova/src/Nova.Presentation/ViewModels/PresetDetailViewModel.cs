using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Domain.Models;

namespace Nova.Presentation.ViewModels;

/// <summary>
/// ViewModel for displaying full preset details.
/// Maps all properties from the Preset domain model to observable UI properties.
/// All properties are read-only for display purposes.
/// </summary>
public partial class PresetDetailViewModel : ObservableObject
{
    // Basic preset info
    [ObservableProperty]
    private int _presetNumber;

    [ObservableProperty]
    private string _presetName = string.Empty;

    // Global parameters
    [ObservableProperty]
    private int _tapTempo;

    [ObservableProperty]
    private int _routing;

    [ObservableProperty]
    private int _levelOutLeft;

    [ObservableProperty]
    private int _levelOutRight;

    // Effect on/off switches
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

    // COMP (Compressor) effect parameters
    [ObservableProperty]
    private int _compType;

    [ObservableProperty]
    private int _compThreshold;

    [ObservableProperty]
    private int _compRatio;

    [ObservableProperty]
    private int _compAttack;

    [ObservableProperty]
    private int _compRelease;

    [ObservableProperty]
    private int _compResponse;

    [ObservableProperty]
    private int _compDrive;

    [ObservableProperty]
    private int _compLevel;

    // DRIVE effect parameters
    [ObservableProperty]
    private int _driveType;

    [ObservableProperty]
    private int _driveGain;

    [ObservableProperty]
    private int _driveLevel;

    // BOOST effect parameters
    [ObservableProperty]
    private int _boostType;

    [ObservableProperty]
    private int _boostGain;

    [ObservableProperty]
    private int _boostLevel;

    // MOD (Modulation) effect parameters
    [ObservableProperty]
    private int _modType;

    [ObservableProperty]
    private int _modSpeed;

    [ObservableProperty]
    private int _modDepth;

    [ObservableProperty]
    private int _modTempo;

    [ObservableProperty]
    private int _modHiCut;

    [ObservableProperty]
    private int _modFeedback;

    [ObservableProperty]
    private int _modDelayOrRange;

    [ObservableProperty]
    private int _modMix;

    // DELAY effect parameters
    [ObservableProperty]
    private int _delayType;

    [ObservableProperty]
    private int _delayTime;

    [ObservableProperty]
    private int _delayTime2;

    [ObservableProperty]
    private int _delayTempo;

    [ObservableProperty]
    private int _delayTempo2OrWidth;

    [ObservableProperty]
    private int _delayFeedback;

    [ObservableProperty]
    private int _delayClipOrFeedback2;

    [ObservableProperty]
    private int _delayHiCut;

    [ObservableProperty]
    private int _delayLoCut;

    [ObservableProperty]
    private int _delayMix;

    // REVERB effect parameters
    [ObservableProperty]
    private int _reverbType;

    [ObservableProperty]
    private int _reverbDecay;

    [ObservableProperty]
    private int _reverbPreDelay;

    [ObservableProperty]
    private int _reverbShape;

    [ObservableProperty]
    private int _reverbSize;

    [ObservableProperty]
    private int _reverbHiColor;

    [ObservableProperty]
    private int _reverbHiLevel;

    [ObservableProperty]
    private int _reverbLoColor;

    [ObservableProperty]
    private int _reverbLoLevel;

    [ObservableProperty]
    private int _reverbRoomLevel;

    [ObservableProperty]
    private int _reverbLevel;

    [ObservableProperty]
    private int _reverbDiffuse;

    [ObservableProperty]
    private int _reverbMix;

    // EQ/GATE parameters
    [ObservableProperty]
    private int _gateType;

    [ObservableProperty]
    private int _gateThreshold;

    [ObservableProperty]
    private int _gateDamp;

    [ObservableProperty]
    private int _gateRelease;

    [ObservableProperty]
    private int _eqFreq1;

    [ObservableProperty]
    private int _eqGain1;

    [ObservableProperty]
    private int _eqWidth1;

    [ObservableProperty]
    private int _eqFreq2;

    [ObservableProperty]
    private int _eqGain2;

    [ObservableProperty]
    private int _eqWidth2;

    [ObservableProperty]
    private int _eqFreq3;

    [ObservableProperty]
    private int _eqGain3;

    [ObservableProperty]
    private int _eqWidth3;

    // PITCH effect parameters
    [ObservableProperty]
    private int _pitchType;

    [ObservableProperty]
    private int _pitchVoice1;

    [ObservableProperty]
    private int _pitchVoice2;

    [ObservableProperty]
    private int _pitchPan1;

    [ObservableProperty]
    private int _pitchPan2;

    [ObservableProperty]
    private int _pitchDelay1;

    [ObservableProperty]
    private int _pitchDelay2;

    [ObservableProperty]
    private int _pitchFeedback1OrKey;

    [ObservableProperty]
    private int _pitchFeedback2OrScale;

    [ObservableProperty]
    private int _pitchLevel1;

    [ObservableProperty]
    private int _pitchLevel2;

    /// <summary>
    /// Loads all preset data from a Preset domain model into the ViewModel.
    /// </summary>
    /// <param name="preset">The preset to load, or null to clear</param>
    public void LoadFromPreset(Preset? preset)
    {
        if (preset == null)
        {
            // Reset to defaults
            PresetNumber = 0;
            PresetName = string.Empty;
            return;
        }

        // Basic info
        PresetNumber = preset.Number;
        PresetName = preset.Name;

        // Global parameters
        TapTempo = preset.TapTempo;
        Routing = preset.Routing;
        LevelOutLeft = preset.LevelOutLeft;
        LevelOutRight = preset.LevelOutRight;

        // Effect switches
        CompressorEnabled = preset.CompressorEnabled;
        DriveEnabled = preset.DriveEnabled;
        ModulationEnabled = preset.ModulationEnabled;
        DelayEnabled = preset.DelayEnabled;
        ReverbEnabled = preset.ReverbEnabled;

        // Compressor parameters
        CompType = preset.CompType;
        CompThreshold = preset.CompThreshold;
        CompRatio = preset.CompRatio;
        CompAttack = preset.CompAttack;
        CompRelease = preset.CompRelease;
        CompResponse = preset.CompResponse;
        CompDrive = preset.CompDrive;
        CompLevel = preset.CompLevel;

        // Drive parameters
        DriveType = preset.DriveType;
        DriveGain = preset.DriveGain;
        DriveLevel = preset.DriveLevel;

        // Boost parameters
        BoostType = preset.BoostType;
        BoostGain = preset.BoostGain;
        BoostLevel = preset.BoostLevel;

        // Modulation parameters
        ModType = preset.ModType;
        ModSpeed = preset.ModSpeed;
        ModDepth = preset.ModDepth;
        ModTempo = preset.ModTempo;
        ModHiCut = preset.ModHiCut;
        ModFeedback = preset.ModFeedback;
        ModDelayOrRange = preset.ModDelayOrRange;
        ModMix = preset.ModMix;

        // Delay parameters
        DelayType = preset.DelayType;
        DelayTime = preset.DelayTime;
        DelayTime2 = preset.DelayTime2;
        DelayTempo = preset.DelayTempo;
        DelayTempo2OrWidth = preset.DelayTempo2OrWidth;
        DelayFeedback = preset.DelayFeedback;
        DelayClipOrFeedback2 = preset.DelayClipOrFeedback2;
        DelayHiCut = preset.DelayHiCut;
        DelayLoCut = preset.DelayLoCut;
        DelayMix = preset.DelayMix;

        // Reverb parameters
        ReverbType = preset.ReverbType;
        ReverbDecay = preset.ReverbDecay;
        ReverbPreDelay = preset.ReverbPreDelay;
        ReverbShape = preset.ReverbShape;
        ReverbSize = preset.ReverbSize;
        ReverbHiColor = preset.ReverbHiColor;
        ReverbHiLevel = preset.ReverbHiLevel;
        ReverbLoColor = preset.ReverbLoColor;
        ReverbLoLevel = preset.ReverbLoLevel;
        ReverbRoomLevel = preset.ReverbRoomLevel;
        ReverbLevel = preset.ReverbLevel;
        ReverbDiffuse = preset.ReverbDiffuse;
        ReverbMix = preset.ReverbMix;

        // Gate parameters
        GateType = preset.GateType;
        GateThreshold = preset.GateThreshold;
        GateDamp = preset.GateDamp;
        GateRelease = preset.GateRelease;

        // EQ parameters
        EqFreq1 = preset.EqFreq1;
        EqGain1 = preset.EqGain1;
        EqWidth1 = preset.EqWidth1;
        EqFreq2 = preset.EqFreq2;
        EqGain2 = preset.EqGain2;
        EqWidth2 = preset.EqWidth2;
        EqFreq3 = preset.EqFreq3;
        EqGain3 = preset.EqGain3;
        EqWidth3 = preset.EqWidth3;

        // Pitch parameters
        PitchType = preset.PitchType;
        PitchVoice1 = preset.PitchVoice1;
        PitchVoice2 = preset.PitchVoice2;
        PitchPan1 = preset.PitchPan1;
        PitchPan2 = preset.PitchPan2;
        PitchDelay1 = preset.PitchDelay1;
        PitchDelay2 = preset.PitchDelay2;
        PitchFeedback1OrKey = preset.PitchFeedback1OrKey;
        PitchFeedback2OrScale = preset.PitchFeedback2OrScale;
        PitchLevel1 = preset.PitchLevel1;
        PitchLevel2 = preset.PitchLevel2;
    }
}
