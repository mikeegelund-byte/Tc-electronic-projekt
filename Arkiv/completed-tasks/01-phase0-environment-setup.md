# Task List: Phase 1 — Environment Setup

## 📋 Phase: Environment Setup
**Duration**: 1-2 hours
**Prerequisite**: Windows 11, admin access, internet
**Output**: NovaApp.sln with 6 projects, all building green

---

## Task 1.1: Install .NET 8 SDK

**Status**: ✅ COMPLETE
**Estimated**: 15 min
**Actual**: Verified installed (SDK 8.0.417)

### Steps
- [x] Download .NET 8 LTS from https://dotnet.microsoft.com/download/dotnet/8.0
- [x] Choose: .NET 8.0.x → Windows x64 → Installer
- [x] Run installer, follow defaults
- [x] Verify: `dotnet --version` → 8.0.x
- [x] Verify: `dotnet --list-sdks` → 8.0.x in list

### Verification Command
```powershell
dotnet --version
# Expected output: 8.0.x
```

### Notes
- Don't close installation window until complete
- If "dotnet" not recognized, restart PowerShell

---

## Task 1.2: Install Visual Studio Community 2022

**Status**: Not started
**Estimated**: 30 min (including download)

### Steps
- [x] Download from https://visualstudio.microsoft.com/downloads/
- [x] Choose Community edition
- [x] Run installer
- [x] Select workloads:
  - [x] .NET Desktop Development
  - [x] .NET Development
  - [x] C++ build tools (optional)
- [x] Ensure components:
  - [x] Git for Windows
  - [x] GitHub Copilot (optional)
- [x] Complete installation (~45 min)
- [x] Launch Visual Studio
- [x] Create/sign in to Microsoft Account

### Notes
- Installation may take 30-45 minutes
- Requires ~15 GB disk space
- Don't interrupt installation

---

## Task 1.3: Install/Verify Git

**Status**: ✅ COMPLETE
**Estimated**: 5 min
**Actual**: Verified (Git 2.52.0)

### Steps
- [x] Verify: `git --version` → 2.40+
- [x] If not found, download from https://git-scm.com/download/win
- [x] Run Git installer, choose defaults
- [x] Verify again: `git --version`
- [x] Configure: `git config --global user.name "Your Name"`
- [x] Configure: `git config --global user.email "your@email.com"`

### Verification
```powershell
git --version
git config --global user.name
git config --global user.email
```

---

## Task 1.4: Create Project Directory

**Status**: Not started
**Estimated**: 2 min

### Steps
- [x] Navigate: `cd "d:\Tc electronic projekt\Nyt program til Nova"`
- [x] Create: `mkdir NovaApp`
- [x] Enter: `cd NovaApp`
- [x] Verify: `pwd` shows correct path

### Commands
```powershell
cd "d:\Tc electronic projekt\Nyt program til Nova"
mkdir NovaApp
cd NovaApp
```

---

## Task 1.5: Create Solution File

**Status**: Not started
**Estimated**: 1 min

### Steps
- [x] In NovaApp directory, run: `dotnet new sln -n NovaApp`
- [x] Verify: `NovaApp.sln` file exists
- [x] Check: `ls *.sln` shows NovaApp.sln

### Commands
```powershell
cd NovaApp
dotnet new sln -n NovaApp
ls *.sln  # Should show NovaApp.sln
```

---

## Task 1.6: Create 6 Project Templates

**Status**: Not started
**Estimated**: 10 min

### Steps

#### Domain Project (Core business logic)
- [x] `dotnet new classlib -n Nova.Domain -f net8.0`

#### Application Project (Use cases)
- [x] `dotnet new classlib -n Nova.Application -f net8.0`

#### Infrastructure Project (File I/O, config)
- [x] `dotnet new classlib -n Nova.Infrastructure -f net8.0`

#### MIDI Project (Hardware abstraction)
- [x] `dotnet new classlib -n Nova.Midi -f net8.0`

#### UI Project (Avalonia)
- [x] `dotnet new avalonia.mvvm -n Nova.Presentation -f net8.0`

#### Test Project (xUnit)
- [x] `dotnet new xunit -n Nova.Tests -f net8.0`

### Commands
```powershell
dotnet new classlib -n Nova.Domain -f net8.0
dotnet new classlib -n Nova.Application -f net8.0
dotnet new classlib -n Nova.Infrastructure -f net8.0
dotnet new classlib -n Nova.Midi -f net8.0
dotnet new avalonia.mvvm -n Nova.Presentation -f net8.0
dotnet new xunit -n Nova.Tests -f net8.0
```

### Verification
```powershell
ls  # Should show 6 folders
```

---

## Task 1.7: Add Projects to Solution

**Status**: Not started
**Estimated**: 2 min

### Steps
- [x] `dotnet sln add Nova.Domain`
- [x] `dotnet sln add Nova.Application`
- [x] `dotnet sln add Nova.Infrastructure`
- [x] `dotnet sln add Nova.Midi`
- [x] `dotnet sln add Nova.Presentation`
- [x] `dotnet sln add Nova.Tests`

### Verification
```powershell
dotnet sln list  # Should show all 6 projects
```

---

## Task 1.8: Set Up Project Dependencies

**Status**: Not started
**Estimated**: 5 min

### Steps

#### Application → Domain
```powershell
cd Nova.Application
dotnet add reference ../Nova.Domain
```

#### MIDI → Domain
```powershell
cd ../Nova.Midi
dotnet add reference ../Nova.Domain
```

#### Infrastructure → Domain + Application
```powershell
cd ../Nova.Infrastructure
dotnet add reference ../Nova.Domain ../Nova.Application
```

#### UI → All layers
```powershell
cd ../Nova.Presentation
dotnet add reference ../Nova.Domain ../Nova.Application ../Nova.Infrastructure ../Nova.Midi
```

#### Tests → All layers
```powershell
cd ../Nova.Tests
dotnet add reference ../Nova.Domain ../Nova.Application ../Nova.Infrastructure ../Nova.Midi ../Nova.Presentation
```

---

## Task 1.9: Install NuGet Packages

**Status**: Not started
**Estimated**: 10 min

### Domain Project
```powershell
cd Nova.Domain
dotnet add package FluentResults
```

### Application Project
```powershell
cd ../Nova.Application
dotnet add package FluentResults
dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions
```

### MIDI Project
```powershell
cd ../Nova.Midi
dotnet add package DryWetMidi --version 7.0.0
dotnet add package FluentResults
```

### Infrastructure Project
```powershell
cd ../Nova.Infrastructure
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Json
```

### UI Project
```powershell
cd ../Nova.Presentation
dotnet add package Avalonia
dotnet add package Avalonia.Desktop
dotnet add package Avalonia.Themes.Fluent
dotnet add package CommunityToolkit.Mvvm
dotnet add package DependencyInjection.Factory
```

### Test Project
```powershell
cd ../Nova.Tests
dotnet add package xunit --version 2.6.0
dotnet add package xunit.runner.visualstudio
dotnet add package Microsoft.NET.Test.Sdk
dotnet add package Moq --version 4.18.0
dotnet add package FluentAssertions
```

### Restore Packages
```powershell
cd ..  # Back to NovaApp root
dotnet restore
```

---

## Task 1.10: Create .gitignore

**Status**: Not started
**Estimated**: 2 min

### Steps
- [x] In NovaApp root, create `.gitignore`
- [x] Add standard C# ignores (see below)
- [x] Verify git sees it: `git status`

### Content for .gitignore
```
bin/
obj/
.vs/
.vscode/
*.user
*.suo
*.sln.DotSettings
packages/
.DS_Store
*.log
*.swp
*.syx.backup
*.bak
~*.*
```

---

## Task 1.11: Initialize Git Repository

**Status**: Not started
**Estimated**: 3 min

### Steps
- [x] `git init`
- [x] `git config user.name "Your Name"`
- [x] `git config user.email "your@email.com"`
- [x] `git add .`
- [x] `git commit -m "Initial commit: empty projects, scaffolded structure"`
- [x] Verify: `git log --oneline` shows 1 commit

### Commands
```powershell
git init
git config user.name "Your Name"
git config user.email "your@email.com"
git add .
git commit -m "Initial commit: empty projects, scaffolded structure"
git log --oneline -5
```

---

## Task 1.12: Verify Build

**Status**: Not started
**Estimated**: 5 min

### Steps
- [x] `dotnet build` → Must succeed
- [x] Verify: "Build succeeded with 0 warnings"
- [x] `dotnet test` → Must show "Test Run Successful"
- [x] Verify: "Total tests: 0" (no tests yet, correct)

### Commands
```powershell
cd NovaApp  # Root of solution
dotnet clean
dotnet build
dotnet test
```

### Expected Output
```
Build succeeded with 0 warnings
...
Test Run Successful.
Total tests: 0
```

---

## Task 1.13: Open in Visual Studio

**Status**: Not started
**Estimated**: 2 min

### Steps
- [x] Double-click `NovaApp.sln` or run: `start NovaApp.sln`
- [x] Visual Studio opens
- [x] All 6 projects visible in Solution Explorer
- [x] Build solution (Ctrl+Shift+B)
- [x] Verify build succeeds

### Troubleshooting
- If projects don't load: Close/reopen solution
- If build fails: Run `dotnet restore` first

---

## Task 1.14: Create README.md

**Status**: Not started
**Estimated**: 3 min

### Steps
- [ ] Create `README.md` in NovaApp root
- [ ] Add basic project info (see template below)
- [ ] Save file

### Template
```markdown
# NovaApp — TC Electronic Nova System Control

Status: **In Development (Modul 0: Environment setup)**

## Quick Start

```bash
dotnet build
dotnet test
dotnet run --project Nova.Presentation
```

## Project Structure

- `Nova.Domain/` — Business logic
- `Nova.Application/` — Use cases
- `Nova.Infrastructure/` — File I/O
- `Nova.Midi/` — MIDI I/O abstraction
- `Nova.Presentation/` — Avalonia UI
- `Nova.Tests/` — Unit + integration tests

## Documentation

See `../docs/` for complete documentation.

## Tech Stack

- C# 11 (.NET 8 LTS)
- Avalonia 11.x (UI)
- DryWetMIDI 7.x (MIDI I/O)
- xUnit 2.6 (Testing)
```

---

## Task 1.15: Create CONTRIBUTING.md

**Status**: Not started
**Estimated**: 2 min

### Steps
- [ ] Create `CONTRIBUTING.md` in NovaApp root
- [ ] Add development guidelines (see template)
- [ ] Save file

### Template
```markdown
# Contributing

## Code Style

- C# 11 conventions
- Immutable records where possible
- Result<T> pattern for errors
- Async/await everywhere

## Before Committing

```bash
dotnet format
dotnet build
dotnet test
```

## Test-Driven Development

1. Write failing test (RED)
2. Implement minimal code (GREEN)
3. Refactor without changing behavior (REFACTOR)
4. Commit with [RED→GREEN→REFACTOR] message

See `../llm-build-system/LLM_BUILD_INSTRUCTIONS.md` for full rules.
```

---

## Task 1.16: Create Dummy Test

**Status**: Not started
**Estimated**: 2 min

### Steps
- [ ] Create file: `Nova.Tests/DummyTest.cs`
- [ ] Add test (see below)
- [ ] Run: `dotnet test`
- [ ] Verify: 1 test passes

### Code
```csharp
namespace Nova.Tests;

public class DummyTest
{
    [Fact]
    public void TrueIsTrue()
    {
        Assert.True(true);
    }
}
```

### Verification
```powershell
dotnet test
# Output: "Passed!  - Failed:  0, Passed:  1, Skipped:  0"
```

---

## Task 1.17: Final Verification

**Status**: Not started
**Estimated**: 5 min

### Checklist
- [ ] `dotnet --version` → 8.0.x
- [ ] `git --version` → 2.40+
- [ ] `dotnet build` → succeeded, 0 warnings
- [ ] `dotnet test` → 1 passed, 0 failed
- [ ] `NovaApp.sln` exists with 6 projects
- [ ] `.gitignore` in root
- [ ] Initial commit made (`git log --oneline` shows 1)
- [ ] README.md exists
- [ ] CONTRIBUTING.md exists
- [ ] Visual Studio opens solution smoothly

### Commands
```powershell
dotnet --version
git --version
git log --oneline -1
ls *.md  # Should show README.md, CONTRIBUTING.md
dotnet build
dotnet test
```

---

## ✅ Phase 1 Complete When

- [x] All 17 tasks finished
- [x] Build green (0 warnings)
- [x] Tests green (1 passing)
- [x] Git initialized with 1 commit
- [x] Solution opens in Visual Studio
- [x] Ready for Modul 1

---

## 🔗 Next Phase

Once complete: `tasks/02-modul1-phase1-foundation.md`

---

## Notes

- Keep this file open while working through setup
- Check off each task as you complete it
- If you get stuck, see Troubleshooting in `docs/12-environment-setup-checklist.md`
