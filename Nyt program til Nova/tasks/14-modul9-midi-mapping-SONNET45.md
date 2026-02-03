# Task List: Modul 9 — MIDI Mapping Editor

## 📋 Module: 9 (MIDI Mapping)
**Duration**: 2 weeks  
**Prerequisite**: Modul 8 complete  
**Output**: Edit CC assignments and expression pedal mapping  

---

## Exit Criteria

- [x] Display current CC assignments
- [x] Edit CC → parameter mappings
- [ ] Expression pedal min/mid/max editor (Domain done, UI pending)
- [ ] Visual response curve
- [x] Save to hardware (via SaveSystemDumpUseCase)
- [x] All tests pass (308 passing)

---

## Phase 1: CC Mapping (1 uge)

### Task 9.1.1: Display CC Assignment Table

**🟡 COMPLEXITY: MEDIUM** — DataGrid med mapping data

**Status**: ✅ DONE  
**Estimated**: 45 min  
**Actual**: ~60 min  
**Commit**: `6ef7524` - GetCCMappingsUseCase, CCMappingViewModel, MidiMappingView  

---

### Task 9.1.2: Edit CC Assignments

**🟡 COMPLEXITY: MEDIUM** — Dropdown per row

**Status**: ✅ DONE  
**Estimated**: 60 min  
**Actual**: ~45 min  
**Commit**: `127606d` - UpdateCCMappingUseCase with validation  

---

### Task 9.1.3: Save CC Mappings

**🟡 COMPLEXITY: MEDIUM** — Update SystemDump + save

**Status**: ✅ DONE  
**Estimated**: 45 min  
**Actual**: Included in 9.1.2 (same commit)  
**Commit**: `127606d` - Editable DataGrid with Save command and dirty tracking  

---

### Task 9.1.4: CC Learn Mode (OPTIONAL)

**🔴 COMPLEXITY: HIGH** — Kræver SONNET 4.5+

**Årsag**: Listen for CC → auto-assign, timeout handling

**Status**: Not started  
**Estimated**: 60 min  

---

## Phase 2: Expression Pedal (1 uge)

### Task 9.2.1: Display Pedal Mapping (Min/Mid/Max)

**🟢 COMPLEXITY: SIMPLE** — 3 NumericUpDown controls

**Status**: 🔄 IN PROGRESS (Domain done, ViewModel+UI remaining)  
**Estimated**: 30 min  
**Commit**: `7696466` - SystemDump pedal getter methods + 5 tests  

---

### Task 9.2.2: Create Response Curve Editor

**🔴 COMPLEXITY: HIGH** — Kræver SONNET 4.5+

**Årsag**: Custom drawing, Bézier curve, interactive control points

**Status**: Not started  
**Estimated**: 120 min  

---

### Task 9.2.3: Pedal Calibration (OPTIONAL)

**🟡 COMPLEXITY: MEDIUM** — Learn min/max from sweep

**Status**: Not started  
**Estimated**: 45 min  

---

### Task 9.2.4: Save Pedal Mapping

**🟢 COMPLEXITY: SIMPLE** — Same as CC save

**Status**: Not started  
**Estimated**: 20 min  

---

## Completion Checklist

- [ ] CC mappings can be edited
- [ ] Expression pedal curve works
- [ ] Settings persist on hardware
- [ ] Commit: `[MODUL-9] Implement MIDI Mapping Editor`

---

**Status**: READY (after Modul 8)
