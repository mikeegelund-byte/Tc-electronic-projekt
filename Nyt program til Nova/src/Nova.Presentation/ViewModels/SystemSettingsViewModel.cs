using CommunityToolkit.Mvvm.ComponentModel;

namespace Nova.Presentation.ViewModels;

/// <summary>
/// ViewModel for displaying read-only system settings from Nova System pedal.
/// </summary>
public partial class SystemSettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private int _midiChannel;

    [ObservableProperty]
    private int _deviceId;

    [ObservableProperty]
    private bool _midiClockEnabled;

    [ObservableProperty]
    private bool _midiProgramChangeEnabled;

    [ObservableProperty]
    private string _version = "1.0.0";
}
