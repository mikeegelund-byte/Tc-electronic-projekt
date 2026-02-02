using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Domain.Models;

namespace Nova.Presentation.ViewModels;

/// <summary>
/// ViewModel for editing global system settings from SystemDump.
/// Supports dirty tracking to detect unsaved changes.
/// </summary>
public partial class SystemSettingsViewModel : ObservableObject
{
    [ObservableProperty] private int _midiChannel;
    [ObservableProperty] private int _deviceId;
    [ObservableProperty] private bool _midiClockEnabled;
    [ObservableProperty] private bool _midiProgramChangeEnabled;
    [ObservableProperty] private string _version = string.Empty;
    
    [ObservableProperty] private bool _hasUnsavedChanges;

    private SystemDump? _originalDump;
    private int _originalMidiChannel;
    private int _originalDeviceId;
    private bool _originalMidiClockEnabled;
    private bool _originalMidiProgramChangeEnabled;

    partial void OnMidiChannelChanged(int value)
    {
        CheckForChanges();
    }

    partial void OnDeviceIdChanged(int value)
    {
        CheckForChanges();
    }

    partial void OnMidiClockEnabledChanged(bool value)
    {
        CheckForChanges();
    }

    partial void OnMidiProgramChangeEnabledChanged(bool value)
    {
        CheckForChanges();
    }

    private void CheckForChanges()
    {
        if (_originalDump == null)
        {
            HasUnsavedChanges = false;
            return;
        }

        HasUnsavedChanges = MidiChannel != _originalMidiChannel ||
                           DeviceId != _originalDeviceId ||
                           MidiClockEnabled != _originalMidiClockEnabled ||
                           MidiProgramChangeEnabled != _originalMidiProgramChangeEnabled;
    }

    /// <summary>
    /// Loads system settings from a SystemDump object.
    /// </summary>
    public void LoadFromDump(SystemDump dump)
    {
        if (dump?.RawSysEx == null || dump.RawSysEx.Length < 10)
            return;

        _originalDump = dump;

        // Parse from raw SysEx bytes
        // Byte 4 = Device ID
        DeviceId = dump.RawSysEx[4];
        _originalDeviceId = DeviceId;
        
        // Byte 8 = MIDI Channel (0-15)
        MidiChannel = dump.RawSysEx[8];
        _originalMidiChannel = MidiChannel;
        
        // Bytes 20-21 = MIDI settings (0x01 = enabled)
        MidiClockEnabled = dump.RawSysEx.Length > 20 && dump.RawSysEx[20] == 0x01;
        _originalMidiClockEnabled = MidiClockEnabled;
        
        MidiProgramChangeEnabled = dump.RawSysEx.Length > 21 && dump.RawSysEx[21] == 0x01;
        _originalMidiProgramChangeEnabled = MidiProgramChangeEnabled;
        
        // Version info from bytes 22-23 (major.minor)
        if (dump.RawSysEx.Length > 23)
        {
            var major = dump.RawSysEx[22];
            var minor = dump.RawSysEx[23];
            Version = $"{major}.{minor}";
        }

        HasUnsavedChanges = false;
    }

    /// <summary>
    /// Reverts all changes back to the original values from the loaded dump.
    /// </summary>
    public void RevertChanges()
    {
        if (_originalDump == null)
            return;

        MidiChannel = _originalMidiChannel;
        DeviceId = _originalDeviceId;
        MidiClockEnabled = _originalMidiClockEnabled;
        MidiProgramChangeEnabled = _originalMidiProgramChangeEnabled;

        HasUnsavedChanges = false;
    }
}
