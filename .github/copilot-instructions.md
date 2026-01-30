# TC Electronic Nova System - AI Coding Agent Instructions

## Project Overview
This project aims to create modern software for controlling the TC Electronic Nova System guitar effects pedal via USB-MIDI. The new software will replace the legacy Java-based NovaManager (v1.20.1) with a "bleeding edge" UI design while maintaining full MIDI protocol compatibility.

**Target Hardware:** TC Electronic Nova System multi-effects pedal  
**Connection:** USB-MIDI converter cable  
**Primary Goal:** Enable AI-driven preset programming and comprehensive pedal control

## Project Structure
```
d:\Tc electronic projekt\
├── Nova manager Original/       # Legacy Java app (REFERENCE ONLY - compiled .class files)
│   └── NovaManager/             # 78 classes documenting MIDI protocol implementation
├── Tc originalt materiale/      # Documentation and MIDI data
│   ├── Nova System Sysex Map.pdf          # CRITICAL: MIDI protocol specification
│   ├── TC Nova Manual.pdf                 # Complete effects/parameters reference
│   ├── Nova-System-LTD_Artists-Presets-for-User-Bank.syx  # Binary preset examples
│   └── NovaSystem_PC_SWUpdater-1_2_02-R688/
└── Nyt program til Nova/        # NEW SOFTWARE GOES HERE (currently empty)
```

## Critical Architecture Knowledge (Reverse-Engineered from Java App)

### MIDI Communication Layers
The legacy app implements **3-tier platform-specific MIDI abstraction**:

1. **MidiDefaultInterface.class** - Java Sound API (cross-platform fallback)
2. **MidiMacInterface.class** - Humatic MMJ library (Mac CoreMIDI native)
3. **RWMidiInterface.class** - RWMidi library (primary implementation)

**Design Pattern:** Strategy pattern with factory selection based on platform capabilities.

**Key Insight:** All MIDI I/O must handle:
- Nibble encoding/decoding (see `Nibble.class`)
- SysEx message parsing with checksums
- Asynchronous input buffering (`MidiInputBuffer` inner classes)

### Effect Block Architecture (15 Types)
Effects are implemented as subclasses of abstract `Block.class`:

**Signal Chain Order:**
```
Drive → Boost → Comp → Gate → EQ → Modulation → Pitch → Delay → Reverb
```

**Additional Blocks:** `Levels.class`, `Routing.class`, `Global.class`, `Tuner.class`, `TapTempo.class`, `Pedal.class`

**Design Patterns:**
- **Template Method:** `Block.class` defines common parameter structure
- **Observer:** Event system for real-time parameter updates (`Block$1.class` nested listeners)
- **Command:** `SendCC.class` and `SendPC.class` for MIDI message queueing

### Data Management Core Classes

**Patch.class** - Single preset representation
- Contains all 15 effect block states
- Handles serialization to/from SysEx format
- Nested `Patch$1.class` for async loading

**SystemDump.class** - Full pedal memory dump
- Bank/preset organization
- Bulk data transfer handling

**Constants.class** - MIDI protocol constants
- CC numbers for each parameter
- SysEx message templates
- Parameter value ranges

**CurrentPreset.class** - Singleton pattern for active preset state

### MIDI Mapping System
**Bidirectional mapping classes:**
- `MidiMapIn.class` / `MidiMapInRow.class` - Incoming MIDI → Pedal actions
- `MidiMapOut.class` / `MidiMapOutRow.class` - Pedal events → Outgoing MIDI
- `MidiSetUp.class` - MIDI port configuration

**Purpose:** Allows external MIDI controllers to control Nova System parameters.

## Development Constraints & Requirements

### Language/Platform Selection (NOT YET DECIDED)
**User requires brainstorming session** before implementation begins. Consider:
- Modern UI frameworks with "bleeding edge" design capabilities
- Cross-platform MIDI library availability (Windows primary, but consider Mac/Linux)
- AI-agent programming interface requirements

### MIDI Protocol Implementation Rules

1. **Always use Nibble encoding** for parameter values in SysEx messages
   - Reference: `Nibble.class` - encodes bytes as 7-bit pairs
   - Example: Value 0x8F → [0x08, 0x0F]

2. **SysEx Message Structure** (infer from Constants.class):
   ```
   F0 [Manufacturer ID] [Device ID] [Command] [Data...] [Checksum] F7
   ```

3. **Parameter Updates** via MIDI CC messages:
   - Each effect parameter has unique CC number (defined in Constants.class)
   - Must send CC to active preset, then save if permanent change desired

4. **Asynchronous I/O Required:**
   - Input buffer to handle unsolicited SysEx dumps from pedal
   - Queue outgoing messages to prevent MIDI buffer overflow

### UI Design Patterns (from legacy app)

**Main Layout:**
- Tabbed interface per effect block (Drive, Delay, Reverb, etc.)
- Real-time parameter visualization
- Preset browser with bank/number selection
- MIDI map editor for external controller integration

**Data Flow:**
```
UI Widget → Effect Block Object → MIDI Interface → USB-MIDI → Nova System Pedal
                ↑                        ↓
                └────────────────────────┘
             (Bidirectional real-time sync)
```

### Cross-Platform Considerations (from Java legacy)

**Mac-Specific Optimizations:**
- `NovaManagerMac.class` - Uses Quaqua Look & Feel for native macOS appearance
- Platform-specific MIDI interface selection logic in main class
- File paths: Use platform-agnostic separators

**Windows Primary Target:**
- Ensure USB-MIDI driver compatibility testing
- NovaManager worked on Windows with Java Sound API fallback

## Key Workflows

### Preset Loading Workflow
```
1. User selects preset number (0-127) and bank (A/B/C/D)
2. Send MIDI Program Change (PC) message
3. Pedal responds with full SysEx dump of preset data
4. Parse SysEx → Populate 15 effect block objects
5. Update UI to reflect all parameter values
```

### Preset Editing & Saving
```
1. User adjusts parameter in UI
2. Send MIDI CC to update pedal in real-time (immediate audio change)
3. Mark preset as "edited" (unsaved changes)
4. On save: Send full preset SysEx dump to pedal memory location
5. Pedal confirms with echo of stored data
```

### System Dump Import/Export
```
Export: Request all presets → Receive bulk SysEx → Save to .syx file
Import: Read .syx file → Parse presets → Send each to pedal → Verify
```

## Critical Files for Reference

**MUST READ BEFORE IMPLEMENTING MIDI:**
- `Tc originalt materiale/Nova System Sysex Map.pdf` - MIDI protocol specification
- `Nova manager Original/NovaManager/nova/Constants.class` - CC numbers, SysEx formats
- `Nova manager Original/NovaManager/nova/MidiInterface.class` - I/O interface contract

**For Effect Parameter Specifications:**
- `TC Nova Manual.pdf` - All effect types, parameter ranges, and behaviors
- Individual Block.class files (Drive.class, Delay.class, etc.) - Parameter structures

**Example Data:**
- `Nova-System-LTD_Artists-Presets-for-User-Bank.syx` - Binary MIDI preset examples to validate parsing

## Dependencies (Legacy Java App Used)

**MIDI Libraries:**
- RWMidi - Primary cross-platform MIDI I/O
- Humatic MMJ - Mac-specific CoreMIDI wrapper
- Java Sound API - Built-in fallback

**UI Frameworks:**
- Quaqua Look and Feel 7.3.4 (Mac native appearance)
- Apache Batik (SVG rendering for graphics)

**Utilities:**
- NanoXML - XML parsing (likely for preset metadata)
- Base64 encoding (possible preset sharing feature)

## Project Philosophy

1. **Systematic Approach Required:** This project failed previously due to "overspringshandlinger" (jumping ahead). Read ALL documentation line-by-line before implementing.

2. **AI-Driven Development:** The new software should expose APIs suitable for AI agent control - consider how an AI will programmatically create/modify presets.

3. **Preserve MIDI Protocol Compatibility:** The Nova System pedal's MIDI implementation is fixed hardware. New software MUST speak the exact protocol documented in legacy app.

4. **Modern UX Priority:** "Bleeding edge UI design" is explicitly requested - this is NOT a 1:1 clone of the Java app's appearance.

## Danish Language Context
User is Danish; technical communication may occur in Danish. Key terms:
- "pedal" = pedal
- "preset" = preset/forudindstilling  
- "effekt" = effect
- "parameter" = parameter

## Next Steps for AI Agents

1. **DO NOT START CODING** until language/platform brainstorming is complete with user
2. Extract MIDI protocol specification from `Nova System Sysex Map.pdf` (currently inaccessible via PDF tools - user must provide text extract)
3. Design modern architecture matching new platform choice
4. Implement MIDI layer first (testable without full UI)
5. Build effect block data models
6. Create preset management system
7. Develop UI layer last

## Questions to Resolve Before Implementation

- [ ] Target programming language and UI framework?
- [ ] Desktop app vs web-based vs hybrid (Electron/Tauri)?
- [ ] MIDI library choice for new platform?
- [ ] AI control interface design (REST API, CLI, embedded scripting)?
- [ ] Preset storage format (proprietary vs standard .syx files)?
