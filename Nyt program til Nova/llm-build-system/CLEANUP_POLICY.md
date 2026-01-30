# CLEANUP_POLICY.md — When & How to Delete/Refactor

## 🎯 Purpose

Maintain code cleanliness without breaking things. Every deletion or major refactor must follow this policy.

---

## ✅ When You MAY Delete

### 1. Unused Test Files
**Condition**: Test class has 0 methods or all are commented out  
**Process**:
```powershell
# Step 1: Verify it's truly unused
git log --follow -- NovaApp.Tests/ObsoleteTests.cs | head -5

# Step 2: Create test-deletion test (meta!)
[Fact]
public void ObsoleteTests_NotUsed_CanDelete()
{
    // Documentation that this was intentionally removed
}

# Step 3: Delete file
git rm NovaApp.Tests/ObsoleteTests.cs

# Step 4: Commit
git commit -m "[CLEANUP] Remove obsolete test file: ObsoleteTests.cs

Reason: No tests use this class (verified in git log)
Impact: 0 impact (no dependencies)
"
```

### 2. Unused Methods/Classes
**Condition**: No calls anywhere (use IDE "Find All References")  
**Process**:
```csharp
// Before deletion, mark as [Obsolete]:
[Obsolete("Not used since Modul 3, ref: commit abc123")]
public void LegacyMethod() { }

// Wait 1 commit cycle to see if anything breaks

// Then delete with documented reason
```

### 3. Dead Code in Tests
**Condition**: Test is [Fact(Skip = "...")] for >1 week  
**Process**:
```powershell
# Step 1: Check when it was skipped
git log --all -S '[Fact(Skip' -- NovaApp.Tests/

# Step 2: If >1 week old, DELETE not SKIP
git rm NovaApp.Tests/path/to/SkippedTest.cs

# Step 3: Commit with reason
git commit -m "[CLEANUP] Remove skipped test: XyzTests

Skipped since: [DATE]
Reason for removal: [Original feature never implemented/changed]
"
```

---

## ❌ When You MUST NOT Delete

### 1. Working Tests
**Never ever delete a passing test.** Even if it seems redundant.

### 2. Public APIs
Never delete public methods without:
- Deprecation period (1 sprint minimum)
- [Obsolete] attribute for 1 sprint first
- Replace with new method

### 3. Documentation
Only delete if:
- Information moved to newer doc (link to it)
- Completely superseded
- Never delete without replacement

---

## 🔄 Refactoring Rules (NO BIG REWRITES)

### Rule 1: Only Refactor Passing Code
```csharp
// WRONG: Refactor while test is failing
[Fact]
public void SomeTest() // <- TEST FAILS
{
    Assert.True(ComplexMethod());  // Now you refactor this?
}

// RIGHT: Test passes, THEN refactor
[Fact]
public void SomeTest() // <- TEST PASSES ✅
{
    Assert.True(ComplexMethod());  // Now you can refactor
}
```

### Rule 2: Refactor = No Behavior Change
```csharp
// WRONG: Refactoring that changes behavior
public int CalculateValue(int x)
{
    return x + 10;  // Was: x * 2
}

// RIGHT: Refactoring, same behavior
public int CalculateValue(int x)
{
    var baseValue = x;
    var increment = 10;
    return baseValue + increment;
}
```

### Rule 3: Refactor Small = Commit Often
```
GOOD: 10 small refactorings (10 commits)
BAD: 1 massive refactoring (1 commit)

Each commit should be reviewable in <5 min
```

### Rule 4: Never Refactor Without Test
```csharp
// All existing tests must pass BEFORE and AFTER refactoring
[Fact]
public void ExistingTest()
{
    // This MUST pass before refactoring
    // This MUST pass after refactoring
    // If it fails either time: REVERT
}
```

---

## 🗑️ Cleanup Categories

### Category A: Safe to Delete Immediately
- Duplicate test methods (exact copy)
- Commented-out code (>3 lines)
- Unused local variables
- Unused imports

**Process**: Delete + commit with reason

### Category B: Require Deprecation Period
- Public methods
- Published APIs
- Interfaces with implementations

**Process**: 
1. Add [Obsolete] attribute
2. Wait 1 week
3. Check if anything broke
4. Delete

### Category C: Require Documentation
- Test classes
- Domain models
- Architecture decisions

**Process**:
1. Document why deleting
2. Update related docs
3. Delete
4. Commit with link to documentation

---

## 📋 Cleanup Checklist

Before deleting ANYTHING:

- [ ] I verified this is truly unused (git log, find references)
- [ ] I searched entire solution (Ctrl+Shift+F)
- [ ] I documented why in commit message
- [ ] I checked tests still pass
- [ ] I checked build has 0 warnings
- [ ] If public: marked [Obsolete] first
- [ ] If docs affected: updated them
- [ ] I created a commit with clear message

---

## 🚨 Cleanup Anti-Patterns

### Anti-Pattern 1: "Cleaning" Someone Else's Code
```csharp
// WRONG: You delete code you didn't write without understanding it
git rm SomeoneElsesCode.cs
git commit -m "Cleanup: removed dead code"

// RIGHT: Ask why it exists first
git log --all --follow -- SomeoneElsesCode.cs
// If unclear: ask in PR comment, not delete
```

### Anti-Pattern 2: Mass Deletion
```powershell
# WRONG: Delete 50 files at once
git rm legacy/**/*.cs
git commit -m "Cleanup"

# RIGHT: Delete 1-2 related files per commit
git rm LegacyFile1.cs
git commit -m "[CLEANUP] Remove legacy/file1.cs - ref: issue #123"

git rm LegacyFile2.cs
git commit -m "[CLEANUP] Remove legacy/file2.cs - ref: issue #123"
```

### Anti-Pattern 3: "Cleanup" That Changes Behavior
```csharp
// WRONG: You "cleanup" and accidentally change how it works
public void OriginalMethod()
{
    var x = GetValue();  // returns 5
    return x * 2;        // returns 10
}

public void RefactoredMethod() // You "cleaned it up"
{
    return GetValue() * 3;  // Now returns 15! (WRONG!)
}

// RIGHT: Exact same behavior, just cleaner
private const int MULTIPLIER = 2;

public void RefactoredMethod()
{
    var x = GetValue();
    return x * MULTIPLIER;
}
```

---

## 📝 Cleanup Commit Message Format

```
[CLEANUP] [Category] Brief description

- Reason: Why delete/refactor
- Impact: What changes (if behavior changes: FORBIDDEN)
- Testing: How verified (must pass tests)
- Related: Link to issue/doc if applicable

Example:
---
[CLEANUP] [DEAD-CODE] Remove unused NibbleEncoder helper

- Reason: Not called anywhere (verified with find references)
- Impact: 0 - method was never used outside of tests
- Testing: All 47 tests pass, same coverage
- Related: Nibble encoding is now in SysExFormat.cs

Verified:
  git log --all -S "NibbleEncoder" -- (empty)
  dotnet test (all pass)
  dotnet build (0 warnings)
```

---

## 🔄 When Refactoring Old Code

### Step-by-Step Refactoring Process

```
1. Tests must be GREEN
   $ dotnet test
   # Output: PASSED ✅

2. Create new (better) implementation
   # In same file or new file, doesn't matter

3. Tests MUST STILL BE GREEN
   $ dotnet test
   # Output: PASSED ✅ (same as step 1)

4. If tests still green: Old code was replaced, behavior preserved
   # Safe to delete old code

5. Delete old implementation

6. Tests MUST STILL BE GREEN
   $ dotnet test
   # Output: PASSED ✅ (same as steps 1 & 3)

7. Commit:
   git commit -m "[REFACTOR] Replace XyzMethod implementation
   
   - Old: [1 line description of old implementation]
   - New: [1 line description of new implementation]
   - Behavior: Identical (all tests pass)
   - Performance: [if changed, document]
   "
```

---

## ⚠️ Never Ever Do This

```csharp
// ❌ Delete test and code together
git rm SomeTest.cs
git rm SomeImplementation.cs
git commit -m "Cleanup"

// ❌ Refactor untested code
// (If no test exists, create one first!)

// ❌ Delete working code
// (Even if you think it's unused, test first)

// ❌ Cleanup without commit message
git rm LegacyCode.cs
git commit -m "cleanup"  // NO - explain WHY
```

---

## 🔗 Related

- [LLM_BUILD_INSTRUCTIONS.md](../LLM_BUILD_INSTRUCTIONS.md)
- [docs/04-testing-strategy.md](../docs/04-testing-strategy.md)

---

## Summary

**CLEANUP RULE:**
```
IF TESTS PASS BEFORE & AFTER: SAFE TO DELETE
IF UNSURE: MARK [Obsolete] FIRST, DELETE LATER
IF DELETING: DOCUMENT WHY IN COMMIT
IF YOU CAN'T EXPLAIN WHY: DON'T DELETE
```
