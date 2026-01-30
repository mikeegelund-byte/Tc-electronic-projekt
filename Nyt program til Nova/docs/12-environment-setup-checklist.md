# Environment Setup Checklist — Windows 11

## Prerequisites
- Windows 11 (Build 22621 or newer) ✓
- 8 GB RAM minimum (16 GB recommended)
- 50 GB free disk space (for SDKs + tools)
- Administrator access for installations
- USB port + Nova System pedal (for future manual testing)

---

## Phase 1: Core Tools Installation

### Step 1.1: Install .NET 8 SDK
```powershell
# Verify if already installed
dotnet --version

# If not found, download from: https://dotnet.microsoft.com/download/dotnet/8.0
# Choose: .NET 8.0.x (LTS) → Windows x64 → Installer
# Run installer, follow defaults
# Verify installation
dotnet --version      # Should show 8.0.x
dotnet --list-sdks    # Should show 8.0.x in list
```

**Why**: .NET 8 LTS is required for C# latest + performance + long-term support.

### Step 1.2: Install Visual Studio Community 2022
```
1. Download: https://visualstudio.microsoft.com/downloads/
2. Choose: Community edition
3. Run installer
4. Select workloads:
   ✓ .NET Desktop Development
   ✓ .NET Development  
   ✓ C++ build tools (for optional native interop later)
5. Components:
   ✓ Git for Windows
   ✓ GitHub Copilot (optional, nice to have)
6. Install → ~45 minutes
7. Launch Visual Studio after install
8. Create free Microsoft Account (or sign in)
```

**Why**: Full IDE, free, includes Git, NuGet integration, excellent C# support.

### Step 1.3: Install Git
```powershell
# Usually included with Visual Studio
git --version    # Should show 2.40+

# If not, download: https://git-scm.com/download/win
# Run installer, choose defaults
# Verify
git --version
git config --global user.name "Your Name"
git config --global user.email "your@email.com"
```

**Why**: Version control, GitHub integration, commit history.

### Step 1.4: Verify all tools
```powershell
dotnet --version          # 8.0.x
git --version             # 2.40+
code --version            # (optional, for VS Code)
```

---

## Phase 2: Project Scaffold

### Step 2.1: Create solution directory
```powershell
cd "d:\Tc electronic projekt\Nyt program til Nova"
mkdir -p NovaApp
cd NovaApp
```

### Step 2.2: Create solution file
```powershell
dotnet new sln -n NovaApp
```

### Step 2.3: Create project structure
```powershell
# Domain layer (core business logic)
dotnet new classlib -n NovaApp.Domain -f net8.0

# Application layer (use cases, commands)
dotnet new classlib -n NovaApp.Application -f net8.0

# Infrastructure layer (file I/O, config)
dotnet new classlib -n NovaApp.Infrastructure -f net8.0

# MIDI layer (hardware abstraction)
dotnet new classlib -n NovaApp.Midi -f net8.0

# UI layer (Avalonia XAML)
dotnet new avalonia.mvvm -n NovaApp.UI -f net8.0

# Test project
dotnet new xunit -n NovaApp.Tests -f net8.0
```

### Step 2.4: Add projects to solution
```powershell
dotnet sln add NovaApp.Domain
dotnet sln add NovaApp.Application
dotnet sln add NovaApp.Infrastructure
dotnet sln add NovaApp.Midi
dotnet sln add NovaApp.UI
dotnet sln add NovaApp.Tests
```

### Step 2.5: Set up project dependencies
```powershell
# Navigate to each project and add references

# Application depends on Domain
cd NovaApp.Application
dotnet add reference ../NovaApp.Domain

# MIDI depends on Domain
cd ../NovaApp.Midi
dotnet add reference ../NovaApp.Domain

# Infrastructure depends on Domain + Application
cd ../NovaApp.Infrastructure
dotnet add reference ../NovaApp.Domain ../NovaApp.Application

# UI depends on all layers
cd ../NovaApp.UI
dotnet add reference ../NovaApp.Domain ../NovaApp.Application ../NovaApp.Infrastructure ../NovaApp.Midi

# Tests depend on all layers
cd ../NovaApp.Tests
dotnet add reference ../NovaApp.Domain ../NovaApp.Application ../NovaApp.Infrastructure ../NovaApp.Midi ../NovaApp.UI
```

---

## Phase 3: NuGet Package Installation

### Step 3.1: Add NuGet packages to each project

#### Domain project (minimal dependencies)
```powershell
cd NovaApp.Domain
dotnet add package FluentResults
```

#### Application project
```powershell
cd ../NovaApp.Application
dotnet add package FluentResults
dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions
```

#### MIDI project
```powershell
cd ../NovaApp.Midi
dotnet add package DryWetMidi --version 7.0.0
dotnet add package FluentResults
```

#### Infrastructure project
```powersharp
cd ../NovaApp.Infrastructure
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Json
```

#### UI project (Avalonia + MVVM)
```powershell
cd ../NovaApp.UI
dotnet add package Avalonia
dotnet add package Avalonia.Desktop
dotnet add package Avalonia.Themes.Fluent
dotnet add package CommunityToolkit.Mvvm
dotnet add package DependencyInjection.Factory
```

#### Test project
```powershell
cd ../NovaApp.Tests
dotnet add package xunit --version 2.6.0
dotnet add package xunit.runner.visualstudio
dotnet add package Microsoft.NET.Test.Sdk
dotnet add package Moq --version 4.18.0
dotnet add package FluentAssertions
```

### Step 3.2: Verify package restore
```powershell
cd d:\Tc electronic projekt\Nyt program til Nova\NovaApp
dotnet restore
# Should download all packages to ~/.nuget/packages
```

---

## Phase 4: Git Setup

### Step 4.1: Initialize repository
```powershell
git init
git config user.name "Your Name"
git config user.email "your@email.com"
```

### Step 4.2: Create .gitignore
```powershell
# Copy standard .gitignore for C#
# Content (save as .gitignore):

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
```

### Step 4.3: Create initial commit
```powershell
git add .
git commit -m "Initial commit: empty projects, scaffolded structure"
```

---

## Phase 5: Verify Build

### Step 5.1: Build solution
```powershell
cd d:\Tc electronic projekt\Nyt program til Nova\NovaApp
dotnet build
```

**Expected output:**
```
Build succeeded with 0 warnings
```

### Step 5.2: Run tests (should be empty)
```powershell
dotnet test
```

**Expected output:**
```
Test Run Successful.
Total tests: 0
```

### Step 5.3: Open in Visual Studio
```powershell
# Or double-click NovaApp.sln
start NovaApp.sln
```

**Expected**: Visual Studio opens, shows solution with 6 projects, builds successfully.

---

## Phase 6: Documentation Setup

### Step 6.1: Create README.md
```markdown
# NovaApp — TC Electronic Nova System Control Software

Status: **In Development (Modul 1: Connection + Bank Dump)**

## Quick Start
1. Download bank: `dotnet run` → click "Download"
2. See 60 presets loaded
3. Save to file (later modules)

## Build
```bash
dotnet build
dotnet test
dotnet run --project NovaApp.UI
```

## Architecture
See `docs/` folder for design documentation.

## Testing
```bash
dotnet test --logger "console;verbosity=detailed"
```

## Roadmap
- Modul 1: Connection (3 weeks)
- Modul 2: Parameter Editing (3 weeks)
- ... (10 moduls total)
```

### Step 6.2: Create CONTRIBUTING.md
```markdown
# Contributing to NovaApp

## Code Style
- C# 11 conventions
- Immutable records where possible
- Result<T> pattern for errors
- Async/await everywhere (no sync methods)
- 100% unit test coverage for Domain layer

## Before Committing
```bash
dotnet format
dotnet build
dotnet test
```

## Pull Request Process
1. Create feature branch: `git checkout -b feature/connection-handler`
2. Commit with clear messages
3. Ensure all tests pass
4. Push: `git push origin feature/connection-handler`
5. Open PR with description
```

### Step 6.3: Update docs/00-index.md
Add link to new Modul 1 tech detail file:
```
- [11-modul1-technical-detail.md](11-modul1-technical-detail.md) — Flow, code, tests
```

---

## Phase 7: First Test Run

### Step 7.1: Create dummy test
```powershell
# Create file: NovaApp.Tests/DummyTest.cs
cat > NovaApp.Tests/DummyTest.cs << 'EOF'
namespace NovaApp.Tests;

public class DummyTest
{
    [Fact]
    public void TrueIsTrue()
    {
        Assert.True(true);
    }
}
EOF
```

### Step 7.2: Run test
```powershell
dotnet test
```

**Expected output:**
```
Passed!  - Failed:  0, Passed:  1, Skipped:  0, Total:  1
```

---

## Troubleshooting

### Problem: `dotnet` command not found
**Solution**: Add to PATH:
```powershell
# In PowerShell (Admin):
$env:Path += ";C:\Program Files\dotnet"
[Environment]::SetEnvironmentVariable("Path", $env:Path, [EnvironmentVariableTarget]::User)
```

### Problem: Visual Studio won't load solution
**Solution**: 
```powershell
dotnet build --no-restore
# Delete bin/ obj/ and retry
```

### Problem: NuGet restore fails
**Solution**:
```powershell
dotnet nuget locals all --clear
dotnet restore
```

### Problem: Tests won't run
**Solution**:
```powershell
dotnet test --verbosity diagnostic
# Check output for specific failures
```

---

## Final Verification Checklist

- [ ] .NET 8 SDK installed (`dotnet --version`)
- [ ] Visual Studio Community 2022 installed and working
- [ ] Git configured with user name + email
- [ ] Solution created (`NovaApp.sln` exists)
- [ ] 6 projects created (Domain, App, Infra, MIDI, UI, Tests)
- [ ] All NuGet packages restored
- [ ] Solution builds clean (`dotnet build` succeeds)
- [ ] Dummy test runs (`dotnet test` shows 1 passed)
- [ ] Git repo initialized with initial commit
- [ ] Documentation updated (.gitignore, README, CONTRIBUTING)
- [ ] Visual Studio opens solution without errors
- [ ] Ready for Modul 1 implementation

**Status after Phase 7: ✅ Environment ready for coding**

---

## Next Steps (Modul 1 Implementation)
1. Create IMidiPort interface (MIDI layer)
2. Implement Unit tests for SysEx parsing (Domain)
3. Create mock MidiPort for testing
4. Build ConnectUseCase (Application)
5. Create MainWindow XAML (UI)
6. Run end-to-end test

**Estimated time to completion of Modul 1: 3 weeks**
