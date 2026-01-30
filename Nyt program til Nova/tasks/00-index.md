# Task Index — All Phases & Modules

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

### ✅ Ready Now

**Phase 0: Environment**
- [01-phase0-environment-setup.md](01-phase0-environment-setup.md)
  - 17 subtasks, ~2 hours total
  - Status: READY TO START

### ⏳ Ready After Phase 0

**Modul 1, Phase 1**
- [02-modul1-phase1-foundation.md](02-modul1-phase1-foundation.md)
  - 5 subtasks (MIDI interface + mock)
  - Status: READY (once Phase 0 done)
  - Estimated: 1 week

**Modul 1, Phase 2**
- [03-modul1-phase2-domain-models.md](03-modul1-phase2-domain-models.md)
   - Domain layer (Preset, UserBankDump)
   - Status: READY (once Phase 0 done)
   - Estimated: 1 week

**Modul 1, Phase 3**
- [04-modul1-phase3-use-cases.md](04-modul1-phase3-use-cases.md)
   - Connect, Download, Upload use cases
   - Status: READY (once Phase 0 done)
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
- **Status**: ⏳ READY TO START
- **Tasks**: 17
- **Estimated**: 1-2 hours
- **Progress**: 0%

```
□ 1.1 Install .NET 8 SDK
□ 1.2 Install Visual Studio
□ 1.3 Install/verify Git
□ 1.4 Create project directory
□ 1.5 Create solution file
□ 1.6 Create 6 projects
□ 1.7 Add projects to solution
□ 1.8 Set up dependencies
□ 1.9 Install NuGet packages
□ 1.10 Create .gitignore
□ 1.11 Initialize Git repo
□ 1.12 Verify build
□ 1.13 Open in Visual Studio
□ 1.14 Create README.md
□ 1.15 Create CONTRIBUTING.md
□ 1.16 Create dummy test
□ 1.17 Final verification
```

### Modul 1, Phase 1: MIDI Foundation
- **Status**: ⏳ PENDING PHASE 0
- **Tasks**: 5
- **Estimated**: 1 week
- **Progress**: 0%

```
□ 2.1 Create IMidiPort interface
□ 2.2 Create MockMidiPort
□ 2.3 Create SysExBuilder
□ 2.4 Create SysExValidator
□ 2.5 Test coverage verification
```

### Modul 1, Phase 2: Domain Models
- **Status**: ⏳ IN PREPARATION
- **Tasks**: 5
- **Estimated**: 1 week
- **Progress**: 0%

```
□ 3.1 Create Preset class
□ 3.2 Create PresetParser
□ 3.3 Create UserBankDump class
□ 3.4 Create roundtrip tests
□ 3.5 Test coverage verification
```

### Modul 1, Phase 3: Use Cases
- **Status**: ⏳ IN PREPARATION
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
Phase 0 (Setup):        1-2 hours
Modul 1 Phase 1:        1 week
Modul 1 Phase 2:        1 week
Modul 1 Phase 3:        1 week
────────────────
Modul 1 Total:          3 weeks (MVP complete!)
```

---

## Status Summary

```
📊 OVERALL PROGRESS

Phase 0 (Env):     ⏳ READY (0%)
Modul 1-Phase 1:   ⏳ PENDING (0%)
Modul 1-Phase 2:   ⏳ IN PREP (0%)
Modul 1-Phase 3:   ⏳ IN PREP (0%)
Modul 2-10:        📋 PLANNED

🎯 NEXT STEP: Start Phase 0 (tasks/01-phase0-environment-setup.md)
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
   → Start with [01-phase0-environment-setup.md](01-phase0-environment-setup.md)

2. **"What if I get stuck?"**
   → Check `llm-build-system/CLEANUP_POLICY.md` or `LLM_BUILD_INSTRUCTIONS.md`

3. **"How do I know I'm done?"**
   → See ✅ section in each task file

4. **"Can I skip a task?"**
   → NO - all tasks are required

5. **"What if tests fail?"**
   → See "If Tests Fail" in `LLM_BUILD_INSTRUCTIONS.md`

---

**Status**: 🟢 READY FOR PHASE 0 STARTUP

*Last updated: [Project initiation]*  
*Next task: Phase 0 environment setup (1-2 hours)*
