# PROJEKT STATUS — Nova Manager

## 📊 Komplet Overblik

| Komponent | Status | Beskrivelse |
|-----------|--------|-------------|
| **Domain Layer** | ✅ 100% | Preset, UserBankDump, SystemDump, SysExBuilder, SysExValidator |
| **Application Layer** | ✅ 100% | ConnectUseCase, DownloadBankUseCase |
| **MIDI Abstraktion** | ✅ 100% | IMidiPort, MockMidiPort (test double) |
| **Infrastructure** | ⬜ 0% | DryWetMidiPort IKKE implementeret |
| **Presentation** | ⬜ 0% | Kun Avalonia template, ingen reel UI |
| **Tests** | ✅ 117+ | Domain 108, MIDI 6, Application 4, Baseline 3 |

---

## 🚦 Moduler

| Modul | Navn | Status | Filer |
|-------|------|--------|-------|
| 0 | Environment Setup | ✅ DONE | tasks/01-phase0-environment-setup.md |
| 1 | Foundation | 🟡 60% | Fase 1-3 ✅, Fase 4-5 ⬜ |
| 2-10 | Viewer/Editor/Release | ⬜ TODO | Se tasks/ mappen |

---

## ⚠️ Kritisk Gap

**App'en kan IKKE kommunikere med hardware endnu!**

Nova.Infrastructure er tom. Der mangler:
- `DryWetMidiPort.cs` — implementer IMidiPort med DryWetMIDI
- Integration tests med ægte hardware
- Error handling for port-tab/timeout

---

## 📁 Modul 1 Detaljer

| Fase | Navn | Status |
|------|------|--------|
| 1.1 | MIDI Abstraction | ✅ DONE |
| 1.2 | Domain Models | ✅ DONE (78 params, 521 bytes) |
| 1.3 | Use Cases | ✅ DONE (Connect, DownloadBank) |
| 1.4 | Infrastructure | ⬜ TODO (DryWetMidiPort) |
| 1.5 | Presentation | ⬜ TODO (Avalonia UI) |

---

## 📂 Projekt Struktur

```
src/
├── Nova.Domain/           ✅ Komplet (Preset, UserBankDump, SystemDump)
├── Nova.Application/      ✅ Komplet (UseCases)
├── Nova.Midi/             ✅ Komplet (IMidiPort, Mock)
├── Nova.Infrastructure/   ⬜ TOM (mangler DryWetMidiPort)
├── Nova.Presentation/     ⬜ PLACEHOLDER (kun template)
└── *.Tests/               ✅ 117+ tests

tasks/                     📋 Alle task-filer (01-15)
docs/                      📚 Reference dokumentation
Arkiv/                     📦 Arkiverede/gamle filer
```

---

## 🎯 Næste Skridt

1. **Modul 1, Fase 4**: Implementer `DryWetMidiPort.cs`
2. **Modul 1, Fase 5**: Byg minimal Avalonia UI
3. **Test**: Verificer hardware-kommunikation
4. **Modul 2+**: Preset Viewer, Editor, osv.

---

## ⏱️ Tidsestimat

| Opgave | Uger |
|--------|------|
| Modul 1 (rest) | 2 |
| Modul 2-4 | 3 |
| Modul 5-6 | 5 |
| Modul 7-10 | 8 |
| **Total** | **~18 uger** |

---

## 🧠 Model Selection Guide

Se `tasks/00-index.md` for kompleksitets-markering:
- 🟢 SIMPLE → Haiku/GPT-4o-mini
- 🟡 MEDIUM → Sonnet/GPT-4o  
- 🔴 HIGH → **SONNET 4.5+** eller **Claude Opus**

---

**Sidst opdateret**: 2025-02-02  
**Næste task**: [tasks/05-modul1-phase4-infrastructure.md](tasks/05-modul1-phase4-infrastructure.md)
