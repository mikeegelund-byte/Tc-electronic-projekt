using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Domain.Models;

namespace Nova.Presentation.ViewModels.Effects;

/// <summary>
/// ViewModel for Pitch effect block.
/// Supports Pitch Shifter, Octaver, Whammy, Detune, and Intelligent Pitch modes.
/// </summary>
public partial class PitchBlockViewModel : ObservableObject
{
    private string _typeName = "Shifter";
    public string TypeName
    {
        get => _typeName;
        private set => SetProperty(ref _typeName, value);
    }

    private int _type;
    public int Type
    {
        get => _type;
        set
        {
            if (value < 0 || value > 4)
                throw new ArgumentOutOfRangeException(nameof(value), "Type must be between 0 and 4");
            if (SetProperty(ref _type, value))
            {
                UpdateTypeName();
                OnPropertyChanged(nameof(IsShifter));
                OnPropertyChanged(nameof(IsOctaver));
                OnPropertyChanged(nameof(IsWhammy));
                OnPropertyChanged(nameof(IsDetune));
                OnPropertyChanged(nameof(IsIntelligent));
                OnPropertyChanged(nameof(HasVoices));
                OnPropertyChanged(nameof(IsPedalPitch));
                OnPropertyChanged(nameof(IsNotIntelligent));
            }
        }
    }
    
    private int _voice1;
    public int Voice1
    {
        get => _voice1;
        set
        {
            var min = Type == 4 ? -13 : -100;
            var max = Type == 4 ? 13 : 100;
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(nameof(value), $"Voice1 must be between {min} and {max}");
            SetProperty(ref _voice1, value);
        }
    }
    
    private int _voice2;
    public int Voice2
    {
        get => _voice2;
        set
        {
            var min = Type == 4 ? -13 : -100;
            var max = Type == 4 ? 13 : 100;
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(nameof(value), $"Voice2 must be between {min} and {max}");
            SetProperty(ref _voice2, value);
        }
    }

    private int _pan1;
    public int Pan1
    {
        get => _pan1;
        set
        {
            if (value < -50 || value > 50)
                throw new ArgumentOutOfRangeException(nameof(value), "Pan1 must be between -50 and 50");
            SetProperty(ref _pan1, value);
        }
    }

    private int _pan2;
    public int Pan2
    {
        get => _pan2;
        set
        {
            if (value < -50 || value > 50)
                throw new ArgumentOutOfRangeException(nameof(value), "Pan2 must be between -50 and 50");
            SetProperty(ref _pan2, value);
        }
    }

    private int _delay1;
    public int Delay1
    {
        get => _delay1;
        set
        {
            if (value < 0 || value > 50)
                throw new ArgumentOutOfRangeException(nameof(value), "Delay1 must be between 0 and 50 ms");
            SetProperty(ref _delay1, value);
        }
    }

    private int _delay2;
    public int Delay2
    {
        get => _delay2;
        set
        {
            if (value < 0 || value > 50)
                throw new ArgumentOutOfRangeException(nameof(value), "Delay2 must be between 0 and 50 ms");
            SetProperty(ref _delay2, value);
        }
    }

    private int _feedback1OrKey;
    public int Feedback1OrKey
    {
        get => _feedback1OrKey;
        set
        {
            var min = 0;
            var max = Type == 4 ? 12 : 100;
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(nameof(value), $"Feedback1/Key must be between {min} and {max}");
            SetProperty(ref _feedback1OrKey, value);
        }
    }

    private int _feedback2OrScale;
    public int Feedback2OrScale
    {
        get => _feedback2OrScale;
        set
        {
            var min = 0;
            var max = Type == 4 ? 13 : 100;
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(nameof(value), $"Feedback2/Scale must be between {min} and {max}");
            SetProperty(ref _feedback2OrScale, value);
        }
    }

    private int _level1;
    public int Level1
    {
        get => _level1;
        set
        {
            if (value < -100 || value > 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Level1 must be between -100 and 0 dB");
            SetProperty(ref _level1, value);
        }
    }

    private int _level2;
    public int Level2
    {
        get => _level2;
        set
        {
            if (Type == 1 || Type == 2)
            {
                if (value != 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Level2 must be 0 for Octaver/Whammy");
            }
            else if (value < -100 || value > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Level2 must be between -100 and 0 dB");
            }
            SetProperty(ref _level2, value);
        }
    }

    private int _direction;
    public int Direction
    {
        get => _direction;
        set
        {
            if (Type == 1 || Type == 2)
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(value), "Direction must be 0 or 1");
            }
            else if (value != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Direction must be 0 for non-Octaver/Whammy");
            }
            SetProperty(ref _direction, value);
        }
    }

    private int _range;
    public int Range
    {
        get => _range;
        set
        {
            if (Type == 1 || Type == 2)
            {
                if (value < 1 || value > 2)
                    throw new ArgumentOutOfRangeException(nameof(value), "Range must be between 1 and 2");
            }
            else if (value != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Range must be 0 for non-Octaver/Whammy");
            }
            SetProperty(ref _range, value);
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
    
    [ObservableProperty] private bool _isEnabled;

    public bool IsShifter => Type == 0;
    public bool IsOctaver => Type == 1;
    public bool IsWhammy => Type == 2;
    public bool IsDetune => Type == 3;
    public bool IsIntelligent => Type == 4;
    public bool HasVoices => Type == 0 || Type == 3 || Type == 4;
    public bool IsPedalPitch => Type == 1 || Type == 2;
    public bool IsNotIntelligent => Type != 4;

    public void LoadFromPreset(Preset? preset)
    {
        if (preset == null) return;

        Type = Math.Clamp(preset.PitchType, 0, 4);
        Voice1 = Type == 4
            ? Math.Clamp(preset.PitchVoice1, -13, 13)
            : Math.Clamp(preset.PitchVoice1, -100, 100);
        Voice2 = Type == 4
            ? Math.Clamp(preset.PitchVoice2, -13, 13)
            : Math.Clamp(preset.PitchVoice2, -100, 100);
        Pan1 = Math.Clamp(preset.PitchPan1, -50, 50);
        Pan2 = Math.Clamp(preset.PitchPan2, -50, 50);
        Delay1 = Math.Clamp(preset.PitchDelay1, 0, 50);
        Delay2 = Math.Clamp(preset.PitchDelay2, 0, 50);
        Feedback1OrKey = Type == 4
            ? Math.Clamp(preset.PitchFeedback1OrKey, 0, 12)
            : Math.Clamp(preset.PitchFeedback1OrKey, 0, 100);
        Feedback2OrScale = Type == 4
            ? Math.Clamp(preset.PitchFeedback2OrScale, 0, 13)
            : Math.Clamp(preset.PitchFeedback2OrScale, 0, 100);
        Level1 = Math.Clamp(preset.PitchLevel1, -100, 0);
        if (Type == 1 || Type == 2)
        {
            Direction = Math.Clamp(preset.PitchDirection, 0, 1);
            Range = Math.Clamp(preset.PitchRange, 1, 2);
            Level2 = 0;
        }
        else
        {
            Direction = 0;
            Range = 0;
            Level2 = Math.Clamp(preset.PitchLevel2, -100, 0);
        }
        Mix = Math.Clamp(preset.PitchMix, 0, 100);
        IsEnabled = preset.PitchEnabled;
    }

    private void UpdateTypeName()
    {
        TypeName = Type switch
        {
            0 => "Shifter",
            1 => "Octaver",
            2 => "Whammy",
            3 => "Detune",
            4 => "Intelligent",
            _ => "Unknown"
        };
    }
}
