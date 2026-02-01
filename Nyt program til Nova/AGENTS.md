# AGENTS.md - Fixed Instructions for LLM Agents (Nova project)

Purpose: Single, always-on rules for any LLM working in this repo.
If anything conflicts, follow this file plus:
- llm-build-system/LLM_BUILD_INSTRUCTIONS.md
- llm-build-system/CLEANUP_POLICY.md
- Current tasks file (tasks/NN-*.md)

## Communication (mandatory)
- All communication is in Danish: text, writing, thoughts, and reasoning.
- Internet use is allowed for agents when needed.

## 0) Scope and read-only zones
- Code lives in NovaApp/src. Tests live in NovaApp/tests.
- docs/, Nova manager Original/, and Tc originalt materiale/ are read-only reference.
- llm-build-system/ is the discipline system; do not change its structure or content unless the user asks.

## 1) Project context and non-negotiables
- Target hardware: TC Electronic Nova System via USB-MIDI.
- Preserve MIDI protocol compatibility exactly (SysEx format, checksum, CC mapping).
- Nibble encoding is required for SysEx values (7-bit pairs).
- MIDI I/O must be asynchronous; queue outgoing messages to avoid buffer overflow.
- Key workflows: Preset load = Program Change -> SysEx -> parse -> UI update; Preset edit = UI change -> CC -> dirty -> save SysEx; System dump = export/import .syx with validation.
- Modern UX is required (not a 1:1 clone of the legacy Java UI).
- AI-first architecture: expose APIs for programmatic preset control.
- Stack is locked to C# 11/.NET 8, Avalonia 11, DryWetMIDI, xUnit, Moq, Serilog (see APPLICATION_MANIFEST.md).
- Legacy Java app is reference-only; do not modify it.
- Critical references (read-only): Tc originalt materiale/Nova System Sysex Map.pdf and TC Nova Manual.pdf; Nova manager Original/ for protocol behavior.

## 2) Start-of-session checklist (mandatory)
- Read: llm-build-system/memory/SESSION_MEMORY.md
- Read: tasks/00-index.md and the current tasks/NN-*.md file
- Read: llm-build-system/LLM_BUILD_INSTRUCTIONS.md
- Read: APPLICATION_MANIFEST.md, PROJECT_KNOWLEDGE.md, docs/01-vision-scope.md (philosophy)
- Read: FOLDER_STRUCTURE.md (where things live)
- Read: LLM_DISCIPLINE_SYSTEM.md and LLM_SYSTEM_COMPLETE.md (discipline overview)
- Verify green start: dotnet build + dotnet test
- Update SESSION_MEMORY.md with todays goal and Do NOT list

## 3) Mandatory development cycle (every change)
- RED: write a failing test first
- GREEN: minimal code to pass
- REFACTOR: clean code without behavior change
- One feature per commit
- No code change without a failing test first

## 4) Gates before any commit
- dotnet format
- dotnet build -> 0 warnings
- dotnet test -> all passing
- Coverage targets met:
  - Domain >= 95%, Application >= 80%, Infrastructure >= 70%, MIDI >= 90%, UI >= 50%
- If MIDI-related: roundtrip tests + manual hardware test when required

## 5) Commit messages
- Default: [MODUL-X] [RED->GREEN->REFACTOR] Short description
- Cleanup: [CLEANUP] [Category] ... with reason/impact/testing in the body
- Never commit WIP or broken builds

## 6) Memory system (required)
- Start: update SESSION_MEMORY.md (module/phase/task/goal/Do NOT)
- After each session and after non-trivial commits:
  - Update BUILD_STATE.md (what was done, tests, coverage)
  - Update PITFALLS_FOUND.md for any issues or lessons
  - Keep timestamps and absolute dates (YYYY-MM-DD)

## 7) Documents that must be maintained
- llm-build-system/memory/SESSION_MEMORY.md (current work and goals)
- llm-build-system/memory/BUILD_STATE.md (commits, test status, coverage)
- llm-build-system/memory/PITFALLS_FOUND.md (lessons and mistakes)
- tasks/00-index.md (progress status and next step)
- AGENTS.md (when agent rules change)
- PROJECT_KNOWLEDGE.md (protocol/workflows/AI notes)
- STATUS.md and DOCUMENTATION_COMPLETE.md (milestones and completion state)
- If structure or philosophy changes: update APPLICATION_MANIFEST.md and FOLDER_STRUCTURE.md

## 8) Cleanup and refactor rules
- Follow llm-build-system/CLEANUP_POLICY.md
- Never delete passing tests
- No mass deletion; small, documented changes only
- Public APIs: mark [Obsolete], wait a sprint, then delete
- Refactor only on green tests; no behavior change

## 9) Coding standards and safety
- Use Serilog for logging; no Console.WriteLine
- No hardcoded values except in tests
- No magic numbers without explanation
- Comments explain WHY, not WHAT
- Async/await correctly; no .Result or .Wait
- Do not change existing tests unless fixing a test bug (document why)

## 10) If something fails
- Stop and read the full error
- Run: dotnet test --verbosity diagnostic
- Check: llm-build-system/memory/BUILD_STATE.md for last good commit
- If still stuck, revert to last good commit only after ensuring no valuable uncommitted work
- Document the issue in PITFALLS_FOUND.md

## 11) End-of-session checklist
- dotnet test (must be green)
- dotnet build (0 warnings)
- Cold build: delete bin/obj, then dotnet clean + dotnet build + dotnet test
- Update SESSION_MEMORY.md, BUILD_STATE.md, PITFALLS_FOUND.md
- Commit session summary

## 12) When unsure
- Ask before guessing
- Do not skip any step in a task file
