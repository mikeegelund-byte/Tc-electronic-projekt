using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Domain.Models;

namespace Nova.Presentation.ViewModels.Effects;

/// <summary>
/// ViewModel for Delay effect block.
/// Displays Clean, Analog, Tape, Dynamic, Dual, or Ping-Pong delay with parameters.
/// </summary>
public partial class DelayBlockViewModel : ObservableObject
{
    [ObservableProperty] private string _type = "Clean";
    [ObservableProperty] private int _typeId = 0;
    [ObservableProperty] private bool _isEnabled;
    
    private int _time;
    public int Time
    {
        get => _time;
        set
        {
            if (value < 0 || value > 1800)
                throw new ArgumentOutOfRangeException(nameof(value), "Time must be between 0 and 1800 ms");
            SetProperty(ref _time, value);
        }
    }
    
    private int _time2;
    public int Time2
    {
        get => _time2;
        set
        {
            if (value < 0 || value > 1800)
                throw new ArgumentOutOfRangeException(nameof(value), "Time2 must be between 0 and 1800 ms");
            SetProperty(ref _time2, value);
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

    private int _tempo2OrWidth;
    public int Tempo2OrWidth
    {
        get => _tempo2OrWidth;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Tempo2/Width must be between 0 and 100");
            SetProperty(ref _tempo2OrWidth, value);
        }
    }
    
    private int _feedback;
    public int Feedback
    {
        get => _feedback;
        set
        {
            if (value < 0 || value > 120)
                throw new ArgumentOutOfRangeException(nameof(value), "Feedback must be between 0 and 120%");
            SetProperty(ref _feedback, value);
        }
    }

    private int _clipOrFeedback2;
    public int ClipOrFeedback2
    {
        get => _clipOrFeedback2;
        set
        {
            if (value < 0 || value > 120)
                throw new ArgumentOutOfRangeException(nameof(value), "Clip/Feedback2 must be between 0 and 120");
            SetProperty(ref _clipOrFeedback2, value);
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

    private int _loCut;
    public int LoCut
    {
        get => _loCut;
        set
        {
            if (value < 0 || value > 61)
                throw new ArgumentOutOfRangeException(nameof(value), "LoCut must be between 0 and 61");
            SetProperty(ref _loCut, value);
        }
    }

    private int _offsetOrPan1;
    public int OffsetOrPan1
    {
        get => _offsetOrPan1;
        set
        {
            if (value < -200 || value > 200)
                throw new ArgumentOutOfRangeException(nameof(value), "Offset/Pan1 must be between -200 and 200");
            SetProperty(ref _offsetOrPan1, value);
        }
    }

    private int _senseOrPan2;
    public int SenseOrPan2
    {
        get => _senseOrPan2;
        set
        {
            if (value < -50 || value > 50)
                throw new ArgumentOutOfRangeException(nameof(value), "Sense/Pan2 must be between -50 and 50");
            SetProperty(ref _senseOrPan2, value);
        }
    }

    private int _damp;
    public int Damp
    {
        get => _damp;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(nameof(value), "Damp must be between 0 and 100");
            SetProperty(ref _damp, value);
        }
    }

    private int _release;
    public int Release
    {
        get => _release;
        set
        {
            if (value < 0 || value > 21)
                throw new ArgumentOutOfRangeException(nameof(value), "Release must be between 0 and 21");
            SetProperty(ref _release, value);
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

    public bool IsClean => TypeId == 0;
    public bool IsAnalog => TypeId == 1;
    public bool IsTape => TypeId == 2;
    public bool IsDynamic => TypeId == 3;
    public bool IsDual => TypeId == 4;
    public bool IsPingPong => TypeId == 5;

    public void LoadFromPreset(Preset? preset)
    {
        if (preset == null) return;

        TypeId = preset.DelayType;
        Type = preset.DelayType switch
        {
            0 => "Clean",
            1 => "Analog",
            2 => "Tape",
            3 => "Dynamic",
            4 => "Dual",
            5 => "Ping-Pong",
            _ => "Unknown"
        };
        
        Time = Math.Clamp(preset.DelayTime, 0, 1800);
        Time2 = Math.Clamp(preset.DelayTime2, 0, 1800);
        Tempo = Math.Clamp(preset.DelayTempo, 0, 16);
        Tempo2OrWidth = Math.Clamp(preset.DelayTempo2OrWidth, 0, 100);
        Feedback = Math.Clamp(preset.DelayFeedback, 0, 120);
        ClipOrFeedback2 = Math.Clamp(preset.DelayClipOrFeedback2, 0, 120);
        HiCut = Math.Clamp(preset.DelayHiCut, 0, 61);
        LoCut = Math.Clamp(preset.DelayLoCut, 0, 61);
        OffsetOrPan1 = Math.Clamp(preset.DelayOffsetOrPan1, -200, 200);
        SenseOrPan2 = Math.Clamp(preset.DelaySenseOrPan2, -50, 50);
        Damp = Math.Clamp(preset.DelayDamp, 0, 100);
        Release = Math.Clamp(preset.DelayRelease, 0, 21);
        Mix = Math.Clamp(preset.DelayMix, 0, 100);
        IsEnabled = preset.DelayEnabled;

        OnPropertyChanged(nameof(IsClean));
        OnPropertyChanged(nameof(IsAnalog));
        OnPropertyChanged(nameof(IsTape));
        OnPropertyChanged(nameof(IsDynamic));
        OnPropertyChanged(nameof(IsDual));
        OnPropertyChanged(nameof(IsPingPong));
    }

    public void WriteToPreset(Preset preset)
    {
        if (preset == null) return;

        preset.DelayType = TypeId;
        preset.DelayTime = Time;
        preset.DelayTime2 = Time2;
        preset.DelayTempo = Tempo;
        preset.DelayTempo2OrWidth = Tempo2OrWidth;
        preset.DelayFeedback = Feedback;
        preset.DelayClipOrFeedback2 = ClipOrFeedback2;
        preset.DelayHiCut = HiCut;
        preset.DelayLoCut = LoCut;
        preset.DelayOffsetOrPan1 = OffsetOrPan1;
        preset.DelaySenseOrPan2 = SenseOrPan2;
        preset.DelayDamp = Damp;
        preset.DelayRelease = Release;
        preset.DelayMix = Mix;
        preset.DelayEnabled = IsEnabled;
    }
}
