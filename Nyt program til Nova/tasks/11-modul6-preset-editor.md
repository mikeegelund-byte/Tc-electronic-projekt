# Task List: Modul 6 — Preset Editor (LARGEST MODULE)

## 📋 Module: 6 (Preset Editor)
**Duration**: 4 weeks  
**Prerequisite**: Modul 5 complete  
**Output**: Full bidirectional editing of all preset parameters  

---

## ⚠️ ADVARSEL: DETTE ER DET STØRSTE MODUL

Dette modul indeholder den mest komplekse logik i hele projektet:
- 78+ parametre der skal kunne redigeres
- Live CC updates til pedal (optional)
- SysEx serialization med korrekt checksum
- Roundtrip verification

**ALLE 🔴 HIGH COMPLEXITY TASKS KRÆVER SONNET 4.5+**

---

## Exit Criteria

- [ ] Alle parametre kan redigeres via UI
- [ ] Ændringer kan gemmes til pedal
- [ ] Checksum er korrekt på alle saves
- [ ] Roundtrip verification virker
- [ ] Parameter validation (min/max)
- [ ] All tests pass (mange!)

---

## Phase 1: Parameter Controls (2 uger)

### Task 6.1.1: Create Editable Drive Controls

**🟡 COMPLEXITY: MEDIUM** — Slider bindings + value conversion

**Status**: Not started  
**Estimated**: 45 min  

```xml
<Slider Minimum="0" Maximum="100" Value="{Binding Gain}" />
<Slider Minimum="-30" Maximum="20" Value="{Binding Level}" />
```

---

### Task 6.1.2: Create Editable Compressor Controls

**🟡 COMPLEXITY: MEDIUM** — Type-dependent parameters

**Status**: Not started  
**Estimated**: 60 min  

**Udfordring**: Forskellige parametre vises baseret på CompType (0/1/2)

---

### Task 6.1.3: Create Editable EQ Controls

**🟡 COMPLEXITY: MEDIUM** — 3-band med knobs

**Status**: Not started  
**Estimated**: 60 min  

---

### Task 6.1.4: Create Editable Modulation Controls

**🟡 COMPLEXITY: MEDIUM** — 6 forskellige typer

**Status**: Not started  
**Estimated**: 90 min  

---

### Task 6.1.5: Create Editable Pitch Controls

**🔴 COMPLEXITY: HIGH** — Kræver SONNET 4.5+

**Årsag**: Intelligent pitch har Key/Scale logic, andre har Voice offsets

**Status**: Not started  
**Estimated**: 90 min  

---

### Task 6.1.6: Create Editable Delay Controls

**🟡 COMPLEXITY: MEDIUM** — 6 typer, ms/tempo sync

**Status**: Not started  
**Estimated**: 60 min  

---

### Task 6.1.7: Create Editable Reverb Controls

**🟡 COMPLEXITY: MEDIUM** — 4 typer, decay/mix

**Status**: Not started  
**Estimated**: 45 min  

---

### Task 6.1.8: Create Global Parameter Controls

**🟢 COMPLEXITY: SIMPLE** — Tap tempo, routing, levels

**Status**: Not started  
**Estimated**: 30 min  

---

## Phase 2: Live CC Updates (1 uge) — OPTIONAL

### Task 6.2.1: Implement CC Mapping Table

**🔴 COMPLEXITY: HIGH** — Kræver SONNET 4.5+

**Årsag**: Kræver komplet forståelse af MIDI_PROTOCOL.md CC assignments

**Status**: Not started  
**Estimated**: 60 min  

---

### Task 6.2.2: Send CC on Parameter Change

**🔴 COMPLEXITY: HIGH** — Kræver SONNET 4.5+

**Årsag**: Throttling, debouncing, async queuing

**Status**: Not started  
**Estimated**: 90 min  

---

### Task 6.2.3: Throttle Rapid Changes

**🟡 COMPLEXITY: MEDIUM** — Debounce pattern

**Status**: Not started  
**Estimated**: 30 min  

---

## Phase 3: Save & Validation (1 uge)

### Task 6.3.1: Implement SavePresetUseCase

**🔴 COMPLEXITY: HIGH** — Kræver SONNET 4.5+

**Årsag**: Komplet SysEx serialization, checksum beregning, 521-byte format

**Status**: Not started  
**Estimated**: 120 min  
**Files**:
- `src/Nova.Application/UseCases/SavePresetUseCase.cs`
- `src/Nova.Domain/Models/Preset.cs` (add ToSysEx serialization)

### Kritisk kode (ToSysEx):
```csharp
public byte[] ToSysEx()
{
    var bytes = new byte[521];
    bytes[0] = 0xF0;
    bytes[1] = 0x00; bytes[2] = 0x20; bytes[3] = 0x1F;
    bytes[4] = 0x00; // Device ID
    bytes[5] = 0x63; // Model ID
    bytes[6] = 0x20; // Dump message
    bytes[7] = 0x01; // Preset type
    bytes[8] = (byte)Number;
    
    // Name (bytes 9-32)
    var nameBytes = Encoding.ASCII.GetBytes(Name.PadRight(24).Substring(0, 24));
    Array.Copy(nameBytes, 0, bytes, 9, 24);
    
    // Parameters using 4-byte encoding
    Encode4ByteValue(bytes, 38, TapTempo);
    Encode4ByteValue(bytes, 42, Routing);
    // ... ALL 78 parameters ...
    
    // Checksum (bytes 34-517)
    int sum = 0;
    for (int i = 34; i <= 517; i++) sum += bytes[i];
    bytes[518] = (byte)(sum & 0x7F);
    
    bytes[520] = 0xF7;
    return bytes;
}
```

---

### Task 6.3.2: Create Save Dialog (Slot Selection)

**🟡 COMPLEXITY: MEDIUM** — Dialog + ComboBox

**Status**: Not started  
**Estimated**: 45 min  

---

### Task 6.3.3: Implement Parameter Validation

**🟡 COMPLEXITY: MEDIUM** — Min/max checks per parameter

**Status**: Not started  
**Estimated**: 60 min  

---

### Task 6.3.4: Implement Roundtrip Verification

**🔴 COMPLEXITY: HIGH** — Kræver SONNET 4.5+

**Årsag**: Async flow: Save → Request → Compare all 78 params

**Status**: Not started  
**Estimated**: 60 min  

---

## Test Requirements (OMFATTENDE)

### Unit Tests (minimum 50 nye tests)
- [ ] Parameter encoding for alle 78 parameters
- [ ] Checksum calculation
- [ ] Validation for hver parameter type
- [ ] ToSysEx → FromSysEx roundtrip

### Integration Tests
- [ ] Full preset save → reload → compare
- [ ] Mock MIDI roundtrip

### Manual Hardware Tests
- [ ] Edit på pedal → ændring vises i app
- [ ] Edit i app → save → verificer på pedal

---

## Completion Checklist

- [ ] Alle 78 parametre kan redigeres
- [ ] Save virker med korrekt checksum
- [ ] Roundtrip verification passed
- [ ] 50+ nye tests passing
- [ ] Commit: `[MODUL-6] Implement Preset Editor`

---

## ⏱️ Estimeret Tid Breakdown

| Phase | Tasks | Model Req | Timer |
|-------|-------|-----------|-------|
| 6.1 Controls | 8 | Mixed | ~8h |
| 6.2 Live CC | 3 | HIGH | ~3h |
| 6.3 Save | 4 | HIGH | ~5h |
| Tests | - | Mixed | ~4h |
| **Total** | 15 | - | **~20h** |

---

**Status**: READY (after Modul 5) — **LARGEST MODULE**
