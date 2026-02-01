# Folder Structure (Reference)

Denne fil beskriver den faktiske struktur i `Nyt program til Nova/` pr. 1. februar 2026.

## Level 1: Project Root

```
Nyt program til Nova/
├── README.md
├── START_HERE.md
├── STATUS.md
├── APPLICATION_MANIFEST.md
├── PROJECT_MANIFEST_COMPLETE.md
├── PROJECT_HEALTH_ASSESSMENT.md
├── STRUCTURAL_ANALYSIS_REPORT.md
├── DOCUMENTATION_COMPLETE.md
├── LLM_DISCIPLINE_SYSTEM.md
├── LLM_SYSTEM_COMPLETE.md
├── ARCHITECTURE_ANALYSIS.md
├── PROJECT_KNOWLEDGE.md
├── MIDI_PROTOCOL.md
├── EFFECT_REFERENCE.md
├── FOLDER_STRUCTURE.md
├── SETUP_AUTOMATION.md
├── setup.ps1
├── verify-commit.ps1
├── global.json
├── Directory.Build.props
├── NovaApp.sln
├── .editorconfig
├── .gitignore
├── docs/
├── tasks/
├── llm-build-system/
└── src/
```

## docs/ (Architecture + project docs)

```
docs/
├── 00-index.md
├── 01-vision-scope.md
├── 02-stack-and-tooling.md
├── 03-architecture.md
├── 04-testing-strategy.md
├── 05-midi-io-contract.md
├── 06-sysex-formats.md
├── 07-module-roadmap.md
├── 08-ui-guidelines.md
├── 09-release-installer.md
├── 10-risk-assumptions.md
├── 11-modul1-technical-detail.md
├── 12-environment-setup-checklist.md
├── 13-test-fixtures.md
├── 14-ready-for-implementation.md
└── SYSEX_MAP_TABLES.md
```

## tasks/ (Execution tasks)

```
tasks/
├── 00-index.md
├── 01-phase0-environment-setup.md
├── 02-modul1-phase1-foundation.md
├── 03-modul1-phase2-domain-models.md
└── 04-modul1-phase3-use-cases.md
```

## src/ (Code)

```
src/
├── Nova.Common/
├── Nova.Domain/
├── Nova.Midi/
├── Nova.Infrastructure/
├── Nova.Application/
├── Nova.Presentation/
├── Nova.Domain.Tests/
├── Nova.Application.Tests/
├── Nova.Infrastructure.Tests/
├── Nova.Midi.Tests/
├── Nova.Presentation.Tests/
└── Nova.HardwareTest/
```

### Test projects
Tests ligger sammen med kode i `src/*Tests`-projekter (ikke i en separat `tests/` mappe).

### Hardware test data
`src/Nova.HardwareTest/` indeholder værktøjer til at optage SysEx-dumps fra hardware.
Genererede filer hedder typisk `nova-dump-YYYYMMDD-HHMMSS-msgNNN.syx` og ignoreres af Git.

## llm-build-system/

Indeholder build-instruktioner og session-memory (interne arbejdsfiler til LLM-workflow).

## Principper

- Ingen ny top-level mappe uden behov.
- Namespace matcher mappeplacering.
- One class = one file (når vi går i gang med rigtig kode).
- Tests afspejler source-projekter (samme navnerum, `*.Tests`).

**Status**: Opdateret til faktisk struktur.
