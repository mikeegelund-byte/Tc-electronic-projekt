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
            if (value < 0 || value > 255)
                throw new ArgumentOutOfRangeException(nameof(value), "TapTempo must be between 0 and 255");
            SetProperty(ref _tapTempo, value);
        }
    }

    private int _routing;
    public int Routing
    {
        get => _routing;
        set
        {
            if (value < 0 || value > 7)
                throw new ArgumentOutOfRangeException(nameof(value), "Routing must be between 0 and 7");
            SetProperty(ref _routing, value);
        }
    }

    private int _levelOutLeft;
    public int LevelOutLeft
    {
        get => _levelOutLeft;
        set
        {
            if (value < -20 || value > 20)
                throw new ArgumentOutOfRangeException(nameof(value), "LevelOutLeft must be between -20 and 20");
            SetProperty(ref _levelOutLeft, value);
        }
    }

    private int _levelOutRight;
    public int LevelOutRight
    {
        get => _levelOutRight;
        set
        {
            if (value < -20 || value > 20)
                throw new ArgumentOutOfRangeException(nameof(value), "LevelOutRight must be between -20 and 20");
            SetProperty(ref _levelOutRight, value);
        }
    }

    public bool HasPreset => !string.IsNullOrEmpty(PresetName);

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
            TapTempo = 0;
            Routing = 0;
            LevelOutLeft = 0;
            LevelOutRight = 0;
            UploadPresetCommand.NotifyCanExecuteChanged();
            return;
        }

        PresetName = preset.Name;
        Position = $"#{preset.Number}";
        PresetNumber = preset.Number;
        TargetSlot = preset.Number;
        StatusMessage = "";
        TapTempo = Math.Clamp(preset.TapTempo, 0, 255);
        Routing = Math.Clamp(preset.Routing, 0, 7);
        LevelOutLeft = Math.Clamp(preset.LevelOutLeft, -20, 20);
        LevelOutRight = Math.Clamp(preset.LevelOutRight, -20, 20);

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

    [RelayCommand(CanExecute = nameof(CanUploadPreset))]
    private async Task UploadPresetAsync()
    {
        if (_currentPreset == null) return;

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
