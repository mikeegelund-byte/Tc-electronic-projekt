using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nova.Application.UseCases;
using Nova.Domain.Models;
using Nova.Presentation.ViewModels.Effects;

namespace Nova.Presentation.ViewModels;

/// <summary>
/// ViewModel for detailed preset view showing all 7 effect blocks.
/// Composes individual effect block ViewModels.
/// </summary>
public partial class PresetDetailViewModel : ObservableObject
{
    private readonly ISavePresetUseCase _savePresetUseCase;
    private Preset? _currentPreset;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPreset))]
    private string _presetName = "";
    [ObservableProperty] private string _position = "";
    [ObservableProperty] private int _presetNumber;

    private int _tapTempo;
    public int TapTempo
    {
        get => _tapTempo;
        set
        {
            if (value < 100 || value > 3000)
                throw new ArgumentOutOfRangeException(nameof(value), "TapTempo must be between 100 and 3000");
            SetProperty(ref _tapTempo, value);
        }
    }

    private int _routing;
    public int Routing
    {
        get => _routing;
        set
        {
            if (value < 0 || value > 2)
                throw new ArgumentOutOfRangeException(nameof(value), "Routing must be between 0 and 2");
            if (SetProperty(ref _routing, value))
            {
                OnPropertyChanged(nameof(IsRoutingSemiParallel));
                OnPropertyChanged(nameof(IsRoutingSerial));
                OnPropertyChanged(nameof(IsRoutingParallel));
            }
        }
    }

    private int _levelOutLeft;
    public int LevelOutLeft
    {
        get => _levelOutLeft;
        set
        {
            if (value < -100 || value > 0)
                throw new ArgumentOutOfRangeException(nameof(value), "LevelOutLeft must be between -100 and 0");
            SetProperty(ref _levelOutLeft, value);
        }
    }

    private int _levelOutRight;
    public int LevelOutRight
    {
        get => _levelOutRight;
        set
        {
            if (value < -100 || value > 0)
                throw new ArgumentOutOfRangeException(nameof(value), "LevelOutRight must be between -100 and 0");
            SetProperty(ref _levelOutRight, value);
        }
    }

    private int _mapParameter;
    public int MapParameter
    {
        get => _mapParameter;
        set
        {
            if (value < 0 || value > 127)
                throw new ArgumentOutOfRangeException(nameof(value), "MapParameter must be between 0 and 127");
            SetProperty(ref _mapParameter, value);
        }
    }

    private int _mapMin;
    public int MapMin
    {
        get => _mapMin;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "MapMin must be between 0 and 100");
            SetProperty(ref _mapMin, value);
        }
    }

    private int _mapMid;
    public int MapMid
    {
        get => _mapMid;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "MapMid must be between 0 and 100");
            SetProperty(ref _mapMid, value);
        }
    }

    private int _mapMax;
    public int MapMax
    {
        get => _mapMax;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "MapMax must be between 0 and 100");
            SetProperty(ref _mapMax, value);
        }
    }

    public bool HasPreset => !string.IsNullOrEmpty(PresetName);
    public bool IsRoutingSemiParallel => Routing == 0;
    public bool IsRoutingSerial => Routing == 1;
    public bool IsRoutingParallel => Routing == 2;

    public DriveBlockViewModel Drive { get; } = new();
    public CompressorBlockViewModel Compressor { get; } = new();
    public EqGateBlockViewModel EqGate { get; } = new();
    public ModulationBlockViewModel Modulation { get; } = new();
    public PitchBlockViewModel Pitch { get; } = new();
    public DelayBlockViewModel Delay { get; } = new();
    public ReverbBlockViewModel Reverb { get; } = new();

    public PresetDetailViewModel(ISavePresetUseCase savePresetUseCase)
    {
        _savePresetUseCase = savePresetUseCase;
    }

    /// <summary>
    /// Loads all effect block parameters from a Preset domain model.
    /// </summary>
    public void LoadFromPreset(Preset? preset)
    {
        _currentPreset = preset;

        if (preset == null)
        {
            PresetName = "";
            Position = "";
            PresetNumber = 0;
            TargetSlot = 1;
            StatusMessage = "";
            TapTempo = 100;
            Routing = 0;
            LevelOutLeft = -100;
            LevelOutRight = -100;
            MapParameter = 0;
            MapMin = 0;
            MapMid = 0;
            MapMax = 0;
            UploadPresetCommand.NotifyCanExecuteChanged();
            return;
        }

        PresetName = preset.Name;
        Position = $"#{preset.Number}";
        PresetNumber = preset.Number;
        TargetSlot = preset.Number;
        StatusMessage = "";
        TapTempo = Math.Clamp(preset.TapTempo, 100, 3000);
        Routing = Math.Clamp(preset.Routing, 0, 2);
        LevelOutLeft = Math.Clamp(preset.LevelOutLeft, -100, 0);
        LevelOutRight = Math.Clamp(preset.LevelOutRight, -100, 0);
        MapParameter = Math.Clamp(preset.MapParameter, 0, 127);
        MapMin = Math.Clamp(preset.MapMin, 0, 100);
        MapMid = Math.Clamp(preset.MapMid, 0, 100);
        MapMax = Math.Clamp(preset.MapMax, 0, 100);

        // Load all effect blocks
        Drive.LoadFromPreset(preset);
        Compressor.LoadFromPreset(preset);
        EqGate.LoadFromPreset(preset);
        Modulation.LoadFromPreset(preset);
        Pitch.LoadFromPreset(preset);
        Delay.LoadFromPreset(preset);
        Reverb.LoadFromPreset(preset);

        UploadPresetCommand.NotifyCanExecuteChanged();
    }

    [ObservableProperty]
    private int _targetSlot = 1;

    [ObservableProperty]
    private string _statusMessage = "";

    /// <summary>
    /// Writes all ViewModel state back into the current Preset, then returns it.
    /// Call this before uploading to ensure all edits are encoded into SysEx.
    /// </summary>
    private Preset? BuildModifiedPreset()
    {
        if (_currentPreset == null) return null;

        // Write global parameters
        _currentPreset.Name = PresetName;
        _currentPreset.Number = PresetNumber;
        _currentPreset.TapTempo = TapTempo;
        _currentPreset.Routing = Routing;
        _currentPreset.LevelOutLeft = LevelOutLeft;
        _currentPreset.LevelOutRight = LevelOutRight;
        _currentPreset.MapParameter = MapParameter;
        _currentPreset.MapMin = MapMin;
        _currentPreset.MapMid = MapMid;
        _currentPreset.MapMax = MapMax;

        // Write all effect blocks
        Drive.WriteToPreset(_currentPreset);
        Compressor.WriteToPreset(_currentPreset);
        EqGate.WriteToPreset(_currentPreset);
        Modulation.WriteToPreset(_currentPreset);
        Pitch.WriteToPreset(_currentPreset);
        Delay.WriteToPreset(_currentPreset);
        Reverb.WriteToPreset(_currentPreset);

        return _currentPreset;
    }

    [RelayCommand(CanExecute = nameof(CanUploadPreset))]
    private async Task UploadPresetAsync()
    {
        if (_currentPreset == null) return;

        // Write all UI changes back into the Preset before sending
        BuildModifiedPreset();

        StatusMessage = $"Uploading preset to slot {TargetSlot}...";

        var result = await _savePresetUseCase.ExecuteAsync(_currentPreset, TargetSlot, verify: true);

        if (result.IsSuccess)
        {
            StatusMessage = $"Preset uploaded to slot {TargetSlot} successfully";
        }
        else
        {
            StatusMessage = $"Upload failed: {result.Errors.First().Message}";
        }
    }

    private bool CanUploadPreset() => _currentPreset != null;
}
