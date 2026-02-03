# Changelog

All notable changes to Nova System Manager will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [1.0.0] - 2026-02-03

### Added

#### Core MIDI Features
- Full SysEx protocol implementation for TC Electronic Nova System
- Bidirectional preset transfer (download from/upload to hardware)
- User Bank dump support (60 presets, ~31KB)
- System dump support (global settings, ~1KB)
- Single preset dump support (520 bytes)
- Automatic checksum validation for data integrity
- MIDI port discovery and connection management
- Device ID configuration (0-126 + broadcast support)

#### Preset Management
- Download individual presets from Nova System
- Upload individual presets to Nova System
- Download entire User Bank (60 presets)
- Upload entire User Bank (60 presets)
- Preset name editing (24 ASCII characters)
- Preset parameter editing for all effect blocks:
  - Drive (Overdrive, Distortion) - NDT™ analog section
  - Compressor (Percussive, Sustaining, Advanced)
  - EQ + Noise Gate (3-band parametric)
  - Modulation (Chorus, Flanger, Vibrato, Phaser, Tremolo, Panner)
  - Pitch (Shifter, Octaver, Whammy, Detune, Intelligent)
  - Delay (Clean, Analog, Tape, Dynamic, Dual, Ping-Pong)
  - Reverb (Spring, Hall, Room, Plate)
- Global preset parameters:
  - Tap Tempo (100-3000 ms)
  - Signal Routing (Serial, Semi-Parallel, Parallel)
  - Output Levels (Left/Right, -100 to 0 dB)
- Effect on/off state management
- Preset copy/paste functionality

#### System Configuration
- System settings editor (global parameters)
- CC MIDI mapping editor:
  - View all CC assignments (CC# 0-127)
  - Edit CC parameter assignments
  - Save CC mapping to hardware
- Expression pedal configuration:
  - Pedal parameter assignment
  - Min/Mid/Max response curve mapping
  - Pedal calibration support
- Program Map editing (MIDI PC remapping)
- Global utility settings

#### File Operations
- Import presets from .syx files (SysEx format)
- Export presets to .syx files
- Import User Bank from .syx files
- Export User Bank to .syx files
- Import system dumps
- Export system dumps
- Backward compatibility with legacy NovaManager v1.20.1 files
- Drag-and-drop file import support

#### User Interface
- Modern, clean UI design built with Avalonia
- Dark theme (default)
- Light theme option
- Theme toggle in settings
- Responsive layout (adapts to window size)
- Professional color scheme:
  - Dark: #1E1E1E background, #2D2D2D panels
  - Light: Clean, high-contrast colors
- Consistent 8px grid spacing
- 4px/8px border radius for modern look
- Smooth animations (100-200ms transitions)
- Hover effects on interactive elements

#### Keyboard Shortcuts
- `Ctrl+S` - Save preset
- `Ctrl+Z` - Undo
- `Ctrl+Y` - Redo
- `Ctrl+C` - Copy preset
- `F5` - Refresh MIDI ports
- `Escape` - Cancel/Close dialog

#### Developer Features
- Clean Architecture implementation
- Domain-Driven Design (DDD)
- MVVM pattern with ReactiveUI
- Comprehensive unit test coverage (308+ tests):
  - Domain layer: 153 tests
  - Application layer: 73 tests
  - Presentation layer: 64 tests
  - Infrastructure layer: 12 tests
  - MIDI layer: 6 tests
- Integration tests for end-to-end flows
- Detailed logging system (Serilog)
- Error handling with user-friendly messages
- Dependency injection container
- Type-safe parameter enums
- Value objects for domain modeling

#### Documentation
- Comprehensive README with quick start guide
- `EFFECT_REFERENCE.md` - Complete effect parameter documentation
- `MIDI_PROTOCOL.md` - Full SysEx protocol specification
- `MANUAL_TEST_GUIDE.md` - Step-by-step testing procedures
- `PROJECT_KNOWLEDGE.md` - Architecture and design decisions
- `PROGRESS.md` - Development progress tracking
- Module-specific documentation in `tasks/` directory
- Code comments for complex logic

#### Build & Deployment
- .NET 8.0 target framework
- WiX-based MSI installer
- Desktop shortcut creation
- Start Menu integration
- Registry entries for uninstall
- Self-contained deployment option
- CI/CD ready with GitHub Actions support

### Technical Implementation Details

#### MIDI Protocol
- **Manufacturer ID**: `00 20 1F` (TC Electronic)
- **Model ID**: `63` (Nova System)
- **Device ID**: Configurable 0-126 or broadcast (127)
- **SysEx Message Types**:
  - `20 01` - Preset Dump
  - `20 02` - System Dump
  - `20 03` - User Bank Dump
  - `45 01` - Preset Request
  - `45 02` - System Request
  - `45 03` - User Bank Request
- **Parameter Encoding**: 4-byte nibble encoding for 16-bit values
- **Checksum**: 7-bit sum of data bytes (offset 34-517 for presets)

#### Architecture Layers
```
Nova.Presentation/    # Avalonia UI, ViewModels, Views
├── Nova.Application/ # Use Cases, Commands, Queries
│   ├── Nova.Domain/  # Entities, Value Objects, Business Logic
│   │   └── Nova.Common/ # Shared utilities, interfaces
│   └── Nova.Infrastructure/ # DryWetMIDI implementation
│       └── Nova.Midi/  # MIDI abstractions
```

#### Dependencies
- **Avalonia** (11.0.x) - Cross-platform UI framework
- **DryWetMIDI** (7.x) - MIDI communication
- **ReactiveUI** (19.x) - MVVM framework
- **Serilog** (3.x) - Structured logging
- **xUnit** (2.x) - Unit testing
- **FluentAssertions** (6.x) - Test assertions

#### Effect Block Offsets (SysEx)
| Block | Offset | Length | Parameters |
|-------|--------|--------|------------|
| Global | 34 | 36 | Tap Tempo, Routing, Output Levels, Pedal Map |
| Compressor | 70 | 64 | Type, Threshold, Ratio, Attack, Release, Level |
| Drive | 134 | 64 | Type, Gain, Tone, Level, Boost |
| Modulation | 198 | 64 | Type, Speed, Depth, Feedback, Delay, Mix |
| Delay | 262 | 64 | Type, Time, Feedback, Hi-Cut, Lo-Cut, Mix |
| Reverb | 326 | 64 | Type, Decay, Pre-Delay, Size, Color, Mix |
| EQ + Gate | 390 | 64 | Band 1-3 (Freq, Gain, Width), Gate (Threshold, Damp) |
| Pitch | 454 | 64 | Type, Voice 1/2, Pan, Delay, Feedback, Key/Scale |

### Fixed
- N/A (initial release)

### Changed
- N/A (initial release)

### Deprecated
- N/A (initial release)

### Removed
- N/A (initial release)

### Security
- Input validation for all user-provided data
- MIDI message validation (header, checksum)
- Safe parameter range enforcement
- No storage of sensitive data
- No network communication (local MIDI only)

---

## [Unreleased]

### Planned for v1.1.0
- macOS support (native .app bundle)
- Automatic update checking
- In-app update installation
- Cloud preset backup (optional, privacy-respecting)
- Extended undo/redo history
- Preset A/B comparison view
- Preset tagging and search
- Preset preview/audition (if hardware connected)

### Planned for v1.2.0
- AI-powered preset generation (natural language input)
- Preset recommendation engine (based on playing style)
- Batch preset operations
- Advanced preset library management
- Custom effect curve editor
- MIDI learn functionality
- Preset morphing/interpolation
- Real-time parameter automation

### Planned for v2.0.0
- Multi-device support (manage multiple Nova Systems)
- Setlist management for live performance
- Backup/restore complete device state
- Advanced MIDI routing options
- Plugin/extension system
- Preset marketplace integration
- Mobile app companion (iOS/Android)

---

## Version History Summary

| Version | Release Date | Status | Highlights |
|---------|--------------|--------|------------|
| 1.0.0 | 2026-02-03 | ✅ Current | Initial release, full preset management, MIDI SysEx, system editor |
| 1.1.0 | TBD | 📅 Planned | macOS support, auto-update, cloud sync |
| 1.2.0 | TBD | 📅 Planned | AI features, advanced library management |
| 2.0.0 | TBD | 💭 Concept | Multi-device, setlist management, marketplace |

---

## Upgrade Notes

### From Legacy NovaManager v1.20.1
Nova System Manager v1.0.0 is a complete rewrite and replacement for the legacy TC Electronic NovaManager software.

**Migration Steps**:
1. Export your presets from NovaManager to .syx files
2. Install Nova System Manager v1.0.0
3. Import your .syx files into Nova System Manager
4. Verify presets by downloading from your Nova System hardware

**Compatibility**:
- ✅ Full .syx file format compatibility
- ✅ All preset parameters supported
- ✅ System dumps fully supported
- ✅ User Bank dumps work identically

**Improvements over NovaManager**:
- Modern, responsive UI
- Faster MIDI communication
- Better error handling
- Cross-platform support (Windows, future: macOS)
- Active development and community support
- Open architecture for future enhancements

---

## Contributing

We welcome contributions! See the repository README for:
- Code style guidelines
- Development setup instructions
- Testing requirements
- Pull request process

---

## Support

### Getting Help
- 📚 Read the documentation in the repository
- 🐛 Report bugs via GitHub Issues
- 💡 Request features via GitHub Issues
- 💬 Join community discussions

### Useful Links
- **Repository**: [GitHub](https://github.com/mikeegelund-byte/Tc-electronic-projekt)
- **Documentation**: See `EFFECT_REFERENCE.md`, `MIDI_PROTOCOL.md`
- **TC Electronic**: [Official Website](https://www.tcelectronic.com/)

---

**Built with ❤️ for the TC Electronic Nova System community**
