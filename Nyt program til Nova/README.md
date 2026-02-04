# Nova System Manager

**Modern MIDI Manager for TC Electronic Nova System Guitar Effects Pedal**

[![Build Status](https://img.shields.io/github/actions/workflow/status/mikeegelund-byte/Tc-electronic-projekt/ci.yml?branch=main)](https://github.com/mikeegelund-byte/Tc-electronic-projekt/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![Version](https://img.shields.io/github/v/release/mikeegelund-byte/Tc-electronic-projekt)](https://github.com/mikeegelund-byte/Tc-electronic-projekt/releases)

Nova System Manager is a desktop application for managing presets and settings on your TC Electronic Nova System multi-effects pedal. Built with .NET 8 and Avalonia, it provides a modern, accessible interface for preset editing, file management, and MIDI configuration.

---

## 🚀 Quick Start

### For End Users

1. **Download** the latest release: [NovaSystemManager-v1.0.0.msi](https://github.com/mikeegelund-byte/Tc-electronic-projekt/releases)
2. **Install** by running the .msi file (Windows 10/11 64-bit)
3. **Connect** your Nova System via USB MIDI interface
4. **Launch** Nova System Manager from desktop shortcut
5. **Download** your presets by clicking "Download Bank" (F5)

📖 **Full guide**: See [User Manual](docs/USER_MANUAL.md) for detailed instructions.

### For Developers

```powershell
# Clone and build
cd "Nyt program til Nova"
dotnet build NovaApp.sln

# Run tests (342 tests)
dotnet test NovaApp.sln

# Run application
dotnet run --project src/Nova.Presentation
```

---

## ✨ Features

### Core Functionality
✅ **Download** all 60 presets from Nova System pedal  
✅ **Edit** all effect parameters across 7 effect blocks  
✅ **Save** modified presets back to hardware  
✅ **Import/Export** presets as .syx files for backup and sharing  
✅ **Expression Pedal** response curve editor with visual control  
✅ **View** system settings (MIDI channel, device ID, global bypass)  
✅ **MIDI Mapping** - view CC assignments and pedal configurations

### Effect Blocks Supported
- **Compressor** (Percussive, Sustaining, Advanced)
- **Drive** (Overdrive, Distortion - NDT™ analog section)
- **Modulation** (Chorus, Flanger, Vibrato, Phaser, Tremolo, Panner)
- **Delay** (Clean, Analog, Tape, Dynamic, Dual, Ping-Pong)
- **Reverb** (Spring, Hall, Room, Plate)
- **EQ + Noise Gate** (3-band parametric EQ)
- **Pitch** (Shifter, Octaver, Whammy, Detune, Intelligent)

### User Experience
✅ Modern dark/light theme with smooth animations  
✅ **WCAG AA accessible** - screen reader support, keyboard navigation  
✅ **Keyboard shortcuts** (Ctrl+S, F5, Ctrl+R, etc.)  
✅ Real-time validation with clear error messages  
✅ Loading indicators for long operations  
✅ Professional icon set and responsive layout

### Technical
✅ Full SysEx protocol implementation (TC Electronic Nova System)  
✅ **342 passing unit tests** (Domain, Application, Presentation layers)  
✅ CI/CD with GitHub Actions  
✅ Self-contained installer (.msi) with automatic updates  
✅ Comprehensive documentation and user manual

---

## 📋 System Requirements

### Minimum
- **OS**: Windows 10 (64-bit) or later
- **RAM**: 4 GB
- **Storage**: 200 MB free space
- **.NET**: .NET 8.0 Desktop Runtime (included in installer)
- **MIDI**: USB MIDI interface or built-in MIDI port

### Recommended
- **OS**: Windows 11 (64-bit)
- **RAM**: 8 GB+
- **Screen**: 1920x1080 or higher

### Hardware
- TC Electronic Nova System guitar effects processor
- USB MIDI interface (class-compliant recommended)
- Standard MIDI cables (5-pin DIN)

---

## 📚 Documentation

| Document | Description |
|----------|-------------|
| [User Manual](docs/USER_MANUAL.md) | Complete guide for end users (17 pages) |
| [Architecture](docs/03-architecture.md) | Clean Architecture design and structure |
| [MIDI Protocol](MIDI_PROTOCOL.md) | SysEx message format specification |
| [Effect Reference](EFFECT_REFERENCE.md) | All effect types and parameters |
| [Testing Strategy](TESTING_STRATEGY.md) | Comprehensive test approach |
| [Changelog](CHANGELOG.md) | Version history and release notes |

---

## 🛠️ Building from Source

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code (optional)
- Git

### Build Steps

```powershell
# Clone repository
git clone https://github.com/mikeegelund-byte/Tc-electronic-projekt.git
cd "Tc electronic projekt/Nyt program til Nova"

# Restore dependencies
dotnet restore NovaApp.sln

# Build solution
dotnet build NovaApp.sln -c Release

# Run all tests (342 tests)
dotnet test NovaApp.sln

# Run application
dotnet run --project src/Nova.Presentation
```

### Building the Installer

```powershell
# Install WiX Toolset (one-time)
dotnet tool install --global wix

# Build MSI installer
.\installer\build-installer.ps1

# Output: installer\output\NovaSystemManager-v1.0.0.msi
```

---

## ⌨️ Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `Ctrl+R` | Refresh MIDI ports |
| `F5` | Download User Bank (60 presets) |
| `Ctrl+S` | Save current preset |
| `Ctrl+Z` | Undo last change |
| `Ctrl+Y` | Redo |
| `Ctrl+C` | Copy preset |
| `Escape` | Cancel/Close dialog |

---

## 🧪 Testing

```powershell
# Run all tests
dotnet test

# Run specific test project
dotnet test src/Nova.Domain.Tests/

# With code coverage
dotnet test --collect:"XPlat Code Coverage"

# Hardware tests (requires Nova System pedal)
dotnet test src/Nova.HardwareTest/
```

**Test Statistics**:
- **342 total tests** - all passing ✅
  - Domain: 160 tests
  - Application: 88 tests
  - Presentation: 76 tests
  - Infrastructure: 12 tests
  - MIDI: 6 tests

---

## 🤝 Contributing

Contributions are welcome! This is an open-source community project.

### Guidelines

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Commit** your changes (`git commit -m 'Add amazing feature'`)
4. **Push** to the branch (`git push origin feature/amazing-feature`)
5. **Open** a Pull Request

### Development Workflow

- Follow Clean Architecture principles
- Write unit tests for new features (target: 80%+ coverage)
- Update documentation for user-facing changes
- Run `dotnet test` before committing
- Follow C# coding conventions and naming standards

### Roadmap for v1.1

See [tasks/00-index.md](tasks/00-index.md) for planned features:
- MIDI CC Learn Mode
- Editable System Settings
- Pedal Calibration Wizard
- Preset Library Browser

---

## 📦 Project Structure

```
Nyt program til Nova/
├── src/
│   ├── Nova.Presentation/      # Avalonia UI layer
│   ├── Nova.Application/       # Use cases and commands
│   ├── Nova.Domain/            # Business logic and entities
│   ├── Nova.Infrastructure/    # MIDI implementation
│   ├── Nova.Midi/             # MIDI abstractions
│   └── Nova.Common/           # Shared utilities
├── docs/                       # Documentation
├── installer/                  # WiX installer project
├── tasks/                      # Development roadmap
└── Arkiv/                     # Completed tasks archive
```

**Architecture**: Clean Architecture with Domain-Driven Design (DDD)  
**UI Framework**: Avalonia 11.3  
**MIDI Library**: DryWetMIDI 7.x  
**Testing**: xUnit, Moq, FluentAssertions

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

```
Copyright (c) 2026 TC Electronic Community

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files...
```

---

## 🙏 Acknowledgments

- **TC Electronic** for creating the Nova System pedal
- **Melanchall** for the excellent DryWetMIDI library
- **Avalonia UI** team for the cross-platform UI framework
- **Community contributors** for testing and feedback

---

## 🔗 Links

- **Releases**: [https://github.com/mikeegelund-byte/Tc-electronic-projekt/releases](https://github.com/mikeegelund-byte/Tc-electronic-projekt/releases)
- **Issues**: [https://github.com/mikeegelund-byte/Tc-electronic-projekt/issues](https://github.com/mikeegelund-byte/Tc-electronic-projekt/issues)
- **Discussions**: [https://github.com/mikeegelund-byte/Tc-electronic-projekt/discussions](https://github.com/mikeegelund-byte/Tc-electronic-projekt/discussions)

---

## ❓ Support

### Need Help?

1. Check the [User Manual](docs/USER_MANUAL.md)
2. Review [Troubleshooting Guide](docs/USER_MANUAL.md#troubleshooting)
3. Search existing [GitHub Issues](https://github.com/mikeegelund-byte/Tc-electronic-projekt/issues)
4. Ask in [Discussions](https://github.com/mikeegelund-byte/Tc-electronic-projekt/discussions)

### Found a Bug?

Please [open an issue](https://github.com/mikeegelund-byte/Tc-electronic-projekt/issues/new) with:
- Description of the problem
- Steps to reproduce
- Expected vs actual behavior
- Screenshots (if applicable)
- System information (Windows version, .NET version)

---

**Made with ❤️ by the TC Electronic Community**

*Nova System Manager v1.0.0 - February 2026*
