# Modul-roadmap (10 moduler)

## Modul 1 — Connection + Bank Dump roundtrip
**Status:** Starting point  
**Duration:** 2-3 weeks

**Mål:**
- App åbner uden fejl
- MIDI port enumeration virker
- Connect til Nova System etableres
- Request User Bank Dump
- Parse dump lokalt (validere checksum)
- Send dump tilbage til pedal

**Success criteria:**
- Roundtrip uden fejl
- Status UI feedback
- Timeout handling
- Retry logic (max 3x)

**Tests:**
- 100% unit tests (parsing, validation)
- 100% integration tests (mock MIDI)
- Manual test med rigtig pedal

**Deliverables:**
- Working MVP app (minimal UI)
- Port selector
- "Download" og "Upload" buttons
- Status messages

---

## Modul 2 — Preset viewer (60 names)
**Status:** After Modul 1 approved  
**Duration:** 1 week

**Mål:**
- Parse User Bank Dump
- Extract alle 60 preset-navne
- Vis i sortable ListView

**Success criteria:**
- All 60 names visible
- Sortable by position
- No crashes on bad data

**Tests:**
- Unit: parsing all 60 presets
- UI: ListView binding

**Deliverables:**
- Preset list view
- Name column + position

---

## Modul 3 — System Dump viewer
**Status:** After Modul 2  
**Duration:** 1 week

**Mål:**
- Request System Dump
- Parse global settings (MIDI channel, Program Map, CC assigns)
- Vis i read-only form

**Success criteria:**
- All settings parsed correctly
- User can see current config

**Tests:**
- 100% parsing tests
- Fixture: real system dump

**Deliverables:**
- System settings UI
- Read-only display

---

## Modul 4 — System settings editor
**Status:** After Modul 3  
**Duration:** 2 weeks

**Mål:**
- Edit global settings (MIDI channel, Program Map entries, CC assigns)
- Save changes til pedal
- Roundtrip validation

**Success criteria:**
- Changes persist on hardware
- Undo on bad roundtrip
- UI shows pending changes

**Tests:**
- SysEx generation tests
- Roundtrip tests (mock)
- Manual: real pedal changes

**Deliverables:**
- Editable system form
- Save/Cancel buttons
- Change validation

---

## Modul 5 — Preset detail viewer (read-only)
**Status:** After Modul 4  
**Duration:** 2 weeks

**Mål:**
- Click preset → show all effect blocks + parameters
- Display only (no editing)
- All 15 effect types

**Success criteria:**
- All parameters displayed correctly
- Responsive UI

**Tests:**
- Parsing for all 15 effect types
- UI binding tests

**Deliverables:**
- Preset detail view
- 15 effect block displays (read-only)

---

## Modul 6 — Preset editing
**Status:** After Modul 5  
**Duration:** 4 weeks

**Mål:**
- Full parameter editing (all effects)
- Live MIDI CC updates (optional)
- Save to user slot
- SysEx generation + roundtrip

**Success criteria:**
- Edit → save → verify on pedal
- Parameter validation (min/max)
- Checksum correct

**Tests:**
- Parameter range validation (all effects)
- SysEx roundtrip tests
- Manual: real pedal edits

**Deliverables:**
- Full preset editor
- All 15 effect editors
- Save dialog

---

## Modul 7 — Preset management
**Status:** After Modul 6  
**Duration:** 2 weeks

**Mål:**
- Copy preset to new slot
- Rename preset
- Delete user preset
- A/B compare
- Undo/Redo (basic)

**Success criteria:**
- Copy → load → verify
- Delete → cannot load
- A/B showing differences

**Tests:**
- Copy/delete/rename tests
- Undo/Redo state tests

**Deliverables:**
- Context menu on presets
- A/B compare UI
- Undo/Redo buttons

---

## Modul 8 — File I/O (.syx files)
**Status:** After Modul 7  
**Duration:** 1 week

**Mål:**
- Export single preset to .syx
- Export user bank to .syx
- Import .syx file
- Backup system dump

**Success criteria:**
- Exported .syx plays in NovaManager
- Import roundtrip works
- File picker UX smooth

**Tests:**
- File write/read tests
- SysEx format validation

**Deliverables:**
- File menu (Open, Save, Export)
- File dialogs

---

## Modul 9 — MIDI mapping editor
**Status:** After Modul 8  
**Duration:** 2 weeks

**Mål:**
- Edit MIDI CC in/out mappings
- Edit expression pedal mapping (min/mid/max)
- Save to System Dump

**Success criteria:**
- CC mapping works on real pedal
- Expression pedal responds

**Tests:**
- MIDI mapping data validation
- Roundtrip tests

**Deliverables:**
- MIDI CC assignment table
- Pedal response curve editor

---

## Modul 10 — Polish & Release
**Status:** Final  
**Duration:** 2-3 weeks

**Mål:**
- UI/UX finish (Apple-agtig design)
- Animations
- Dark/Light mode
- Installer (.msi for Windows)
- Auto-update pipeline setup
- Documentation

**Success criteria:**
- App looks professional
- Installer works smoothly
- Auto-updates functional

**Tests:**
- UI stress tests
- Installer verification
- Update flow tests

**Deliverables:**
- Polished UI
- Windows installer
- Release notes
- User manual

---

## Timeline estimate
| Modul | Dage | Cumulative |
|-------|------|-----------|
| 1 | 14 | 2 weeks |
| 2 | 7 | 3 weeks |
| 3 | 7 | 4 weeks |
| 4 | 14 | 6 weeks |
| 5 | 14 | 8 weeks |
| 6 | 28 | 14 weeks |
| 7 | 14 | 16 weeks |
| 8 | 7 | 17 weeks |
| 9 | 14 | 19 weeks |
| 10 | 21 | 23 weeks |

**Total:** ~5-6 months (full-time effort)

---

## Gate requirements
- Each modul must have 100% passing tests
- Manual testing on real hardware at milestones (1, 3, 4, 6, 9)
- Code review before merge to main
- No hotfixes without tests
