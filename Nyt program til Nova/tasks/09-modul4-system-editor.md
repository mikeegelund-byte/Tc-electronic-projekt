# Task List: Modul 4 — System Settings Editor

## 📋 Module: 4 (System Editor)
**Duration**: 2 weeks  
**Prerequisite**: Modul 3 complete  
**Output**: Editable system settings with save to pedal  

---

## Overview

**Goal**: Allow user to modify global settings and save them back to the pedal.

---

## Exit Criteria

- [x] All settings editable via UI controls
- [x] Dirty tracking shows unsaved changes
- [x] Save sends valid SysEx to pedal
- [x] Roundtrip verification (save → re-read → compare)
- [x] All tests pass

---

## Task 4.1: Make Controls Editable

**🟡 COMPLEXITY: MEDIUM** — Two-way binding

**Status**: ✅ COMPLETE  
**Estimated**: 45 min  

### Convert read-only TextBlocks to:
- ComboBox for MIDI Channel (1-16)
- NumericUpDown for Device ID (0-126)
- ToggleSwitch for boolean settings

---

## Task 4.2: Implement Dirty Tracking

**🟡 COMPLEXITY: MEDIUM** — State comparison

**Status**: ✅ COMPLETE  
**Estimated**: 30 min  

### Add to SystemSettingsViewModel:
```csharp
[ObservableProperty] 
private bool _hasUnsavedChanges;

private SystemDump? _originalDump;

partial void OnMidiChannelChanged(int value)
{
    HasUnsavedChanges = _originalDump != null && value != _originalDump.MidiChannel;
}
// ... repeat for all properties
```

---

## Task 4.3: Create SaveSystemDumpUseCase

**🔴 COMPLEXITY: HIGH** — Kræver SONNET 4.5+

**Årsag**: SysEx serialization med korrekt checksum, async flow, error handling

**Status**: ✅ COMPLETE  
**Estimated**: 60 min  
**Files**:
- `src/Nova.Application/UseCases/SaveSystemDumpUseCase.cs`

---

## Task 4.4: Implement Roundtrip Verification

**🔴 COMPLEXITY: HIGH** — Kræver SONNET 4.5+

**Årsag**: Kompleks async flow: Save → Wait → Request → Compare → Report

**Status**: ✅ COMPLETE  
**Estimated**: 45 min  

---

## Task 4.5: Add Save/Cancel Buttons

**🟢 COMPLEXITY: SIMPLE** — XAML + commands

**Status**: ✅ COMPLETE  
**Estimated**: 20 min  

---

## Completion Checklist

- [x] All tests pass
- [x] Settings persist on hardware
- [x] Roundtrip verification works
- [x] Commit: `[MODUL-4][TASK-4.5] Wire Save/Cancel commands to System Settings UI`

---

**Status**: ✅ COMPLETE (All tasks done, 244 tests passing)
