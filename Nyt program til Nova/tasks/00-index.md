# Task Index — Nova Manager

## 🎯 Project Goal
Windows application til at læse, redigere og skrive presets/system-settings til TC Electronic Nova System guitar effects processor via MIDI SysEx.

---

## 📊 Samlet Overblik

| Modul | Navn | Status | Filer |
|-------|------|--------|-------|
| 0 | Environment Setup | ✅ DONE | [01-phase0-environment-setup.md](01-phase0-environment-setup.md) |
| 1 | Foundation | 🟡 60% | Se faser nedenfor |
| 2 | Preset Viewer | ⬜ TODO | [07-modul2-preset-viewer.md](07-modul2-preset-viewer.md) |
| 3 | System Viewer | ⬜ TODO | [08-modul3-system-viewer.md](08-modul3-system-viewer.md) |
| 4 | System Editor | ⬜ TODO | [09-modul4-system-editor.md](09-modul4-system-editor.md) |
| 5 | Preset Detail | ⬜ TODO | [10-modul5-preset-detail.md](10-modul5-preset-detail.md) |
| 6 | Preset Editor | ⬜ TODO | [11-modul6-preset-editor.md](11-modul6-preset-editor.md) |
| 7 | Preset Management | ⬜ TODO | [12-modul7-preset-management.md](12-modul7-preset-management.md) |
| 8 | File I/O | ⬜ TODO | [13-modul8-file-io.md](13-modul8-file-io.md) |
| 9 | MIDI Mapping | ⬜ TODO | [14-modul9-midi-mapping.md](14-modul9-midi-mapping.md) |
| 10 | Release | ⬜ TODO | [15-modul10-release.md](15-modul10-release.md) |

---

## 📁 Modul 1: Foundation (Detaljeret)

| Fase | Navn | Status | Fil |
|------|------|--------|-----|
| 1.1 | MIDI Abstraction | ✅ DONE | [02-modul1-phase1-foundation.md](02-modul1-phase1-foundation.md) |
| 1.2 | Domain Models | ✅ DONE | [03-modul1-phase2-domain-models.md](03-modul1-phase2-domain-models.md) |
| 1.3 | Use Cases | ✅ DONE | [04-modul1-phase3-use-cases.md](04-modul1-phase3-use-cases.md) |
| 1.4 | Infrastructure | ⬜ TODO | [05-modul1-phase4-infrastructure.md](05-modul1-phase4-infrastructure.md) |
| 1.5 | Presentation | ⬜ TODO | [06-modul1-phase5-presentation.md](06-modul1-phase5-presentation.md) |

---

## 🧠 Complexity Legend (Model Selection)

| Symbol | Niveau | Model krav |
|--------|--------|------------|
| 🟢 | TRIVIAL / SIMPLE | Enhver model (Haiku, GPT-4o-mini, etc.) |
| 🟡 | MEDIUM | Haiku / Sonnet / GPT-4o |
| 🔴 | HIGH / COMPLEX | **SONNET 4.5+** eller **Claude Opus** |

**Brugsanvisning**: Tjek task-filen før du starter. Hvis opgaven er markeret 🔴, brug en stærk model. Hvis 🟢, kan du spare penge med en billigere model.

---

## 🚀 Anbefalet Rækkefølge

```
Phase 0 ✅ → Modul 1 (1.1-1.3 ✅, 1.4-1.5 TODO) → Modul 2 → Modul 3 → ...
```

### ⚠️ Kritisk Næste Skridt
**Modul 1, Fase 4: Infrastructure** — Uden DryWetMidiPort kan appen ikke kommunikere med hardware!

---

## ⏱️ Tidsestimat

| Modul | Uger |
|-------|------|
| Modul 1 (rest) | 2 |
| Modul 2 | 1 |
| Modul 3-4 | 2 |
| Modul 5-6 | 5 |
| Modul 7-8 | 3 |
| Modul 9-10 | 5 |
| **Total** | **~18 uger** |

---

## 📚 Reference Dokumentation

| Dokument | Beskrivelse |
|----------|-------------|
| [../docs/03-architecture.md](../docs/03-architecture.md) | 4-lags arkitektur |
| [../docs/05-midi-io-contract.md](../docs/05-midi-io-contract.md) | MIDI interface kontrakt |
| [../docs/06-sysex-formats.md](../docs/06-sysex-formats.md) | SysEx byte layout |
| [../docs/SYSEX_MAP_TABLES.md](../docs/SYSEX_MAP_TABLES.md) | Parameter offset tabeller |
| [../docs/08-ui-guidelines.md](../docs/08-ui-guidelines.md) | UI design retningslinjer |

---

## 📂 Arkiverede Filer

Gamle/færdige dokumenter er flyttet til `Arkiv/` mappen.

---

## How to Use This System

### For Each Task File

1. **Read completely** before starting
2. **Do tasks in order** (listed 1-N)
3. **Follow TEST FIRST always**:
   - Write test that fails (RED)
   - Write minimal code (GREEN)
   - Clean up code (REFACTOR)
   - Commit with [RED→GREEN→REFACTOR]

4. **Don't skip**:
   - Tests ❌
   - Verification ❌
   - Commits ❌
   - Coverage checks ❌

5. **Mark complete** in this file

---

**Sidst opdateret**: 2025-02-02  
**Næste skridt**: Modul 1, Fase 4 (Infrastructure)
