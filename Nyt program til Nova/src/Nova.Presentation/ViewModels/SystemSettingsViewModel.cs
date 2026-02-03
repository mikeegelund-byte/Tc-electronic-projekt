using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Domain.Models;

namespace Nova.Presentation.ViewModels;

/// <summary>
/// ViewModel for displaying global system settings from SystemDump.
/// Read-only display of MIDI channel, device ID, and feature flags.
/// </summary>
public partial class SystemSettingsViewModel : ObservableObject
{
    [ObservableProperty] private int _midiChannel;
    [ObservableProperty] private int _deviceId;
    [ObservableProperty] private bool _midiClockEnabled;
    [ObservableProperty] private bool _midiProgramChangeEnabled;
    [ObservableProperty] private string _version = string.Empty;

    /// <summary>
    /// Loads system settings from a SystemDump object.
    /// </summary>
    public void LoadFromDump(SystemDump dump)
    {
        if (dump?.RawSysEx == null || dump.RawSysEx.Length < 10)
            return;

        // Parse from SystemDump nibble values
        DeviceId = dump.GetSysExId();
        MidiChannel = dump.GetMidiChannel();
        MidiClockEnabled = dump.GetMidiClockEnabled();
        MidiProgramChangeEnabled = dump.GetProgramChangeInEnabled() || dump.GetProgramChangeOutEnabled();
        
        // Version info from bytes 22-23 (major.minor)
        if (dump.RawSysEx.Length > 23)
        {
            var major = dump.RawSysEx[22];
            var minor = dump.RawSysEx[23];
            Version = $"{major}.{minor}";
        }
    }
}
