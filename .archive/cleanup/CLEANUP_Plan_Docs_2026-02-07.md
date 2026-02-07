# DOCS/ CLEANUP PLAN - Detaljeret Eksekverings Plan

**Dato:** 2026-02-07
**Status:** PLAN KLAR TIL GODKENDELSE
**Scope:** 19 filer i docs/ mappen

---

## 📋 EXECUTIVE SUMMARY

**Nuværende tilstand:**
- 19 filer i docs/
- 4 filer er totalt outdated (beskriver planlagt udvikling for færdigt projekt)
- 9 filer indeholder forældet info men har stadig værdi
- 6 filer er korrekte og skal beholdes

**Målsætning:**
- Slet 4 irrelevante planningsdokumenter
- Opdater 9 filer til at reflektere nuværende projekt status
- Behold 6 korrekte tekniske referencer
- **Resultat:** 15 relevante, opdaterede docs filer

---

## 🎯 FASE 1: SLET OUTDATED FILER (4 filer)

### Rationale for sletning
Disse filer beskriver "hvordan projektet skal bygges" - men projektet ER allerede bygget!
- 453/458 tests passing (98.9%)
- Clean architecture implementeret
- MIDI kommunikation fungerer
- App kan downloade/uploade presets

### Filer der slettes:

#### 1. `07-module-roadmap.md` (285 linjer)
**Hvorfor slet:**
- Beskriver 10 modulers udvikling over "5-6 måneder"
- Linje 3: "Modul 1 — Status: Starting point"
- Men Modul 1-6 er allerede implementeret!
- Timeline estimate (linje 262-276) er irrelevant

**Indhold værd at bevare:** INGEN
- Alt er enten implementeret eller irrelevant

**Action:** `git rm docs/07-module-roadmap.md`

---

#### 2. `11-modul1-technical-detail.md` (324 linjer)
**Hvorfor slet:**
- Implementation guide for "Modul 1"
- Linje 3: "Efter denne modul, ved vi at MIDI kommunikation virker stabilt"
- Men MIDI kommunikation VIRKER allerede (8e37993 commit)
- Deliverables checklist (linje 316-323) allerede completeret

**Indhold værd at bevare:** DELVIS
- Use case flows (linjer 8-88) er god dokumentation af faktisk implementation
- Data structures (linjer 91-234) matcher koden

**Alternative:**
- Gem flows i `03-architecture.md` under "Real Implementation Examples"
- ELLER bare slet alt (flows kan læses fra kildekode)

**Recommended Action:** SLET (kildekode er sandhed)
- `git rm docs/11-modul1-technical-detail.md`

---

#### 3. `12-environment-setup-checklist.md` (268 linjer)
**Hvorfor slet:**
- Setup guide til at bygge projekt fra scratch
- Phase 1-7 beskriver installation af .NET, VS, Git, osv.
- Men projekt eksisterer allerede! (migreret, bygger, tests kører)

**Indhold værd at bevare:** MINIMAL
- Troubleshooting section (linje 245-260) har generiske tips
- Men disse er bedre placeret i README.md eller TROUBLESHOOTING_PLAN.md

**Recommended Action:** SLET
- `git rm docs/12-environment-setup-checklist.md`
- Hvis troubleshooting tips er unikke, flyt dem til TROUBLESHOOTING_PLAN.md først

---

#### 4. `14-ready-for-implementation.md` (289 linjer)
**Hvorfor slet:**
- "Pre-Launch Checklist" for projekt der aldrig blev launchet
- Linje 2: "Pre-Launch Checklist"
- Linje 28: "Next Phase: Environment Setup"
- Men development er FÆRDIGT! (tests passing, app virker)

**Indhold værd at bevare:** INGEN
- Checklist allerede completeret
- Flowguide irrelevant for færdigt projekt

**Recommended Action:** SLET
- `git rm docs/14-ready-for-implementation.md`

---

## 🔧 FASE 2: OPDATER FILER (9 filer)

### 1. `00-index.md` - Navigation index

**Nuværende problemer:**
- Linje 13: `../AGENTS.md` eksisterer ikke
- Linje 27: `../EFFECT_REFERENCE.md` skal være `EFFECT_REFERENCE.md` (flyttet til docs/)
- Mangler referencer til: SYSEX_MAP_TABLES.md, USER_MANUAL.md

**Changes:**
```diff
- - `../AGENTS.md` – LLM masterkontekst og regler
+ (SLET DENNE LINJE - filen eksisterer ikke)

- - `..\MIDI_PROTOCOL.md` – detaljeret MIDI protokol
- - `..\EFFECT_REFERENCE.md` – alle effekter
+ - `../MIDI_PROTOCOL.md` – detaljeret MIDI protokol
+ - `EFFECT_REFERENCE.md` – alle effekter (i docs/)
+ - `SYSEX_MAP_TABLES.md` – detaljeret SysEx mapping tables
+ - `USER_MANUAL.md` – bruger manual
```

**Estimated effort:** 5 min

---

### 2. `01-vision-scope.md` - Vision dokument

**Nuværende problemer:**
- Beskriver vision som fremtidig, men MVP er færdigt
- Success Criteria (linje 31-44) allerede opfyldt

**Changes:**
Tilføj note i toppen:
```markdown
# Vision & Scope

**STATUS:** MVP (Modul 1-6) COMPLETERET - 453/458 tests passing (98.9%)
Dette dokument beskriver den oprindelige vision. Se CLAUDE.md for nuværende status.

---

## Vision
...
```

**Estimated effort:** 5 min

---

### 3. `02-stack-and-tooling.md` - Tech stack

**Nuværende problemer:**
- Linje 6: Siger ".NET 8 LTS" men app kører .NET 10
- Linje 57-67: Projekt struktur matcher ikke realiteten

**Changes:**
```diff
- | **Sprog** | C# | .NET 8 LTS |
+ | **Sprog** | C# | .NET 10 |

- NovaApp/
- ├── Nova.Presentation/                  # Avalonia UI, XAML, ViewModels
- ├── Nova.Application/         # Use cases, commands
- ├── Nova.Domain/              # Presets, params, validering
- ├── Nova.Midi/                # DryWetMIDI wrapper, I/O
- ├── Nova.Infrastructure/      # File I/O, config
- ├── Nova.Tests/               # xUnit + Moq test suite
- └── NovaApp.sln                  # Solution file
+ src/
+ ├── Nova.Presentation/          # Avalonia UI (79 tests)
+ ├── Nova.Application/           # Use cases (97 tests)
+ ├── Nova.Application.Tests/
+ ├── Nova.Domain/                # Domain models (261 tests)
+ ├── Nova.Domain.Tests/
+ ├── Nova.Midi/                  # MIDI abstraction (8 tests)
+ ├── Nova.Midi.Tests/
+ ├── Nova.Infrastructure/        # MIDI + File I/O (13 tests)
+ ├── Nova.Infrastructure.Tests/
+ ├── Nova.Presentation.Tests/
+ ├── Nova.Common/                # Shared utilities
+ └── Nova.HardwareTest/          # Hardware integration tests
```

**Estimated effort:** 10 min

---

### 4. `04-testing-strategy.md` - Test strategi

**Nuværende problemer:**
- Linje 103-113: Beskriver test fixtures i "Nova.Tests/" men struktur er anderledes
- Linje 143-153: CI/CD sektion irrelevant (local-first workflow)

**Changes:**
```diff
- ## Test fixtures
- ```
- Nova.Tests/
- ├── Fixtures/
- │   ├── user-bank-dump-valid.syx
- ...
- ```
+ ## Test fixtures
+ Fixtures er organiseret per test projekt:
+ - Nova.Domain.Tests/Fixtures/ (SysEx dumps, presets)
+ - Nova.Application.Tests/TestHelpers.cs (valid preset factory)
+ - Nova.Presentation.Tests/TestHelpers.cs (valid preset factory)

- ## CI/CD (GitHub Actions, later)
- ```yaml
- on: [push, pull_request]
- ...
- ```
+ ## CI/CD
+ CI er slået fra. GitHub bruges kun som backup (local-first workflow).
+ Se CLAUDE.md for backup procedure.
```

**Estimated effort:** 10 min

---

### 5. `08-ui-guidelines.md` - UI design

**Nuværende problemer:**
- Linje 163-172: "Modul 1 MVP UI" sektion - Modul 1 er færdigt!

**Changes:**
```diff
- ## Modul 1 MVP UI
- **Minimum for Modul 1:**
- - Window with title
- - Port picker
- - Connect button
- - Download Bank button
- - Status message area
- - (No presets list yet)
-
- **Keep it simple. More UI in Modul 2+.**
+ (SLET HELE DENNE SEKTION - Modul 1 er implementeret)
```

**Estimated effort:** 2 min

---

### 6. `09-release-installer.md` - Release proces

**Nuværende problemer:**
- Linje 46: `net8.0-windows` men app bruger .NET 10
- Linje 142-161: GitHub Actions sektion irrelevant

**Changes:**
```diff
- **Output:** `NovaApp/bin/Release/net8.0-windows/win-x64/publish/`
+ **Output:** `NovaApp/bin/Release/net10.0-windows/win-x64/publish/`

- ## GitHub Actions (later)
- ```yaml
- name: Release
- ...
- ```
+ ## GitHub Actions
+ IKKE BRUGT - Local-first workflow. GitHub kun backup.
```

**Estimated effort:** 5 min

---

### 7. `10-risk-assumptions.md` - Risici

**Nuværende problemer:**
- Linje 157-171: Mitigation timeline beskriver "Before Modul X" men moduler færdige
- Mange risici allerede mitigated

**Changes:**
Tilføj status note i toppen:
```markdown
# Risici & Antagelser

**MITIGATION STATUS:** De fleste kritiske risici er allerede håndteret (se CLAUDE.md).

---
```

Opdater mitigation timeline:
```diff
- ## Mitigation timeline
-
- ### Before Modul 1 release
- - Checksum validation ✓
- - Timeout + retry logic ✓
- - SysEx buffering ✓
- - Error UI ✓
+ ## Mitigation Status (2026-02-07)
+
+ ### IMPLEMENTERET ✅
+ - Checksum validation ✅ (Domain validation, commit 8e37993)
+ - Timeout + retry logic ✅
+ - SysEx buffering ✅
+ - Error UI ✅
+ - MIDI memory leaks fixed ✅ (commit 8e37993)
```

**Estimated effort:** 15 min

---

### 8. `13-test-fixtures.md` - Test fixtures

**Nuværende problemer:**
- Linje 4: "needed for Modul 1 testing" - Modul 1 færdigt
- Linje 15-34: Fixture organization ikke opdateret til faktisk struktur

**Changes:**
```diff
- ## Overview
- This document defines all test fixtures (real SysEx data) needed for Modul 1 testing.
+ ## Overview
+ This document defines test fixtures (real SysEx data) used across test projekter.

- ## Test Data Organization
- ```
- src/Nova.Domain.Tests/
- ├── Fixtures/
- │   ├── BankDumps/
- ...
- ```
+ ## Test Data Organization
+ Fixtures organiseret per test projekt:
+ - Nova.Domain.Tests/Fixtures/ (raw SysEx data)
+ - Nova.Application.Tests/TestHelpers.cs (CreateValidPresetSysEx)
+ - Nova.Presentation.Tests/TestHelpers.cs (CreateValidPreset)
```

Fjern alle "Modul 1" references gennem dokumentet.

**Estimated effort:** 20 min

---

### 9. `USER_MANUAL.md` - Bruger manual

**Nuværende problemer:**
- Linje 3: "Last Updated: February 3, 2026" - outdated
- Linje 64: GitHub Releases URL - men local-first workflow
- Beskriver .msi installer der ikke eksisterer
- Beskriver features måske ikke implementeret

**Changes:**
```diff
- **Last Updated**: February 3, 2026
+ **Last Updated**: February 7, 2026
+ **STATUS**: DRAFT - Ikke alle features implementeret endnu

- 1. Navigate to the [GitHub Releases page](...)
- 2. Download the latest `NovaSystemManager-v1.0.0.msi` file
+ 1. Installer endnu ikke tilgængelig (under udvikling)
+ 2. Byg fra kildekode: `dotnet run --project src/Nova.Presentation`
```

Tilføj warning om ikke-implementerede features.

**Estimated effort:** 15 min

---

## ✅ FASE 3: VERIFY BEHOLD FILER (6 filer)

Disse filer er korrekte og skal IKKE ændres:

1. ✅ `03-architecture.md` - Korrekt arkitektur beskrivelse
2. ✅ `05-midi-io-contract.md` - Kritisk MIDI interface spec
3. ✅ `06-sysex-formats.md` - SysEx reference
4. ✅ `EFFECT_REFERENCE.md` - Effekt teknisk reference
5. ✅ `SYSEX_MAP_TABLES.md` - Detaljeret mapping tables
6. ✅ `TROUBLESHOOTING_PLAN.md` - Opdateret tidligere (stier rettet)

**Action:** Ingen ændringer

---

## 🔍 FASE 4: GAP ANALYSE

### Mangler der professionelle dev filer?

**Tjek for manglende standard filer:**
- ✅ `README.md` (eksisterer i root)
- ✅ `CLAUDE.md` (AI memory)
- ❓ `.editorconfig` (kode formatering standard)
- ❓ `LICENSE` (open source licens?)
- ❓ `CONTRIBUTING.md` (contribution guidelines)
- ❓ `CODE_OF_CONDUCT.md` (hvis open source)

**Recommended additions:**
- `.editorconfig` - For konsistent kode formatering
- `LICENSE` - Hvis projektet skal deles

**NOTE:** Dette er for Task #15 (Gap-Analyse) - IKKE denne task!

---

## 📊 EKSEKVERINGS RÆKKEFØLGE

### Step 1: Slet outdated filer (FASE 1)
```bash
git rm docs/07-module-roadmap.md
git rm docs/11-modul1-technical-detail.md
git rm docs/12-environment-setup-checklist.md
git rm docs/14-ready-for-implementation.md
```

**Rationale:** Slet først for at reducere forvirring

### Step 2: Opdater navigation først (FASE 2.1)
```bash
# Ret 00-index.md først (så navigation er korrekt)
```

### Step 3: Opdater tekniske docs (FASE 2.2-2.9)
```bash
# Opdater i rækkefølge:
# 01, 02, 04, 08, 09, 10, 13, USER_MANUAL
```

### Step 4: Verify (FASE 3)
```bash
# Tjek at BEHOLD filer stadig er korrekte
```

### Step 5: Commit
```bash
git add docs/
git commit -m "docs: Cleanup outdated planning docs, update to reflect implemented status

- SLET: 4 outdated planning docs (module roadmap, setup guides)
- OPDATER: 9 docs til nuværende projekt status (.NET 10, implemented features)
- BEHOLD: 6 tekniske reference docs (korrekte)

Rationale: Fjern pre-implementation planning docs for færdigt projekt (453/458 tests passing)
"
```

---

## ⏱️ TIDS ESTIMAT

| Fase | Opgave | Tid |
|------|--------|-----|
| 1 | Slet 4 filer | 2 min |
| 2.1 | Opdater 00-index.md | 5 min |
| 2.2-2.9 | Opdater 8 andre filer | 82 min |
| 3 | Verify BEHOLD filer | 10 min |
| 4 | Git commit | 2 min |
| **TOTAL** | **~100 min (1.5 timer)** | |

---

## 🚨 RISICI & MITIGATIONS

### Risiko 1: Tab af værdifuld information
**Mitigation:** Git bevarer alt! Hvis vi sletter noget vigtigt, kan vi restore fra historik.

### Risiko 2: Opdateringer introducerer fejl
**Mitigation:** Verify efter hver opdatering. Hvis tvivl, lav mindre ændringer.

### Risiko 3: USER_MANUAL.md for tidligt?
**Mitigation:** Marker som DRAFT og tilføj warnings om ikke-implementerede features.

---

## ✅ GODKENDELSES SPØRGSMÅL

Før eksekvering, bekræft:

1. **Er du OK med at slette 4 filer?**
   - 07-module-roadmap.md
   - 11-modul1-technical-detail.md
   - 12-environment-setup-checklist.md
   - 14-ready-for-implementation.md

2. **Er du OK med at opdatere 9 filer?**
   - Ja til alle 9? Eller skal nogle springes over?

3. **Skal USER_MANUAL.md markeres DRAFT eller helt slettes?**
   - DRAFT (recommended)
   - SLET

4. **Eksekvér nu eller gem plan?**
   - Eksekvér hele planen nu
   - Eksekvér fase-for-fase (pause mellem faser)
   - Gem planen, eksekvér senere

---

**PLAN STATUS:** KLAR TIL GODKENDELSE ✨

**Næste action:** Afvent bruger beslutning på de 4 spørgsmål.
