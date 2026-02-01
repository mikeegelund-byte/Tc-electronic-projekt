# PITFALLS_FOUND.md — Lessons Learned

Status: læst

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

### [2026-01-31] DryWetMIDI F0/F7 delimiter handling confusion
- **Symptom**: Tried to send/receive SysEx WITH F0/F7 bytes included, got protocol errors
- **Root cause**: DryWetMIDI library auto-adds F0/F7, expects data without them for SendEvent(), but returns data without F0/F7 in EventReceived
- **Fix**: Strip F0/F7 when sending via NormalSysExEvent, add F0/F7 when saving to .syx files
- **Prevention**: Always read library documentation for SysEx handling expectations
- **Related**: src/Nova.HardwareTest/Program.cs, SendTest.cs

### [2026-01-31] USB MIDI device name assumptions
- **Symptom**: Couldn't find MIDI device by searching for "Nova" in device name
- **Root cause**: USB-MIDI converter doesn't contain pedal name, uses generic "USB MIDI Interface"
- **Fix**: Search for "USB MIDI" instead of hardware-specific names
- **Prevention**: Never assume device names, use Contains() with generic terms
- **Related**: src/Nova.HardwareTest/Program.cs line 15-20

### [2026-01-31] Windows MIDI exclusive access
- **Symptom**: OUT_OPENRESULT_NOMEMORY error when opening MIDI port
- **Root cause**: Windows MIDI ports are exclusive-access, another process had it locked
- **Fix**: Close all MIDI apps, restart VS Code, physical USB reconnect
- **Prevention**: Always check for zombie processes (Task Manager), close hardware test app before rebuilding
- **Related**: Microsoft.Win32.SafeHandles errors

### [2026-01-31] Nova System passive MIDI protocol
- **Symptom**: Built request message (F0 00 20 1F 00 63 45 03 F7), pedal didn't respond
- **Root cause**: Nova System doesn't respond to SysEx requests - user must manually send dumps from pedal
- **Fix**: Listen passively, user triggers dump from pedal: UTILITY → MIDI → Send Dump
- **Prevention**: Read hardware manual first - not all MIDI devices respond to requests
- **Related**: docs/MIDI_PROTOCOL.md, src/Nova.HardwareTest/Program.cs

### [2026-01-31] System Dump detection timeout
- **Symptom**: Listener waited for 60 presets when System Dump sent (only 1 file)
- **Root cause**: System Dump is single 527-byte file (byte[6]=0x02), not 60 files like User Bank
- **Fix**: Detect System Dump by checking byte[6]==0x02, stop immediately
- **Prevention**: Different dump types have different structures, check message type byte
- **Related**: src/Nova.HardwareTest/Program.cs line 45-50

### [2026-01-31] Timestamp change in .syx filenames
- **Symptom**: Integration test failed looking for "msg032.syx" with timestamp 181507
- **Root cause**: Capture session crossed second boundary, timestamp changed 181507→181508 at msg032
- **Fix**: Use Directory.GetFiles() with wildcard pattern, sort alphabetically instead of hardcoded names
- **Prevention**: Never hardcode timestamps in file paths, use dynamic file discovery
- **Related**: src/Nova.Domain.Tests/UserBankDumpRealDataTests.cs

### [2026-01-31] Not following LLM build system discipline
- **Symptom**: Made 8 commits without updating SESSION_MEMORY.md, BUILD_STATE.md, PITFALLS_FOUND.md
- **Root cause**: Focused on implementation, forgot to maintain memory system after each session/commit
- **Fix**: Updated all memory files after user reminded me
- **Prevention**: **MUST update memory files after EVERY commit** - add checklist step before commit
- **Related**: llm-build-system/memory/*.md files

### [2026-01-31] Moq cannot mock sealed classes
- **Symptom**: MainViewModelTests failing with "Type to mock (ConnectUseCase) must be an interface, a delegate, or a non-sealed, non-static class"
- **Root cause**: UseCases (ConnectUseCase, DownloadBankUseCase) are sealed classes. Moq can only mock interfaces, abstract classes, or non-sealed classes.
- **Fix**: Either:
  1. Extract IConnectUseCase and IDownloadBankUseCase interfaces from UseCases (RECOMMENDED for testability)
  2. Make UseCases non-sealed (violates design)
  3. Use real instances in tests instead of mocks (not ideal for unit tests)
- **Prevention**: Always design for testability - prefer interfaces for dependencies that need mocking
- **Status**: ⚠️ DEFERRED - MainViewModel code is complete and compiles, tests will be fixed later when extracting UseCase interfaces
- **Related**: src/Nova.Presentation.Tests/ViewModels/MainViewModelTests.cs, src/Nova.Application/UseCases/

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
