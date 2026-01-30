# 📁 Detaljeret Mappestruktur (Folder Structure Reference)

**Dokumentation af komplette mappestrukturer, formål og indhold**

---

## 🌳 Generelle Regler

1. **Read-Only Reference**: Folders markeret `[R]` må IKKE ændres efter init
2. **Git Tracked**: Kun `src/` og `tests/` committest til Git
3. **Generated**: Folders markeret `[G]` ignoreres af Git
4. **Documentation**: Inline folder README.md in hver major folder

---

## 📂 Level 1: Project Root

```
d:\Tc electronic projekt\Nyt program til Nova\
├── README.md                              [Projekt-oversigt]
├── APPLICATION_MANIFEST.md                [Denne fil - filosofi + værktøjer]
├── FOLDER_STRUCTURE.md                    [Du er her]
├── SETUP_AUTOMATION.md                    [Setup scripts]
└── DISCIPLINE_OVERVIEW.md                 [Discipline system summary]
```

**Formål**: Projektet starter med klare, læsbare manifest-filer som første kontakt.

---

## 📂 Level 2: Dokumentation & Reference

### `/docs/` — Arkitektur Dokumentation [R]

```
docs/
├── 01-nova-system-overview.md             [Hardware specs, MIDI protocol overview]
├── 02-legacy-app-analysis.md              [Java app reverse engineering]
├── 03-architecture-vision.md              [Layered architecture design]
├── 04-midi-protocol-spec.md               [Complete MIDI command reference]
├── 05-effect-blocks-reference.md          [All 15 effect types, parameters]
├── 06-data-models.md                      [Patch, Preset, SystemDump structures]
├── 07-ui-design-specification.md          [Avalonia XAML mockups]
├── 08-testing-strategy.md                 [Test pyramid, coverage goals]
├── 09-dependency-injection.md             [Service registration, DI container]
├── 10-api-design.md                       [REST endpoint specifications]
├── 11-deployment-guide.md                 [Windows installer, versioning]
├── 12-environment-setup-checklist.md      [Installation step-by-step]
├── 13-implementation-roadmap.md           [Timeline, phase breakdown]
└── 14-ready-for-implementation.md         [Go/no-go decision]
```

**Formål**: Arkitektur-reference for hele projektet. Læses FØR kodning, IKKE under udvikling.

---

### Root Reference Files — Protocol & Effect Reference [R]

```
MIDI_PROTOCOL.md                       [Complete SysEx spec, reverse-engineered]
EFFECT_REFERENCE.md                    [All effect parameters, ranges]
ARCHITECTURE_ANALYSIS.md               [Java app class structures]
PROJECT_KNOWLEDGE.md                   [Lessons learned, design patterns]
```

**Formål**: Når MIDI ikke virker eller effekter gør uventet, søg her først.

---

### `/Nova manager Original/` — Legacy App Reference [R]

```
Nova manager Original/
└── NovaManager/
    ├── nova/                              [Core MIDI logic]
    │   ├── Patch.class                    [Preset structure]
    │   ├── Block.class                    [Effect block base class]
    │   ├── Constants.class                [MIDI CC numbers]
    │   ├── MidiInterface.class            [Abstraction layer]
    │   ├── MidiDefaultInterface.class     [Java Sound API]
    │   ├── MidiMacInterface.class         [Mac CoreMIDI]
    │   ├── RWMidiInterface.class          [RWMidi library wrapper]
    │   └── [10 effect types]              [Drive, Delay, Reverb, etc.]
    │
    ├── ch/randelshofer/quaqua/            [Look & feel resources]
    └── [UI components]                    [Swing-based interface]
```

**Formål**: Bytekode-referencer når C# implementering gør det forkert.

---

### `/Tc originalt materiale/` — Officielle Specs [R]

```
Tc originalt materiale/
├── Nova System Sysex Map.pdf              [KRITISK: MIDI protocol spec]
├── TC Nova Manual.pdf                     [Effect parameter documentation]
├── Nova-System-LTD_Artists-Presets-for-User-Bank.syx    [Binary preset examples]
├── NovaSystem_PC_SWUpdater-1_2_02-R688/   [Legacy PC updater utility]
└── [Andre PDF'er, manualer]               [Hardware documentation]
```

**Formål**: Officielle dokumentation fra TC Electronic. Last resort, men autorisativ.

---

## 📂 Level 3: Discipline System

### `/llm-build-system/` — Development Discipline Enforcement

```
llm-build-system/
├── LLM_BUILD_INSTRUCTIONS.md              [Unskippable rules (RED→GREEN→REFACTOR)]
├── CLEANUP_POLICY.md                      [Safe code deletion procedures]
├── LLM_DISCIPLINE_SYSTEM.md               [Full discipline system explanation]
│
└── memory/                                [Session state tracking]
    ├── SESSION_MEMORY.md                  [What am I working on RIGHT NOW?]
    ├── BUILD_STATE.md                     [What's been built (commits, tests)]
    └── PITFALLS_FOUND.md                  [Common mistakes to avoid]
```

**Formål**: 
- `LLM_BUILD_INSTRUCTIONS.md`: Læses inden HVER session start
- `memory/` opdateres ved END OF EVERY SESSION
- `CLEANUP_POLICY.md`: Konsulteres før kode slettes

**Vigtig**: Denne mappestruktur ændres IKKE. Det er enforcement-systemet.

---

### `/tasks/` — Sequential Task Files

```
tasks/
├── README.md                              [Tasks folder overview]
├── 00-index.md                            [Master task index + progress tracking]
│
├── 01-phase0-environment-setup.md         [17 tasks: .NET, VS, Git, project init]
│   └── Contains 17 subtasks (1.1-1.17), checkboxes, verification commands
│
├── 02-modul1-phase1-foundation.md         [5 tasks: MIDI layer foundation]
│   └── IMidiPort, MockMidiPort, SysExBuilder, SysExValidator
│
├── 03-modul1-phase2-domain-models.md      [5 tasks: Data models]
│   └── Patch, PresetBank, EffectBlock entities + value objects
│
└── 04-modul1-phase3-use-cases.md          [5 tasks: Application logic]
    └── LoadPreset, SavePreset, CreatePreset, EditEffect use cases
```

**Formål**:
- Læs ét task-fil ad gangen
- Følg tasknumre i orden (kan IKKE hoppes over)
- Opdater `00-index.md` checklist ved completion
- Hver task har: Estimated time, status, verification commands

---

## 📂 Level 4: Source Code (Created During Development)

### `/src/` — Application Source Code

```
NovaApp/                                   [Main .NET solution root]
│
├── NovaApp.sln                            [Visual Studio solution file]
│
└── src/
    │
    ├── Nova.Common/                       [Shared utilities]
    │   ├── Nova.Common.csproj
    │   ├── Exceptions/
    │   │   ├── InvalidMidiMessageException.cs
    │   │   └── NovaPedalNotFoundException.cs
    │   ├── Logging/
    │   │   └── SerilogSetup.cs
    │   └── Constants/
    │       ├── MidiConstants.cs           [CC numbers, SysEx format]
    │       └── EffectConstants.cs         [Effect type IDs]
    │
    ├── Nova.Domain/                       [Business logic, no dependencies]
    │   ├── Nova.Domain.csproj
    │   ├── Entities/
    │   │   ├── Patch.cs                   [Single preset (120 parameters)]
    │   │   ├── PresetBank.cs              [128 presets (Patch array)]
    │   │   └── SystemDump.cs              [All 4 banks (A/B/C/D)]
    │   ├── ValueObjects/
    │   │   ├── EffectBlockId.cs           [Strongly-typed effect block ID]
    │   │   ├── ParameterValue.cs          [0-127 safe wrapper]
    │   │   ├── SysExMessage.cs            [F0...F7 wrapper]
    │   │   └── MidiCC.cs                  [CC number wrapper]
    │   ├── Specifications/
    │   │   ├── BlockSpecification.cs      [Effect parameter spec]
    │   │   └── ParameterSpecification.cs  [Min/max/default for each param]
    │   └── Events/
    │       ├── PresetLoadedEvent.cs
    │       └── EffectParameterChangedEvent.cs
    │
    ├── Nova.Infrastructure/               [MIDI I/O, persistence]
    │   ├── Nova.Infrastructure.csproj
    │   ├── Midi/
    │   │   ├── IMidiPort.cs               [MIDI device abstraction]
    │   │   ├── MidiPortWin32.cs           [Windows MIDI API wrapper]
    │   │   ├── MidiPortMac.cs             [Mac CoreMIDI wrapper (future)]
    │   │   ├── MidiMessageValidator.cs    [SysEx validation]
    │   │   └── SysExBuilder.cs            [Builds valid SysEx messages]
    │   ├── Persistence/
    │   │   ├── FilePresetRepository.cs    [Save/load .syx files]
    │   │   └── JsonPresetRepository.cs    [Save/load .json presets]
    │   └── DeviceEnumeration/
    │       ├── NovaSystemDeviceFinder.cs  [Detect connected pedal]
    │       └── DeviceInfo.cs              [Device name, port, vendor ID]
    │
    ├── Nova.Application/                  [Business logic orchestration]
    │   ├── Nova.Application.csproj
    │   ├── UseCases/
    │   │   ├── LoadPresetUseCase.cs       [Send PC, wait for SysEx response]
    │   │   ├── SavePresetUseCase.cs       [Receive SysEx, validate, store]
    │   │   ├── EditEffectUseCase.cs       [Change parameter, send CC]
    │   │   ├── CreatePresetUseCase.cs     [New preset from scratch]
    │   │   └── ListPresetsUseCase.cs      [Enumerate available presets]
    │   ├── Services/
    │   │   ├── PresetManager.cs           [Coordinate Load/Save/Edit]
    │   │   ├── NovaPedalService.cs        [Detect, connect, communicate]
    │   │   └── BackupService.cs           [Auto-backup before edits]
    │   └── Dtos/
    │       ├── PresetDto.cs               [Transfer object for presets]
    │       ├── EffectBlockDto.cs          [Transfer object for effects]
    │       └── ParameterDto.cs            [Transfer object for parameters]
    │
    └── Nova.Presentation/                 [Avalonia UI, XAML]
        ├── Nova.Presentation.csproj
        ├── App.axaml                      [Application root]
        ├── App.axaml.cs                   [Application code-behind]
        ├── Views/
        │   ├── MainWindow.axaml           [Main UI window]
        │   ├── PresetBrowserView.axaml    [Preset selection UI]
        │   ├── EffectEditorView.axaml     [Effect parameter editing]
        │   └── MidiStatusView.axaml       [Connection status display]
        ├── ViewModels/
        │   ├── MainWindowViewModel.cs     [Coordinates all views]
        │   ├── PresetBrowserViewModel.cs  [Preset list logic]
        │   ├── EffectEditorViewModel.cs   [Effect editing logic]
        │   └── MidiStatusViewModel.cs     [Connection status logic]
        ├── Converters/
        │   ├── ParameterValueConverter.cs [0-127 → display string]
        │   └── EffectIconConverter.cs     [Effect type → icon]
        └── Resources/
            ├── Icons/
            │   ├── effect-drive.svg
            │   ├── effect-delay.svg
            │   └── [13 more effect icons]
            └── Themes/
                └── DefaultTheme.axaml     [Dark mode colors, fonts]
```

**Vigtige Regler**:
- ✅ `/src/` er hvor NEU KOD skrives
- ✅ Følg mappestruktur nøjagtigt (ikke opfind dine egne mapper)
- ✅ Navngivning: PascalCase for klasser, filer = klassename + .cs
- ✅ Lag isolation: Domain ← Application ← Presentation/Infrastructure

---

## 📂 Level 5: Tests

### `/tests/` — Unit & Integration Tests

```
NovaApp/
└── tests/
    │
    ├── Nova.Domain.Tests/                 [Domain layer tests]
    │   ├── Nova.Domain.Tests.csproj
    │   ├── Entities/
    │   │   ├── PatchTests.cs              [Create, modify, validate patch]
    │   │   ├── PresetBankTests.cs         [Bank management]
    │   │   └── SystemDumpTests.cs         [All 4 banks]
    │   ├── ValueObjects/
    │   │   ├── ParameterValueTests.cs     [0-127 boundary conditions]
    │   │   └── SysExMessageTests.cs       [F0...F7 parsing]
    │   └── Specifications/
    │       └── BlockSpecificationTests.cs [Spec validation]
    │
    ├── Nova.Infrastructure.Tests/         [MIDI & I/O tests]
    │   ├── Nova.Infrastructure.Tests.csproj
    │   ├── Midi/
    │   │   ├── MockMidiPort.cs            [Fake MIDI device for testing]
    │   │   ├── MidiMessageValidatorTests.cs
    │   │   ├── SysExBuilderTests.cs       [Build correct SysEx commands]
    │   │   └── MidiPortIntegrationTests.cs [Real device tests (optional)]
    │   └── Persistence/
    │       ├── FilePresetRepositoryTests.cs
    │       └── JsonPresetRepositoryTests.cs
    │
    ├── Nova.Application.Tests/            [Use case tests]
    │   ├── Nova.Application.Tests.csproj
    │   ├── UseCases/
    │   │   ├── LoadPresetUseCaseTests.cs  [Send PC, receive SysEx]
    │   │   ├── SavePresetUseCaseTests.cs  [Parse SysEx, store]
    │   │   ├── EditEffectUseCaseTests.cs  [Change effect parameter]
    │   │   └── CreatePresetUseCaseTests.cs
    │   └── Services/
    │       ├── PresetManagerTests.cs
    │       ├── NovaPedalServiceTests.cs   [Device detection]
    │       └── BackupServiceTests.cs
    │
    └── Nova.Presentation.Tests/           [UI tests]
        ├── Nova.Presentation.Tests.csproj
        ├── ViewModels/
        │   ├── MainWindowViewModelTests.cs
        │   ├── PresetBrowserViewModelTests.cs
        │   └── EffectEditorViewModelTests.cs
        └── Snapshot/                      [UI rendering snapshots]
            ├── MainWindow.snap
            └── EffectEditor.snap
```

**Vigtige Regler**:
- ✅ Testfil navn = `{ClassName}Tests.cs`
- ✅ Test klassenavn = `{ClassName}Tests`
- ✅ Test metodenavn = `{Method}_{Scenario}_{Expected}`
- ✅ Eksempel: `SysExBuilder_ValidBankDumpRequest_ReturnsNineByteMessage()`
- ✅ En test = EN ting (AAA pattern: Arrange, Act, Assert)
- ✅ Coverage: Kør `dotnet test /p:CollectCoverage=true` efter hver commit

---

## 📂 Level 6: Build Output [G] — Git Ignored

```
NovaApp/
├── bin/                                   [Generated: Compiled binaries]
│   ├── Debug/
│   │   ├── net8.0/
│   │   │   ├── NovaApp.exe
│   │   │   ├── NovaApp.dll
│   │   │   └── [dependencies]
│   │   └── [test binaries]
│   └── Release/
│       └── [production binaries]
│
├── obj/                                   [Generated: Intermediate objects]
│   ├── Debug/
│   │   └── net8.0/
│   │       ├── .NETCoreApp...
│   │       └── [IL code]
│   └── Release/
│
└── .vs/                                   [Generated: Visual Studio cache]
    └── [IntelliSense, build cache]
```

**Vigtig**: Disse mapper genereres automatisk af `dotnet build`. De er i `.gitignore` og committed IKKE.

---

## 📂 Level 7: Configuration Files

```
NovaApp/
├── .gitignore                             [Git ignore rules]
├── Directory.Build.props                  [Shared project settings]
├── global.json                            [Lock .NET 8.0 version]
├── nuget.config                           [NuGet package sources]
└── .git/                                  [Git repository data]
    ├── config
    ├── objects/                           [Commit objects]
    └── refs/heads/main                    [Branch pointers]
```

**Vigtige Filer**:
- `.gitignore`: Prevents bin/, obj/, .vs/ from being committed
- `global.json`: Forces .NET 8 (prevents version surprises)
- `Directory.Build.props`: Shared C# version, nullable, warnings-as-errors

---

## 📂 Level 8: Distribution [G]

```
releases/
├── v1.0.0/
│   ├── NovaApp.exe                        [Standalone executable]
│   ├── NovaApp.dll                        [Core library]
│   ├── settings.json                      [Default configuration]
│   ├── README.md                          [Release notes]
│   └── LICENSE                            [MIT or similar]
│
└── v1.0.1/
    └── [Patch release]
```

**Genereres af**:
```powershell
dotnet publish -c Release -o releases/v1.0.0
```

---

## 🎯 Vigtige Principper

### 1. **Never Invent New Folders**
Hvis der mangler en organisering, tilføj til eksisterende eller ask first.

### 2. **One Class = One File**
Filen hedder nøjagtigt som klassen:
- `Patch.cs` indeholder kun `class Patch`
- `IMidiPort.cs` indeholder kun `interface IMidiPort`

### 3. **Namespace = Folder Path**
Hvis klassen er i `src/Nova.Infrastructure/Midi/MidiPortWin32.cs`:
```csharp
namespace Nova.Infrastructure.Midi;

public class MidiPortWin32 : IMidiPort { }
```

### 4. **Tests Mirror Source**
- Kode: `src/Nova.Domain/Entities/Patch.cs`
- Test: `tests/Nova.Domain.Tests/Entities/PatchTests.cs`

### 5. **Reference Materials Are Immutable**
Hvis du finder bug i `/docs` eller `/reference`, sørg for den dokumenteres i `/llm-build-system/memory/PITFALLS_FOUND.md` før ændring.

---

## 🚀 Quick Navigation

| Du ønsker at... | Gå til... |
|-----------------|-----------|
| Forstå projektet | `APPLICATION_MANIFEST.md` |
| Læse arkitektur | `docs/03-architecture-vision.md` |
| Se MIDI spec | `reference/MIDI_PROTOCOL.md` |
| Løse MIDI problem | `reference/EFFECT_REFERENCE.md` + `Nova manager Original/` |
| Skrive ny kode | `src/` (følg mappestruktur) |
| Skrive test | `tests/` (mirror src structure) |
| Slette kode | Læs `llm-build-system/CLEANUP_POLICY.md` først |
| Tracker progress | Opdater `tasks/00-index.md` |
| Tracker commits | Check `llm-build-system/memory/BUILD_STATE.md` |

---

**Version**: 1.0  
**Last Updated**: 30. januar 2026  
**Status**: ✅ Complete — Ready for Development
