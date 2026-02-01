# AGENTS.md — Obligatorisk Pipeline for LLM Agenter

**Sprog**: Alt skrives på dansk.  
**Vigtigst**: Følg denne fil fra start til slut. Ingen spring. Ingen genveje.

---

## PIPELINE TRIN 1: Læs disse filer (i denne rækkefølge)

```
1. MASTER_FILE_INDEX.md (rod-mappen) — Oversigt over ALLE filer
2. llm-build-system/memory/SESSION_MEMORY.md
3. llm-build-system/memory/BUILD_STATE.md
4. PROGRESS.md (rod-mappen)
5. tasks/00-index.md
6. Den aktuelle task-fil (se PROGRESS.md for hvilken)
```

Læs HELE filen. Ikke kun overskrifter.

---

## PIPELINE TRIN 2: Verificer grøn start

Kør disse kommandoer:

```powershell
cd "c:\Users\mike_\Desktop\Tc electronic projekt\Nyt program til Nova"
dotnet build --verbosity quiet
dotnet test --verbosity quiet
```

**Krav**: 0 fejl, 0 warnings, alle tests grønne.

Hvis ikke grøn: STOP. Løs problemet før du fortsætter.

---

## PIPELINE TRIN 3: Opdater SESSION_MEMORY.md

Åbn `llm-build-system/memory/SESSION_MEMORY.md` og skriv:

```markdown
## Session: [DATO]

### Mål
[Hvad skal laves i dag - kopieret fra task-filen]

### Nuværende task
[Fil: tasks/XX-navn.md, Task: X.X]
```

---

## PIPELINE TRIN 4: Udfør opgaven

Følg task-filen PRÆCIST. Hvert task har:

1. **TEST FØRST** (RED) — Skriv test der fejler
2. **KOD** (GREEN) — Minimal kode til at bestå
3. **REFACTOR** — Ryd op, ingen adfærdsændring
4. **COMMIT** — Med format: `[MODUL-X][PHASE-Y] Beskrivelse`

**Aldrig**: Skriv kode uden test først.  
**Aldrig**: Commit uden grøn build.  
**Aldrig**: Spring et task over.

---

## PIPELINE TRIN 5: Efter hver commit

Kør:

```powershell
dotnet build --verbosity quiet
dotnet test --verbosity quiet
```

Opdater disse filer:

1. `llm-build-system/memory/BUILD_STATE.md` — Hvad blev lavet
2. `PROGRESS.md` — Opdater procent og current task

---

## PIPELINE TRIN 6: Ved session-slut

1. Kør fuld test: `dotnet test`
2. Opdater `SESSION_MEMORY.md` med hvad der blev gjort
3. Opdater `BUILD_STATE.md` med commits
4. Opdater `PROGRESS.md` med ny procent
5. Commit: `[SESSION] Afslut session DATO`

---

## REGLER (ingen undtagelser)

| Regel | Handling |
|-------|----------|
| Test fejler | STOP. Fiks testen. |
| Build fejler | STOP. Fiks buildet. |
| Usikker på noget | SPØRG brugeren. Gæt aldrig. |
| Task-fil siger "SONNET 4.5+" | Model-krav. Brug ikke svagere model. |
| Vil ændre eksisterende test | SPØRG først. Dokumenter hvorfor. |

---

## COMMIT FORMAT

```
[MODUL-X][PHASE-Y] Kort beskrivelse

- Hvad blev ændret
- Hvilke tests tilføjet
- Eventuelle noter
```

Eksempel:
```
[MODUL-1][PHASE-4] Implement DryWetMidiPort.ListPorts()

- Added ListAvailablePorts() method
- 3 unit tests added
- Uses DryWetMIDI InputDevice/OutputDevice enumeration
```

---

## COVERAGE KRAV

| Lag | Minimum |
|-----|---------|
| Domain | 95% |
| Application | 80% |
| Infrastructure | 70% |
| MIDI | 90% |
| Presentation | 50% |

---

## FILER DER SKAL OPDATERES

| Fil | Hvornår |
|-----|---------|
| `SESSION_MEMORY.md` | Start + slut af session |
| `BUILD_STATE.md` | Efter hver commit |
| `PROGRESS.md` | Efter hver commit |
| `PITFALLS_FOUND.md` | Når der opstår problemer |

---

## HVIS NOGET GÅR GALT

1. Læs fejlmeddelelsen HELT
2. Kør: `dotnet test --verbosity diagnostic`
3. Tjek `BUILD_STATE.md` for sidste gode commit
4. Hvis stuck: `git status`, `git diff`, spørg brugeren
5. Dokumenter i `PITFALLS_FOUND.md`

---

## SCOPE (hvad må ændres)

| Mappe | Status |
|-------|--------|
| `src/` | ✅ Skriv kode her |
| `tasks/` | ✅ Opdater status |
| `llm-build-system/memory/` | ✅ Opdater state |
| `docs/` | ⛔ READ-ONLY |
| `Nova manager Original/` | ⛔ READ-ONLY |
| `Tc originalt materiale/` | ⛔ READ-ONLY |
| `Arkiv/` | ⛔ READ-ONLY |

---

## QUICK REFERENCE

**Start session**:
```
1. Læs SESSION_MEMORY.md
2. Læs PROGRESS.md
3. Læs aktuel task-fil
4. dotnet build && dotnet test
5. Opdater SESSION_MEMORY.md
```

**Per task**:
```
1. Skriv test (RED)
2. Skriv kode (GREEN)
3. Refactor
4. dotnet build && dotnet test
5. git commit
6. Opdater BUILD_STATE.md + PROGRESS.md
```

**Slut session**:
```
1. dotnet test (skal være grøn)
2. Opdater alle memory-filer
3. Commit session-afslutning
```

---

**Denne fil er lov. Følg den.**
