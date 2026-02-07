# Nova System Manager

**Modern MIDI Manager for TC Electronic Nova System Guitar Effects Pedal**

Nova System Manager is a desktop application for managing presets and settings on your TC Electronic Nova System multi-effects pedal. Built with .NET 8 and Avalonia, it provides a modern, accessible interface for preset editing, file management, and MIDI configuration.

**Local-first development:** This repository is developed locally. GitHub is used only as backup.

---

## Quick Start

### Run Locally

1. Build the solution: `dotnet build NovaApp.sln`
2. Run the app: `dotnet run --project src/Nova.Presentation`
3. Optional: build the installer with `.\installer\build-installer.ps1`

See `docs/USER_MANUAL.md` for detailed instructions.

### Local Development

```powershell
# Build
cd "C:\Projekter\Mikes preset app"
dotnet build NovaApp.sln

# Run tests (458 tests)
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
✅ **458 passing unit tests** (Domain, Application, Presentation, Infrastructure layers)  
✅ Local MSI installer build (optional)  
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
# Open the local repo
cd "Nyt program til Nova"

# Restore dependencies
dotnet restore NovaApp.sln

# Build solution
dotnet build NovaApp.sln -c Release

# Run all tests (458 tests)
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
- **458 total tests** - all passing ✅
  - Domain: 160 tests
  - Application: 88 tests
  - Presentation: 76 tests
  - Infrastructure: 12 tests
  - MIDI: 6 tests

---

## Local-first workflow

This project is developed primarily on a single developer machine. GitHub (or any remote) is
kept only as an off-site backup — not as the primary working copy. The guidance below
helps avoid accidental pushes or repository operations that might affect your local history.

- Remote name: the project's GitHub remote has been renamed to `backup` to avoid accidental
  pushes from tools that assume a remote named `origin`.
- Pushes are disabled by default for `backup` (push URL set to `no_push`). To enable a manual
  push, run:

```powershell
git remote set-url --push backup https://github.com/mikeegelund-byte/Tc-electronic-projekt.git
git push backup main
git remote set-url --push backup no_push
```

- Manual backup (recommended): use the provided script to create a git bundle you can copy
  offline or to cloud storage:

```powershell
.\scripts\backup-git.ps1
# creates a file under ./backups/repo-backup-YYYYMMDD_HHMMSS.bundle
```

- If you need to re-enable push temporarily, set the push URL as shown above and push.

Notes:
- Avoid force-pushing `main`. If you must restore or rewrite history, coordinate with any
  collaborators (or re-clone the repo yourself afterwards).
- Keep large binary files out of the repository; use external storage or Git LFS if necessary.

### Restore from backup

From a local bundle:

```powershell
git clone "path\\to\\repo-backup-YYYYMMDD_HHMMSS.bundle" "Tc electronic projekt"
```

From GitHub backup (only for recovery):

```powershell
git clone https://github.com/mikeegelund-byte/Tc-electronic-projekt.git
```

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
**MIDI Library**: DryWetMIDI 8.0.3  
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

*Nova System Manager v1.0.0 - February 2026*
