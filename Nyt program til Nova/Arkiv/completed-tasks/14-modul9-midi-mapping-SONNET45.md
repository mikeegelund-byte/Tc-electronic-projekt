# Task List: Modul 9 — MIDI Mapping Editor

## 📋 Module: 9 (MIDI Mapping)
**Duration**: 2 weeks  
**Prerequisite**: Modul 8 complete  
**Output**: Edit CC assignments and expression pedal mapping  

---

## Exit Criteria

- [x] Display current CC assignments
- [x] Edit CC → parameter mappings
- [x] CC Learn Mode
- [x] Expression pedal min/mid/max editor (Domain + UI done)
- [x] Visual response curve (Bézier curve editor with interactive drag)
- [x] Pedal calibration (learn min/max from pedal sweep)
- [x] Save pedal mapping (UpdatePedalMappingUseCase)
- [x] Save to hardware (via SaveSystemDumpUseCase)
- [x] All tests pass (342 passing: 160 Domain, 6 Midi, 12 Infrastructure, 88 Application, 76 Presentation)

---

## Phase 1: CC Mapping (1 uge)

### Task 9.1.1: Display CC Assignment Table

**🟡 COMPLEXITY: MEDIUM** — DataGrid med mapping data

**Status**: ✅ COMPLETE  
**Estimated**: 45 min  
**Actual**: ~60 min  
**Commit**: `6ef7524` - GetCCMappingsUseCase, CCMappingViewModel, MidiMappingView  

---

### Task 9.1.2: Edit CC Assignments

**🟡 COMPLEXITY: MEDIUM** — Dropdown per row

**Status**: ✅ COMPLETE  
**Estimated**: 60 min  
**Actual**: ~45 min  
**Commit**: `127606d` - UpdateCCMappingUseCase with validation  

---

### Task 9.1.3: Save CC Mappings

**🟡 COMPLEXITY: MEDIUM** — Update SystemDump + save

**Status**: ✅ COMPLETE  
**Estimated**: 45 min  
**Actual**: Included in 9.1.2 (same commit)  
**Commit**: `127606d` - Editable DataGrid with Save command and dirty tracking  

---

### Task 9.1.4: CC Learn Mode

**🔴 COMPLEXITY: HIGH** — Kræver SONNET 4.5+

**Årsag**: Listen for CC → auto-assign, timeout handling

**Status**: ✅ COMPLETE  
**Estimated**: 60 min  
**Actual**: ~90 min  
**Commit**: `6ff9152` - CCLearnModeUseCase + ReceiveCCAsync in IMidiPort + 6 tests  

---

## Phase 2: Expression Pedal (1 uge)

### Task 9.2.1: Display Pedal Mapping (Min/Mid/Max)

**🟢 COMPLEXITY: SIMPLE** — 4 NumericUpDown controls

**Status**: ✅ COMPLETE  
**Estimated**: 30 min  
**Actual**: ~60 min (Domain + UI)  
**Commits**:  
- `7696466` - SystemDump pedal getter methods (GetPedalParameter/Min/Mid/Max) + 5 tests  
- `a7d1ada` - PedalMappingViewModel + PedalMappingView.axaml with 4 NumericUpDown controls + 5 tests  

---

### Task 9.2.2: Create Response Curve Editor

**🔴 COMPLEXITY: HIGH** — Kræver SONNET 4.5+

**Årsag**: Custom drawing, Bézier curve, interactive control points

**Status**: ✅ COMPLETE  
**Estimated**: 120 min  
**Actual**: ~90 min  
**Commit**: `e8fb7f7` - ResponseCurveEditor custom control (400x300px Canvas) med:
- Cubic Bézier curve implementation (P0, P1, P2, P3)
- Interactive drag functionality (P1 og P2 draggable)
- Grid rendering (10% increments)
- Curve evaluation metode (0-1 normalized)
- GetControlPoints/SetControlPoints serialization
- 7 unit tests (EvaluateCurve, GetControlPoints, SetControlPoints, monotonicity)

---

### Task 9.2.3: Pedal Calibration

**🟡 COMPLEXITY: MEDIUM** — Learn min/max from sweep

**Status**: ✅ COMPLETE  
**Estimated**: 45 min  
**Actual**: ~45 min  
**Commit**: `c7d0eed` - CalibrateExpressionPedalUseCase med:
- CalibrateAsync metode med timeout parameter (e.g., 5 sekunder)
- Lytter til CC messages via IMidiPort.ReceiveCCAsync
- Tracker min/max værdier over tidsvindue
- Returns (min, max) tuple
- 5 unit tests (multiple CC, full range, not connected, zero timeout, no CC)

---

### Task 9.2.4: Save Pedal Mapping

**🟡 COMPLEXITY: MEDIUM** — Update SystemDump + save

**Status**: ✅ COMPLETE  
**Estimated**: 20 min  
**Actual**: ~30 min  
**Commit**: `228b168` - UpdatePedalMappingUseCase + SystemDump setters med:
- UpdateAsync metode (parameter, min, mid, max)
- UpdatePedalParameter/Min/Mid/Max metoder i SystemDump
- Kalder ISaveSystemDumpUseCase.ExecuteAsync
- Validering af ranges (parameter 0-127, min/mid/max 0-100)
- 4 unit tests (valid values, invalid parameter, invalid min, save fails)
- +7 Domain tests for pedal setters

---

## 🎉 MODULE 9 STATUS: 100% COMPLETE 🎉

**Final Test Count**: 342 tests passing (100%)
- Domain: 160 tests (+7 pedal setter tests)
- Midi: 6 tests
- Infrastructure: 12 tests
- Application: 88 tests (+9 CC mapping/learn tests, +5 calibration tests, +4 pedal update tests)
- Presentation: 76 tests (+5 pedal mapping tests, +7 response curve tests)

**Build Status**: GREEN (0 warnings, 0 errors)

**All Tasks Complete**:
✅ Phase 1: CC Mapping (Tasks 9.1.1-9.1.4)
✅ Phase 2: Expression Pedal (Tasks 9.2.1-9.2.4)

**Next Module**: Modul 10 - Release & Installer (0% complete)

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
