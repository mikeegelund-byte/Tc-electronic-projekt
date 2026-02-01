# LLM Build Instructions — Uundgåeligt Testregime

## ⚠️ LÆS FØRST: `llm-build-system/AGENTS.md`

AGENTS.md indeholder den komplette pipeline. Denne fil er supplement.

---

Status: læst

## ⚠️ KRITISK REGEL: **NO CODE WITHOUT TESTS**

Du SKAL følge denne regel 100% - ingen undtagelser:

```
┌─────────────────────────────────────────────────────────┐
│ ORDEN FOR ENHVER KODEÆNDRING:                           │
│                                                         │
│ 1. LÆS test-filen der skal ændres                      │
│ 2. SKAB faldet med rødt (test skal fejle)               │
│ 3. SKRIVE minimal kode (test skal gå grønt)             │
│ 4. REFACTOR uden at ændre test-adfærd                   │
│ 5. COMMIT med besked: "[RED→GREEN→REFACTOR]"            │
│ 6. KOLD BUILD + TEST (verifikation)                     │
│                                                         │
│ HVIS DU SPRINGER TRIN OVER: FEJLER                      │
└─────────────────────────────────────────────────────────┘
```

---

## 📋 Proceskrav Per Commit

### Før enhver commit

```powershell
# 1. Verificer tests fejler (hvis nyt feature)
dotnet test --filter "NameOf::YourNewTest"

# 2. Verificer tests går grønt
dotnet test

# 3. Verificer build uden fejl
dotnet build

# 4. Verificer ingen compiler warnings
# (output skal være: "Build succeeded with 0 warnings")

# 5. Kør code formatter
dotnet format

# 6. Verificer igen
dotnet build
dotnet test
```

### Commit besked format

```
[MODUL-X] [RED→GREEN→REFACTOR] Brief description

- [RED]: Test file created: XyzTests.cs
- [GREEN]: Minimal implementation in Xyz.cs (line X-Y)
- [REFACTOR]: Extracted method ExtractedName() (optional)
- Test coverage: +5 new tests (now at 87%)
- Manual verification: [LIST ANY MANUAL CHECKS]
```

---

## 🧠 LLM Memory System

### Når du starter en session

1. **Læs altid først:**
   ```
   llm-build-system/memory/SESSION_MEMORY.md
   ```

2. **Opdater SESSION_MEMORY.md med:**
   - Hvilken modul/fase du arbejder på
   - Hvad du skal lave (from tasks/XX-*.md)
   - Hvad du IKKE skal gøre
   - Test-strategi for denne session

3. **Tjek BUILD_STATE.md:**
   - Hvilke commits er allerede lavet
   - Hvad mangler endnu
   - Hvad fejler lige nu

### Efter hver session

**Opdater ALTID:**
```
llm-build-system/memory/
├── SESSION_MEMORY.md ........... Hvad jeg lavede
├── BUILD_STATE.md ............. Kommits + test status
└── PITFALLS_FOUND.md .......... Fejl jeg gjorde
```

---

## ✅ Test Gate System (UUNDGÅELIGT)

Du kan **IKKE** pushes til næste modul uden:

```
┌─────────────────────────────────────────────────────────┐
│ TEST GATES (ALLE SKAL VÆRE GRØNT)                       │
├─────────────────────────────────────────────────────────┤
│ Gate 1: Alle unit tests passar                    ✅   │
│ Gate 2: Coverage ≥ 85% (eller specificeret goal)  ✅   │
│ Gate 3: Build uden warnings                       ✅   │
│ Gate 4: No compiler errors                        ✅   │
│ Gate 5: Roundtrip test (serialize/parse/compare)  ✅   │
│ Gate 6: Manual test på real hardware (hvis MIDI)  ✅   │
│ Gate 7: Code review passed (hvis team)            ✅   │
│ Gate 8: Deployment test (dotnet run succeeds)     ✅   │
└─────────────────────────────────────────────────────────┘
```

**Hvis NOGEN gate er rød: DU STOPPER OG FIXER**

---

## 🔴 The Red-Green-Refactor Cycle (TVUNGET)

### RED Phase (3-5 min)

```csharp
// Skriv test der fejler:
[Fact]
public void ParsePreset_ValidSysEx_ReturnsCorrectName()
{
    var sysex = File.ReadAllBytes("Fixtures/preset-001.bin");
    var result = Preset.FromSysEx(sysex);
    
    Assert.True(result.IsSuccess);
    Assert.Equal("My Preset", result.Value.Name);
}
```

**Kør:**
```powershell
dotnet test --filter "ParsePreset_ValidSysEx"
# Output: FAILED (expected - method doesn't exist yet)
```

### GREEN Phase (10-20 min)

```csharp
// Minimal implementation - BARE NOK TIL AT TESTEN GÅR GRØNT
public static Result<Preset> FromSysEx(byte[] data)
{
    if (data == null || data.Length != 520)
        return Result<Preset>.Failure("Invalid length");
    
    var name = Encoding.ASCII.GetString(data, 9, 24).Trim('\0');
    
    return Result<Preset>.Success(new Preset 
    { 
        Name = name,
        RawSysEx = data,
        Checksum = data[517]
    });
}
```

**Kør:**
```powershell
dotnet test --filter "ParsePreset_ValidSysEx"
# Output: PASSED ✅
```

### REFACTOR Phase (5-10 min)

```csharp
// Forbedring uden at ændre test-adfærd
private static string ExtractPresetName(byte[] data)
{
    return Encoding.ASCII.GetString(data, 9, 24).Trim('\0');
}

public static Result<Preset> FromSysEx(byte[] data)
{
    if (!ValidateSysExStructure(data))
        return Result<Preset>.Failure("Invalid SysEx structure");
    
    var name = ExtractPresetName(data);
    
    return Result<Preset>.Success(new Preset 
    { 
        Name = name,
        RawSysEx = data,
        Checksum = data[517]
    });
}

private static bool ValidateSysExStructure(byte[] data)
    => data?.Length == 520 && data[0] == 0xF0 && data[519] == 0xF7;
```

**Kør:**
```powershell
dotnet test
# Output: PASSED ✅ (all tests)
dotnet build
# Output: succeeded with 0 warnings
```

---

## 🛑 ABSOLUTE NO-NOs

Du SKAL **IKKE** gøre følgende:

### ❌ NO-NO 1: Skipppe tests
```csharp
[Fact(Skip = "Will fix later")]  // ❌ FORBIDDEN
public void SomeTest() { }
```

### ❌ NO-NO 2: Commit uden grønt
```powershell
git commit -m "WIP: will test later"  # ❌ FORBIDDEN
```

### ❌ NO-NO 3: Skrive for meget kode ad gangen
```csharp
// 500 linjer kode uden test før-hand  # ❌ FORBIDDEN
public class HugeImplementation { ... }
```

### ❌ NO-NO 4: Ændre eksisterende tests
```csharp
[Fact]
public void ExistingTest()
{
    // Hvis du ændrer dette uden at have dårlig grund: ❌ FORBIDDEN
}
```

### ❌ NO-NO 5: "Just delete" uden dokumentation
```csharp
// Hvis du sletter en fil: DU SKAL dokumentere hvorfor
// (Se CLEANUP_POLICY.md)
```

---

## 📊 Coverage Requirements (Uundgåeligt)

**Domain Layer**: ≥ 95% coverage
```powershell
# After every change:
dotnet test /p:CollectCoverage=true
# Check: Domain coverage must be ≥ 95%
```

**Application Layer**: ≥ 80% coverage
**Infrastructure Layer**: ≥ 70% coverage
**UI Layer**: ≥ 50% coverage (will improve with Avalonia TestHost)

---

## 🔍 Code Review Checklist (TVUNGET)

Før du siger "færdig":

- [ ] Alle tests passar
- [ ] Coverage-mål nået (se docs/04-testing-strategy.md)
- [ ] Ingen compiler warnings
- [ ] Build succede uden fejl
- [ ] Commit-besked følger format
- [ ] Roundtrip test passes (if applicable)
- [ ] No hardcoded values (except in tests)
- [ ] No magic numbers (explain all numbers)
- [ ] Comments for WHY, not WHAT
- [ ] Error handling documented
- [ ] No Console.WriteLine() (use Serilog)
- [ ] Async/await used correctly
- [ ] No sync-over-async (no .Result)

---

## 🚨 If Tests Fail

**Når du kører `dotnet test` og det fejler:**

1. **STOP OG LÆS fejlmeldingen fuld**
   ```powershell
   dotnet test --verbosity diagnostic
   ```

2. **Skriv ned:** Hvad fejler? Hvor? Hvorfor?

3. **Check BUILD_STATE.md:**
   - Hvad var sidste kommit?
   - Hvad ændrede du siden?

4. **Revert hvis nødvendigt:**
   ```powershell
   git log --oneline (see last 5 commits)
   git reset --hard <commit-hash>  # Only if truly stuck
   ```

5. **UPDATE PITFALLS_FOUND.md:**
   ```
   ## [DATO] Test failed: XYZ
   - Root cause: [explanation]
   - How to fix: [steps]
   - Prevention: [future check]
   ```

---

## 📝 Mandatory Logging During Development

Alle vigtige operationer skal logges (Serilog):

```csharp
using Serilog;

public class PresetLoader
{
    private readonly ILogger _logger = Log.ForContext<PresetLoader>();
    
    public async Task<Result<Preset>> LoadAsync(string path)
    {
        _logger.Information("Loading preset from {Path}", path);
        
        try
        {
            var data = await File.ReadAllBytesAsync(path);
            _logger.Debug("Read {Bytes} bytes", data.Length);
            
            var result = Preset.FromSysEx(data);
            
            if (result.IsSuccess)
                _logger.Information("Preset loaded: {Name}", result.Value.Name);
            else
                _logger.Warning("Preset parse failed: {Error}", result.Error);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to load preset");
            return Result<Preset>.Failure(ex.Message);
        }
    }
}
```

**INGEN Console.WriteLine() - ALTID Serilog!**

---

## 🔐 Build Lock-In

**Inden du afslutter session:**

```powershell
# Cold build (delete bin/obj first)
Remove-Item bin, obj -Recurse -Force
dotnet clean
dotnet build

# All tests must pass
dotnet test

# No warnings allowed
# (Check output: "Build succeeded with 0 warnings")
```

**Hvis noget fejler her: FIX FØRST, COMMIT EFTER**

---

## 💾 Commit Before Each Session

```powershell
# Status
git status

# See what changed
git diff

# If all looks good:
git add .
git commit -m "[MODUL-X] Session complete: [list what done]"

# Verify
git log --oneline -5
```

---

## 🎯 Session Template

```
# START OF SESSION

1. Read: llm-build-system/memory/SESSION_MEMORY.md
2. Read: tasks/XX-phase-modul.md (current phase)
3. Read: docs/04-testing-strategy.md (refresh test rules)
4. Run: dotnet build && dotnet test (verify green start)
5. Update: SESSION_MEMORY.md with [TODAY'S GOALS]

# DURING SESSION

For each TODO:
  - Create test (RED)
  - Implement minimal code (GREEN)
  - Refactor (CLEAN)
  - Commit with [RED→GREEN→REFACTOR] message
  - Verify: dotnet build && dotnet test (must be GRØNT)

# END OF SESSION

1. Run: dotnet test (MUST BE GRØNT)
2. Run: dotnet build (MUST BE GRØNT, 0 warnings)
3. Update: BUILD_STATE.md (what was done, what's next)
4. Update: PITFALLS_FOUND.md (any issues found)
5. Git commit: "[MODUL-X] Session complete: [summary]"
6. Save: SESSION_MEMORY.md for next session
```

---

## 🔗 Related Files

- [SESSION_MEMORY.md](memory/SESSION_MEMORY.md) — Current session state
- [BUILD_STATE.md](memory/BUILD_STATE.md) — What's been built
- [PITFALLS_FOUND.md](memory/PITFALLS_FOUND.md) — Mistakes to avoid
- [CLEANUP_POLICY.md](CLEANUP_POLICY.md) — When to delete/refactor
- [../tasks/](../tasks/) — Individual phase/modul todo lists

---

## ⚡ TL;DR

**RULE 1:** TEST FIRST, CODE SECOND  
**RULE 2:** RED → GREEN → REFACTOR  
**RULE 3:** NO SKIPPED TESTS  
**RULE 4:** NO COMMITS WITHOUT GRØNT  
**RULE 5:** COVERAGE GOALS MUST BE MET  
**RULE 6:** EVERY TEST MUST PASS BEFORE NEXT PHASE  

**Status:** Ready to enforce discipline ✅
