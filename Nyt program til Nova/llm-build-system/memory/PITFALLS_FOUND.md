# PITFALLS_FOUND.md — Lessons Learned

## Format

```
## [DATE] [ISSUE TITLE]
- **Symptom**: What went wrong
- **Root cause**: Why it happened
- **Fix**: How to fix it
- **Prevention**: How to avoid next time
- **Related**: Link to relevant code/docs
```

---

## Issues Found

### None yet - Project starting

Once development begins, all issues will be documented here to prevent repetition.

---

## Anti-Patterns to Watch

1. **Skipping test for "quick fix"**
   - Symptom: Code works but builds fail in CI
   - Prevention: ALWAYS do RED→GREEN→REFACTOR

2. **Mixing concerns in one commit**
   - Symptom: Hard to review, hard to revert if issues
   - Prevention: One feature per commit, one responsibility per class

3. **Hardcoded test data**
   - Symptom: Tests fail when data changes
   - Prevention: Use fixtures/ SysEx files, not magic numbers

4. **Forgetting Serilog logging**
   - Symptom: Can't debug production issues
   - Prevention: Log at INFO level for business events, DEBUG for details

5. **Async/await misuse**
   - Symptom: Deadlocks, UI freezes, tests timeout
   - Prevention: Never use .Result or .Wait() on Tasks

---

## Common LLM Mistakes (To Avoid)

### Mistake 1: "I'll refactor later"
**Why it's bad**: You won't, tests get messy, coverage drops  
**Fix**: REFACTOR IMMEDIATELY after GREEN

### Mistake 2: "This test is obvious, skip it"
**Why it's bad**: The "obvious" part is where bugs hide  
**Fix**: WRITE THE TEST, make it pass

### Mistake 3: "Just this once, I'll skip the gate"
**Why it's bad**: One skipped gate = system failure  
**Fix**: NEVER skip gates - document if you need an exception

### Mistake 4: "I'll commit multiple things at once"
**Why it's bad**: Can't revert specific changes, hard to review  
**Fix**: ONE FEATURE PER COMMIT

### Mistake 5: "My code is too simple to test"
**Why it's bad**: Simple code is where bugs multiply  
**Fix**: EVERY CODE CHANGE NEEDS A TEST

---

## Related

- [LLM_BUILD_INSTRUCTIONS.md](../LLM_BUILD_INSTRUCTIONS.md)
- [SESSION_MEMORY.md](SESSION_MEMORY.md)
