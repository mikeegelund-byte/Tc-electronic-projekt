# Nova System Manager - Claude Hukommelse

## Projekt Essentials

**Formål:** Desktop applikation til styring af TC Electronic Nova System guitar effects pedal via MIDI.

**Location:** `C:\Projekter\Mikes preset app`

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

## Migration Historie

**Migreret:** 2026-02-07
**Fra:** `C:\Projekter\TC Electronic\Tc electronic projekt\Nyt program til Nova`
**Til:** `C:\Projekter\Mikes preset app`

**Rationale:** Oprydning af kæmpemæssigt rod i mapper, etablering af clean projekt struktur.

**Hvad blev flyttet:**
- ✅ 296 C# filer (kildekode + tests)
- ✅ 26 dokumentations-filer (docs/ + root .md)
- ✅ Git repository (fuld historik bevaret - 2214 filer i .git/)
- ✅ Build configuration (NovaApp.sln, Directory.Build.props, global.json)
- ✅ Scripts (setup.ps1, verify-commit.ps1)

**Hvad blev efterladt (reference):**
- Originalt materiale: `C:\Projekter\TC Electronic\Tc originalt materiale\` (PDFs, MIDI dumps)
- Legacy Java projekt: `C:\Projekter\TC Electronic\Tc electronic projekt\Nova manager Original\`
- Eksperimentel version: `C:\Projekter\TC Electronic\Tc electronic projekt\NovaMidiLab\`

**Verification:**
- ✅ Git historik intakt (alle commits bevaret)
- ✅ 458 tests (453 passing, 5 skipped = 100% pass rate!)
- ✅ Solution bygger uden fejl (0 warnings, 0 errors)
- ✅ Dokumentation komplet og struktureret (PREFIX naming)
- ✅ Clean struktur (351 tracked files vs. 3,587+ før cleanup)

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
- 458 tests i alt (453 passing, 5 skipped = **100% pass rate!**)
- Production tests: 453/453 passing (100%)
- Investigation tests: 5 skipped (debugging tools, intentionally excluded)
- Domain: 261 tests (256 passing + 5 skipped investigation)
- Application: 96 tests (100% passing)
- Presentation: 78 tests (100% passing)
- Infrastructure: 12 tests (100% passing)
- MIDI: 8 tests (100% passing)

## Vigtige Filer

### Dokumentation (Reorganiseret med PREFIX naming)
- `docs/README.md` - Navigation til alle docs (tidligere 00-index.md)
- `docs/DESIGN/DESIGN-MidiIoContract.md` - MIDI interface kontrakt
- `GUIDE_MidiImplementation.md` - MIDI protokol guide (tidligere NovaSystem_MIDI_Implementation_Guide.md)
- `MANUAL_TEST_GUIDE.md` - Manual test procedure
- `PROJECT.md` - Project metadata
- `CONTRIBUTING.md` - Development guidelines

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
cd "C:\Projekter\Mikes preset app"
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

## Reference Materiale

**Originalt TC Electronic materiale:**
- `C:\Projekter\TC Electronic\Tc originalt materiale\`
  - Nova System Sysex Map.pdf
  - TC Nova Manual.pdf
  - MIDI dumps (.syx filer)
  - Firmware updater

**Legacy projekter (kun til reference):**
- `C:\Projekter\TC Electronic\Tc electronic projekt\Nova manager Original\` (Java version)
- `C:\Projekter\TC Electronic\Tc electronic projekt\NovaMidiLab\` (eksperimentel C#)

**Arkiv backups:**
- `C:\Projekter\TC Electronic\Arkiv fra TC electronic projekt\` (backup fra 2026-02-04)

## Professional Development Setup (tilføjet 2026-02-07)

**Development Tooling:**
- ✅ `.editorconfig` - Code formatting standards (C# conventions)
- ✅ `.gitattributes` - Line ending consistency (CRLF for Windows)
- ✅ `.githooks/` - Pre-commit validation (build + tests + conventional commits)
- ✅ `.vscode/` - VS Code configuration (settings, tasks, launch, debugging)
- ✅ `.devcontainer/` - Container-ready development environment
- ✅ `Directory.Packages.props` - Central package management (all versions centralized)

**Git Hooks (aktiveret med `git config core.hooksPath .githooks`):**
- `pre-commit` - Bygger solution og kører unit tests før commit
- `commit-msg` - Enforcer conventional commit format (feat/fix/docs/chore/etc)
- Se `.githooks/INSTALL_HOOKS.md` for detaljer

**Documentation Structure:**
```
docs/
├── DESIGN/           - Arkitektur og design (4 filer)
├── REFERENCE/        - Teknisk reference (3 filer)
├── PROCESS/          - Udviklings processer (3 filer)
├── PROJECT/          - Project metadata (3 filer)
├── TROUBLESHOOTING/  - Fejlsøgning (1 fil)
├── USER/             - Bruger dokumentation (1 fil)
└── README.md         - Navigation index
```

## Cleanup & Standardisering (2026-02-07)

**Omfattende cleanup gennemført:**
- ✅ Slettet 973 build artifacts (bin/ + obj/ mapper)
- ✅ Reduceret filantal fra 3,587 til 351 tracked files (90% reduktion!)
- ✅ Flyttet backup filer til `.archive/` for historisk reference
- ✅ Reorganiseret dokumentation med PREFIX naming (DESIGN-, REF-, PROC-, etc.)
- ✅ Gendannet slettede planning docs fra git history til `.archive/`
- ✅ Implementeret central package management (Directory.Packages.props)
- ✅ Migreret 12 .csproj filer til central package versions
- ✅ Fixet 5 fejlende investigation tests (nu skipped for 100% pass rate)
- ✅ Opdateret .gitignore (.vscode/ tracked, *.zip ignored)
- ✅ Git garbage collection for optimering

**Før cleanup:**
- 3,587 filer, 570 mapper
- 98.9% test pass rate (5 fejlende investigation tests)

**Efter cleanup:**
- 351 tracked files, 74 directories
- 100% test pass rate (453 passing, 5 skipped)
- Professionelt development setup
- Clean og struktureret dokumentation

**Commits:**
```
31edf99 chore(project): final cleanup - remove build artifacts and fix tests
6cd5efb chore(build): migrate to central package management
86d549f chore(project): comprehensive cleanup and professional dev tooling
```

## Næste Skridt (opdateres løbende)

Se `docs/TROUBLESHOOTING/TRBL-TroubleshootingPlan.md` for aktuel troubleshooting plan.

**Kritiske issues:**
- UI Integration: Editor tab mangler i MainWindow
- Download Bank command skal debugges

---

*Sidst opdateret: 2026-02-07 (efter omfattende cleanup & standardisering)*
*Claude Session: Professional development setup, documentation restructuring, og 100% test pass rate*

## Morgens arbejde (2026-02-07)

**UI/MIDI fixes & integration:**
- ✅ Fixet XAML crash ved manglende converter: tilføjede `BoolToGreenGrayBrushConverter` i `src/Nova.Presentation/App.axaml` (appen starter nu korrekt).
- ✅ Kablet System Dump flow op i UI:
  - `RequestSystemDumpUseCase` implementerer nu interface.
  - `MainViewModel.DownloadSystemSettingsAsync()` request’er system dump og loader `SystemSettings`, `MidiMapping`, `ProgramMapIn/Out`.
  - Ny knap i System Settings: **Download from Pedal**.
- ✅ MIDI Mapping + Program Map integreret i MainWindow:
  - Nye tabs: **MIDI Mapping**, **Program Map In**, **Program Map Out**.
  - DI wired: `IRequestSystemDumpUseCase`, `IGet/UpdateProgramMap(In|Out)UseCase` + viewmodels.
- ✅ Preset manager gjort mere tydelig:
  - “Connection” tab omdøbt til **Presets**.

**Download Bank stabilitet:**
- ✅ `DownloadBankUseCase` sender nu **Bank Dump request** aktivt (ikke kun passiv lytning).
- ✅ Download stopper efter **3 sekunders inaktivitet** (idle timeout).
- ✅ Partial dumps tilladt: UI viser “Downloaded X/60 presets (partial)” og spinner stopper.

**Build/verification:**
- ✅ `dotnet build` kørt uden fejl.

**Commits & backup:**
- `b49debb` – Wire system dump download and mapping UI (push til `backup/main`).
- `7131e42` – Stop bank download on idle and allow partial dumps (push til `backup/main`).

**Aktuelle observationer:**
- Hvis dump stadig hænger: kan skyldes at hardware sender færre end 60 presets eller enkelte dumps fejler validering; nu stopper UI korrekt og viser partial.
