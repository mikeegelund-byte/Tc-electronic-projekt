# Task List: Modul 7 — Preset Management

## 📋 Module: 7 (Preset Management)
**Duration**: 2 weeks  
**Prerequisite**: Modul 6 complete  
**Output**: Copy, rename, delete presets + A/B compare + Undo/Redo  

---

## Exit Criteria

- [ ] Copy preset til anden slot
- [ ] Rename preset (24 chars max)
- [ ] Delete user preset
- [ ] A/B sammenligning
- [ ] Undo/Redo for edits
- [ ] All tests pass

---

## Phase 1: Basic Operations (1 uge)

### Task 7.1.1: Implement Copy Preset

**🟡 COMPLEXITY: MEDIUM** — Slot selection dialog

**Status**: ✅ COMPLETE  
**Estimated**: 45 min  
**Actual**: ~50 min  
**Commit**: `d3f704f` - CopyPresetUseCase + 7 tests (268 tests total)  

---

### Task 7.1.2: Implement Rename Preset

**🟢 COMPLEXITY: SIMPLE** — TextBox + validation

**Status**: ✅ COMPLETE  
**Estimated**: 30 min  
**Actual**: ~40 min  
**Commit**: `41e281b` - RenamePresetUseCase + 8 tests (277 tests total)  

---

### Task 7.1.3: Implement Delete Preset

**🟢 COMPLEXITY: SIMPLE** — Confirm dialog + clear slot

**Status**: ✅ COMPLETE  
**Estimated**: 30 min  
**Actual**: ~35 min  
**Commit**: `7ad179b` - DeletePresetUseCase + 7 tests (284 tests total)  

---

### Task 7.1.4: Add Context Menu

**🟢 COMPLEXITY: SIMPLE** — XAML ContextMenu

**Status**: ✅ COMPLETE  
**Estimated**: 20 min  
**Actual**: ~25 min  
**Commit**: `932bf82` - Context menu with keyboard shortcuts (Ctrl+C, Del, F2)  

---

## Phase 2: A/B Compare + Undo (1 uge)

### Task 7.2.1: Implement A/B Compare Data Logic

**🔴 COMPLEXITY: HIGH** — Kræver SONNET 4.5+

**Årsag**: Sammenlign 78 parametre, identificer forskelle, gruppér per effect block

**Status**: ⏸️ DEFERRED TO V1.1  
**Estimated**: 60 min  
**Note**: A/B Compare is a nice-to-have feature for comparing two presets side-by-side  

---

### Task 7.2.2: Create Difference View UI

**🟡 COMPLEXITY: MEDIUM** — Side-by-side eller diff highlighting

**Status**: Not started  
**Estimated**: 60 min  

---

### Task 7.2.3: Implement Undo/Redo Stack

**🔴 COMPLEXITY: HIGH** — Kræver SONNET 4.5+

**Årsag**: Command pattern, memento pattern, state management

**Status**: Not started  
**Estimated**: 90 min  

---

### Task 7.2.4: Add Undo/Redo Buttons + Shortcuts

**🟢 COMPLEXITY: SIMPLE** — XAML + KeyBindings

**Status**: Not started  
**Estimated**: 20 min  

---

## Completion Checklist

- [x] Copy/Rename/Delete virker
- [ ] A/B compare viser forskelle (DEFERRED TO V1.1)
- [ ] Undo/Redo fungerer (DEFERRED TO V1.1)
- [x] Commit: `[MODUL-7] Implement Preset Management`

---

**Status**: ✅ COMPLETE (Phase 1) — Phase 2 (A/B Compare, Undo/Redo) deferred to V1.1  
**Commits**: d3f704f (Copy), 41e281b (Rename), 7ad179b (Delete), 932bf82 (Context Menu)
