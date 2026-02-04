# Task List: Modul 8 — File I/O (.syx Files)

## 📋 Module: 8 (File I/O)
**Duration**: 1 week  
**Prerequisite**: Modul 7 complete  
**Output**: Import/export .syx files compatible with NovaManager  
**********Noget af dette modul er allerede lavet, men ikke dokumenteret i opgavelisten.**********
---

## Exit Criteria

- [ ] Export single preset to .syx
- [ ] Export user bank to .syx
- [ ] Export system dump to .syx
- [ ] Import .syx (auto-detect type)
- [ ] Compatibility med original NovaManager
- [ ] All tests pass

---

## Phase 1: Export (3 dage)

### Task 8.1.1: Export Single Preset

**🟢 COMPLEXITY: SIMPLE** — SaveFileDialog + File.WriteAllBytes

**Status**: ✅ COMPLETE  
**Estimated**: 30 min  
**Actual**: Included in 8.1 batch  
**Commit**: `5c91fe5` - Export Preset/Bank/System use cases  

```csharp
public async Task ExportPresetAsync(Preset preset, string filePath)
{
    var sysex = preset.ToSysEx();
    await File.WriteAllBytesAsync(filePath, sysex);
}
```

---

### Task 8.1.2: Export User Bank

**🟢 COMPLEXITY: SIMPLE** — Concatenate 60 presets

**Status**: ✅ COMPLETE  
**Estimated**: 30 min  
**Actual**: Included in 8.1 batch  
**Commit**: `5c91fe5` - ExportBankUseCase with concatenation  

---

### Task 8.1.3: Export System Dump

**🟢 COMPLEXITY: SIMPLE** — Same pattern

**Status**: ✅ COMPLETE  
**Estimated**: 20 min  
**Actual**: Included in 8.1 batch  
**Commit**: `5c91fe5` - ExportSystemDumpUseCase  

---

## Phase 2: Import (4 dage)

### Task 8.2.1: Auto-Detect .syx Type

**🟡 COMPLEXITY: MEDIUM** — Parse header to determine type

**Status**: ✅ COMPLETE  
**Estimated**: 45 min  
**Actual**: ~40 min  
**Commit**: `e3c42b0` - SysEx type detection logic  

```csharp
public SysExType DetectType(byte[] data)
{
    if (data.Length == 521 && data[7] == 0x01) return SysExType.Preset;
    if (data.Length == 527 && data[7] == 0x02) return SysExType.SystemDump;
    if (data.Length > 31000) return SysExType.UserBank;
    return SysExType.Unknown;
}
```

---

### Task 8.2.2: Import Preset (Choose Slot)

**🟡 COMPLEXITY: MEDIUM** — Dialog for destination

**Status**: ✅ COMPLETE  
**Estimated**: 45 min  
**Actual**: Combined with 8.2.3  
**Commit**: `107145e` - Import SysEx use case

---

### Task 8.2.3: Import Bank (Confirm Overwrite)

**🟡 COMPLEXITY: MEDIUM** — Warning dialog

**Status**: ✅ COMPLETE  
**Estimated**: 30 min  
**Actual**: Combined with 8.2.2  
**Commit**: `107145e` - Import with overwrite confirmation  

---

### Task 8.2.4: Validate NovaManager Compatibility

**🟢 COMPLEXITY: SIMPLE** — Cross-test med original app

**Status**: ✅ COMPLETE  
**Estimated**: 30 min  
**Actual**: ~20 min  
**Commit**: `533dfc3` - UI integration verified compatibility  

### Test Procedure:
1. Export preset fra vores app
2. Åbn i original NovaManager
3. Verificer alle parametre matcher
4. Export fra NovaManager
5. Import i vores app
6. Verificer roundtrip

---

## Completion Checklist

- [x] Export virker for alle 3 typer
- [x] Import virker med auto-detect
- [x] NovaManager compatibility verified
- [x] Commit: `[MODUL-8] Implement File I/O`

---

**Status**: ✅ COMPLETE — All export/import functionality done  
**Commits**: 5c91fe5 (Export), e3c42b0 (Auto-detect), 107145e (Import), 533dfc3 (UI integration)
