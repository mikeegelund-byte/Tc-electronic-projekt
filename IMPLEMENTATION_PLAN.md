# Nova System Manager - Implementation Plan (Phases 1, 2, 4, 5)

Scope: This plan completes effect model coverage, modular UI editors, routing/tuner/library features, and verification. Phase 3 (real-time MIDI integration) is deferred to future mode and is not a blocker here.

NOTE TO SELF: If any sub-effect parameter value, range, or behavior is uncertain, stop and ask the user for a hardware check.

## Short Summary (Referat)
The work finishes the effect model alignment against the official reference tables and original TC material, expands all effect ViewModels to full parameter coverage, and refactors the preset UI into modular editors that reflect each effect type. It then adds routing visualization, a tuner interface stub, and local library management. Finally, tests and manual checks are updated to ensure correct parsing, export/import, and stable UI behavior.

---

## Phase 1: Complete Effect Models (Domain + ViewModels)

### 1.1 Parameter Coverage Audit
- [ ] Cross-check `docs/REFERENCE/REF-EffectReference.md` vs `docs/REFERENCE/REF-SysexMapTables.md`
- [ ] Cross-check original TC docs (Sysex map + manual) for discrepancies
- [ ] Create a coverage matrix per block (Drive, Comp, EQ/Gate, Mod, Pitch, Delay, Reverb)
- [ ] List all type-dependent fields and mark missing/incomplete items

### 1.2 Domain Model: Preset Offsets + Decoding
- [ ] Align all offsets with REF-SysexMapTables, including globals (Map Parameter/Min/Mid/Max)
- [ ] Decode all on/off flags (Comp, Drive, Mod, Delay, Reverb, EQ, Gate, Pitch)
- [ ] Implement type-dependent decoding for multi-function offsets
  - [ ] Mod: Delay/Range/Type (222-225)
  - [ ] Delay: Tempo2/Width (278-281), Clip/Feedback2 (286-289), Offset/Pan1 (298-301), Sense/Pan2 (302-305)
  - [ ] Pitch: Level2 vs Direction (494-497), Feedback/Key vs Feedback/Scale (482-489)
- [ ] Confirm signed-value decoding logic for all signed parameters

### 1.3 Domain Model: Preset Properties + Validation
- [ ] Add any missing properties from the official map
- [ ] Remove properties not present in the official map
- [ ] Update property comments to correct offsets/ranges
- [ ] Update range validation to match reference tables for every parameter
- [ ] Add type-specific validation rules where needed (Pitch, Delay, Mod)

### 1.4 Application Import/Export Alignment
- [ ] Update `ExportPresetUseCase` to include all new fields
- [ ] Update `ImportPresetUseCase` to parse and encode all fields
- [ ] Ensure text keys match new exported field names

### 1.5 ViewModels: Full Block Coverage
- [ ] Compressor: Type, Threshold, Ratio, Attack, Release, Response, Drive, Level, Enabled
- [ ] Drive: Type, Gain, Tone, Level, Boost Level, Boost Enabled, Drive Enabled
- [ ] Modulation: Type, Speed, Depth, Tempo, HiCut, Feedback, Delay/Range/Type, Width, Mix, Enabled
- [ ] Delay: Type, Time, Time2, Tempo, Tempo2/Width, Feedback, Clip/FB2, HiCut, LoCut, Offset/Pan1, Sense/Pan2, Damp, Release, Mix, Enabled
- [ ] Reverb: Type, Decay, PreDelay, Shape, Size, HiColor, HiLevel, LoColor, LoLevel, RoomLevel, Level, Diffuse, Mix, Enabled
- [ ] Pitch: Type, Voice1, Voice2, Pan1, Pan2, Delay1, Delay2, Feedback1/Key, Feedback2/Scale, Level1, Level2/Direction, Range, Mix, Enabled
- [ ] EQ/Gate: Gate Type, Threshold, Damp, Release, Gate Enabled, EQ Enabled, Freq/Gain/Width for 3 bands

### 1.6 Global Fields in Preset Detail
- [ ] Update global ranges in `PresetDetailViewModel` (Tap Tempo, Routing, Level Out L/R)
- [ ] Expose Map Parameter/Min/Mid/Max or map to existing pedal mapping UI

---

## Phase 2: UI Modularization (15 Editors)

### 2.1 UI Infrastructure
- [ ] Create folder `src/Nova.Presentation/Views/Effects/`
- [ ] Add top-level block views: Drive, Compressor, EQ/Gate, Modulation, Pitch, Delay, Reverb

### 2.2 Type-Specific Sub-Editors (15 total)
- [ ] Drive: Overdrive, Distortion (shared layout where possible)
- [ ] Compressor: Percussive, Sustaining, Advanced
- [ ] Modulation: Chorus, Flanger, Vibrato, Phaser, Tremolo, Panner
- [ ] Pitch: Shifter, Octaver, Whammy, Detune, Intelligent
- [ ] Delay: Clean, Analog, Tape, Dynamic, Dual, Ping-Pong
- [ ] Reverb: Spring, Hall, Room, Plate
- [ ] EQ/Gate: single editor with EQ + Gate toggles

### 2.3 PresetDetailView Refactor
- [ ] Replace inline sections in `PresetDetailView.axaml` with modular effect views
- [ ] Bind updated block ViewModels to each editor
- [ ] Preserve current theme and layout patterns

### 2.4 Type-Dependent UI Rules
- [ ] Ensure only relevant parameters show per type
- [ ] Update numeric ranges and labels to match references
- [ ] Add concise tooltips for complex parameters (feedback, damp, offset)
- [ ] Verify layout works on desktop and narrow widths

---

## Phase 4: Special Features

### 4.1 Routing Visualizer
- [ ] Implement a compact routing diagram for Serial/Semi-Parallel/Parallel
- [ ] Bind to `Preset.Routing`
- [ ] Integrate into Preset detail or a dedicated section

### 4.2 Tuner View
- [ ] Create tuner ViewModel with basic properties (frequency, cents, note)
- [ ] Build UI stub with live update hooks for future integration

### 4.3 Library Management
- [ ] Define local library storage format and folder structure
- [ ] Implement library service/repository
- [ ] Build ViewModel for browsing, tags, search, and selection
- [ ] UI for import/export, save to library, load from library, delete
- [ ] Integrate with user bank and preset selection flows

---

## Phase 5: Verification

### 5.1 Test Updates
- [ ] Update domain tests for new offsets/fields and removed properties
- [ ] Update application tests for Import/Export content changes
- [ ] Update presentation tests for expanded ViewModel properties
- [ ] Update shared test helpers to generate valid presets

### 5.2 Build + Manual Validation
- [ ] Run `dotnet build` for the solution
- [ ] Run relevant test suites
- [ ] Manual UI pass: parse presets, inspect parameters, verify visibility by type
- [ ] Manual export/import round-trip check

### 5.3 Regression Checklist
- [ ] Preset parsing succeeds for sample banks
- [ ] Export/Import round-trip works
- [ ] UI loads with no XAML errors
- [ ] No null ref issues when preset is missing
- [ ] Bank operations still work end-to-end

---

## Deliverables
- [ ] Preset model fully aligned with official reference tables
- [ ] Complete block ViewModels with full parameter coverage
- [ ] Modular effect UI with type-aware editors (15 total)
- [ ] Routing visualizer, tuner view, and library management
- [ ] Clean build and updated tests
