# MASTER_FILE_INDEX.md — Fil-oversigt og formål

## ⚠️ LÆS DETTE FØRST

Denne fil beskriver ALLE filer i projektet og deres formål.

---

## 📂 ROD-FILER (Nyt program til Nova/)

| Fil | Type | Formål | Opdateret |
|-----|------|--------|-----------|
| `PROGRESS.md` | **AKTIV** | Procent-tracker. Opdateres efter HVER commit. | ✅ |
| `STATUS.md` | **AKTIV** | Projekt-status oversigt. | ✅ |
| `README.md` | **AKTIV** | Kort intro til projektet. | ✅ |
| `global.json` | **CONFIG** | Låser .NET SDK til 8.0.417. | ✅ |
| `Directory.Build.props` | **CONFIG** | Fælles build-settings (net8.0, C# 11). | ✅ |
| `NovaApp.sln` | **CONFIG** | Visual Studio solution fil. | ✅ |
| `.gitignore` | **CONFIG** | Git ignore regler. | ✅ |
| `.editorconfig` | **CONFIG** | Code style regler. | ✅ |
| `setup.ps1` | **SCRIPT** | Automatisk projekt-opsætning. Bruges ved ny installation. | ⏸️ |
| `verify-commit.ps1` | **SCRIPT** | Pre-commit verificering. Bruges ved CI/CD setup. | ⏸️ |

### Reference-filer (READ-ONLY)

| Fil | Formål | Hvornår bruges |
|-----|--------|----------------|
| `MIDI_PROTOCOL.md` | SysEx format, byte offsets, checksum | Når du skriver MIDI kode |
| `EFFECT_REFERENCE.md` | Alle 15 effekt-typer med parametre | Når du bygger UI for effekter |
| `PROJECT_KNOWLEDGE.md` | Syntese af al hardware-viden | General reference |

**VIGTIGT**: Reference-filer må IKKE ændres. De er ekstraheret fra TC Electronic dokumentation.

---

## 📂 llm-build-system/

| Fil | Type | Formål |
|-----|------|--------|
| `AGENTS.md` | **OBLIGATORISK** | Pipeline for LLM agenter. Læs FØRST. |
| `LLM_BUILD_INSTRUCTIONS.md` | **REFERENCE** | Detaljerede build-regler. |
| `CLEANUP_POLICY.md` | **REFERENCE** | Regler for sletning/refactoring. |
| `memory/SESSION_MEMORY.md` | **AKTIV** | Opdateres hver session. |
| `memory/BUILD_STATE.md` | **AKTIV** | Opdateres efter commits. |
| `memory/PITFALLS_FOUND.md` | **AKTIV** | Fejl og lessons learned. |

---

## 📂 tasks/

| Fil | Status | Kræver |
|-----|--------|--------|
| `00-index.md` | **AKTIV** | Navigation til alle tasks |
| `03-modul1-phase2-domain-models.md` | ✅ DONE | - |
| `04-modul1-phase3-use-cases.md` | ✅ DONE | - |
| `05-modul1-phase4-infrastructure.md` | ⬜ **CURRENT** | Alle modeller |
| `06-modul1-phase5-presentation-SONNET45.md` | ⬜ TODO | **Sonnet 4.5+** |
| `07-modul2-preset-viewer.md` | ⬜ TODO | Alle modeller |
| `08-modul3-system-viewer.md` | ⬜ TODO | Alle modeller |
| `09-modul4-system-editor.md` | ⬜ TODO | Alle modeller |
| `10-modul5-preset-detail.md` | ⬜ TODO | Alle modeller |
| `11-modul6-preset-editor-SONNET45.md` | ⬜ TODO | **Sonnet 4.5+** |
| `12-modul7-preset-management.md` | ⬜ TODO | Alle modeller |
| `13-modul8-file-io.md` | ⬜ TODO | Alle modeller |
| `14-modul9-midi-mapping-SONNET45.md` | ⬜ TODO | **Sonnet 4.5+** |
| `15-modul10-release-SONNET45.md` | ⬜ TODO | **Sonnet 4.5+** |

---

## 📂 docs/ (READ-ONLY Reference)

Disse filer er arkitektur-dokumentation. Læs dem, men ændr dem IKKE.

| Fil | Indhold |
|-----|---------|
| `00-index.md` | Oversigt over docs |
| `01-vision-scope.md` | Projektets formål |
| `02-stack-and-tooling.md` | Teknologi-valg |
| `03-architecture.md` | 4-lags arkitektur |
| `04-testing-strategy.md` | Test-discipline |
| `05-midi-io-contract.md` | IMidiPort interface |
| `06-sysex-formats.md` | SysEx byte layout |
| `07-module-roadmap.md` | Modul-plan |
| `08-ui-guidelines.md` | UI design regler |
| `09-release-installer.md` | Installer spec |
| `10-risk-assumptions.md` | Risici |
| `SYSEX_MAP_TABLES.md` | Parameter offset tabeller |

---

## 📂 src/ (Kode)

| Projekt | Status | Indhold |
|---------|--------|---------|
| `Nova.Domain/` | ✅ KOMPLET | Preset, UserBankDump, SystemDump |
| `Nova.Application/` | ✅ KOMPLET | ConnectUseCase, DownloadBankUseCase, SaveBankUseCase, LoadBankUseCase |
| `Nova.Midi/` | ✅ KOMPLET | IMidiPort, MockMidiPort |
| `Nova.Infrastructure/` | ⬜ TOM | Mangler DryWetMidiPort |
| `Nova.Presentation/` | ⬜ TEMPLATE | Kun Avalonia skabelon |
| `Nova.Common/` | ⬜ TOM | Delte utilities |
| `Nova.HardwareTest/` | ✅ VÆRKTØJ | Console app til MIDI test |
| `*.Tests/` | ✅ 195 tests | Unit tests (156 passing, 39 pre-existing failures) |

---

## 📂 Arkiv/

Gamle/forældede filer. Brug dem IKKE.

---

## 🔧 SCRIPTS — Hvornår bruges de?

| Script | Hvornår |
|--------|---------|
| `setup.ps1` | Ved ny installation af projektet |
| `verify-commit.ps1` | Ved CI/CD pipeline opsætning (Modul 10) |

---

## ✅ BESLUTNINGER (Låst)

| Emne | Beslutning |
|------|------------|
| .NET version | 8.0 LTS |
| C# version | 11 |
| UI framework | Avalonia 11.x |
| MIDI library | DryWetMIDI 7.x |
| Test framework | xUnit 2.6 + Moq 4.18 |
| Arkitektur | 4-lags Clean Architecture |
| Git workflow | Lokal repository, ingen GitHub |

---

**Sidst opdateret**: 2026-02-01
