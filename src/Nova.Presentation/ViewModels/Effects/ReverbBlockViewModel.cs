using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Domain.Models;

namespace Nova.Presentation.ViewModels.Effects;

/// <summary>
/// ViewModel for Reverb effect block.
/// Displays Spring, Hall, Room, or Plate reverb with parameters.
/// </summary>
public partial class ReverbBlockViewModel : ObservableObject
{
    [ObservableProperty] private string _type = "Spring";
    [ObservableProperty] private int _typeId = 0;
    [ObservableProperty] private bool _isEnabled;
    
    private int _decay;
    public int Decay
    {
        get => _decay;
        set
        {
            if (value < 1 || value > 200)
                throw new ArgumentOutOfRangeException(nameof(value), "Decay must be between 1 and 200");
            SetProperty(ref _decay, value);
        }
    }
    
    private int _preDelay;
    public int PreDelay
    {
        get => _preDelay;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "PreDelay must be between 0 and 100 ms");
            SetProperty(ref _preDelay, value);
        }
    }

    private int _shape;
    public int Shape
    {
        get => _shape;
        set
        {
            if (value < 0 || value > 2)
                throw new ArgumentOutOfRangeException(nameof(value), "Shape must be between 0 and 2");
            SetProperty(ref _shape, value);
        }
    }

    private int _size;
    public int Size
    {
        get => _size;
        set
        {
            if (value < 0 || value > 7)
                throw new ArgumentOutOfRangeException(nameof(value), "Size must be between 0 and 7");
            SetProperty(ref _size, value);
        }
    }

    private int _hiColor;
    public int HiColor
    {
        get => _hiColor;
        set
        {
            if (value < 0 || value > 6)
                throw new ArgumentOutOfRangeException(nameof(value), "Hi Color must be between 0 and 6");
            SetProperty(ref _hiColor, value);
        }
    }

    private int _hiLevel;
    public int HiLevel
    {
        get => _hiLevel;
        set
        {
            if (value < -25 || value > 25)
                throw new ArgumentOutOfRangeException(nameof(value), "Hi Level must be between -25 and 25 dB");
            SetProperty(ref _hiLevel, value);
        }
    }

    private int _loColor;
    public int LoColor
    {
        get => _loColor;
        set
        {
            if (value < 0 || value > 6)
                throw new ArgumentOutOfRangeException(nameof(value), "Lo Color must be between 0 and 6");
            SetProperty(ref _loColor, value);
        }
    }

    private int _loLevel;
    public int LoLevel
    {
        get => _loLevel;
        set
        {
            if (value < -25 || value > 25)
                throw new ArgumentOutOfRangeException(nameof(value), "Lo Level must be between -25 and 25 dB");
            SetProperty(ref _loLevel, value);
        }
    }

    private int _roomLevel;
    public int RoomLevel
    {
        get => _roomLevel;
        set
        {
            if (value < -100 || value > 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Room Level must be between -100 and 0 dB");
            SetProperty(ref _roomLevel, value);
        }
    }

    private int _reverbLevel;
    public int ReverbLevel
    {
        get => _reverbLevel;
        set
        {
            if (value < -100 || value > 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Reverb Level must be between -100 and 0 dB");
            SetProperty(ref _reverbLevel, value);
        }
    }

    private int _diffuse;
    public int Diffuse
    {
        get => _diffuse;
        set
        {
            if (value < -25 || value > 25)
                throw new ArgumentOutOfRangeException(nameof(value), "Diffuse must be between -25 and 25 dB");
            SetProperty(ref _diffuse, value);
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

    public bool IsSpring => TypeId == 0;
    public bool IsHall => TypeId == 1;
    public bool IsRoom => TypeId == 2;
    public bool IsPlate => TypeId == 3;

    public void LoadFromPreset(Preset? preset)
    {
        if (preset == null) return;

        TypeId = preset.ReverbType;
        Type = preset.ReverbType switch
        {
            0 => "Spring",
            1 => "Hall",
            2 => "Room",
            3 => "Plate",
            _ => "Unknown"
        };
        
        Decay = Math.Clamp(preset.ReverbDecay, 1, 200);
        PreDelay = Math.Clamp(preset.ReverbPreDelay, 0, 100);
        Shape = Math.Clamp(preset.ReverbShape, 0, 2);
        Size = Math.Clamp(preset.ReverbSize, 0, 7);
        HiColor = Math.Clamp(preset.ReverbHiColor, 0, 6);
        HiLevel = Math.Clamp(preset.ReverbHiLevel, -25, 25);
        LoColor = Math.Clamp(preset.ReverbLoColor, 0, 6);
        LoLevel = Math.Clamp(preset.ReverbLoLevel, -25, 25);
        RoomLevel = Math.Clamp(preset.ReverbRoomLevel, -100, 0);
        ReverbLevel = Math.Clamp(preset.ReverbLevel, -100, 0);
        Diffuse = Math.Clamp(preset.ReverbDiffuse, -25, 25);
        Mix = Math.Clamp(preset.ReverbMix, 0, 100);
        IsEnabled = preset.ReverbEnabled;

        OnPropertyChanged(nameof(IsSpring));
        OnPropertyChanged(nameof(IsHall));
        OnPropertyChanged(nameof(IsRoom));
        OnPropertyChanged(nameof(IsPlate));
    }
}
