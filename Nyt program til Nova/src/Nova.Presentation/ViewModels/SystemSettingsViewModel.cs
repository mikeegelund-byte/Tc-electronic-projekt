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

        // Parse from raw SysEx bytes
        // Byte 4 = Device ID
        DeviceId = dump.RawSysEx[4];
        
        // Byte 8 = MIDI Channel (0-15)
        MidiChannel = dump.RawSysEx[8];
        
        // Bytes 20-21 = MIDI settings (0x01 = enabled)
        MidiClockEnabled = dump.RawSysEx.Length > 20 && dump.RawSysEx[20] == 0x01;
        MidiProgramChangeEnabled = dump.RawSysEx.Length > 21 && dump.RawSysEx[21] == 0x01;
        
        // Version info from bytes 22-23 (major.minor)
        if (dump.RawSysEx.Length > 23)
        {
            var major = dump.RawSysEx[22];
            var minor = dump.RawSysEx[23];
            Version = $"{major}.{minor}";
        }
    }
}
