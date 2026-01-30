# Task List: Phase 1 — Environment Setup

## 📋 Phase: Environment Setup
**Duration**: 1-2 hours  
**Prerequisite**: Windows 11, admin access, internet  
**Output**: NovaApp.sln with 6 projects, all building green  

---

## Task 1.1: Install .NET 8 SDK

**Status**: Not started  
**Estimated**: 15 min

### Steps
- [ ] Download .NET 8 LTS from https://dotnet.microsoft.com/download/dotnet/8.0
- [ ] Choose: .NET 8.0.x → Windows x64 → Installer
- [ ] Run installer, follow defaults
- [ ] Verify: `dotnet --version` → 8.0.x
- [ ] Verify: `dotnet --list-sdks` → 8.0.x in list

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
- [ ] Download from https://visualstudio.microsoft.com/downloads/
- [ ] Choose Community edition
- [ ] Run installer
- [ ] Select workloads:
  - [ ] .NET Desktop Development
  - [ ] .NET Development
  - [ ] C++ build tools (optional)
- [ ] Ensure components:
  - [ ] Git for Windows
  - [ ] GitHub Copilot (optional)
- [ ] Complete installation (~45 min)
- [ ] Launch Visual Studio
- [ ] Create/sign in to Microsoft Account

### Notes
- Installation may take 30-45 minutes
- Requires ~15 GB disk space
- Don't interrupt installation

---

## Task 1.3: Install/Verify Git

**Status**: Not started  
**Estimated**: 5 min

### Steps
- [ ] Verify: `git --version` → 2.40+
- [ ] If not found, download from https://git-scm.com/download/win
- [ ] Run Git installer, choose defaults
- [ ] Verify again: `git --version`
- [ ] Configure: `git config --global user.name "Your Name"`
- [ ] Configure: `git config --global user.email "your@email.com"`

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
- [ ] Navigate: `cd "d:\Tc electronic projekt\Nyt program til Nova"`
- [ ] Create: `mkdir NovaApp`
- [ ] Enter: `cd NovaApp`
- [ ] Verify: `pwd` shows correct path

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
- [ ] In NovaApp directory, run: `dotnet new sln -n NovaApp`
- [ ] Verify: `NovaApp.sln` file exists
- [ ] Check: `ls *.sln` shows NovaApp.sln

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
- [ ] `dotnet new classlib -n Nova.Domain -f net8.0`

#### Application Project (Use cases)
- [ ] `dotnet new classlib -n Nova.Application -f net8.0`

#### Infrastructure Project (File I/O, config)
- [ ] `dotnet new classlib -n Nova.Infrastructure -f net8.0`

#### MIDI Project (Hardware abstraction)
- [ ] `dotnet new classlib -n Nova.Midi -f net8.0`

#### UI Project (Avalonia)
- [ ] `dotnet new avalonia.mvvm -n Nova.Presentation -f net8.0`

#### Test Project (xUnit)
- [ ] `dotnet new xunit -n Nova.Tests -f net8.0`

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
- [ ] `dotnet sln add Nova.Domain`
- [ ] `dotnet sln add Nova.Application`
- [ ] `dotnet sln add Nova.Infrastructure`
- [ ] `dotnet sln add Nova.Midi`
- [ ] `dotnet sln add Nova.Presentation`
- [ ] `dotnet sln add Nova.Tests`

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
- [ ] In NovaApp root, create `.gitignore`
- [ ] Add standard C# ignores (see below)
- [ ] Verify git sees it: `git status`

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
- [ ] `git init`
- [ ] `git config user.name "Your Name"`
- [ ] `git config user.email "your@email.com"`
- [ ] `git add .`
- [ ] `git commit -m "Initial commit: empty projects, scaffolded structure"`
- [ ] Verify: `git log --oneline` shows 1 commit

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
- [ ] `dotnet build` → Must succeed
- [ ] Verify: "Build succeeded with 0 warnings"
- [ ] `dotnet test` → Must show "Test Run Successful"
- [ ] Verify: "Total tests: 0" (no tests yet, correct)

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
- [ ] Double-click `NovaApp.sln` or run: `start NovaApp.sln`
- [ ] Visual Studio opens
- [ ] All 6 projects visible in Solution Explorer
- [ ] Build solution (Ctrl+Shift+B)
- [ ] Verify build succeeds

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
