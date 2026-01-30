# LLM Development Discipline System

## 🎯 Purpose

Ensure that **any LLM** building this project follows strict discipline:
- **Test-driven development** (non-negotiable)
- **Incremental commits** (traceable)
- **Code quality gates** (automated verification)
- **Clear memory** (session state tracking)

---

## 🔴 The Unskippable Test Regime

### Golden Rule

```
╔═══════════════════════════════════════════════════════════╗
║                                                           ║
║  NO CODE CHANGE WITHOUT FAILING TEST FIRST               ║
║  NO COMMIT WITHOUT PASSING TESTS                         ║
║  NO NEXT PHASE WITHOUT 100% COVERAGE GOAL MET            ║
║                                                           ║
╚═══════════════════════════════════════════════════════════╝
```

### The Cycle (Mandatory, Every Single Change)

1. **RED**: Write test that fails
   ```powershell
   dotnet test --filter "NewFeatureTest"
   # Output: FAILED ❌
   ```

2. **GREEN**: Write minimal code
   ```powershell
   dotnet test --filter "NewFeatureTest"
   # Output: PASSED ✅
   ```

3. **REFACTOR**: Clean up (tests still pass)
   ```powershell
   dotnet test
   # Output: PASSED ✅ (all tests)
   ```

4. **COMMIT**: Save with clear message
   ```bash
   git commit -m "[RED→GREEN→REFACTOR] Feature name"
   ```

---

## 📋 Pre-Session Checklist

**BEFORE you start any work:**

```
□ Read: llm-build-system/memory/SESSION_MEMORY.md
□ Read: Current task file (tasks/NN-*.md)
□ Run: dotnet build && dotnet test
□ Verify: All green at start
□ Update: SESSION_MEMORY.md with [TODAY'S GOAL]
```

---

## 🧠 Memory System (REQUIRED)

### SESSION_MEMORY.md
```markdown
# Modul: 1
# Phase: 1
# Date: [TODAY]
# Task: [From tasks/02-*.md]

## Goal
[Copy from task file]

## Do NOT
- Skip tests
- Commit broken builds
- Mix concerns in one commit

## Progress
[Will update as work proceeds]
```

### BUILD_STATE.md
```markdown
# Recent Commits
- [MODUL-1] Task 2.1 complete (IMidiPort)

# Test Status
✅ All 13 tests passing
✅ Coverage: 95% (Domain)

# What's Next
Task 2.2: MockMidiPort implementation
```

### PITFALLS_FOUND.md
```markdown
## [DATE] Test Timeout Issue
- Symptom: Test hangs after 30 sec
- Root cause: CancellationToken not respected
- Fix: Use task.Wait(timeout) instead
- Prevention: Check token in all async loops
```

---

## ✅ Commit Quality Gate

**BEFORE every commit:**

```powershell
# 1. All tests pass
dotnet test
# Output: "Passed!  - Failed:  0, Passed:  NN"

# 2. No compiler warnings
dotnet build
# Output: "Build succeeded with 0 warnings"

# 3. Format clean
dotnet format

# 4. Verify again
dotnet build && dotnet test
# Output: BOTH SUCCEED

# 5. Then commit
git commit -m "[MODUL-X] [RED→GREEN→REFACTOR] Clear description"
```

---

## 📊 Coverage Requirements (Non-Negotiable)

### Per Layer

| Layer | Minimum | How to Verify |
|-------|---------|---------------|
| Domain | 95% | `dotnet test /p:CollectCoverage=true` |
| Application | 80% | Same |
| Infrastructure | 70% | Same |
| MIDI | 90% | Same |
| UI | 50% | Later (Avalonia TestHost) |

### Check After Each Task

```powershell
dotnet test /p:CollectCoverage=true
# Check: Domain ≥ 95%
# If below: Write more tests
```

---

## 🚫 Forbidden Actions

You **CANNOT** do these (system will prevent/flag):

### ❌ NO-NO 1: Skip Tests
```csharp
[Fact(Skip = "todo")]  // ❌ FORBIDDEN
public void SomeTest() { }
```
**Violation**: Immediate commit rejection

### ❌ NO-NO 2: Commit Broken Build
```powershell
dotnet build  # ❌ HAS ERRORS
git commit -m "WIP"
```
**Violation**: Caught by pre-commit hook

### ❌ NO-NO 3: Merge Multiple Features
```bash
# ❌ FORBIDDEN: 2 features in one commit
git commit -m "Fixed parsing + Added UI button"

# ✅ CORRECT: Separate commits
git commit -m "[RED→GREEN→REFACTOR] Fixed parsing"
git commit -m "[RED→GREEN→REFACTOR] Added UI button"
```

### ❌ NO-NO 4: Modify Existing Test
```csharp
// If this test existed before:
[Fact]
public void ExistingTest()
{
    // Changing this = ❌ FORBIDDEN
    // (unless you're fixing a bug in the test itself)
}
```

### ❌ NO-NO 5: Delete Code Without Docs
```bash
# ❌ FORBIDDEN
git rm SomeClass.cs
git commit -m "cleanup"

# ✅ CORRECT
git rm SomeClass.cs
git commit -m "[CLEANUP] Remove unused SomeClass.cs

Reason: Not called anywhere (verified with find refs)
Related: Issue #123
Verified: All tests pass, coverage unchanged
"
```

---

## 📈 Test Execution Ritual

**After EVERY code change:**

```powershell
# Step 1: Format
dotnet format

# Step 2: Clean build
dotnet clean
dotnet build

# Step 3: Run tests with details
dotnet test --verbosity normal

# Step 4: Check coverage
dotnet test /p:CollectCoverage=true

# Step 5: Verify no warnings
dotnet build  # Check: "0 warnings"

# Step 6: Ready to commit
git add .
git commit -m "[RED→GREEN→REFACTOR] Description"
```

---

## 🔍 Code Review Checklist

Before saying "done", verify:

- [ ] Tests exist (not skipped)
- [ ] Tests pass (dotnet test shows PASSED)
- [ ] Coverage goal met (≥ threshold)
- [ ] No compiler warnings (Build shows "0 warnings")
- [ ] No Console.WriteLine() (use Serilog)
- [ ] No hardcoded values (except in tests)
- [ ] Comments explain WHY, not WHAT
- [ ] Error cases documented
- [ ] Async/await used correctly (no .Result)
- [ ] Commit message follows format
- [ ] Roundtrip test passes (if applicable)
- [ ] Related documentation updated

---

## 🛡️ Automated Safeguards

The system includes:

1. **Task Files** (tasks/00-index.md, tasks/NN-*.md)
   - Clear, sequential steps
   - RED, GREEN, REFACTOR cycle enforced
   - Can't skip steps

2. **Memory System** (llm-build-system/memory/)
   - SESSION_MEMORY.md (current work)
   - BUILD_STATE.md (what's done)
   - PITFALLS_FOUND.md (lessons learned)

3. **LLM Build Instructions** (LLM_BUILD_INSTRUCTIONS.md)
   - Discipline rules
   - Per-commit requirements
   - Error recovery procedures

4. **Cleanup Policy** (CLEANUP_POLICY.md)
   - When to delete code
   - Deprecation procedures
   - Commit message format

5. **This Master Document** (You are here)
   - Overview of entire system
   - Cross-references

---

## 🚨 If Something Breaks

### Broken Tests
```powershell
# Step 1: See what failed
dotnet test --verbosity diagnostic

# Step 2: Check BUILD_STATE.md for last good commit
cat llm-build-system/memory/BUILD_STATE.md

# Step 3: Revert if stuck
git reset --hard <last-good-commit>

# Step 4: Document in PITFALLS_FOUND.md
# (See llm-build-system/memory/PITFALLS_FOUND.md)
```

### Coverage Below Goal
```powershell
# Step 1: See which lines aren't covered
dotnet test /p:CollectCoverage=true
# (Look at coveragereport/)

# Step 2: Write missing tests
# (Don't commit until coverage met)

# Step 3: Verify
dotnet test /p:CollectCoverage=true
# Check: Goal met
```

### Compiler Warnings
```powershell
# Step 1: See warnings
dotnet build

# Step 2: Fix each warning (don't ignore)
# (Edit code to eliminate warning)

# Step 3: Verify
dotnet build
# Check: "Build succeeded with 0 warnings"
```

---

## 📝 Session Template

**Copy this at start of session:**

```markdown
# Session: Modul 1, Phase 2

**Date**: [TODAY]
**Goal**: Complete Task 3.1 (Preset class) + Task 3.2 (PresetParser)
**Estimated**: 3 hours

## Pre-Flight
- [x] Read SESSION_MEMORY.md
- [x] Read tasks/03-modul1-phase2-domain-models.md
- [x] Build is green (`dotnet build`)
- [x] Tests are green (`dotnet test`)

## Work Log
- 10:00 - Starting Task 3.1
- 10:15 - [RED] Test created, failing as expected
- 10:25 - [GREEN] Minimal implementation done
- 10:35 - [REFACTOR] Extracted method
- 10:40 - COMMIT: [MODUL-1] [RED→GREEN→REFACTOR] Preset class basic

- 10:45 - Starting Task 3.2
- ... (similar pattern)

## Issues Found
- [Issue name]: [description, fix, prevention]

## Commits Made
1. [MODUL-1] [RED→GREEN→REFACTOR] Preset class
2. [MODUL-1] [RED→GREEN→REFACTOR] PresetParser methods

## Status
- Tests: ✅ 8/8 passing
- Coverage: ✅ 96% (Domain)
- Build: ✅ 0 warnings
- Ready for: Phase 3
```

---

## 🎓 Key Concepts

### RED Phase (Embrace Failure)
- Write test for feature that DOESN'T EXIST yet
- Test MUST fail (red)
- This is expected and good
- Don't move to GREEN until test is clearly failing

### GREEN Phase (Minimum Viable)
- Write ONLY the code needed to pass test
- Don't optimize
- Don't refactor
- Don't add features
- Just make test green

### REFACTOR Phase (Polish)
- Change implementation without changing behavior
- Extract methods
- Rename variables
- Improve readability
- Tests must STILL pass

### COMMIT Phase (Record)
- Save state with clear message
- Include: what changed, why, tests status
- One feature per commit
- Use format: `[MODUL-X] [RED→GREEN→REFACTOR] Description`

---

## 🔗 Essential Files

**ALWAYS READ THESE FIRST:**

1. [llm-build-system/LLM_BUILD_INSTRUCTIONS.md](../llm-build-system/LLM_BUILD_INSTRUCTIONS.md)
   - The unskippable rules

2. [tasks/00-index.md](../tasks/00-index.md)
   - What to work on next

3. [llm-build-system/memory/SESSION_MEMORY.md](../llm-build-system/memory/SESSION_MEMORY.md)
   - What you're working on right now

4. [llm-build-system/CLEANUP_POLICY.md](../llm-build-system/CLEANUP_POLICY.md)
   - When/how to delete code

---

## ✅ Session Completion Checklist

**Before ending session:**

- [ ] All code changes have tests
- [ ] `dotnet test` shows all PASSING
- [ ] `dotnet build` shows 0 warnings
- [ ] Coverage goal met (check DOMAIN especially)
- [ ] All commits pushed/saved
- [ ] SESSION_MEMORY.md updated
- [ ] BUILD_STATE.md updated
- [ ] PITFALLS_FOUND.md updated (if issues)

---

## 🎯 Summary

**This is a disciplined, automated, test-first development system.**

**No shortcuts:**
- Tests first, always
- Coverage matters
- Commits are traceable
- Memory is preserved
- Every action documented

**No excuses:**
- Can't skip tests
- Can't commit broken code
- Can't ignore warnings
- Can't delete without reason
- Can't change existing tests lightly

**Benefits:**
- High code quality
- Easy to review
- Easy to revert
- Easy for next session
- Confidence in system

---

**Status**: 🟢 **READY TO ENFORCE DISCIPLINE**

**Next**: Start Phase 0 in `tasks/01-phase0-environment-setup.md`
