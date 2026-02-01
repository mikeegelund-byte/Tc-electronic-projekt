# Task Index — All Phases & Modules

Status: læst

## 📋 Complete Roadmap

```
PHASE 0: Environment Setup
├── Task 01 (1-2 hours)
└── Output: NovaApp.sln ready

MODUL 1: Connection + Bank Dump (MVP)
├── Phase 1: MIDI Layer Foundation (1 week)
│   ├── Task 02 (5 tasks)
│   └── Output: IMidiPort + MockMidiPort
├── Phase 2: Domain Models (1 week)
│   ├── Task 03 (5 tasks)
│   └── Output: Preset + UserBankDump classes
├── Phase 3: Application Layer (1 week)
│   ├── Task 04 (3 tasks)
│   └── Output: ConnectUseCase + DownloadBankUseCase
└── [Total: 3 weeks]

MODUL 2: Parameter Editing (3 weeks)
├── Phase 1: UI Foundation
├── Phase 2: Parameter Binding
└── Phase 3: Real-time Updates

... (MODUL 3-10 similar structure)
```

---

## Task Files (in execution order)

### ✅ Completed

**Phase 0: Environment**
- [01-phase0-environment-setup.md](01-phase0-environment-setup.md)
  - 17 subtasks
  - Status: ✅ COMPLETE (2026-01-31)

**Modul 1, Phase 2: Domain Models**
- [03-modul1-phase2-domain-models.md](03-modul1-phase2-domain-models.md)
  - 5 subtasks
  - Status: ✅ COMPLETE (2026-02-01)
  - Results: Preset entity (521 bytes), UserBankDump (60 presets), SystemDump (527 bytes), 100% test pass

### 🟡 In Progress

**Modul 1, Phase 3: Application Layer**
- [04-modul1-phase3-use-cases.md](04-modul1-phase3-use-cases.md)
  - 3 subtasks
  - Status: ✅ COMPLETE (2026-02-01)
  - Results: ConnectUseCase, DownloadBankUseCase, 90% coverage

### 🟡 In Progress

**Modul 2, Phase 1: UI Foundation**
- (Task file pending)
- Status: ⏳ READY TO START

**Modul 1, Phase 2: Domain Models**
- [03-modul1-phase2-domain-models.md](03-modul1-phase2-domain-models.md)
  - Domain layer (Preset, UserBankDump, SystemDump)
  - Status: 🟡 95% COMPLETE
  - Results: 108 domain tests, 55 parameters extracted
  - Remaining: EQ/Gate parameters

### ⏳ Ready to Start

**Modul 1, Phase 3: Use Cases**
- [04-modul1-phase3-use-cases.md](04-modul1-phase3-use-cases.md)
  - Connect, Download, Upload use cases
  - Status: ⏳ PENDING (ready after Phase 2)
  - Estimated: 1 week

---

## How to Use This System

### For Each Task File

1. **Read completely** before starting
2. **Do tasks in order** (listed 1-N)
3. **Follow TEST FIRST always**:
   - Write test that fails (RED)
   - Write minimal code (GREEN)
   - Clean up code (REFACTOR)
   - Commit with [RED→GREEN→REFACTOR]

4. **Don't skip**:
   - Tests ❌
   - Verification ❌
   - Commits ❌
   - Coverage checks ❌

5. **Mark complete** in this file

---

## Progress Tracking

### Phase 0: Environment Setup
- **Status**: ✅ COMPLETE
- **Tasks**: 17
- **Completed**: 2026-01-31
- **Progress**: 100%

```
✅ 1.1 Install .NET 8 SDK (8.0.417)
✅ 1.2 Install Visual Studio
✅ 1.3 Install/verify Git (2.52.0)
✅ 1.4 Create project directory
✅ 1.5 Create solution file (NovaApp.sln)
✅ 1.6 Create 11 projects (6 production + 5 test)
✅ 1.7 Add projects to solution
✅ 1.8 Set up dependencies (FluentResults, Avalonia, DryWetMIDI)
✅ 1.9 Install NuGet packages
✅ 1.10 Create .gitignore
✅ 1.11 Initialize Git repo
✅ 1.12 Verify build (0 warnings, 0 errors)
✅ 1.13 Open in Visual Studio
✅ 1.14 Create README.md
✅ 1.15 Create CONTRIBUTING.md
✅ 1.16 Create baseline tests (4 projects)
✅ 1.17 Final verification
```

### Modul 1, Phase 1: MIDI Foundation
- **Status**: ✅ COMPLETE
- **Tasks**: 5
- **Completed**: 2026-01-31
- **Progress**: 100%

```
✅ 2.1 Create IMidiPort interface (FluentResults, async/await)
✅ 2.2 Create MockMidiPort (test double for unit testing)
✅ 2.3 Create SysExBuilder (BuildBankDumpRequest: F0 00 20 1F 00 63 45 03 F7)
✅ 2.4 Create SysExValidator (7-bit LSB checksum validation)
✅ 2.5 Test coverage verification (13 MIDI tests passing)
✅ BONUS: Nova.HardwareTest console app (60 presets + system dump captured)
```

### Modul 1, Phase 2: Domain Models
- **Status**: ✅ COMPLETE (parameter extraction 100%)
- **Tasks**: 5 + parameter extraction
- **Started**: 2026-01-31
- **Completed**: 2026-01-31
- **Progress**: 100% (extraction complete, validation pending)

```
✅ 3.1 Create Preset class (FromSysEx, ToSysEx, 521 bytes)
✅ 3.2 Create PresetParser (4-byte little-endian decoding)
✅ 3.3 Create UserBankDump class (60 presets, immutable collection)
✅ 3.4 Create roundtrip tests (parse → serialize → parse)
✅ 3.5 Test coverage verification (117 domain tests passing)

🎉 Parameter Extraction Status: 78 parameters COMPLETE (ALL 9 effect blocks):
✅ Global: TapTempo, Routing, LevelOutLeft, LevelOutRight (4)
✅ Enable flags: Comp, Drive, Mod, Delay, Reverb (5)
✅ COMP: Type, Threshold, Ratio, Attack, Release, Response, Drive, Level (8) [commit b8a1f59]
✅ DRIVE: Type, Gain, Level (3) [commit bc54946]
✅ BOOST: Type, Gain, Level (3) [commit 6b95144]
✅ MOD: Type, Speed, Depth, Tempo, HiCut, Feedback, DelayOrRange, Mix (8) [commit 40e0e39]
✅ DELAY: Type, Time, Time2, Tempo, Tempo2OrWidth, Feedback, ClipOrFeedback2, HiCut, LoCut, Mix (10) [commit c553118]
✅ REVERB: Type, Decay, PreDelay, Shape, Size, HiColor, HiLevel, LoColor, LoLevel, RoomLevel, Level, Diffuse, Mix (13) [commit 7bb5a38]
✅ EQ/GATE: GateType, GateThreshold, GateDamp, GateRelease, EqFreq1-3, EqGain1-3, EqWidth1-3 (13) [commit 3a5bb9e]
✅ PITCH: Type, Voice1-2, Pan1-2, Delay1-2, Feedback1OrKey, Feedback2OrScale, Level1-2 (11) [commit 8e6c2cf]

⚠️ Technical Debt: ~25 signed dB parameters store raw encoded values (offset decoding layer pending)
⏳ Next: Parameter validation logic + Phase 3 Use Cases
```

### Modul 1, Phase 3: Use Cases
- **Status**: ⏳ PENDING (ready to start after Phase 2)
- **Tasks**: 3
- **Estimated**: 1 week
- **Progress**: 0%

```
□ 4.1 Create ConnectUseCase
□ 4.2 Create DownloadBankUseCase
□ 4.3 Create UploadBankUseCase
```

---

## Key Principles (MANDATORY)

### ✅ DO

- Test FIRST (RED state)
- Write MINIMAL code (GREEN state)
- CLEAN UP (REFACTOR state)
- COMMIT often
- VERIFY every step
- READ the whole task file
- ASK questions if unclear

### ❌ DON'T

- Skip tests (ever)
- Commit without green build
- Write code without test
- Change existing tests lightly
- Delete code without documenting
- Merge multiple tasks into one commit
- Work on multiple tasks simultaneously

---

## Memory System Integration

Before starting each task:
1. Read: `llm-build-system/memory/SESSION_MEMORY.md`
2. Update with current task
3. Note goal + strategy

After each task:
1. Update: `llm-build-system/memory/BUILD_STATE.md` (what's done)
2. Update: `llm-build-system/memory/PITFALLS_FOUND.md` (if issues)
3. Commit with clear message

---

## LLM Build Instructions

**CRITICAL**: Always follow `llm-build-system/LLM_BUILD_INSTRUCTIONS.md`

**Golden Rule**: NO CODE WITHOUT TESTS

**Per-Commit Checklist**:
- [ ] Tests written FIRST (RED)
- [ ] Minimal code (GREEN)
- [ ] Tests passing
- [ ] Build 0 warnings
- [ ] Commit with [RED→GREEN→REFACTOR] message
- [ ] `dotnet build && dotnet test` before next task

---

## Cleanup Policy

When deleting/refactoring, follow: `llm-build-system/CLEANUP_POLICY.md`

**Simple rule**: If unsure, don't delete. Mark [Obsolete] first.

---

## Quick Navigation

| File | Purpose |
|------|---------|
| [01-phase0-environment-setup.md](01-phase0-environment-setup.md) | Initial setup (start here) |
| [02-modul1-phase1-foundation.md](02-modul1-phase1-foundation.md) | MIDI layer (after Phase 0) |
| [03-modul1-phase2-domain-models.md](03-modul1-phase2-domain-models.md) | Preset parsing (after Phase 1) |
| [04-modul1-phase3-use-cases.md](04-modul1-phase3-use-cases.md) | Use cases (after Phase 2) |

---

## Estimated Timeline

```
Phase 0 (Setup):        ✅ COMPLETE (2026-01-31)
Modul 1 Phase 1:        ✅ COMPLETE (2026-01-31)
Modul 1 Phase 2:        ✅ COMPLETE (2026-01-31) - Parameter extraction 100%!
Modul 1 Phase 3:        ⏳ Ready to start (~1 week)
────────────────────────────────────────────────
Modul 1 Total:          ~85% complete toward MVP!
```

---

## Status Summary

```
📊 OVERALL PROGRESS (Updated: 2026-01-31 21:52)

Phase 0 (Env):     ✅ COMPLETE (100%)
Modul 1-Phase 1:   ✅ COMPLETE (100%) - MIDI Foundation
Modul 1-Phase 2:   ✅ COMPLETE (100%) - Domain Models + Parameter Extraction 🎉
Modul 1-Phase 3:   ⏳ READY TO START (0%) - Use Cases
Modul 2-10:        📋 PLANNED

📈 TEST STATUS: 117 tests passing (108 Domain + 6 Midi + 3 Baseline)
🔧 BUILD STATUS: 0 warnings, 0 errors
📦 PARAMETERS: 78 of 78 extracted from preset SysEx (100% COMPLETE!) 🎆

🎯 NEXT STEP: Parameter validation logic OR start Phase 3 Use Cases
```

---

## Related Documentation

**LLM Build System**:
- [LLM_BUILD_INSTRUCTIONS.md](../llm-build-system/LLM_BUILD_INSTRUCTIONS.md)
- [CLEANUP_POLICY.md](../llm-build-system/CLEANUP_POLICY.md)
- [memory/SESSION_MEMORY.md](../llm-build-system/memory/SESSION_MEMORY.md)

**Project Documentation**:
- [../docs/00-index.md](../docs/00-index.md) — Main docs index
- [../docs/11-modul1-technical-detail.md](../docs/11-modul1-technical-detail.md) — MVP design
- [../docs/04-testing-strategy.md](../docs/04-testing-strategy.md) — Test rules

---

## Questions?

1. **"Which task should I do?"**
   → Phase 2 parameter extraction: ✅ COMPLETE!
   → Next: [04-modul1-phase3-use-cases.md](04-modul1-phase3-use-cases.md)
   → Optional: Add parameter validation logic in Phase 2

2. **"What if I get stuck?"**
   → Check `llm-build-system/CLEANUP_POLICY.md` or `LLM_BUILD_INSTRUCTIONS.md`

3. **"How do I know I'm done?"**
   → See ✅ section in each task file

4. **"Can I skip a task?"**
   → NO - all tasks are required

5. **"What if tests fail?"**
   → See "If Tests Fail" in `LLM_BUILD_INSTRUCTIONS.md`

---

**Status**: ✅ MODUL 1, PHASE 2 COMPLETE! 🎉 → Ready for Phase 3

*Last updated: 2026-01-31 21:52*
*Next task: Start Phase 3 Use Cases OR add parameter validation logic*
