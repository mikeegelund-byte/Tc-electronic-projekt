using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Domain.Models;

namespace Nova.Presentation.ViewModels;

/// <summary>
/// ViewModel for displaying global system settings from SystemDump.
/// Maps SystemDump domain model to UI-friendly properties following MVVM Toolkit pattern.
/// </summary>
public partial class SystemSettingsViewModel : ObservableObject
{
    [ObservableProperty] 
    private int _midiChannel;        // 0-15

    [ObservableProperty] 
    private int _deviceId;            // 0-127

    [ObservableProperty] 
    private bool _midiClockEnabled;

    [ObservableProperty] 
    private bool _midiProgramChangeEnabled;

    [ObservableProperty] 
    private string _version = string.Empty;

    /// <summary>
    /// Loads system settings from a SystemDump into the ViewModel properties.
    /// </summary>
    /// <param name="dump">The SystemDump containing global system settings</param>
    public void LoadFromDump(SystemDump dump)
    {
        MidiChannel = dump.MidiChannel;
        DeviceId = dump.DeviceId;
        MidiClockEnabled = dump.IsMidiClockEnabled;
        MidiProgramChangeEnabled = dump.IsMidiProgramChangeEnabled;
        Version = dump.GetVersionString();
    }
}
