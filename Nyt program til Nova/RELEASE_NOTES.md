# Nova System Manager v1.0.0

**Release Date**: February 3, 2026

## What's New

First official release of **Nova System Manager** - a modern, cross-platform application for managing the TC Electronic Nova System multi-effects pedal.

This release represents a complete reimplementation of the legacy NovaManager software with a modern UI, improved MIDI communication, and professional-grade architecture.

## Key Features

### Preset Management
- **Download/Upload User Bank**: Full bidirectional transfer of all 60 user presets
- **Individual Preset Operations**: Download, upload, and manage individual presets
- **Preset Editing**: Edit all preset parameters including:
  - Effect block settings (Drive, Compressor, Modulation, Delay, Reverb, EQ, Pitch)
  - Global parameters (Tap Tempo, Routing, Output Levels)
  - Effect on/off states
  - Preset names (24 characters)
- **Real-time Preview**: See changes immediately as you edit

### MIDI Communication
- **Full SysEx Protocol Implementation**: 100% compatible with Nova System hardware
- **Bidirectional Sync**: Download from and upload to hardware
- **Multiple Dump Types**:
  - Single preset dumps (520 bytes)
  - User Bank dumps (60 presets, ~31KB)
  - System dumps (global settings, ~1KB)
- **Automatic Checksum Validation**: Ensures data integrity
- **Device Detection**: Automatic MIDI port discovery and connection

### System Configuration
- **Global Settings Management**: Edit system-wide parameters
- **CC MIDI Mapping**: Configure Control Change assignments for real-time parameter control
- **Expression Pedal Setup**:
  - Pedal parameter assignment (which parameter to control)
  - Response curve mapping (Min/Mid/Max values)
  - Calibration support
- **Program Map**: Customize preset recall via MIDI Program Change messages

### File Operations
- **Import/Export**: Full .syx (SysEx) file support
- **Compatibility**: Import presets from legacy NovaManager (v1.20.1)
- **Bulk Operations**: Export entire User Bank or individual presets
- **Preset Library**: Organize and categorize your preset collection

### User Interface
- **Modern Design**: Clean, intuitive interface built with Avalonia UI
- **Dark/Light Theme**: Toggle between dark and light modes
- **Cross-Platform**: Runs on Windows 10/11 (64-bit)
- **Responsive Layout**: Adapts to different window sizes
- **Keyboard Shortcuts**: Efficient workflow with keyboard commands

### Professional Architecture
- **Clean Architecture**: Domain-driven design with clear separation of concerns
- **Comprehensive Testing**: 308+ unit and integration tests
- **Robust Error Handling**: Graceful failure with helpful error messages
- **Logging**: Detailed diagnostic logging for troubleshooting
- **.NET 8.0**: Built on the latest .NET platform for performance and security

## System Requirements

### Minimum Requirements
- **Operating System**: Windows 10 (64-bit) or Windows 11
- **Framework**: .NET 8.0 Runtime (included in installer)
- **RAM**: 512 MB
- **Disk Space**: 50 MB
- **Display**: 1280x720 or higher

### Hardware Requirements
- **TC Electronic Nova System**: Multi-effects pedal (any firmware version)
- **MIDI Interface**: USB-MIDI adapter or audio interface with MIDI I/O
- **USB Cable**: For connecting MIDI interface to computer

## Installation

### Windows Installer (.msi)
1. Download `NovaSystemManager.msi` from the release page
2. Double-click the installer and follow the prompts
3. Launch **Nova System Manager** from the Start Menu or desktop shortcut

### Manual Installation
1. Download the release archive
2. Extract to your preferred location
3. Run `Nova.Presentation.exe`

### First Connection
1. Connect your Nova System to your MIDI interface
2. Connect the MIDI interface to your computer via USB
3. Launch Nova System Manager
4. Select your MIDI input/output ports from the dropdown menus
5. Click **Connect** to establish communication

## Documentation

### Getting Started
- See `MANUAL_TEST_GUIDE.md` for step-by-step usage instructions
- Review `EFFECT_REFERENCE.md` for detailed effect parameter descriptions
- Consult `MIDI_PROTOCOL.md` for technical MIDI implementation details

### Effect Reference
The Nova System features **15 effect types** across **7 effect blocks**:
1. **Drive Block**: Overdrive, Distortion (NDT™ analog technology)
2. **Compressor**: Percussive, Sustaining, Advanced
3. **EQ + Noise Gate**: 3-band parametric EQ with noise gate
4. **Modulation**: Chorus, Flanger, Vibrato, Phaser, Tremolo, Panner
5. **Pitch**: Shifter, Octaver, Whammy, Detune, Intelligent
6. **Delay**: Clean, Analog, Tape, Dynamic, Dual, Ping-Pong
7. **Reverb**: Spring, Hall, Room, Plate

### Signal Routing Options
- **Serial**: Traditional effects chain
- **Semi-Parallel**: Delay and Reverb in parallel
- **Parallel**: All time-based effects in parallel

## Known Issues

None at release. This is a stable v1.0.0 release with comprehensive testing.

### Reporting Issues
If you encounter any problems:
1. Check the log files in `%APPDATA%\NovaSystemManager\logs\`
2. Report issues on the GitHub repository issue tracker
3. Include your log files and steps to reproduce

## Future Roadmap (v1.1+)

### Planned Features
- **macOS Support**: Native macOS build
- **Auto-Update**: Automatic update checking and installation
- **Cloud Sync**: Optional cloud backup for presets (privacy-respecting)
- **AI Preset Generation**: Natural language preset creation
- **Preset Recommendation**: Smart suggestions based on playing style
- **A/B Comparison**: Side-by-side preset comparison
- **Undo/Redo**: Multi-level undo for preset editing

### Community Contributions
This is an open-source project. Contributions are welcome!
- Report bugs and request features via GitHub Issues
- Submit pull requests for improvements
- Share your preset libraries with the community

## Technical Details

### Architecture
- **Presentation Layer**: Avalonia UI (MVVM pattern)
- **Application Layer**: Use case implementations
- **Domain Layer**: Business logic and entities
- **Infrastructure Layer**: MIDI I/O (DryWetMIDI library)
- **Testing**: xUnit with comprehensive coverage

### MIDI Implementation
- **Manufacturer ID**: 00 20 1F (TC Electronic)
- **Model ID**: 63 (Nova System)
- **SysEx Format**: Fully documented in `MIDI_PROTOCOL.md`
- **Nibble Encoding**: 4-byte encoding for 16-bit parameters
- **Checksum**: 7-bit checksum validation for data integrity

### Dependencies
- **.NET 8.0**: Modern, performant runtime
- **Avalonia UI**: Cross-platform UI framework
- **DryWetMIDI**: Professional MIDI library
- **ReactiveUI**: Reactive MVVM framework

## Credits

**Nova System Manager** is built with ❤️ for the TC Electronic community.

### Development
- **Architecture**: Clean Architecture with Domain-Driven Design
- **Testing**: TDD (Test-Driven Development) discipline
- **Documentation**: Comprehensive MIDI protocol reverse-engineering

### Special Thanks
- TC Electronic for creating the amazing Nova System pedal
- The DryWetMIDI library for excellent MIDI support
- The Avalonia UI team for a fantastic cross-platform framework
- The Nova System user community for feedback and support

### License
This software is provided as-is for the TC Electronic community. Check the repository for license details.

---

**Enjoy creating amazing tones with Nova System Manager!** 🎸🎶
