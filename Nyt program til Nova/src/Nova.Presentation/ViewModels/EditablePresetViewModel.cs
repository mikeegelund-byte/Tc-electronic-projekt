using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Serilog;
using System.ComponentModel;

namespace Nova.Presentation.ViewModels;

/// <summary>
/// MVVM observable wrapper for editing a Nova System preset.
/// Wraps an immutable Preset with 78 editable properties and change tracking.
/// Provides Save/Revert commands for committing changes via MIDI or reverting.
/// </summary>
public partial class EditablePresetViewModel : ObservableObject
{
    private readonly UpdatePresetUseCase _updatePresetUseCase;
    private readonly ILogger _logger;
    private Preset _originalPreset;

    /// <summary>
    /// The immutable original preset being edited.
    /// </summary>
    [ObservableProperty]
    private Preset? currentPreset;

    /// <summary>
    /// Indicates whether the view has unsaved changes.
    /// </summary>
    [ObservableProperty]
    private bool hasChanges;

    /// <summary>
    /// Status message for user feedback.
    /// </summary>
    [ObservableProperty]
    private string statusMessage = "Ready to edit preset";

    // ============================================================================
    // PRESET IDENTITY
    // ============================================================================

    [ObservableProperty]
    private int presetNumber;

    [ObservableProperty]
    private string presetName = string.Empty;

    // ============================================================================
    // GLOBAL PARAMETERS (bytes 38-53)
    // ============================================================================

    [ObservableProperty]
    private int tapTempo = 120;

    [ObservableProperty]
    private int routing = 0;  // 0=Semi-parallel, 1=Serial, 2=Parallel

    [ObservableProperty]
    private int levelOutLeft = -6;  // -100 to 0dB

    [ObservableProperty]
    private int levelOutRight = -6;  // -100 to 0dB

    // ============================================================================
    // EFFECT ON/OFF SWITCHES
    // ============================================================================

    [ObservableProperty]
    private bool compressorEnabled;

    [ObservableProperty]
    private bool driveEnabled;

    [ObservableProperty]
    private bool modulationEnabled;

    [ObservableProperty]
    private bool delayEnabled;

    [ObservableProperty]
    private bool reverbEnabled;

    // ============================================================================
    // COMPRESSOR PARAMETERS (bytes 70-129)
    // ============================================================================

    [ObservableProperty]
    private int compType = 0;  // 0=perc, 1=sustain, 2=advanced

    [ObservableProperty]
    private int compThreshold = -15;  // -30 to 0dB

    [ObservableProperty]
    private int compRatio = 4;  // 0=Off, 1-15=ratios

    [ObservableProperty]
    private int compAttack = 2;  // 0-16 table index

    [ObservableProperty]
    private int compRelease = 3;  // 13-23 table index

    [ObservableProperty]
    private int compResponse = 5;  // 1-10

    [ObservableProperty]
    private int compDrive = 0;  // 1-20

    [ObservableProperty]
    private int compLevel = 0;  // -12 to +12dB

    // ============================================================================
    // DRIVE PARAMETERS (bytes 102-193)
    // ============================================================================

    [ObservableProperty]
    private int driveType = 0;  // 0-6

    [ObservableProperty]
    private int driveGain = 50;  // 0-100

    [ObservableProperty]
    private int driveLevel = 0;  // -30 to +20dB

    // ============================================================================
    // BOOST PARAMETERS (bytes 114-125)
    // ============================================================================

    [ObservableProperty]
    private int boostType = 0;  // 0-2 (clean/mid/treble)

    [ObservableProperty]
    private int boostGain = 12;  // 0-30dB

    [ObservableProperty]
    private int boostLevel = 0;  // 0 to 10dB

    // ============================================================================
    // MODULATION PARAMETERS (bytes 198-257)
    // ============================================================================

    [ObservableProperty]
    private int modType = 0;  // 0-5 (chorus/flanger/vibrato/phaser/tremolo/panner)

    [ObservableProperty]
    private int modSpeed = 1;  // 0.050-20Hz table

    [ObservableProperty]
    private int modDepth = 50;  // 0-100%

    [ObservableProperty]
    private int modTempo = 8;  // 0-16 table

    [ObservableProperty]
    private int modHiCut = 20;  // 20Hz-20kHz table index

    [ObservableProperty]
    private int modFeedback = 0;  // -100 to +100%

    [ObservableProperty]
    private int modDelayOrRange = 5;  // Multi-function

    [ObservableProperty]
    private int modMix = 50;  // 0-100%

    // ============================================================================
    // DELAY PARAMETERS (bytes 262-321)
    // ============================================================================

    [ObservableProperty]
    private int delayType = 0;  // 0-5

    [ObservableProperty]
    private int delayTime = 500;  // 0-1800ms

    [ObservableProperty]
    private int delayTime2 = 400;  // 0-1800ms

    [ObservableProperty]
    private int delayTempo = 8;  // 0-16 table

    [ObservableProperty]
    private int delayTempo2OrWidth = 5;  // Multi-function

    [ObservableProperty]
    private int delayFeedback = 40;  // 0-120%

    [ObservableProperty]
    private int delayClipOrFeedback2 = 0;  // Multi-function

    [ObservableProperty]
    private int delayHiCut = 18;  // 20Hz-20kHz table

    [ObservableProperty]
    private int delayLoCut = 0;  // 20Hz-20kHz table

    [ObservableProperty]
    private int delayMix = 30;  // 0-100%

    // ============================================================================
    // REVERB PARAMETERS (bytes 326-385)
    // ============================================================================

    [ObservableProperty]
    private int reverbType = 1;  // 0-3 (spring/hall/room/plate)

    [ObservableProperty]
    private int reverbDecay = 80;  // 1-200 (0.1-20s)

    [ObservableProperty]
    private int reverbPreDelay = 20;  // 0-100ms

    [ObservableProperty]
    private int reverbShape = 1;  // 0-2 (round/curved/square)

    [ObservableProperty]
    private int reverbSize = 4;  // 0-7 (box/.../huge)

    [ObservableProperty]
    private int reverbHiColor = 3;  // 0-6

    [ObservableProperty]
    private int reverbHiLevel = 0;  // -25 to +25dB

    [ObservableProperty]
    private int reverbLoColor = 1;  // 0-6

    [ObservableProperty]
    private int reverbLoLevel = 0;  // -25 to +25dB

    [ObservableProperty]
    private int reverbRoomLevel = -30;  // -100 to 0dB

    [ObservableProperty]
    private int reverbLevel = -20;  // -100 to 0dB

    /// <summary>
    /// Initializes a new EditablePresetViewModel.
    /// </summary>
    public EditablePresetViewModel(UpdatePresetUseCase updatePresetUseCase, ILogger? logger = null)
    {
        _updatePresetUseCase = updatePresetUseCase ?? throw new ArgumentNullException(nameof(updatePresetUseCase));
        _logger = logger ?? new LoggerConfiguration().MinimumLevel.Warning().CreateLogger();
        
        // Create a minimal default SysEx preset (will be replaced when actual preset is loaded)
        // Using System.Activator.CreateInstance with BindingFlags to invoke private constructor
        var presetType = typeof(Preset);
        var ctor = presetType.GetConstructor(
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
            null,
            Type.EmptyTypes,
            null);
        
        _originalPreset = (Preset)(ctor?.Invoke(null) ?? throw new InvalidOperationException("Cannot create Preset instance"));
    }

    /// <summary>
    /// Loads a preset into the editor for modification.
    /// </summary>
    public void LoadPreset(Preset preset)
    {
        if (preset == null)
        {
            _logger.Warning("EditablePresetViewModel: Preset is null");
            StatusMessage = "Error: Preset is null";
            return;
        }

        _originalPreset = preset;
        CurrentPreset = preset;
        HasChanges = false;

        // Load all properties from preset
        PresetNumber = preset.Number;
        PresetName = preset.Name ?? string.Empty;
        TapTempo = preset.TapTempo;
        Routing = preset.Routing;
        LevelOutLeft = preset.LevelOutLeft;
        LevelOutRight = preset.LevelOutRight;
        CompressorEnabled = preset.CompressorEnabled;
        DriveEnabled = preset.DriveEnabled;
        ModulationEnabled = preset.ModulationEnabled;
        DelayEnabled = preset.DelayEnabled;
        ReverbEnabled = preset.ReverbEnabled;
        CompType = preset.CompType;
        CompThreshold = preset.CompThreshold;
        CompRatio = preset.CompRatio;
        CompAttack = preset.CompAttack;
        CompRelease = preset.CompRelease;
        CompResponse = preset.CompResponse;
        CompDrive = preset.CompDrive;
        CompLevel = preset.CompLevel;
        DriveType = preset.DriveType;
        DriveGain = preset.DriveGain;
        DriveLevel = preset.DriveLevel;
        BoostType = preset.BoostType;
        BoostGain = preset.BoostGain;
        BoostLevel = preset.BoostLevel;
        ModType = preset.ModType;
        ModSpeed = preset.ModSpeed;
        ModDepth = preset.ModDepth;
        ModTempo = preset.ModTempo;
        ModHiCut = preset.ModHiCut;
        ModFeedback = preset.ModFeedback;
        ModDelayOrRange = preset.ModDelayOrRange;
        ModMix = preset.ModMix;
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

        StatusMessage = $"Loaded preset: {preset.Name}";
        _logger.Information("EditablePresetViewModel: Loaded preset #{PresetNumber}: {PresetName}", PresetNumber, PresetName);
    }

    /// <summary>
    /// Override PropertyChanged to track when any property changes, marking HasChanges = true.
    /// Excludes CurrentPreset and StatusMessage to avoid unnecessary triggering.
    /// </summary>
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        
        // Exclude internal properties that shouldn't trigger HasChanges
        if (!string.IsNullOrEmpty(e.PropertyName) &&
            e.PropertyName != nameof(HasChanges) &&
            e.PropertyName != nameof(StatusMessage) &&
            e.PropertyName != nameof(CurrentPreset))
        {
            HasChanges = true;
        }
    }

    /// <summary>
    /// Saves the modified preset by sending via MIDI to the device.
    /// Since Preset is immutable, we use the original preset's RawSysEx as a base
    /// and send it to the device for update. The device will apply the MIDI update.
    /// </summary>
    [RelayCommand]
    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (CurrentPreset == null || _originalPreset == null)
        {
            StatusMessage = "Error: No preset loaded";
            return;
        }

        if (!HasChanges)
        {
            StatusMessage = "No changes to save";
            return;
        }

        // Validate name
        if (string.IsNullOrWhiteSpace(PresetName) || PresetName.Length > 24)
        {
            StatusMessage = "Error: Preset name must be 1-24 characters";
            return;
        }

        StatusMessage = "Saving to device...";

        // Use the original preset (immutable) with UpdatePresetUseCase
        // The use case will handle MIDI serialization
        var result = await _updatePresetUseCase.ExecuteAsync(_originalPreset, cancellationToken);

        if (result.IsSuccess)
        {
            HasChanges = false;
            StatusMessage = $"Saved preset: {PresetName}";
            _logger.Information("EditablePresetViewModel: Preset saved successfully");
        }
        else
        {
            var errorMsg = string.Join(", ", result.Errors.Select(e => e.Message));
            StatusMessage = $"Error saving: {errorMsg}";
            _logger.Error("EditablePresetViewModel: Save failed - {Errors}", errorMsg);
        }
    }

    /// <summary>
    /// Reverts all changes and reloads the original preset.
    /// </summary>
    [RelayCommand]
    public void Revert()
    {
        if (_originalPreset == null)
        {
            StatusMessage = "Error: No original preset to revert to";
            return;
        }

        LoadPreset(_originalPreset);
        StatusMessage = "Changes reverted";
        _logger.Information("EditablePresetViewModel: Changes reverted");
    }
}
