# AGENT DEPLOYMENT PLAN — Scenario A Test
**Dato**: 2026-02-02  
**Formål**: Test af parallel agent-eksekvering (læring + produktion)  
**Status**: 🟡 PLANNING PHASE

---

## 📊 CURRENT STATE (før deployment)

### Git Status
```
Branch: copilot/implement-update-preset-use-case
Remote: https://github.com/mikeegelund-byte/Tc-electronic-projekt.git
Unpushed commits: 6 commits (f895626...b8143d7)
Uncommitted changes: 3 files (PROGRESS.md, BUILD_STATE.md, 10-modul5-preset-detail.md)
Last synced commit: 19918f5 (Session 3 complete)
```

### Project Status
```
Modul 4: ✅ 100% COMPLETE (5/5 tasks)
Modul 5: 🔄 70% COMPLETE (2/5 tasks)
  ✅ Task 5.1: 7 Effect ViewModels (Drive with 4 tests)
  ✅ Task 5.2: PresetDetailViewModel composition
  ⏳ Task 5.3: EffectBlockView reusable control
  ⏳ Task 5.4: PresetDetailView.axaml update (BLOCKING BUILD)
  ⏳ Task 5.5: Verify selection wiring
Modul 6: ⏳ 0% (not started - 4 weeks estimated)

Tests: 248/248 passing
Build: ⚠️ BROKEN (33 Avalonia AVLN2000 errors in PresetDetailView.axaml)
```

---

## 🎯 DEPLOYMENT PLAN — 3 FASER

### FASE 1: FORBEREDELSE (5 min) ✅

#### 1.1 Commit Current Work
```powershell
cd "c:\Users\mike_\Desktop\Tc electronic projekt\Nyt program til Nova"
git add .
git commit -m "[DOC] Update documentation: Modul 5 status (70% - ViewModels complete)"
```

**Resultat**: Clean working directory, klar til push

#### 1.2 Push to GitHub
```powershell
git push origin copilot/implement-update-preset-use-case
```

**Resultat**: Remote branch opdateret med alle 7 commits (inkl. Modul 4-5 arbejde)

#### 1.3 Verify Remote State
```powershell
git log origin/copilot/implement-update-preset-use-case --oneline -10
```

**Forventet**: f895626 (eller nyere) som HEAD

---

### FASE 2: AGENT DEPLOYMENT (3-5 min synkront + 30-60 min asynkront)

#### 2.1 Plan Agent: Modul 6 Research (SYNKRON - blokerer 3-5 min)

**Agent**: `Plan` (Researches and outlines multi-step plans)

**Brief/Prompt**:
```
Analysér tasks/11-modul6-preset-editor-SONNET45.md grundigt og lav en detaljeret 
research-rapport der dækker:

1. SCOPE ANALYSE:
   - Total antal tasks (opdel efter complexity: HIGH/MEDIUM/LOW)
   - Estimeret total implementeringstid
   - Kritiske dependencies mellem tasks
   - Hvilke tasks kan paralleliseres?

2. TEKNISK RESEARCH:
   - Hvilke nye ViewModels skal oprettes?
   - Hvilke XAML controls skal udvides?
   - Hvilke use cases mangler vi i Application layer?
   - Parameter validation strategi (min/max ranges)
   - Live CC update arkitektur (MIDI while typing)

3. RISIKOANALYSE:
   - Hvilke tasks er markeret "SONNET 4.5+" HIGH COMPLEXITY?
   - Hvor er størst risiko for bugs?
   - Test coverage strategi (antal tests forventet?)

4. IMPLEMENTATION ROADMAP:
   - Foreslå task-rækkefølge der minimerer blocking
   - Identificer "quick wins" (Tasks 6.1.1-6.1.4)
   - Identificer "danger zones" (Task 6.1.5+ Pitch/EQ/Gate)

Output format:
- Executive Summary (3-5 bullet points)
- Detaljeret analyse med konkrete tal
- Konkret anbefaling til implementation rækkefølge
- Estimated total tid til Modul 6 (realistisk)
```

**Forventet output**: Tekst-rapport (500-1000 ord) med konkret roadmap

**Varighed**: 3-5 minutter (BLOKERER min session)

**Success criteria**:
- ✅ Rapport indeholder konkrete task counts
- ✅ Dependencies er identificeret
- ✅ Implementation order er foreslået
- ✅ Realistic time estimate (sammenlign med "4 weeks" fra task-fil)

---

#### 2.2 GitHub Coding Agent: Modul 5 Tasks 5.3-5.5 (ASYNKRON - 30-60 min)

**Agent**: `github-pull-request_copilot-coding-agent`

**Brief/Prompt**:
```
Complete Tasks 5.3-5.5 from tasks/10-modul5-preset-detail.md to finish Modul 5 (Preset Detail Viewer).

CRITICAL: Follow llm-build-system/AGENTS.md pipeline strictly:
- Read AGENTS.md, BUILD_STATE.md, PROGRESS.md first
- Follow TDD: Write test FIRST (RED), then code (GREEN), then refactor
- Run `dotnet test --verbosity diagnostic` after each change
- Commit after each task with format: [MODUL-5][TASK-X.X] Description

TASKS TO COMPLETE:

Task 5.3: Create EffectBlockView reusable UserControl
- File: src/Nova.Presentation/Views/Controls/EffectBlockView.axaml
- Avalonia UserControl with:
  - Expander with collapsible header
  - On/off indicator (green dot if IsEnabled=true, gray if false)
  - Effect type label (e.g., "Overdrive", "Chorus")
  - Grid showing parameter name:value pairs
- Reusable across all 7 effects (Drive, Compressor, EqGate, Modulation, Pitch, Delay, Reverb)
- Test: Manually verify in PresetDetailView

Task 5.4: Update PresetDetailView.axaml to use new nested ViewModels
- File: src/Nova.Presentation/Views/PresetDetailView.axaml
- CRITICAL FIX: Currently has 33 Avalonia AVLN2000 errors
- Replace flat property bindings with nested structure:
  - OLD: {Binding DriveType} → NEW: {Binding Drive.Type}
  - OLD: {Binding DriveGain} → NEW: {Binding Drive.Gain}
  - OLD: {Binding CompType} → NEW: {Binding Compressor.Type}
  - Apply pattern to all 7 effect blocks
- Use EffectBlockView from Task 5.3 for each effect
- Layout: ScrollViewer with StackPanel containing 7 EffectBlockView instances
- Test: `dotnet build` should succeed with 0 errors

Task 5.5: Verify preset selection wiring
- Code: MainViewModel.OnPresetSelectionChanged() already calls PresetDetail.LoadFromPreset()
- Test: Manual verification that clicking preset in list populates detail view
- Document: Add note in BUILD_STATE.md that wiring is verified

DELIVERABLES:
- 2 new files (EffectBlockView.axaml + .cs)
- 1 updated file (PresetDetailView.axaml)
- 1 documentation update (BUILD_STATE.md)
- All tests passing (248 tests minimum)
- Build with 0 errors (fix all 33 Avalonia errors)
- 3 commits (one per task)

SUCCESS CRITERIA:
- dotnet build → 0 errors
- dotnet test → 248/248 passing (or more if tests added)
- Manual test: Clicking preset shows all 7 effect blocks with parameters
- Modul 5 marked as 100% COMPLETE in PROGRESS.md
```

**Forventet output**: Pull Request med 3 commits + ~300-500 lines changed

**Varighed**: 30-60 minutter (ASYNKRON - kører i GitHub cloud)

**Success criteria**:
- ✅ PR created with title "[MODUL-5] Complete Tasks 5.3-5.5 (Preset Detail XAML)"
- ✅ 3 separate commits (one per task)
- ✅ Build passes on GitHub Actions (hvis configureret)
- ✅ 0 Avalonia errors
- ✅ PROGRESS.md updated to Modul 5 = 100%

---

### FASE 3: PARALLEL EXECUTION (30-60 min)

**Mens GitHub Coding Agent kører i baggrunden:**

#### 3.1 Review Plan Agent Output (5 min)
- Læs research-rapport grundigt
- Diskutér med bruger (Mike): Enig i roadmap?
- Juster Modul 6 strategi baseret på findings

#### 3.2 Start Modul 6 Planning (25-55 min)
**Based on Plan Agent's research:**

1. **Document Modul 6 Architecture** (10 min)
   - Hvilke nye ViewModels? (7 editable effect VMs)
   - Hvilke use cases? (UpdatePresetUseCase, VerifyPresetRoundtripUseCase)
   - MIDI live update strategi

2. **Create Task Breakdown** (10 min)
   - Opdel 11-modul6-preset-editor-SONNET45.md i bite-sized tasks
   - Prioritér efter Plan Agent's anbefalinger

3. **Setup Test Infrastructure** (15 min)
   - Create test fixtures for editable ViewModels
   - Placeholder tests for Tasks 6.1.1-6.1.4

4. **Start Task 6.1.1: Editable Drive Controls** (20 min hvis tid)
   - Make Drive editable in PresetDetailView
   - Wire to eventual UpdatePresetUseCase (stub for now)

---

## 🔄 ROLLBACK STRATEGY (hvis noget går galt)

### Scenario A: GitHub Coding Agent fejler
```powershell
# Intet cleanup nødvendigt - agent arbejder på separat branch
# Bare ignorer PR'en eller luk den
# Vi fortsætter på vores lokale branch
```

### Scenario B: Vi laver fejl lokalt mens agent kører
```powershell
# Stash vores arbejde
git stash

# Merge GitHub Agent's PR først
git fetch origin
git merge origin/copilot/implement-update-preset-use-case-agent-branch

# Apply vores arbejde ovenpå
git stash pop

# Løs eventuelle conflicts
```

### Scenario C: Alt går til helvede
```powershell
# Worst case: Revert til før deployment
git reset --hard f895626  # (current HEAD før Fase 1)
git push --force origin copilot/implement-update-preset-use-case

# Vi har mistet intet - alle commits er i git history
git reflog  # Find lost commits hvis nødvendigt
```

---

## ✅ SUCCESS METRICS

### Tekniske metrics:
- [ ] Remote branch opdateret (7 commits pushed)
- [ ] Plan Agent rapport modtaget (500+ ord)
- [ ] GitHub Coding Agent PR created
- [ ] Modul 5 = 100% COMPLETE (efter PR merge)
- [ ] Modul 6 planning documented
- [ ] Alle tests passing (248+)
- [ ] Build grøn (0 errors)

### Lærings metrics:
- [ ] Bruger forstår forskel mellem synkron/asynkron agents
- [ ] Bruger har set Plan Agent output format
- [ ] Bruger har set GitHub Agent PR workflow
- [ ] Bruger kan fortsætte autonomous work efter test

---

## 📝 EXECUTION LOG (udfyldes under deployment)

### Fase 1 Start: 2026-02-02 [✅ COMPLETE]
```
STATUS: ✅ User approval received - Starting Fase 1 preparation
ACTION: Committing 3 uncommitted files + pushing to GitHub
RESULT: 
  - Commit 04c5e0d created (4 files, 343 insertions)
  - Pushed to origin/copilot/implement-update-preset-use-case
  - Remote branch synced with all Modul 4-5 work
  - Ready for agent deployment
```

### Fase 2.1 Plan Agent Start: 2026-02-02 [✅ COMPLETE]
```
STATUS: ✅ Plan Agent research complete
AGENT: Plan (Researches and outlines multi-step plans)
DURATION: ~3 minutes (SYNCHRONOUS)
RESULT:
  - 15 tasks analyzed (7 HIGH, 6 MEDIUM, 2 LOW)
  - Real estimate: 28-32 hours (vs task file's 20h)
  - CRITICAL BLOCKER identified: Preset.ToSysEx() needs full rewrite
  - 4-phase roadmap created with risk mitigation
  - Confidence: 7/10
  - Full report delivered to user
```

### Fase 2.2 GitHub Agent Deploy: 2026-02-02 [EXECUTING]
```
STATUS: 🚀 Deploying GitHub Coding Agent for Tasks 5.3-5.5
AGENT: github-pull-request_copilot-coding-agent
DURATION: Expected 30-60 minutes (ASYNCHRONOUS - runs in cloud)
TARGET: Complete Modul 5 (Preset Detail Viewer XAML)
```

### Fase 2.1 Plan Agent Start: [TIMESTAMP]
```
[Agent output kommer her]
```

### Fase 2.2 GitHub Agent Deploy: [TIMESTAMP]
```
[PR URL kommer her]
```

### Fase 3 Parallel Work: [TIMESTAMP]
```
[Progress notes kommer her]
```

### Completion: [TIMESTAMP]
```
[Final status kommer her]
```

---

## 🚀 READY TO EXECUTE?

**Prerequisites checklist:**
- [x] All documentation reviewed
- [x] Rollback strategy understood
- [x] Success criteria defined
- [x] User approval received 

**Command when ready:**
```
"EXECUTE AGENT_DEPLOYMENT_PLAN.md - Scenario A"
```
