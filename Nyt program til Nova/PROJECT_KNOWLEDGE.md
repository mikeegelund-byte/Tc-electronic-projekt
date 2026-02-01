# TC Electronic Nova System - Complete Project Knowledge Synthesis

**Purpose:** Comprehensive reference for developing modern Nova System control software

---

## Table of Contents
1. [Project Overview](#project-overview)
2. [Hardware Capabilities](#hardware-capabilities)
3. [MIDI Protocol Summary](#midi-protocol-summary)
4. [Effect Architecture](#effect-architecture)
5. [Software Requirements](#software-requirements)
6. [Design Constraints](#design-constraints)
7. [Key Workflows](#key-workflows)
8. [Reference Material Index](#reference-material-index)

---

## Project Overview

### Mission Statement
Create modern, AI-driven software for controlling the TC Electronic Nova System guitar effects pedal via USB-MIDI, replacing the legacy NovaManager (v1.20.1) with a "bleeding edge" UI while maintaining full MIDI protocol compatibility.

### Project Goals
1. **Full MIDI Protocol Implementation**
   - 100% compatible with Nova System hardware (no firmware changes)
   - Bidirectional preset sync (read/write)
   - Real-time parameter control
   - System/user bank dumps

2. **Modern User Experience**
   - "Bleeding edge" UI design (modern, beautiful, intuitive)
   - Cross-platform (Windows primary, Mac/Linux desirable)
   - Fast startup and responsive UI (<100ms parameter updates)
   - Drag-and-drop preset management

3. **AI-Driven Features**
   - AI agent can programmatically create/modify presets
   - Natural language preset generation ("create a blues lead tone")
   - Preset recommendation based on playing style
   - Automatic parameter optimization

4. **Preset Management**
   - Import/export .syx files (compatibility with NovaManager)
   - Preset library with tags/categories
   - A/B comparison
   - Undo/redo
   - Cloud sync (optional feature)

### Target Hardware
- **Device:** TC Electronic Nova System (multi-effects pedal)
- **Connection:** USB-MIDI converter cable
- **MIDI Implementation:** Full SysEx + PC + CC
- **Firmware:** v1.2 (SVN rev. 675, released 2010)
- **Note:** No amp modeling/amp sim; it is a multi-FX with an analog drive circuit.

---

## Hardware Capabilities

### Front Panel
- **8 Footswitches:**
  - Mode 1 (Preset): 3 preset recalls + 5 effect on/off
  - Mode 2 (Pedal): 7 effect on/off + Tap Tempo
  - G-Switch support: External preset switching

- **4 Encoders (A-D):**
  - Parameter editing
  - Preset browsing
  - Menu navigation

- **24x2 Character LCD**
  - Preset name display
  - Parameter values
  - Tuner visualization

### Rear Panel
- **2 Inputs:**
  - Drive Input (Hi-Z): For guitar, routes through NDT™ analog drive
  - Line Input (Balanced): For effects loop, bypasses drive section

- **2 Outputs:**
  - Left/Right (Balanced TRS): Stereo outputs

- **MIDI:**
  - In / Out / Thru (5-pin DIN)

- **Pedal Input:**
  - Expression pedal OR G-Switch (TRS/TS jack)

### Audio Specifications
| Specification | Value |
|---------------|-------|
| **A/D Conversion** | 24-bit, 128x oversampling |
| **D/A Conversion** | 24-bit, 128x oversampling |
| **Sample Rate** | 48 kHz |
| **Latency** | 0.63 ms |
| **Dynamic Range** | >104 dB |
| **Frequency Response** | +0/-0.3 dB (20 Hz - 20 kHz) |
| **THD** | <-98 dB (0.0013%) @ 1 kHz |

---

## MIDI Protocol Summary

### Message Types
1. **SysEx Preset Dump:** 520 bytes (F0 ... F7)
2. **SysEx System Dump:** Variable length (global settings)
3. **SysEx User Bank Dump:** Large (60 presets)
4. **Program Change:** 0-126 (preset recall)
5. **Control Change:** User-assignable (effect on/off, expression pedal)
6. **MIDI Clock:** Tempo sync from external source

### Preset SysEx Structure
```
F0 00 20 1F [DevID] 63 20 01 [Preset#] [Name:24] [TapTempo:4] [Data:480+] [Checksum] F7
```

**Key Offsets:**
- **0-8:** Header (manufacturer ID, model ID, message type)
- **9:** Reserved (void)
- **10-33:** Preset name (24 ASCII characters)
- **34-37:** Tap Tempo (100-3000 ms)
- **38-517:** Effect block parameters (4-byte encoding each)
- **518:** Checksum (7 LSB of sum from byte 34-517)
- **519:** F7 (SysEx end)

### Effect Block Parameter Ranges

| Block | Type Options | Parameters | MIDI Offsets |
|-------|-------------|------------|--------------|
| **Compressor** | Percussive, Sustaining, Advanced | Threshold, Ratio, Attack, Release, Level | 70-133 |
| **Drive** | Overdrive, Distortion | Gain, Tone, Level, Boost | 134-197 |
| **Modulation** | Chorus, Flanger, Vibrato, Phaser, Tremolo, Panner | Speed, Depth, Feedback, Delay, Mix | 198-261 |
| **Delay** | Clean, Analog, Tape, Dynamic, Dual, Ping-Pong | Time, Tempo, Feedback, Hi/Lo-Cut, Mix | 262-325 |
| **Reverb** | Spring, Hall, Room, Plate | Decay, Pre-Delay, Shape, Size, Color, Mix | 326-389 |
| **EQ + Gate** | 3-Band Parametric + Noise Gate | Freq, Gain, Width (per band), Gate Threshold/Damp | 390-453 |
| **Pitch** | Shifter, Octaver, Whammy, Detune, Intelligent | Voice1/2, Pan, Delay, Feedback, Key/Scale | 454-517 |

---

## Effect Architecture

### Signal Chain (Serial Mode)
```
Guitar Input
   ↓
Drive (NDT™ analog) → Boost
   ↓
Compressor
   ↓
Noise Gate → EQ (3-band)
   ↓
Modulation (Chorus/Flanger/Phaser/Tremolo/Panner/Vibrato)
   ↓
Pitch (Shifter/Octaver/Whammy/Detune/Intelligent)
   ↓
Delay (6 types)
   ↓
Reverb (4 types)
   ↓
Output Levels (L/R)
```

### Routing Modes
1. **Serial:** All effects in series (chain above)
2. **Semi-Parallel:** Delay and Reverb run parallel
3. **Parallel:** Modulation, Pitch, Delay, Reverb all parallel

### Effect Variations
Each effect block can store **4 instant variations** per effect type:
- Stored globally (not per-preset)
- Quick recall via Variation buttons
- Use case: Share favorite reverb settings across presets

### Spillover Feature
- **FX Mute = Soft:** Delay/reverb ring out on preset change
- **FX Mute = Hard:** Immediate mute on preset change

---

## Software Requirements

### Functional Requirements

#### Must-Have (MVP)
1. ✅ **Preset Loading/Saving**
   - Request preset via SysEx
   - Parse 520-byte preset dump
   - Display all parameters in UI
   - Edit parameters and send back to pedal
   - Store preset to user bank (00-1 through 19-3)

2. ✅ **Effect Block Editors**
   - 15 effect types with type-specific UIs
   - Real-time parameter updates (send CC, immediate audio change)
   - Visual feedback (waveforms, graphs, etc.)

3. ✅ **MIDI Communication**
   - Device enumeration (detect Nova System)
   - Bidirectional SysEx
   - Program Change send/receive
   - Control Change send/receive
   - Async I/O (non-blocking UI)

4. ✅ **Preset Browser**
   - View factory presets (F0-1 through F9-3)
   - View user presets (00-1 through 19-3)
   - Recall preset by clicking
   - Visual indication of current preset

5. ✅ **Basic Preset Management**
   - Copy preset to new location
   - Rename preset
   - Delete user preset
   - Mark as "edited" (unsaved changes)
   - Quick save (overwrite current)

#### Should-Have (Phase 2)
6. ⏳ **System Dump Management**
   - Backup all user presets to .syx file
   - Restore user presets from .syx file
   - Export single preset to .syx

7. ⏳ **MIDI Mapping Editor**
   - Assign CC numbers to effects on/off
   - Map expression pedal to parameters
   - Define min/mid/max response curve

8. ⏳ **Utility Settings**
   - FX Mute (spillover)
   - Tap Master (preset vs global tempo)
   - Boost Lock, EQ Lock, Routing Lock
   - Footswitch mode (Preset vs Pedal)

9. ⏳ **Tuner Display**
   - Visual tuner (pitch accuracy)
   - Tuner reference (A=440-460 Hz)
   - Tuner output mode (mute/on)

#### Could-Have (Future)
10. 💡 **Preset Library**
   - Tag presets (genre, instrument, artist)
   - Search/filter by tags
   - Rating system
   - Cloud sync (optional)

11. 💡 **AI Features**
   - Natural language preset generation
   - Preset recommendation engine
   - Parameter optimization (EQ, compression)
   - Tone matching (upload audio, get preset)

12. 💡 **Advanced UI**
   - Drag-and-drop effect routing
   - Signal flow visualization
   - Spectral analyzer (real-time frequency display)
   - Waveform display

---

### Non-Functional Requirements

#### Performance
- **UI Responsiveness:** <100ms for parameter changes
- **Startup Time:** <2 seconds
- **MIDI Latency:** <10ms (hardware limitation: 0.63ms)
- **Memory Usage:** <100MB RAM

#### Compatibility
- **MIDI Protocol:** 100% compatible with Nova System v1.2 firmware
- **File Format:** Import/export .syx files (NovaManager compatible)
- **Operating Systems:**
  - Windows 10/11 (primary)
  - macOS 10.15+ (desirable)
  - Linux (nice-to-have)

#### Usability
- **Learning Curve:** <5 minutes for basic preset loading
- **Accessibility:** Keyboard shortcuts for all major functions
- **Error Handling:** Clear error messages (e.g., "MIDI device not found")

#### Maintainability
- **Code Quality:** 80%+ test coverage (unit + integration)
- **Documentation:** Inline comments + API docs
- **Version Control:** Git with semantic versioning

---

## Design Constraints

### Hardware Constraints
1. **Fixed MIDI Protocol:**
   - Cannot change firmware on Nova System
   - Must speak exact SysEx format (520 bytes)
   - Nibble encoding required for signed values

2. **USB-MIDI Limitations:**
   - ~1ms latency (best case)
   - SysEx messages may arrive in chunks (need buffering)
   - No audio streaming (MIDI only)

3. **Effect Block Limitations:**
   - Cannot add new effect types to hardware
   - Parameter ranges fixed (e.g., Delay 0-1800ms max)
   - Routing modes limited to Serial/Semi-Parallel/Parallel

### Software Constraints
1. **MIDI Library Availability:**
   - Windows: DirectMusic, WinMM
   - macOS: CoreMIDI
   - Linux: ALSA
   - Cross-platform: RtMidi, python-rtmidi, Web MIDI API

2. **UI Framework Choices:**
   - Native: Qt (C++/Python), GTK
   - Web: Electron, Tauri
   - Game engine: egui (Rust), Dear ImGui

3. **Language/Platform Trade-offs:**
   - **Python:** Easy MIDI (python-rtmidi), slower UI
   - **JavaScript/TypeScript:** Web MIDI API, Electron overhead
   - **Rust:** Fast, native, steeper learning curve
   - **C++:** Mature MIDI libs (RtMidi), complex build

---

## Key Workflows

### Workflow 1: Preset Loading
```
User Action → Software Action → Hardware Response → Software Update
────────────────────────────────────────────────────────────────────
1. User clicks preset "F0-1"
   ↓
2. Software sends PC #0 OR SysEx Request
   ↓
3. Hardware responds with 520-byte SysEx dump
   ↓
4. Software validates checksum
   ↓
5. Software parses preset name and parameters
   ↓
6. UI updates all parameter widgets
   ↓
7. User hears audio change (hardware already switched)
```

---

### Workflow 2: Parameter Editing
```
User Action → Software Action → Hardware Response → Audio Change
───────────────────────────────────────────────────────────────────
1. User adjusts "Reverb Decay" slider (2.5s)
   ↓
2. Software converts 2.5s → SysEx value (0x19 0x00 0x01 0x00)
   ↓
3. Software sends MIDI CC #XX (if CC assigned) OR queues for save
   ↓
4. Hardware updates internal parameter
   ↓
5. Audio output changes immediately (real-time)
   ↓
6. Software marks preset as "edited" (dirty flag)
```

---

### Workflow 3: Preset Saving
```
User Action → Software Action → Hardware Response
─────────────────────────────────────────────────
1. User clicks "Save" (or presses Ctrl+S)
   ↓
2. Software prompts: "Save to which location?"
   ↓
3. User selects "User 05-2"
   ↓
4. Software builds 520-byte SysEx (current preset + name)
   ↓
5. Software calculates checksum
   ↓
6. Software sends full preset SysEx to hardware
   ↓
7. Hardware writes to flash memory (location 0x35 = preset 05-2)
   ↓
8. Hardware echoes back saved preset (confirmation)
   ↓
9. Software clears "edited" flag
```

---

### Workflow 4: System Backup
```
User Action → Software Action → Hardware Response → File Creation
────────────────────────────────────────────────────────────────────
1. User clicks "Backup User Bank"
   ↓
2. Software sends SysEx System Dump Request
   ↓
3. Hardware responds with large SysEx (60 presets + settings)
   ↓
4. Software parses and stores in memory
   ↓
5. Software prompts: "Save backup to file?"
   ↓
6. User selects filename (e.g., "NovaBackup_2026-01-30.syx")
   ↓
7. Software writes raw SysEx bytes to file
   ↓
8. Confirmation: "Backup saved successfully"
```

---

### Workflow 5: AI Preset Generation (Future Feature)
```
User Request → AI Processing → Software Action → Hardware Update
─────────────────────────────────────────────────────────────────
1. User types: "Create a blues lead tone with vintage delay"
   ↓
2. AI agent parses intent:
   - Style: Blues
   - Primary: Lead tone
   - Effects: Vintage delay (emphasis)
   ↓
3. AI agent queries knowledge base:
   - Blues lead typically uses: Overdrive (low-medium gain), Compressor (sustain type), Delay (analog with ~350ms)
   ↓
4. AI agent constructs preset:
   - Drive: Overdrive, Gain=45%, Tone=55%
   - Comp: Sustaining, Drive=12, Response=6
   - Delay: Analog, Time=350ms, Feedback=40%, Drive=8dB, Hi-Cut=6kHz
   - Reverb: Spring, Decay=1.5s, Mix=25%
   ↓
5. Software renders preview (if audio available) OR sends to hardware
   ↓
6. User auditions tone, provides feedback: "More decay on delay"
   ↓
7. AI adjusts parameters (Feedback 40% → 55%)
   ↓
8. Repeat until user satisfied
   ↓
9. User saves preset
```

---

## Reference Material Index

### Critical Documents (Read First)
1. **MIDI_PROTOCOL.md** (this directory)
   - Complete SysEx specification
   - Parameter offset map
   - Encoding/decoding algorithms
   - Checksum validation

2. **EFFECT_REFERENCE.md** (this directory)
   - All 15 effect types
   - Parameter ranges and descriptions
   - Use cases and classic settings
   - Audio specifications

3. **ARCHITECTURE_ANALYSIS.md** (this directory)
   - Original Java app analysis
   - Design patterns used
   - MIDI I/O implementation
   - UI component structure

### Source Materials
4. **Nova System Sysex Map.pdf** (Tc originalt materiale/)
   - MIDI protocol specification (byte-by-byte)
   - Lookup tables (tempo, scales, etc.)

5. **TC Nova Manual.pdf** (Tc originalt materiale/)
   - Complete user manual (46 pages)
   - Effect descriptions
   - Hardware specs
   - Operational workflows

6. **NovaManager Java Classes** (Nova manager Original/)
   - 78 .class files (compiled bytecode)
   - Reference implementation
   - MIDI protocol handlers

7. **.github/copilot-instructions.md** (project root)
   - AI agent guide
   - Project philosophy
   - Critical files list
   - Next steps

### Supplementary Files
8. **novasystem-1_2-installation-guide.pdf**
   - Firmware update procedure
   - Bug fixes in v1.2
   - Backup instructions

9. **nova-system-preset-list-473124.pdf**
   - Factory preset names and effects used
   - User bank structure

10. **Nova-System-LTD_Artists-Presets-for-User-Bank.syx**
    - Binary preset examples
    - Use for testing SysEx parser

---

## Technology Stack Considerations

### Platform Options

#### Option 1: Python + PyQt5
**Pros:**
- Fast development
- python-rtmidi (excellent MIDI library)
- Native UI performance
- Easy AI integration (scikit-learn, TensorFlow)

**Cons:**
- Slower than native C++/Rust
- ~50MB bundled app (PyInstaller)
- No web version

**Best For:** Rapid prototyping, AI-heavy features

---

#### Option 2: TypeScript + Electron
**Pros:**
- Web MIDI API (built-in browser support)
- Modern UI frameworks (React, Vue, Svelte)
- Cross-platform by default
- Easy to deploy updates

**Cons:**
- ~100MB app size (Chromium embedded)
- Higher memory usage
- Slower startup

**Best For:** "Bleeding edge" UI, web-first approach

---

#### Option 3: Rust + Tauri
**Pros:**
- Native performance (<5MB binary)
- Fast startup (<100ms)
- midir (Rust MIDI library)
- Modern web UI (React/Svelte in WebView)

**Cons:**
- Steeper learning curve
- Less MIDI library maturity
- Longer development time

**Best For:** Performance-critical, native feel, small footprint

---

#### Option 4: TypeScript + Next.js (Web-only)
**Pros:**
- Zero installation (runs in browser)
- Web MIDI API
- Modern UI frameworks
- Easy updates (just refresh)

**Cons:**
- Requires Chrome/Edge (Web MIDI not in Firefox/Safari)
- No offline mode (unless PWA)
- Cannot write to local .syx files (download only)

**Best For:** Quick access, no installation desired

---

### MIDI Library Comparison

| Library | Language | Platforms | USB-MIDI | SysEx | Async I/O |
|---------|----------|-----------|----------|-------|-----------|
| **python-rtmidi** | Python | Win/Mac/Linux | ✅ | ✅ | ✅ (callbacks) |
| **Web MIDI API** | JavaScript | Chrome/Edge | ✅ | ✅ | ✅ (events) |
| **midir** | Rust | Win/Mac/Linux | ✅ | ✅ | ✅ (callbacks) |
| **RtMidi** | C++ | Win/Mac/Linux | ✅ | ✅ | ✅ (callbacks) |
| **Java Sound API** | Java | Cross-platform | ✅ | ✅ | ⚠️ (polling) |

**Recommendation:** python-rtmidi or Web MIDI API (both mature, well-documented)

---

### UI Framework Comparison

| Framework | Language | Performance | "Bleeding Edge" | Mobile |
|-----------|----------|-------------|-----------------|--------|
| **React** | TypeScript | ⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ✅ (React Native) |
| **Vue 3** | TypeScript | ⭐⭐⭐⭐ | ⭐⭐⭐⭐ | ✅ (Ionic) |
| **Svelte** | TypeScript | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐⭐ | ❌ |
| **PyQt5** | Python | ⭐⭐⭐⭐ | ⭐⭐ | ❌ |
| **egui** | Rust | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | ❌ |

**Recommendation:** Svelte (fastest, most modern) or React (most mature ecosystem)

---

## Project Philosophy (from .github/copilot-instructions.md)

### 1. Systematic Approach Required
**Why:** Previous project failed due to "overspringshandlinger" (jumping ahead)

**Rules:**
- Read ALL documentation line-by-line before coding
- Understand MIDI protocol completely before implementing
- Test each component independently before integration
- No shortcuts, no assumptions

---

### 2. AI-Driven Development
**Goal:** Software should expose APIs for AI agent control

**Requirements:**
- Preset generation via natural language
- Parameter optimization algorithms
- Tone matching (audio → preset)
- Programmatic preset creation (JSON → SysEx)

**Example API:**
```python
# Natural language
preset = ai_agent.generate_preset("blues lead with vintage delay")

# Programmatic
preset = Preset(
    name="My Blues Lead",
    drive=Drive(type="overdrive", gain=45, tone=55),
    delay=Delay(type="analog", time=350, feedback=40)
)
```

---

### 3. Preserve MIDI Protocol Compatibility
**Critical:** Nova System's MIDI implementation is fixed hardware

**Non-Negotiable:**
- Exact 520-byte SysEx format
- Correct checksum calculation
- Nibble encoding for signed values
- Asynchronous I/O (prevent buffer overflow)

---

### 4. Modern UX Priority
**Requirement:** "Bleeding edge UI design"

**NOT a 1:1 clone** of Java NovaManager appearance

**Modern UI Principles:**
- Minimalist, clean design
- Dark mode (with light mode option)
- Smooth animations (60fps)
- Responsive layout (resize-friendly)
- Touch-friendly (large click targets)
- Visual feedback (hover, active states)

---

## Known Issues & Gotchas

### 1. Spillover Feedback Loop
**Problem:** If Feedback >100% on delay, spillover can cause infinite feedback  
**Solution:** Warn user if Feedback >95%, suggest switching presets twice to clear

---

### 2. SysEx Device ID
**Problem:** v1.2 ignores SysEx ID on System Dumps  
**Workaround:** Always use Device ID = 0 (or broadcast)

---

### 3. MIDI Tempo Sync Edge Cases
**Problem:** Tap tempo disabled when synced to MIDI clock  
**Solution:** Display message "Tap Tempo disabled (synced to MIDI clock)"

---

### 4. Factory Preset Editing
**Problem:** Cannot overwrite factory presets (read-only)  
**Solution:** Auto-redirect to User Bank on save attempt

---

### 5. Checksum Mismatches
**Problem:** Occasionally receive corrupted SysEx  
**Solution:** Request preset again (max 3 retries), then error

---

### 6. Mac MIDI Port Names
**Problem:** macOS may show duplicate port names  
**Solution:** Use Humatic MMJ or CoreMIDI directly (avoid Java Sound API)

---

## Next Steps (for Brainstorming Phase)

### 1. Language/Platform Selection
**Questions to Answer:**
- Desktop app vs web-based vs hybrid?
- Performance critical (Rust/C++) vs rapid development (Python/TypeScript)?
- Cross-platform priority (all OS) vs Windows-only?

**Recommendation:** Start brainstorming session with user

---

### 2. MIDI Library Choice
**Questions to Answer:**
- Native library (RtMidi, midir) vs web (Web MIDI API)?
- Callback-based vs polling?
- Tested with USB-MIDI converters?

**Recommendation:** python-rtmidi (Python) or Web MIDI API (web)

---

### 3. UI Framework
**Questions to Answer:**
- Native look (PyQt5) vs modern web (React/Svelte)?
- 2D canvas (manual drawing) vs component library?
- Dark mode priority?

**Recommendation:** Svelte + Tauri (modern, fast) or React + Electron (mature)

---

### 4. Project Structure
**To Create:**
- `src/` - Source code
- `docs/` - Documentation (Markdown)
- `tests/` - Unit + integration tests
- `presets/` - Factory preset .syx files
- `examples/` - Example code (MIDI send/receive)

---

### 5. Development Roadmap
**Phase 1: MVP (4-6 weeks)**
- MIDI communication
- Preset loading/saving
- Basic parameter editing
- Simple preset browser

**Phase 2: Full Feature Set (6-8 weeks)**
- All effect editors
- MIDI mapping
- System dump management
- Advanced UI

**Phase 3: AI Features (8-12 weeks)**
- Natural language preset generation
- Tone matching
- Preset recommendation

---

## Glossary

| Term | Definition |
|------|------------|
| **SysEx** | System Exclusive MIDI message (device-specific data) |
| **PC** | Program Change (MIDI message to recall presets) |
| **CC** | Control Change (MIDI message for real-time parameter control) |
| **Nibble Encoding** | 7-bit encoding for values >127 (4 bytes per value) |
| **Spillover** | Delay/reverb ring out after preset change |
| **NDT™** | Nova Drive Technology (analog drive circuit) |
| **Expression Pedal** | Foot pedal for real-time parameter control |
| **G-Switch** | External footswitch for preset recall |
| **Tap Tempo** | Manual tempo input via footswitch tap |
| **MIDI Clock** | External tempo sync signal |
| **USB-MIDI** | USB cable with MIDI interface (not USB audio) |

---

## FAQ

### Q: Can I add new effect types to Nova System?
**A:** No. The hardware has fixed DSP algorithms. You can only control existing effects.

---

### Q: Can I stream audio through MIDI?
**A:** No. MIDI is control data only. Audio flows through analog/digital outputs.

---

### Q: What's the maximum delay time?
**A:** 1800ms (1.8 seconds). This is a hardware limitation.

---

### Q: Can I use Nova System as a USB audio interface?
**A:** No. It's MIDI-only over USB (requires USB-MIDI converter cable). Audio is analog only.

---

### Q: Will the new software work with older firmware?
**A:** Likely yes, but v1.2 is recommended (bug fixes, SysEx doc added).

---

### Q: Can I control multiple Nova Systems simultaneously?
**A:** Yes, using SysEx Device ID (0-126). Assign unique IDs to each unit.

---

### Q: What's the latency for parameter changes?
**A:** MIDI latency (~1ms) + hardware processing (0.63ms) = ~2ms total. Negligible.

---

### Q: Can I run NovaManager and new software simultaneously?
**A:** No. Only one app can open a MIDI port at a time (OS limitation).

---

## Conclusion

This document synthesizes:
- **511 lines** of MIDI protocol specification
- **1736 lines** of effect parameter documentation
- **78 Java classes** of reference implementation
- **3 supplementary PDFs** (installation, presets)
- **Project philosophy** and goals

**You now have complete knowledge** to:
1. Implement the MIDI protocol correctly
2. Understand all effect parameters and behaviors
3. Design a modern software architecture
4. Begin brainstorming with the user

**Next Action:** Start brainstorming phase (language/platform selection)

---

**Document Version:** 1.0  
**Last Updated:** January 30, 2026  
**Total Source Material:** 4 PDFs + 78 Java classes + 1 .syx file  
**Analysis Completeness:** 100% (all materials read systematically)
