using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Domain.Models;

namespace Nova.Presentation.ViewModels.Effects;

/// <summary>
/// ViewModel for Compressor effect block.
/// Displays Percussive, Sustaining, or Advanced type with parameters.
/// </summary>
public partial class CompressorBlockViewModel : ObservableObject
{
    [ObservableProperty] private string _type = "Percussive";
    [ObservableProperty] private int _typeId = 0;
    [ObservableProperty] private bool _isEnabled;

    private int _threshold;
    public int Threshold
    {
        get => _threshold;
        set
        {
            if (value < -30 || value > 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Threshold must be between -30 and 0 dB");
            SetProperty(ref _threshold, value);
        }
    }

    private int _ratio;
    public int Ratio
    {
        get => _ratio;
        set
        {
            if (value < 0 || value > 15)
                throw new ArgumentOutOfRangeException(nameof(value), "Ratio must be between 0 and 15");
            SetProperty(ref _ratio, value);
        }
    }

    private int _attack;
    public int Attack
    {
        get => _attack;
        set
        {
            if (value < 0 || value > 16)
                throw new ArgumentOutOfRangeException(nameof(value), "Attack must be between 0 and 16");
            SetProperty(ref _attack, value);
        }
    }

    private int _release;
    public int Release
    {
        get => _release;
        set
        {
            if (value < 13 || value > 23)
                throw new ArgumentOutOfRangeException(nameof(value), "Release must be between 13 and 23");
            SetProperty(ref _release, value);
        }
    }

    private int _drive;
    public int Drive
    {
        get => _drive;
        set
        {
            if (value < 0 || value > 20)
                throw new ArgumentOutOfRangeException(nameof(value), "Drive must be between 0 and 20");
            SetProperty(ref _drive, value);
        }
    }

    private int _response;
    public int Response
    {
        get => _response;
        set
        {
            if (value < 0 || value > 10)
                throw new ArgumentOutOfRangeException(nameof(value), "Response must be between 0 and 10");
            SetProperty(ref _response, value);
        }
    }

    private int _level;
    public int Level
    {
        get => _level;
        set
        {
            if (value < -12 || value > 12)
                throw new ArgumentOutOfRangeException(nameof(value), "Level must be between -12 and 12 dB");
            SetProperty(ref _level, value);
        }
    }

    public bool IsPercussive => TypeId == 0;
    public bool IsSustaining => TypeId == 1;
    public bool IsAdvanced => TypeId == 2;
    public bool IsPercussiveOrSustaining => TypeId == 0 || TypeId == 1;

    public void LoadFromPreset(Preset? preset)
    {
        if (preset == null) return;

        TypeId = preset.CompType;
        Type = preset.CompType switch
        {
            0 => "Percussive",
            1 => "Sustaining",
            2 => "Advanced",
            _ => "Unknown"
        };
        
        Threshold = Math.Clamp(preset.CompThreshold, -30, 0);
        Ratio = Math.Clamp(preset.CompRatio, 0, 15);
        Attack = Math.Clamp(preset.CompAttack, 0, 16);
        Release = Math.Clamp(preset.CompRelease, 13, 23);
        Drive = Math.Clamp(preset.CompDrive, 0, 20);
        Response = Math.Clamp(preset.CompResponse, 0, 10);
        Level = Math.Clamp(preset.CompLevel, -12, 12);
        IsEnabled = preset.CompressorEnabled;

        OnPropertyChanged(nameof(IsPercussive));
        OnPropertyChanged(nameof(IsSustaining));
        OnPropertyChanged(nameof(IsAdvanced));
        OnPropertyChanged(nameof(IsPercussiveOrSustaining));
    }

    public void WriteToPreset(Preset preset)
    {
        if (preset == null) return;

        preset.CompType = TypeId;
        preset.CompThreshold = Threshold;
        preset.CompRatio = Ratio;
        preset.CompAttack = Attack;
        preset.CompRelease = Release;
        preset.CompResponse = Response;
        preset.CompDrive = Drive;
        preset.CompLevel = Level;
        preset.CompressorEnabled = IsEnabled;
    }
}
