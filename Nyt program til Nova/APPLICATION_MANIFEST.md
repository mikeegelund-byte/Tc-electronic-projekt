# 📋 Application Manifest: TC Electronic Nova System Controller

**Version**: 1.0.0  
**Status**: Pre-Development (Architecture Phase Complete)  
**Date**: 30. januar 2026  
**Language**: C# 11 (.NET 8 LTS)

---

## 🎯 Filosofi (Philosophy)

### Core Vision
Replace legacy Java NovaManager (v1.20.1) with modern, AI-capable software for controlling TC Electronic Nova System multi-effects pedal via USB-MIDI.

**Key Principles:**
1. **Reverse-Engineered MIDI Fidelity** — Exact protocol compatibility with original hardware (bitwise equivalent)
2. **Test-Driven Everything** — No code without tests (RED→GREEN→REFACTOR mandatory)
3. **AI-First Architecture** — APIs designed for programmatic preset generation/modification
4. **Cross-Platform Ready** — Windows primary (tested), Mac/Linux future-ready
5. **Modern UX** — "Bleeding edge" design, not 1:1 Java clone

### Design Tenets
- **Transparency**: Every MIDI command logged, traceable
- **Reliability**: 95% Domain layer test coverage minimum
- **Extensibility**: Plugin architecture for future effects/features
- **Documentation**: Code is documentation (no ambiguity)
- **Discipline**: Unskippable test gates prevent shortcuts

---

## 🛠️ Værktøjsoversigt (Tool Inventory)

### Development Stack

| Kategori | Værktøj | Version | Formål | Installation |
|----------|---------|---------|--------|--------------|
| **Language** | C# | 11 | Modern, type-safe, .NET ecosystem | Via .NET 8 SDK |
| **Runtime** | .NET | 8 LTS | Long-term support, cross-platform | dotnet.microsoft.com |
| **IDE** | Visual Studio Community | 2022 | Full C# debugging, Git integration | visualstudio.com |
| **UI Framework** | Avalonia | 11.x | XAML-based, cross-platform rendering | NuGet |
| **MIDI Library** | DryWetMIDI | 7.x | SysEx parsing, MIDI device enumeration | NuGet |
| **Testing** | xUnit | 2.6 | TDD framework, parallelizable | NuGet |
| **Mocking** | Moq | 4.18 | Interface mocking for tests | NuGet |
| **Logging** | Serilog | 3.x | Structured logging, MIDI trace support | NuGet |
| **DI Container** | Microsoft.Extensions | Latest | Dependency injection, configuration | NuGet |
| **Build Tool** | dotnet CLI | Built-in | Build, test, publish orchestration | Via .NET 8 SDK |
| **Version Control** | Git | 2.40+ | Local repository, commit history | git-scm.com |

### Lokalt Versionsstyring (No GitHub Required)

**Git Repository**: `d:\Tc electronic projekt\Nyt program til Nova\`

- **Type**: Bare local repository (not pushed to GitHub)
- **Branch Strategy**: main (always deployable) + feature branches
- **Pre-commit Hooks**: Enforce no warnings, all tests pass
- **Commit Protocol**: `[RED→GREEN→REFACTOR] Feature description`

**Why Local Git?**
- Full history offline
- Fast operations
- No external dependency
- Later: Can push to GitHub, Gitea, or any Git host if needed
- Can sync to external drives for backup

### Support Tools (Optional)

| Værktøj | Formål | Installation |
|---------|--------|--------------|
| **Gitea** | Self-hosted Git UI (if you want web interface) | Docker or standalone |
| **VS Code** | Alternative to Visual Studio (lighter weight) | code.visualstudio.com |
| **Windows Terminal** | Better PowerShell experience | Windows Store |
| **7-Zip** | Extract project archives | 7-zip.org |
| **Notepad++** | Quick file editing | notepad-plus-plus.org |

---

## 📁 Mappestruktur (Folder Structure)

```
d:\Tc electronic projekt\Nyt program til Nova\
│
├── 📋 PROJECT MANIFEST & DOCUMENTATION
│   ├── APPLICATION_MANIFEST.md           ← Du er her
│   ├── FOLDER_STRUCTURE.md               ← Detaljeret mappestruktur
│   ├── SETUP_AUTOMATION.md               ← Setup scripts guide
│   ├── START_HERE.md                     ← Projektoversigt
│   └── README.md                         ← GitHub-stil readme
│
├── 📚 ORIGINAL REFERENCE MATERIALS
│   ├── docs/                             ← 14 arkitektur-dokumenter
│   ├── MIDI_PROTOCOL.md                  ← Reference (root)
│   ├── EFFECT_REFERENCE.md               ← Reference (root)
│   ├── ARCHITECTURE_ANALYSIS.md          ← Reference (root)
│   ├── PROJECT_KNOWLEDGE.md              ← Reference (root)
│   ├── Nova manager Original/            ← Legacy Java app (read-only reference)
│   └── Tc originalt materiale/           ← MIDI specs, manuals, presets
│
├── 🔧 LLM DISCIPLINE SYSTEM
│   ├── llm-build-system/
│   │   ├── LLM_BUILD_INSTRUCTIONS.md     ← Unskippable rules
│   │   ├── CLEANUP_POLICY.md             ← Safe deletion
│   │   └── memory/
│   │       ├── SESSION_MEMORY.md         ← Current work
│   │       ├── BUILD_STATE.md            ← What's built
│   │       └── PITFALLS_FOUND.md         ← Lessons learned
│   │
│   └── tasks/                            ← Sequential task files
│       ├── 00-index.md                   ← Task master index
│       ├── 01-phase0-environment-setup.md
│       ├── 02-modul1-phase1-foundation.md
│       ├── 03-modul1-phase2-domain-models.md
│       └── 04-modul1-phase3-use-cases.md
│
├── 💻 SOURCE CODE (Created during development)
│   ├── NovaApp/                          ← Main .NET solution root
│   │   ├── NovaApp.sln                   ← Visual Studio solution
│   │   │
│   │   ├── src/
│   │   │   ├── Nova.Application/         ← Use cases, services
│   │   │   ├── Nova.Domain/              ← Entities, value objects
│   │   │   ├── Nova.Infrastructure/      ← MIDI I/O, file storage
│   │   │   ├── Nova.Presentation/        ← Avalonia UI
│   │   │   └── Nova.Common/              ← Shared utilities
│   │   │
│   │   ├── tests/
│   │   │   ├── Nova.Domain.Tests/        ← Domain unit tests
│   │   │   ├── Nova.Application.Tests/   ← Application integration
│   │   │   ├── Nova.Infrastructure.Tests/ ← MIDI mock tests
│   │   │   └── Nova.Presentation.Tests/  ← UI snapshot tests
│   │   │
│   │   ├── .gitignore                    ← Git ignore rules
│   │   ├── Directory.Build.props         ← Shared project settings
│   │   ├── global.json                   ← Lock .NET 8 version
│   │   └── nuget.config                  ← NuGet package sources
│   │
│   └── .git/                             ← Local Git repository
│       └── (all version history)
│
├── 🧪 BUILD ARTIFACTS (Generated, ignored by Git)
│   ├── bin/                              ← Compiled binaries
│   ├── obj/                              ← Intermediate objects
│   └── packages/                         ← NuGet cache (local)
│
└── 📱 DEPLOYMENT (Created during release)
    └── releases/
        └── v1.0.0/                       ← Final executable
            ├── NovaApp.exe
            ├── NovaApp.dll
            └── settings.json
```

**Vigtige Noter:**
- `/docs`, root reference files, `/Nova manager Original` = READ-ONLY reference
- `/llm-build-system` = Discipline enforcement (don't modify structure)
- `/src` = WHERE NEW CODE GOES
- `/tests` = WHERE TESTS GO (parallel to src)
- `.git/` = Created by Phase 0 setup

---

## 🔐 Git Setup (Lokalt Versionsstyring)

### Initial Setup (Phase 0)

```powershell
# Navigate to project root
cd "d:\Tc electronic projekt\Nyt program til Nova"

# Initialize Git repository
git init

# Add all files to tracking
git add .

# First commit
git commit -m "[INIT] Initial project structure and documentation"

# Verify
git log --oneline
```

### Branch Strategy

```
main (production-ready, all tests pass)
  ├── feature/midi-layer (Modul 1 Phase 1)
  ├── feature/domain-models (Modul 1 Phase 2)
  └── feature/use-cases (Modul 1 Phase 3)
```

**Rules:**
- `main` is always deployable (all tests pass, 0 warnings)
- Features branch from `main`, tested locally, merged back
- Commit messages: `[RED→GREEN→REFACTOR] Brief description`
- Pre-commit hook: `dotnet build` + `dotnet test` (must both pass)

### No GitHub Required

This is a **self-contained Git repository**. You can:
- ✅ Work offline, commit freely
- ✅ Revert changes anytime
- ✅ View complete history locally
- ✅ Later: Push to GitHub/Gitea/GitLab if desired
- ✅ Back up via: Copy entire folder to external drive

---

## 📊 Development Phases

| Fase | Mål | Værktøjer | Tid |
|------|-----|----------|-----|
| **0** | Miljø setup | Git, .NET SDK, Visual Studio | 1-2 h |
| **1.1** | MIDI layer | DryWetMIDI, xUnit, MockMidiPort | 1 uge |
| **1.2** | Domain models | C# entities, value objects | 1 uge |
| **1.3** | Use cases | Application services, DI container | 1 uge |
| **2** | UI prototype | Avalonia, XAML, data binding | 2 uge |
| **3** | Real hardware | Nova System pedal, USB-MIDI cable | 1 uge |
| **4** | AI integration | REST API, preset generation | 2 uge |
| **Release** | Distribution | dotnet publish, installer | 1 uge |

---

## ✅ Kvalitetsstandarder (Quality Standards)

### Test Coverage Minimums

- **Domain Layer**: ≥95% (business logic)
- **Application Layer**: ≥80% (use cases)
- **Infrastructure Layer**: ≥70% (MIDI I/O)
- **Presentation Layer**: ≥50% (UI snapshot tests)
- **Overall**: ≥75%

### Build Requirements

- ✅ Zero compiler warnings
- ✅ All tests pass
- ✅ Code coverage meets minimums
- ✅ No security vulnerabilities (dependency check)
- ✅ Git history is clean (meaningful commits)

### Code Style

- C# naming: `PascalCase` (classes), `camelCase` (fields)
- SOLID principles enforced (S, O, L, I, D)
- Comments: "Why", not "What" (code is self-documenting)
- Async/await used for I/O operations

---

## 📞 Kontakt & Support

**Hvis fejl opstår:**
1. Check `llm-build-system/memory/PITFALLS_FOUND.md` (common mistakes)
2. Read `llm-build-system/LLM_BUILD_INSTRUCTIONS.md` (discipline rules)
3. Verify: `dotnet build` (0 warnings), `dotnet test` (all pass)

**Hvis Git er kørt fast:**
```powershell
git status              # See current state
git log --oneline       # See commit history
git diff                # See uncommitted changes
git reset --hard HEAD   # Undo uncommitted changes (⚠️ destructive)
```

---

## 🎯 Næste Trin (Next Steps)

1. **Read**: `FOLDER_STRUCTURE.md` (detaljeret mappestruktur)
2. **Read**: `SETUP_AUTOMATION.md` (automatiseret setup)
3. **Start**: `tasks/01-phase0-environment-setup.md` (miljøopsætning)
4. **Initialize**: `setup.ps1` script (Git + .NET setup)

---

**Manifest Version**: 1.0  
**Last Updated**: 30. januar 2026  
**Status**: ✅ Ready for Phase 0 Execution
