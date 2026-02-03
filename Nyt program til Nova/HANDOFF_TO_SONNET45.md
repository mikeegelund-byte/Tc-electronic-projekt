# HANDOFF_TO_SONNET45.md — Modul 9 Ready for Claude Sonnet 4.5

**Date**: 2026-02-03  
**From**: Claude Haiku 4.5  
**To**: Claude Sonnet 4.5  
**Priority**: HIGH — Next module requires advanced model capabilities

---

## 🎯 OBJECTIVE: Complete Modul 9 — MIDI Mapping Editor

### What is Modul 9?
Enable user to:
1. View and edit CC (Control Change) assignments
2. Configure expression pedal min/mid/max values
3. Create response curves for pedal sensitivity
4. Save settings to hardware

### Why Sonnet 4.5?
- **Task 9.1.4**: CC Learn Mode (complex async timeout handling)
- **Task 9.2.2**: Response Curve Editor (custom drawing, Bézier curves, interactive controls)
- These require sophisticated architectural patterns

---

## 📊 PROJECT STATUS

### ✅ COMPLETE (All Tests Green)
- **Modules 1-8**: 100% COMPLETE
- **Test Suite**: 277/277 passing ✅
- **Build**: 0 errors, 0 warnings ✅

### 🔄 PARTIAL (50%)
- **Module 7**: Tasks 7.1.1-7.1.4 DONE
  - ✅ CopyPresetUseCase (7 tests)
  - ✅ RenamePresetUseCase (8 tests)
  - ✅ DeletePresetUseCase (7 tests)
  - ✅ Context Menu UI (Copy/Rename/Delete with Ctrl+C/F2/Del)
  - ⬜ Tasks 7.2.1-7.2.4 NOT STARTED (A/B Compare, Undo/Redo)

### ⬜ NOT STARTED (Next Tasks)
- **Module 9**: MIDI Mapping (Your focus)
- **Module 10**: Release & Installer

---

## 🚀 MODUL 9 BREAKDOWN

### Phase 1: CC Mapping (Week 1)

```
9.1.1: Display CC Assignment Table      [MEDIUM]  45 min
  → DataGrid showing current CC→param mappings
  
9.1.2: Edit CC Assignments              [MEDIUM]  60 min
  → Dropdown per row to change CC assignment
  
9.1.3: Save CC Mappings                 [MEDIUM]  45 min
  → Update SystemDump with new mappings, send to hardware
  
9.1.4: CC Learn Mode (OPTIONAL)         [HIGH]    60 min ⭐ SONNET 4.5+
  → Listen for incoming CC, auto-assign to clicked parameter
  → Timeout handling: 3 second wait
```

### Phase 2: Expression Pedal (Week 2)

```
9.2.1: Display Pedal Mapping            [SIMPLE]  30 min
  → 3 NumericUpDown: Min (0), Mid (64), Max (127)
  
9.2.2: Create Response Curve Editor     [HIGH]   120 min ⭐ SONNET 4.5+
  → Custom drawing: Bézier curve visualization
  → Interactive control points on curve
  → Real-time preview
  
9.2.3: Pedal Calibration (OPTIONAL)     [MEDIUM]  45 min
  → Learn min/max from physical pedal sweep
  
9.2.4: Save Pedal Mapping               [SIMPLE]  20 min
  → Same pattern as CC save
```

---

## 🏗️ ARCHITECTURE CONTEXT

### Domain Models
```csharp
// Existing (from Modul 8)
class Preset           // 521 bytes SysEx
class SystemDump       // 527 bytes SysEx
class UserBankDump     // 60 presets × 521 = ~31KB

// SystemDump has:
// - Byte[0-63]:    CC assignments (64 CCs)
// - Byte[64-66]:   Expression pedal min/mid/max
// - Byte[67-90]:   Response curve (Bézier or linear lookup)
```

### New UseCases to Implement
```csharp
// Phase 1
IUpdateCCMappingUseCase           // Update SystemDump CC bytes
IListAvailableCCsUseCase          // Get 0-127 CC list
ICCLearnModeUseCase               // Listen + auto-assign (SONNET 4.5+)

// Phase 2
IUpdatePedalMappingUseCase        // Update min/mid/max bytes
IGenerateResponseCurveUseCase     // Compute Bézier/linear curve
IGetPedalCalibrationUseCase       // Learn sweep range (optional)
```

### New ViewModels to Implement
```csharp
// MidiMappingViewModel           // Main coordinator
//   CCListViewModel              // Binding list with CC assignments
//   PedalMappingViewModel        // Min/Mid/Max + curve editor
//   CCLearnViewModel             // Learn mode UI (SONNET 4.5+)
//   ResponseCurveViewModel       // Curve visualization (SONNET 4.5+)
```

### New Views to Implement
```xaml
<!-- MidiMappingView.axaml -->
<Grid>
  <TabControl>
    <TabItem Header="CC Assignments">
      <DataGrid ItemsSource="{Binding CCList}" />
    </TabItem>
    
    <TabItem Header="Expression Pedal">
      <StackPanel>
        <StackPanel Orientation="Horizontal">
          <Label>Min:</Label>
          <NumericUpDown Value="{Binding PedalMin}" />
          <Label>Mid:</Label>
          <NumericUpDown Value="{Binding PedalMid}" />
          <Label>Max:</Label>
          <NumericUpDown Value="{Binding PedalMax}" />
        </StackPanel>
        
        <!-- ResponseCurveView (custom drawing) -->
        <Border BorderThickness="1" BorderBrush="Black" Height="200">
          <!-- Curve visualization -->
        </Border>
      </StackPanel>
    </TabItem>
  </TabControl>
</Grid>
```

---

## 📋 TDD CHECKLIST (per AGENTS.md)

For each task:
1. ✅ Write test first (RED)
2. ✅ Implement minimal code (GREEN)
3. ✅ Refactor if needed
4. ✅ Run full suite: `dotnet build && dotnet test`
5. ✅ Commit with format: `[MODUL-9][PHASE-1] Descrition`
6. ✅ Update BUILD_STATE.md + PROGRESS.md

---

## 💾 FILES TO MODIFY

```
src/Nova.Domain/UseCases/
  ├── IUpdateCCMappingUseCase.cs          [NEW]
  ├── UpdateCCMappingUseCase.cs           [NEW]
  ├── IUpdatePedalMappingUseCase.cs       [NEW]
  ├── UpdatePedalMappingUseCase.cs        [NEW]
  ├── ICCLearnModeUseCase.cs              [NEW - SONNET 4.5+]
  └── CCLearnModeUseCase.cs               [NEW - SONNET 4.5+]

src/Nova.Application.Tests/UseCases/
  ├── UpdateCCMappingUseCaseTests.cs      [NEW]
  ├── UpdatePedalMappingUseCaseTests.cs   [NEW]
  └── CCLearnModeUseCaseTests.cs          [NEW - SONNET 4.5+]

src/Nova.Presentation/ViewModels/
  ├── MidiMappingViewModel.cs             [NEW]
  ├── CCListViewModel.cs                  [NEW]
  ├── PedalMappingViewModel.cs            [NEW]
  ├── ResponseCurveViewModel.cs           [NEW - SONNET 4.5+]
  └── CCLearnViewModel.cs                 [NEW - SONNET 4.5+]

src/Nova.Presentation/Views/
  ├── MidiMappingView.axaml               [NEW]
  ├── CCListView.axaml                    [NEW]
  ├── PedalMappingView.axaml              [NEW]
  └── ResponseCurveView.axaml             [NEW - SONNET 4.5+]

src/Nova.Presentation.Tests/ViewModels/
  ├── MidiMappingViewModelTests.cs        [NEW]
  ├── PedalMappingViewModelTests.cs       [NEW]
  └── CCLearnViewModelTests.cs            [NEW - SONNET 4.5+]
```

---

## 🧪 EXPECTED TEST COUNT

```
Before: 277 tests
After:  ~350 tests (estimated)
  - UpdateCCMapping:      8 tests
  - UpdatePedalMapping:   6 tests
  - CCLearnMode:         12 tests (SONNET 4.5+)
  - ResponseCurve:       15 tests (SONNET 4.5+)
  - ViewModels:          20 tests
  - UI Tests:             8 tests
```

---

## 🔑 KEY FILES TO READ FIRST (In Order)

1. [AGENTS.md](llm-build-system/AGENTS.md) — **MANDATORY PIPELINE**
2. [PROGRESS.md](PROGRESS.md) — Current status
3. [MIDI_PROTOCOL.md](MIDI_PROTOCOL.md) — SystemDump byte layout
4. [EFFECT_REFERENCE.md](EFFECT_REFERENCE.md) — CC param mapping
5. [tasks/14-modul9-midi-mapping-SONNET45.md](tasks/14-modul9-midi-mapping-SONNET45.md) — Detailed task spec

---

## ✅ VERIFICATION CHECKLIST BEFORE YOU START

- [ ] Read AGENTS.md COMPLETELY (206 lines)
- [ ] Read SESSION_MEMORY.md (current session context)
- [ ] Read BUILD_STATE.md (what's been built)
- [ ] Run: `cd "c:\Users\mike_\Desktop\Tc electronic projekt\Nyt program til Nova" && dotnet build --verbosity quiet`
- [ ] Run: `dotnet test --verbosity quiet` (should show ~277 passing)
- [ ] Update SESSION_MEMORY.md with your session start time
- [ ] Read entire task file: tasks/14-modul9-midi-mapping-SONNET45.md
- [ ] Ask clarifying questions if anything is unclear

---

## 🎯 SUCCESS CRITERIA

When you've completed Modul 9, verify:

1. ✅ All 277 original tests still pass (no regressions)
2. ✅ New tests added (estimate: +50-70 tests)
3. ✅ Build: 0 errors, 0 warnings
4. ✅ CC assignments can be edited and saved
5. ✅ Expression pedal curve works
6. ✅ Settings persist on hardware
7. ✅ All commits follow format: `[MODUL-9][PHASE-X] Description`
8. ✅ BUILD_STATE.md + PROGRESS.md updated after each commit

---

## 🚨 CRITICAL RULES (No Exceptions)

| Rule | Consequence |
|------|------------|
| Skip AGENTS.md pipeline | Will cause duplicated work, missed commits |
| Don't run tests before commit | Will merge broken code |
| Don't modify reference files | Will corrupt documentation |
| Don't skip task file reading | Will miss requirements |
| Create files without test first | RED → GREEN → REFACTOR always |

---

## 📞 HANDOFF SUMMARY

**What's ready for you**:
- ✅ Complete working codebase (Modules 1-8)
- ✅ 277 passing tests (regression baseline)
- ✅ All memory files updated
- ✅ MIDI protocol documented
- ✅ Build system verified
- ✅ Dependency injection configured
- ✅ MVVM patterns established

**What you need to build**:
- CC mapping display, edit, learn mode
- Expression pedal configuration + curve editor
- Integration with existing MainViewModel
- 50-70 new tests

**Estimated duration**: 2 weeks (following task spec)

---

**Next step**: Claude Sonnet 4.5 takes over.  
**Location**: `c:\Users\mike_\Desktop\Tc electronic projekt\Nyt program til Nova\`  
**Command to start**: Read AGENTS.md → Read PROGRESS.md → Begin Task 9.1.1

🚀 **Go forth and build!**
