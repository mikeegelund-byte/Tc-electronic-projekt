using CommunityToolkit.Mvvm.ComponentModel;
using Nova.Domain.Models;

namespace Nova.Presentation.ViewModels;

/// <summary>
/// ViewModel for displaying and editing global system settings from SystemDump.
/// </summary>
public partial class SystemSettingsViewModel : ObservableObject
{
    private SystemDump? _originalDump;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasUnsavedChanges))]
    private int _midiChannel;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasUnsavedChanges))]
    private int _deviceId;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasUnsavedChanges))]
    private bool _midiClockEnabled;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasUnsavedChanges))]
    private bool _midiProgramChangeEnabled;

    [ObservableProperty] private string _version = string.Empty;

    public bool HasUnsavedChanges => _originalDump != null && HasChangesFromOriginal();

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

    public void RevertChanges()
    {
        if (_originalDump != null)
        {
            LoadFromDump(_originalDump);
        }
    }

    private bool HasChangesFromOriginal()
    {
        if (_originalDump?.RawSysEx == null || _originalDump.RawSysEx.Length < 22)
            return false;

        return MidiChannel != _originalDump.RawSysEx[8]
               || DeviceId != _originalDump.RawSysEx[4]
               || MidiClockEnabled != (_originalDump.RawSysEx[20] == 0x01)
               || MidiProgramChangeEnabled != (_originalDump.RawSysEx[21] == 0x01);
    }
}
