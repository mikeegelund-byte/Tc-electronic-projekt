# Task List: Modul 3 — System Dump Viewer

## 📋 Module: 3 (System Dump Viewer)
**Duration**: 1 week  
**Prerequisite**: Modul 2 complete  
**Output**: Read-only display of global system settings  

---

## Overview

**Goal**: Download and display the Nova System's global settings (MIDI channel, Program Map, CC assignments).

---

## Exit Criteria

- [ ] Can request system dump from pedal
- [ ] All global settings parsed correctly
- [ ] Settings displayed in read-only UI
- [ ] All tests pass

---

## Task 3.1: Extend SysExBuilder for System Dump Request

**🟢 COMPLEXITY: SIMPLE** — Følger eksisterende pattern

**Status**: Not started  
**Estimated**: 20 min  
**Files**:
- `src/Nova.Domain/Midi/SysExBuilder.cs`
- `src/Nova.Domain.Tests/Midi/SysExBuilderTests.cs`

### Test First (RED)
```csharp
[Fact]
public void BuildSystemDumpRequest_ReturnsCorrectBytes()
{
    var request = SysExBuilder.BuildSystemDumpRequest();
    
    request.Should().StartWith(new byte[] { 0xF0, 0x00, 0x20, 0x1F });
    request[6].Should().Be(0x45); // Request message
    request[7].Should().Be(0x02); // System dump type
    request[^1].Should().Be(0xF7);
}
```

### Code (GREEN)
```csharp
public static byte[] BuildSystemDumpRequest(byte deviceId = 0x00)
{
    return new byte[]
    {
        0xF0,       // SysEx start
        0x00, 0x20, 0x1F,  // TC Electronic manufacturer ID
        deviceId,   // Device ID
        0x63,       // Nova System model ID
        0x45,       // Request message type
        0x02,       // System dump
        0xF7        // SysEx end
    };
}
```

---

## Task 3.2: Create RequestSystemDumpUseCase

**🟢 COMPLEXITY: SIMPLE** — Følger ConnectUseCase pattern

**Status**: Not started  
**Estimated**: 20 min  
**Files**:
- `src/Nova.Application/UseCases/RequestSystemDumpUseCase.cs`
- `src/Nova.Application.Tests/UseCases/RequestSystemDumpUseCaseTests.cs`

---

## Task 3.3: Create SystemSettingsViewModel

**🟡 COMPLEXITY: MEDIUM** — Mange properties

**Status**: Not started  
**Estimated**: 30 min  
**Files**:
- `src/Nova.Presentation/ViewModels/SystemSettingsViewModel.cs`

### Code
```csharp
public partial class SystemSettingsViewModel : ObservableObject
{
    [ObservableProperty] private int _midiChannel;
    [ObservableProperty] private int _deviceId;
    [ObservableProperty] private bool _midiClockEnabled;
    // ... add all system properties from SystemDump

    public void LoadFromDump(SystemDump dump)
    {
        MidiChannel = dump.MidiChannel;
        DeviceId = dump.DeviceId;
        // ... etc
    }
}
```

---

## Task 3.4: Create SystemSettingsView.axaml

**🟡 COMPLEXITY: MEDIUM** — Form layout

**Status**: Not started  
**Estimated**: 45 min  
**Files**:
- `src/Nova.Presentation/Views/SystemSettingsView.axaml`

---

## Completion Checklist

- [ ] All tests pass
- [ ] System settings display correctly
- [ ] Update task index and BUILD_STATE.md
- [ ] Commit: `[MODUL-3] Implement System Dump Viewer`

---

**Status**: READY (after Modul 2)
