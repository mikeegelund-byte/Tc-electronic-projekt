using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Domain.Models;

namespace Nova.Presentation.ViewModels.Effects;

/// <summary>
/// ViewModel for Modulation effect block.
/// Displays Chorus, Flanger, Vibrato, Phaser, Tremolo, or Panner with parameters.
/// </summary>
public partial class ModulationBlockViewModel : ObservableObject
{
    [ObservableProperty] private string _type = "Chorus";
    [ObservableProperty] private int _typeId = 0;
    [ObservableProperty] private bool _isEnabled;

    private int _speed;
    public int Speed
    {
        get => _speed;
        set
        {
            if (value < 0 || value > 81)
                throw new ArgumentOutOfRangeException(nameof(value), "Speed must be between 0 and 81");
            SetProperty(ref _speed, value);
        }
    }

    private int _depth;
    public int Depth
    {
        get => _depth;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Depth must be between 0 and 100%");
            SetProperty(ref _depth, value);
        }
    }

    private int _tempo;
    public int Tempo
    {
        get => _tempo;
        set
        {
            if (value < 0 || value > 16)
                throw new ArgumentOutOfRangeException(nameof(value), "Tempo must be between 0 and 16");
            SetProperty(ref _tempo, value);
        }
    }

    private int _hiCut;
    public int HiCut
    {
        get => _hiCut;
        set
        {
            if (value < 0 || value > 61)
                throw new ArgumentOutOfRangeException(nameof(value), "HiCut must be between 0 and 61");
            SetProperty(ref _hiCut, value);
        }
    }

    private int _feedback;
    public int Feedback
    {
        get => _feedback;
        set
        {
            if (value < -100 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Feedback must be between -100 and 100");
            SetProperty(ref _feedback, value);
        }
    }

    private int _delayOrRange;
    public int DelayOrRange
    {
        get => _delayOrRange;
        set
        {
            if (value < 0 || value > 500)
                throw new ArgumentOutOfRangeException(nameof(value), "Delay/Range must be between 0 and 500");
            SetProperty(ref _delayOrRange, value);
        }
    }

    private int _width;
    public int Width
    {
        get => _width;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Width must be between 0 and 100%");
            SetProperty(ref _width, value);
        }
    }

    private int _mix;
    public int Mix
    {
        get => _mix;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Mix must be between 0 and 100%");
            SetProperty(ref _mix, value);
        }
    }

    public bool IsChorus => TypeId == 0;
    public bool IsFlanger => TypeId == 1;
    public bool IsVibrato => TypeId == 2;
    public bool IsPhaser => TypeId == 3;
    public bool IsTremolo => TypeId == 4;
    public bool IsPanner => TypeId == 5;

    public void LoadFromPreset(Preset? preset)
    {
        if (preset == null) return;

        TypeId = preset.ModType;
        Type = preset.ModType switch
        {
            0 => "Chorus",
            1 => "Flanger",
            2 => "Vibrato",
            3 => "Phaser",
            4 => "Tremolo",
            5 => "Panner",
            _ => "Unknown"
        };
        
        Speed = Math.Clamp(preset.ModSpeed, 0, 81);
        Depth = Math.Clamp(preset.ModDepth, 0, 100);
        Tempo = Math.Clamp(preset.ModTempo, 0, 16);
        HiCut = Math.Clamp(preset.ModHiCut, 0, 61);
        Feedback = Math.Clamp(preset.ModFeedback, -100, 100);
        DelayOrRange = Math.Clamp(preset.ModDelayOrRange, 0, 500);
        Width = Math.Clamp(preset.ModWidth, 0, 100);
        Mix = Math.Clamp(preset.ModMix, 0, 100);
        IsEnabled = preset.ModulationEnabled;

        OnPropertyChanged(nameof(IsChorus));
        OnPropertyChanged(nameof(IsFlanger));
        OnPropertyChanged(nameof(IsVibrato));
        OnPropertyChanged(nameof(IsPhaser));
        OnPropertyChanged(nameof(IsTremolo));
        OnPropertyChanged(nameof(IsPanner));
    }

    public void WriteToPreset(Preset preset)
    {
        if (preset == null) return;

        preset.ModType = TypeId;
        preset.ModSpeed = Speed;
        preset.ModDepth = Depth;
        preset.ModTempo = Tempo;
        preset.ModHiCut = HiCut;
        preset.ModFeedback = Feedback;
        preset.ModDelayOrRange = DelayOrRange;
        preset.ModWidth = Width;
        preset.ModMix = Mix;
        preset.ModulationEnabled = IsEnabled;
    }
}
