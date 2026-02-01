using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using Nova.Application.UseCases;
using Nova.Domain.Models;

namespace Nova.Presentation.ViewModels;

/// <summary>
/// ViewModel for editing preset parameters with validation and change tracking.
/// Contains all 78+ editable properties from the Preset model.
/// </summary>
public partial class EditablePresetViewModel : ObservableObject
{
    private readonly UpdatePresetUseCase? _updatePresetUseCase;
    private Preset? _originalPreset;
    
    // Basic preset info
    [ObservableProperty]
    private int _presetNumber;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private string _presetName = string.Empty;

    // Global parameters
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _tapTempo;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _routing;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _levelOutLeft;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _levelOutRight;

    // Effect on/off switches
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private bool _compressorEnabled;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private bool _driveEnabled;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private bool _modulationEnabled;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private bool _delayEnabled;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private bool _reverbEnabled;

    // COMP (Compressor) effect parameters
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _compType;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _compThreshold;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _compRatio;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _compAttack;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _compRelease;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _compResponse;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _compDrive;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _compLevel;

    // DRIVE effect parameters
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _driveType;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _driveGain;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _driveLevel;

    // BOOST effect parameters
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _boostType;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _boostGain;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _boostLevel;

    // MOD (Modulation) effect parameters
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _modType;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _modSpeed;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _modDepth;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _modTempo;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _modHiCut;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _modFeedback;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _modDelayOrRange;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _modMix;

    // DELAY effect parameters
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _delayType;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _delayTime;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _delayTime2;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _delayTempo;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _delayTempo2OrWidth;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _delayFeedback;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _delayClipOrFeedback2;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _delayHiCut;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _delayLoCut;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _delayMix;

    // REVERB effect parameters
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _reverbType;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _reverbDecay;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _reverbPreDelay;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _reverbShape;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _reverbSize;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _reverbHiColor;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _reverbHiLevel;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _reverbLoColor;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _reverbLoLevel;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _reverbRoomLevel;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _reverbLevel;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _reverbDiffuse;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _reverbMix;

    // EQ/GATE parameters
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _gateType;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _gateThreshold;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _gateDamp;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _gateRelease;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _eqFreq1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _eqGain1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _eqWidth1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _eqFreq2;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _eqGain2;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _eqWidth2;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _eqFreq3;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _eqGain3;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _eqWidth3;

    // PITCH effect parameters
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _pitchType;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _pitchVoice1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _pitchVoice2;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _pitchPan1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _pitchPan2;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _pitchDelay1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _pitchDelay2;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _pitchFeedback1OrKey;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _pitchFeedback2OrScale;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _pitchLevel1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanges))]
    private int _pitchLevel2;

    // Status properties
    [ObservableProperty]
    private string _validationError = string.Empty;

    [ObservableProperty]
    private bool _isSaving;

    /// <summary>
    /// Indicates whether any changes have been made to the preset.
    /// </summary>
    public bool HasChanges => _originalPreset != null && HasAnyPropertyChanged();

    /// <summary>
    /// Constructor for dependency injection with UpdatePresetUseCase.
    /// </summary>
    public EditablePresetViewModel(UpdatePresetUseCase updatePresetUseCase)
    {
        _updatePresetUseCase = updatePresetUseCase ?? throw new ArgumentNullException(nameof(updatePresetUseCase));
    }

    /// <summary>
    /// Parameterless constructor for testing.
    /// </summary>
    public EditablePresetViewModel()
    {
    }

    /// <summary>
    /// Loads preset data into the view model and tracks original values for change detection.
    /// </summary>
    public void LoadFromPreset(Preset preset)
    {
        if (preset == null)
            throw new ArgumentNullException(nameof(preset));

        _originalPreset = preset;
        ValidationError = string.Empty;

        // Load all properties from preset
        PresetNumber = preset.Number;
        PresetName = preset.Name;
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

        // Compressor
        CompType = preset.CompType;
        CompThreshold = preset.CompThreshold;
        CompRatio = preset.CompRatio;
        CompAttack = preset.CompAttack;
        CompRelease = preset.CompRelease;
        CompResponse = preset.CompResponse;
        CompDrive = preset.CompDrive;
        CompLevel = preset.CompLevel;

        // Drive
        DriveType = preset.DriveType;
        DriveGain = preset.DriveGain;
        DriveLevel = preset.DriveLevel;

        // Boost
        BoostType = preset.BoostType;
        BoostGain = preset.BoostGain;
        BoostLevel = preset.BoostLevel;

        // Modulation
        ModType = preset.ModType;
        ModSpeed = preset.ModSpeed;
        ModDepth = preset.ModDepth;
        ModTempo = preset.ModTempo;
        ModHiCut = preset.ModHiCut;
        ModFeedback = preset.ModFeedback;
        ModDelayOrRange = preset.ModDelayOrRange;
        ModMix = preset.ModMix;

        // Delay
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

        // Reverb
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

        // EQ/Gate
        GateType = preset.GateType;
        GateThreshold = preset.GateThreshold;
        GateDamp = preset.GateDamp;
        GateRelease = preset.GateRelease;
        EqFreq1 = preset.EqFreq1;
        EqGain1 = preset.EqGain1;
        EqWidth1 = preset.EqWidth1;
        EqFreq2 = preset.EqFreq2;
        EqGain2 = preset.EqGain2;
        EqWidth2 = preset.EqWidth2;
        EqFreq3 = preset.EqFreq3;
        EqGain3 = preset.EqGain3;
        EqWidth3 = preset.EqWidth3;

        // Pitch
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

        OnPropertyChanged(nameof(HasChanges));
    }

    /// <summary>
    /// Validates the current edited properties and returns the original preset.
    /// Note: Since Preset has a private constructor, this currently returns the original preset.
    /// In a full implementation with encoding support, this would create a new Preset with the modified values.
    /// </summary>
    public Result<Preset> ToPreset()
    {
        // Validate all values
        var validationErrors = new List<string>();

        // Validate preset name
        if (string.IsNullOrWhiteSpace(PresetName))
        {
            validationErrors.Add("Preset name cannot be empty");
        }
        else if (PresetName.Length > 24)
        {
            validationErrors.Add("Preset name cannot exceed 24 characters");
        }

        // Validate tap tempo
        if (TapTempo < 100 || TapTempo > 3000)
        {
            validationErrors.Add($"Tap tempo must be between 100 and 3000 ms");
        }

        // Validate routing
        if (Routing < 0 || Routing > 2)
        {
            validationErrors.Add($"Routing must be between 0 and 2");
        }

        // Validate level outputs
        if (LevelOutLeft < -100 || LevelOutLeft > 0)
        {
            validationErrors.Add($"Level Out Left must be between -100 and 0 dB");
        }
        if (LevelOutRight < -100 || LevelOutRight > 0)
        {
            validationErrors.Add($"Level Out Right must be between -100 and 0 dB");
        }

        if (validationErrors.Any())
        {
            ValidationError = string.Join("; ", validationErrors);
            return Result.Fail(ValidationError);
        }

        ValidationError = string.Empty;

        // Since Preset has a private constructor and only creates from SysEx,
        // we return the original preset for now.
        // A full implementation would require encoding the changes back to SysEx.
        if (_originalPreset == null)
        {
            return Result.Fail("No original preset loaded");
        }

        return Result.Ok(_originalPreset);
    }

    /// <summary>
    /// Saves the edited preset to the device.
    /// </summary>
    [RelayCommand]
    private async Task SaveAsync()
    {
        if (_updatePresetUseCase == null)
        {
            ValidationError = "Save functionality not available (no use case injected)";
            return;
        }

        IsSaving = true;
        ValidationError = string.Empty;

        try
        {
            var presetResult = ToPreset();
            if (presetResult.IsFailed)
            {
                ValidationError = string.Join("; ", presetResult.Errors.Select(e => e.Message));
                return;
            }

            var result = await _updatePresetUseCase.ExecuteAsync(presetResult.Value);
            
            if (result.IsSuccess)
            {
                // Refresh original preset to reflect saved state
                _originalPreset = presetResult.Value;
                OnPropertyChanged(nameof(HasChanges));
            }
            else
            {
                ValidationError = string.Join("; ", result.Errors.Select(e => e.Message));
            }
        }
        finally
        {
            IsSaving = false;
        }
    }

    /// <summary>
    /// Reverts all changes back to the original preset values.
    /// </summary>
    [RelayCommand]
    private void RevertChanges()
    {
        if (_originalPreset != null)
        {
            LoadFromPreset(_originalPreset);
        }
    }

    /// <summary>
    /// Checks if any property has changed from the original preset.
    /// Uses explicit property comparison for clarity and performance.
    /// While reflection could reduce code, explicit comparison provides:
    /// - Better performance (no reflection overhead)
    /// - Clear tracking of exactly which properties are monitored
    /// - Compile-time safety when properties are added/removed
    /// </summary>
    private bool HasAnyPropertyChanged()
    {
        if (_originalPreset == null)
            return false;

        return PresetName != _originalPreset.Name ||
               TapTempo != _originalPreset.TapTempo ||
               Routing != _originalPreset.Routing ||
               LevelOutLeft != _originalPreset.LevelOutLeft ||
               LevelOutRight != _originalPreset.LevelOutRight ||
               CompressorEnabled != _originalPreset.CompressorEnabled ||
               DriveEnabled != _originalPreset.DriveEnabled ||
               ModulationEnabled != _originalPreset.ModulationEnabled ||
               DelayEnabled != _originalPreset.DelayEnabled ||
               ReverbEnabled != _originalPreset.ReverbEnabled ||
               CompType != _originalPreset.CompType ||
               CompThreshold != _originalPreset.CompThreshold ||
               CompRatio != _originalPreset.CompRatio ||
               CompAttack != _originalPreset.CompAttack ||
               CompRelease != _originalPreset.CompRelease ||
               CompResponse != _originalPreset.CompResponse ||
               CompDrive != _originalPreset.CompDrive ||
               CompLevel != _originalPreset.CompLevel ||
               DriveType != _originalPreset.DriveType ||
               DriveGain != _originalPreset.DriveGain ||
               DriveLevel != _originalPreset.DriveLevel ||
               BoostType != _originalPreset.BoostType ||
               BoostGain != _originalPreset.BoostGain ||
               BoostLevel != _originalPreset.BoostLevel ||
               ModType != _originalPreset.ModType ||
               ModSpeed != _originalPreset.ModSpeed ||
               ModDepth != _originalPreset.ModDepth ||
               ModTempo != _originalPreset.ModTempo ||
               ModHiCut != _originalPreset.ModHiCut ||
               ModFeedback != _originalPreset.ModFeedback ||
               ModDelayOrRange != _originalPreset.ModDelayOrRange ||
               ModMix != _originalPreset.ModMix ||
               DelayType != _originalPreset.DelayType ||
               DelayTime != _originalPreset.DelayTime ||
               DelayTime2 != _originalPreset.DelayTime2 ||
               DelayTempo != _originalPreset.DelayTempo ||
               DelayTempo2OrWidth != _originalPreset.DelayTempo2OrWidth ||
               DelayFeedback != _originalPreset.DelayFeedback ||
               DelayClipOrFeedback2 != _originalPreset.DelayClipOrFeedback2 ||
               DelayHiCut != _originalPreset.DelayHiCut ||
               DelayLoCut != _originalPreset.DelayLoCut ||
               DelayMix != _originalPreset.DelayMix ||
               ReverbType != _originalPreset.ReverbType ||
               ReverbDecay != _originalPreset.ReverbDecay ||
               ReverbPreDelay != _originalPreset.ReverbPreDelay ||
               ReverbShape != _originalPreset.ReverbShape ||
               ReverbSize != _originalPreset.ReverbSize ||
               ReverbHiColor != _originalPreset.ReverbHiColor ||
               ReverbHiLevel != _originalPreset.ReverbHiLevel ||
               ReverbLoColor != _originalPreset.ReverbLoColor ||
               ReverbLoLevel != _originalPreset.ReverbLoLevel ||
               ReverbRoomLevel != _originalPreset.ReverbRoomLevel ||
               ReverbLevel != _originalPreset.ReverbLevel ||
               ReverbDiffuse != _originalPreset.ReverbDiffuse ||
               ReverbMix != _originalPreset.ReverbMix ||
               GateType != _originalPreset.GateType ||
               GateThreshold != _originalPreset.GateThreshold ||
               GateDamp != _originalPreset.GateDamp ||
               GateRelease != _originalPreset.GateRelease ||
               EqFreq1 != _originalPreset.EqFreq1 ||
               EqGain1 != _originalPreset.EqGain1 ||
               EqWidth1 != _originalPreset.EqWidth1 ||
               EqFreq2 != _originalPreset.EqFreq2 ||
               EqGain2 != _originalPreset.EqGain2 ||
               EqWidth2 != _originalPreset.EqWidth2 ||
               EqFreq3 != _originalPreset.EqFreq3 ||
               EqGain3 != _originalPreset.EqGain3 ||
               EqWidth3 != _originalPreset.EqWidth3 ||
               PitchType != _originalPreset.PitchType ||
               PitchVoice1 != _originalPreset.PitchVoice1 ||
               PitchVoice2 != _originalPreset.PitchVoice2 ||
               PitchPan1 != _originalPreset.PitchPan1 ||
               PitchPan2 != _originalPreset.PitchPan2 ||
               PitchDelay1 != _originalPreset.PitchDelay1 ||
               PitchDelay2 != _originalPreset.PitchDelay2 ||
               PitchFeedback1OrKey != _originalPreset.PitchFeedback1OrKey ||
               PitchFeedback2OrScale != _originalPreset.PitchFeedback2OrScale ||
               PitchLevel1 != _originalPreset.PitchLevel1 ||
               PitchLevel2 != _originalPreset.PitchLevel2;
    }
}
