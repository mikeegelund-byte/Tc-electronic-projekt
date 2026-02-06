# Nova System Manager - Claude Hukommelse

## Projekt Essentials

**Formål:** Desktop applikation til styring af TC Electronic Nova System guitar effects pedal via MIDI.

**Tech Stack:**
- C# .NET 8+ (pt. kører .NET 10)
- Avalonia 11.x (cross-platform UI framework)
- DryWetMIDI 8.0.3 (MIDI kommunikation)
- xUnit + Moq + FluentAssertions (testing)

**Arkitektur:** Clean Architecture (4 lag)
- Presentation (Avalonia UI)
- Application (Use Cases)
- Domain (Business Logic)
- Infrastructure (MIDI, File I/O)

## MIDI Kommunikation - Kritisk Viden

### Hardware Setup
- **Input Port:** "USB MIDI Interface 0" (fra pedal til PC)
- **Output Port:** "USB MIDI Interface 1" (fra PC til pedal)
- **VIGTIGT:** Vælg ALDRIG "Microsoft GS Wavetable Synth" - den blokerer MIDI totalt!

### SysEx Kommunikation
- **Bank Dump:** 60 separate SysEx beskeder (518 bytes hver)
- **System Dump:** 1 SysEx besked (system konfiguration)
- **Preset Send:** Enkelt SysEx med 50-100ms pause mellem beskeder
- **Format:** F0 ... F7 (starter med 0xF0, slutter med 0xF7)
- **Ingen bekræftelse fra pedal** - "fire and forget"

Se `NovaSystem_MIDI_Implementation_Guide.md` for detaljeret protokol.

## Projekt Status

**Lokalt-først workflow:**
- GitHub bruges KUN som backup (remote navngivet "backup", push disabled)
- Al udvikling sker lokalt
- Brug `.\scripts\backup-git.ps1` til manuel backup

**Test Coverage:**
- 458 tests i alt (453/458 passing = 98.9%)
- Production tests: 438/438 passing (100%)
- Investigation tests: 15/20 passing (exploratory, non-critical)
- Domain: 261 tests (256 production + 5 investigation)
- Application: 97 tests
- Presentation: 79 tests
- Infrastructure: 13 tests
- MIDI: 8 tests

## Vigtige Filer

### Dokumentation
- `docs/00-index.md` - Navigation til alle docs
- `docs/05-midi-io-contract.md` - MIDI interface kontrakt
- `NovaSystem_MIDI_Implementation_Guide.md` - MIDI protokol guide
- `MANUAL_TEST_GUIDE.md` - Manual test procedure

### Kildekode
- `src/Nova.Midi/IMidiPort.cs` - MIDI abstraction interface
- `src/Nova.Infrastructure/Midi/DryWetMidiPort.cs` - DryWetMIDI implementation
- `src/Nova.Application/UseCases/ConnectUseCase.cs` - Connection logic
- `src/Nova.Presentation/ViewModels/MainViewModel.cs` - Main UI

### Tests
- `src/Nova.Midi.Tests/MidiPortContractTests.cs` - Contract verification
- `src/Nova.Infrastructure.Tests/Midi/DryWetMidiPortTests.cs` - Implementation tests

## Kendte Problemer & Løsninger

### MIDI Connection Issues (løst 2026-02-05)
**Problem:** App kunne ikke modtage MIDI data fra pedal
**Root Cause:** DryWetMidiPort manglede `StartEventsListening()` call
**Løsning:** Call `_inputDevice.StartEventsListening()` i `ConnectAsync()`

### Device Selection
**Problem:** Forkert device kan blive valgt
**Løsning:** Valider device navn indeholder "USB MIDI Interface"

### MIDI Memory Leaks & Race Conditions (løst 2026-02-06)
**Problem:** Memory leaks, duplikerede events, crashes ved disconnect
**Root Cause:** Event handlers subscribed multiple times, channels never cleaned up
**Løsning:**
- Added `_handlersSubscribed` flag to prevent duplicate subscriptions
- `DisconnectAsync()` now unsubscribes handlers and completes channels
- Defensive null checks in `OnEventReceived()`
**Commit:** 8e37993

### Domain Validation Missing (løst 2026-02-06)
**Problem:** 101 validation tests failing, ~40 parameters unvalidated
**Root Cause:** Only 2 parameters had range validation in `Preset.FromSysEx()`
**Løsning:** Created `ValidateAllParameters()` method validating all 42 parameters
**Commit:** 8e37993
**Note:** FluentResults upgraded 3.* → 4.0.0

### XAML Startup Crash (løst 2026-02-06)
**Problem:** InvalidCastException on app startup at MainWindow.axaml:99
**Root Cause:** StackPanel.Spacing expects double, but got Thickness resource
**Løsning:** Replace `{StaticResource SpacingMedium}` with `{StaticResource Spacing16}`
**Commit:** 2ae4c72

### UI Integration Incomplete (KRITISK - ikke løst)
**Problem:** Editor UI eksisterer men er ikke integreret i MainWindow
**Status:** PresetDetailView.axaml bygget, men ingen Editor tab i MainWindow
**Impact:** Download Bank virker ikke, ingen preset editing mulig
**Root Cause:** UI bygget men aldrig connected til MainViewModel
**Next Steps:** Integrer PresetDetailView som tab, debug DownloadBankCommand

## Arbejdsflow

### Build & Run
```powershell
cd "Nyt program til Nova"
dotnet build NovaApp.sln          # Build hele solution
dotnet test NovaApp.sln            # Kør alle tests
dotnet run --project src/Nova.Presentation  # Start app
```

### **KRITISK: Verificer Program Virker FØR Backend Changes**
```powershell
# 1. Test GUI starter uden crash
dotnet run --project src/Nova.Presentation

# 2. Test basic funktionalitet
# - Connect til MIDI device
# - Download Bank (verificer presets modtages)
# - Open preset i editor (hvis tab eksisterer)

# 3. Kun efter verification: Start backend changes
```
**Lesson:** Test frontend integration før du ændrer backend. Backend tests kan passe mens GUI er brudt.

### Test Specifikt Projekt
```powershell
dotnet test src/Nova.Midi.Tests/
dotnet test src/Nova.Infrastructure.Tests/
```

### Hardware Test (kræver Nova System pedal)
```powershell
dotnet run --project src/Nova.HardwareTest
```

## Coding Standards

**Naming:**
- Classes: PascalCase (`UserBankDump`)
- Methods: PascalCase (`ParseSysEx()`)
- Private fields: `_camelCase`
- Async methods: suffix `Async` (`ConnectAsync()`)

**Error Handling:**
- Exceptions for exceptional cases (hardware failure, checksum error)
- Result<T> pattern for expected failures (invalid port name)

**Testing:**
- TDD approach - tests først
- Mock IMidiPort for unit tests
- Separate hardware tests i Nova.HardwareTest
- **Test Helpers:** Use `TestHelpers.cs` for valid preset creation
  - `TestHelpers.CreateValidPresetSysEx(number, name)` - 521-byte SysEx with valid parameter defaults
  - `TestHelpers.CreateValidPreset(number, name)` - Preset object from valid SysEx
  - Ensures all 42 parameters pass domain validation
  - Locations: `Nova.Application.Tests/`, `Nova.Presentation.Tests/`

## Ofte Brugte Kommandoer

```powershell
# Byg kun presentation layer
dotnet build src/Nova.Presentation

# Kør specific test med filter
dotnet test --filter "FullyQualifiedName~ConnectUseCase"

# Kør med verbose logging
dotnet run --project src/Nova.Presentation --verbosity detailed

# Clean rebuild
dotnet clean && dotnet build
```

## Arkiv & Historie

Historisk udviklings-materiale ligger i:
- `C:\Projekter\TC Electronic\Arkiv fra TC electronic projekt\`

Dette indeholder:
- Tidligere design dokumenter
- Completed tasks
- Development notes
- Session logs

## Næste Skridt (opdateres løbende)

Se `docs/TROUBLESHOOTING_PLAN.md` for aktuel troubleshooting plan.

---

*Sidst opdateret: 2026-02-06 22:55*
*Claude Session: MIDI fixes, domain validation, XAML crash fix - UI integration incomplete*
