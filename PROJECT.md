# Nova System Manager - Project Metadata

## Project Info
- **Name:** Nova System Manager
- **Version:** 1.0.0-alpha
- **Target:** TC Electronic Nova System Guitar Effects Pedal
- **Platform:** Windows 11 (primary), macOS (future)
- **Language:** C# / .NET 8
- **UI Framework:** Avalonia 11.x

## Quick Start
```powershell
# Build
dotnet build

# Test
dotnet test

# Run
dotnet run --project src/Nova.Presentation
```

## Development Status

- ✅ MIDI Communication (98.9% tests passing)
- ✅ Domain Model & Validation
- ✅ Clean Architecture
- ⚠️ UI Integration (Editor not connected)
- ❌ Preset Library Features
- ❌ System Settings Editor

## Documentation

- **Architecture:** docs/DESIGN/DESIGN-Architecture.md
- **Testing:** docs/PROCESS/PROC-TestingStrategy.md
- **MIDI Protocol:** docs/REFERENCE/REF-MidiProtocol.md
- **User Manual:** docs/USER/USER-Manual.md

## Test Coverage

458 tests total (453 passing = 98.9%):
- Domain: 261 tests (256 production + 5 investigation)
- Application: 97 tests
- Presentation: 79 tests
- Infrastructure: 13 tests
- MIDI: 8 tests

## Project Structure

```
C:\Projekter\Mikes preset app\
├── src/
│   ├── Nova.Domain/           - Core business logic
│   ├── Nova.Application/      - Use cases
│   ├── Nova.Presentation/     - Avalonia UI
│   ├── Nova.Infrastructure/   - MIDI, File I/O
│   └── Nova.Midi/            - MIDI abstractions
├── tests/
│   └── Nova.*.Tests/         - Unit tests per layer
└── docs/                      - Documentation
```

## Local-First Workflow

- GitHub used ONLY as backup (remote named "backup", push disabled)
- All development happens locally
- Use `.\scripts\backup-git.ps1` for manual backup

## Known Issues

See `docs/TROUBLESHOOTING/TRBL-TroubleshootingPlan.md` for current troubleshooting plan.

Critical issues:
- UI Editor not integrated in MainWindow
- Download Bank command needs debugging
