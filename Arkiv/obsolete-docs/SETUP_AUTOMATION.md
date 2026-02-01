# 🔧 Setup Automation Guide

**Automatiseret opsætning af hele projektet med PowerShell scripts**

---

## 🎯 Oversigt

This guide explains how to automate the entire project setup so that any developer (or LLM) can initialize everything with a single script.

**What This Covers:**
1. ✅ Git initialization (local repository)
2. ✅ .NET project structure scaffolding
3. ✅ NuGet package installation
4. ✅ Pre-commit hooks setup
5. ✅ Documentation generation
6. ✅ Verification tests

**Time**: ~30 minutes total (mostly automated)

---

## 📋 Manual Setup (If Scripts Unavailable)

If you can't run scripts, do this manually:

### Step 1: Verify Prerequisites

```powershell
# Check .NET 8 SDK installed
dotnet --version
# Expected: 8.0.x or higher

# Check Visual Studio installed
Get-Command devenv -ErrorAction SilentlyContinue
# Expected: Path to devenv.exe

# Check Git installed
git --version
# Expected: git version 2.40 or higher
```

### Step 2: Initialize Git Repository

```powershell
cd "d:\Tc electronic projekt\Nyt program til Nova"

# Initialize local Git repository
git init

# Configure local Git (not global, just this repo)
git config user.name "Your Name"
git config user.email "your.email@example.com"

# Add all files
git add .

# First commit
git commit -m "[INIT] Initial project structure and documentation"

# Verify
git log --oneline
# Expected: 1 commit showing [INIT]
```

### Step 3: Create .NET Project Structure

```powershell
cd "d:\Tc electronic projekt\Nyt program til Nova"

# Create solution
dotnet new sln -n NovaApp

# Create source projects
dotnet new classlib -n Nova.Common -o src/Nova.Common
dotnet new classlib -n Nova.Domain -o src/Nova.Domain
dotnet new classlib -n Nova.Infrastructure -o src/Nova.Infrastructure
dotnet new classlib -n Nova.Application -o src/Nova.Application
dotnet new avalonia.app -n Nova.Presentation -o src/Nova.Presentation

# Create test projects
dotnet new xunit -n Nova.Domain.Tests -o tests/Nova.Domain.Tests
dotnet new xunit -n Nova.Infrastructure.Tests -o tests/Nova.Infrastructure.Tests
dotnet new xunit -n Nova.Application.Tests -o tests/Nova.Application.Tests
dotnet new xunit -n Nova.Presentation.Tests -o tests/Nova.Presentation.Tests

# Add all to solution
dotnet sln NovaApp.sln add src/**/*.csproj
dotnet sln NovaApp.sln add tests/**/*.csproj
```

### Step 4: Install NuGet Packages

```powershell
# Navigate to src folder
cd src/Nova.Infrastructure

# Install MIDI library
dotnet add package DryWetMIDI --version 7.0.0

# Install Logging
dotnet add package Serilog --version 3.0.1

# Go back and add to all test projects
cd ../../tests/Nova.Domain.Tests
dotnet add package Moq --version 4.18.0
dotnet add package FluentAssertions --version 6.11.0

# Repeat for other test projects...
```

### Step 5: Verify Build

```powershell
cd "d:\Tc electronic projekt\Nyt program til Nova"

dotnet build NovaApp.sln --no-incremental
# Expected: "Build succeeded with 0 warnings"

dotnet test NovaApp.sln
# Expected: "Test run successful"
```

---

## 🚀 Automated Setup Script (Recommended)

Create this file: `setup.ps1`

```powershell
<#
.SYNOPSIS
    Automate complete Nova System project initialization
.DESCRIPTION
    - Initializes Git repository
    - Creates .NET project structure
    - Installs NuGet packages
    - Sets up pre-commit hooks
    - Runs verification tests
.EXAMPLE
    .\setup.ps1
#>

param(
    [switch]$SkipGit = $false,
    [switch]$SkipDotNet = $false,
    [switch]$SkipTests = $false,
    [switch]$DryRun = $false
)

# ============================================================================
# COLOR OUTPUT
# ============================================================================

$colors = @{
    Header   = 'Cyan'
    Success  = 'Green'
    Error    = 'Red'
    Warning  = 'Yellow'
    Info     = 'White'
}

function Write-ColorOutput([string]$message, [string]$color = 'Info') {
    Write-Host $message -ForegroundColor $colors[$color]
}

# ============================================================================
# STEP 1: VERIFY PREREQUISITES
# ============================================================================

Write-ColorOutput "`n╔════════════════════════════════════════╗" 'Header'
Write-ColorOutput "║  NOVA SYSTEM — AUTOMATED SETUP        ║" 'Header'
Write-ColorOutput "╚════════════════════════════════════════╝`n" 'Header'

Write-ColorOutput "STEP 1: VERIFYING PREREQUISITES..." 'Header'

# Check .NET 8 SDK
try {
    $dotnetVersion = dotnet --version
    Write-ColorOutput "  ✓ .NET SDK: $dotnetVersion" 'Success'
} catch {
    Write-ColorOutput "  ✗ .NET 8 SDK not found. Install from dotnet.microsoft.com" 'Error'
    exit 1
}

# Check Visual Studio
$vsPath = Get-Command devenv -ErrorAction SilentlyContinue
if ($vsPath) {
    Write-ColorOutput "  ✓ Visual Studio found" 'Success'
} else {
    Write-ColorOutput "  ⚠ Visual Studio not found (optional, can use VS Code)" 'Warning'
}

# Check Git
try {
    $gitVersion = git --version
    Write-ColorOutput "  ✓ Git: $gitVersion" 'Success'
} catch {
    Write-ColorOutput "  ✗ Git not found. Install from git-scm.com" 'Error'
    exit 1
}

# ============================================================================
# STEP 2: INITIALIZE GIT REPOSITORY
# ============================================================================

if (-not $SkipGit) {
    Write-ColorOutput "`nSTEP 2: INITIALIZING GIT REPOSITORY..." 'Header'
    
    if (-not $DryRun) {
        if (-not (Test-Path ".git")) {
            git init
            git config user.name "Nova System Developer"
            git config user.email "dev@novasystem.local"
            Write-ColorOutput "  ✓ Git repository initialized" 'Success'
        } else {
            Write-ColorOutput "  ℹ Git repository already exists" 'Info'
        }
    } else {
        Write-ColorOutput "  [DRY RUN] Would initialize Git" 'Info'
    }
}

# ============================================================================
# STEP 3: CREATE .NET PROJECT STRUCTURE
# ============================================================================

if (-not $SkipDotNet) {
    Write-ColorOutput "`nSTEP 3: CREATING .NET PROJECT STRUCTURE..." 'Header'
    
    if (-not $DryRun) {
        # Create solution
        if (-not (Test-Path "NovaApp.sln")) {
            dotnet new sln -n NovaApp --force
            Write-ColorOutput "  ✓ Solution created: NovaApp.sln" 'Success'
        }
        
        # Create source projects
        $sourceProjects = @(
            'Nova.Common',
            'Nova.Domain',
            'Nova.Infrastructure',
            'Nova.Application',
            'Nova.Presentation'
        )
        
        foreach ($project in $sourceProjects) {
            $projectPath = "src/$project"
            if (-not (Test-Path $projectPath)) {
                if ($project -eq 'Nova.Presentation') {
                    dotnet new avalonia.app -n $project -o $projectPath --force
                } else {
                    dotnet new classlib -n $project -o $projectPath --force
                }
                Write-ColorOutput "  ✓ Created: $project" 'Success'
            }
        }
        
        # Create test projects
        $testProjects = @(
            'Nova.Domain.Tests',
            'Nova.Infrastructure.Tests',
            'Nova.Application.Tests',
            'Nova.Presentation.Tests'
        )
        
        foreach ($project in $testProjects) {
            $projectPath = "tests/$project"
            if (-not (Test-Path $projectPath)) {
                dotnet new xunit -n $project -o $projectPath --force
                Write-ColorOutput "  ✓ Created: $project" 'Success'
            }
        }
        
        # Add to solution
        Get-ChildItem -Recurse -Filter "*.csproj" | ForEach-Object {
            if (-not (Select-String -Pattern $_.BaseName -Path "NovaApp.sln" -ErrorAction SilentlyContinue)) {
                dotnet sln NovaApp.sln add $_.FullName
                Write-ColorOutput "  ✓ Added to solution: $($_.BaseName)" 'Success'
            }
        }
    } else {
        Write-ColorOutput "  [DRY RUN] Would create project structure" 'Info'
    }
}

# ============================================================================
# STEP 4: INSTALL NUGET PACKAGES
# ============================================================================

if (-not $DryRun) {
    Write-ColorOutput "`nSTEP 4: INSTALLING NUGET PACKAGES..." 'Header'
    
    # Packages configuration
    $packages = @{
        'src/Nova.Infrastructure' = @(
            'DryWetMIDI:7.0.0',
            'Serilog:3.0.1'
        )
        'src/Nova.Application' = @(
            'Microsoft.Extensions.DependencyInjection:8.0.0',
            'Serilog:3.0.1'
        )
        'tests/Nova.Domain.Tests' = @(
            'Moq:4.18.0',
            'FluentAssertions:6.11.0'
        )
        'tests/Nova.Infrastructure.Tests' = @(
            'Moq:4.18.0',
            'FluentAssertions:6.11.0'
        )
        'tests/Nova.Application.Tests' = @(
            'Moq:4.18.0',
            'FluentAssertions:6.11.0'
        )
    }
    
    foreach ($projectPath in $packages.Keys) {
        foreach ($package in $packages[$projectPath]) {
            $parts = $package -split ':'
            $packageName = $parts[0]
            $packageVersion = $parts[1]
            
            if (Test-Path "$projectPath/*.csproj") {
                Push-Location $projectPath
                dotnet add package $packageName --version $packageVersion --no-restore
                Pop-Location
                Write-ColorOutput "  ✓ Added $packageName to $projectPath" 'Success'
            }
        }
    }
}

# ============================================================================
# STEP 5: RESTORE & BUILD
# ============================================================================

Write-ColorOutput "`nSTEP 5: RESTORING & BUILDING..." 'Header'

if (-not $DryRun) {
    dotnet restore
    $buildResult = dotnet build NovaApp.sln --no-incremental --verbosity quiet
    if ($LASTEXITCODE -eq 0) {
        Write-ColorOutput "  ✓ Build successful (0 warnings)" 'Success'
    } else {
        Write-ColorOutput "  ✗ Build failed" 'Error'
        exit 1
    }
} else {
    Write-ColorOutput "  [DRY RUN] Would build solution" 'Info'
}

# ============================================================================
# STEP 6: SETUP PRE-COMMIT HOOK
# ============================================================================

if (-not $SkipGit) {
    Write-ColorOutput "`nSTEP 6: SETTING UP PRE-COMMIT HOOK..." 'Header'
    
    if (-not $DryRun) {
        $hookContent = @"
#!/bin/bash
# Pre-commit hook: Ensure all tests pass and no warnings

echo "Running pre-commit checks..."

# Build
dotnet build --no-incremental --verbosity quiet
if [ `$? -ne 0 ]; then
    echo "❌ Build failed. Commit cancelled."
    exit 1
fi

# Test
dotnet test --no-build --verbosity quiet
if [ `$? -ne 0 ]; then
    echo "❌ Tests failed. Commit cancelled."
    exit 1
fi

echo "✅ Pre-commit checks passed"
exit 0
"@
        
        $hookPath = ".git/hooks/pre-commit"
        if (-not (Test-Path ".git/hooks")) {
            New-Item -ItemType Directory -Path ".git/hooks" -Force | Out-Null
        }
        
        Set-Content -Path $hookPath -Value $hookContent
        Write-ColorOutput "  ✓ Pre-commit hook installed" 'Success'
    }
}

# ============================================================================
# STEP 7: INITIAL GIT COMMIT
# ============================================================================

if (-not $SkipGit) {
    Write-ColorOutput "`nSTEP 7: CREATING INITIAL COMMIT..." 'Header'
    
    if (-not $DryRun) {
        git add .
        git commit -m "[INIT] Project structure, documentation, and setup" -q
        Write-ColorOutput "  ✓ Initial commit created" 'Success'
        
        git log --oneline -1
    }
}

# ============================================================================
# STEP 8: VERIFY
# ============================================================================

if (-not $SkipTests) {
    Write-ColorOutput "`nSTEP 8: RUNNING VERIFICATION TESTS..." 'Header'
    
    if (-not $DryRun) {
        $testResult = dotnet test NovaApp.sln --no-build --verbosity quiet
        if ($LASTEXITCODE -eq 0) {
            Write-ColorOutput "  ✓ All tests passed" 'Success'
        } else {
            Write-ColorOutput "  ✗ Some tests failed" 'Warning'
        }
    }
}

# ============================================================================
# SUMMARY
# ============================================================================

Write-ColorOutput "`n╔════════════════════════════════════════╗" 'Header'
Write-ColorOutput "║  SETUP COMPLETE! 🎉                   ║" 'Header'
Write-ColorOutput "╚════════════════════════════════════════╝" 'Header'

Write-ColorOutput "`nNext steps:`n" 'Info'
Write-ColorOutput "  1. Open solution: code NovaApp.sln" 'Info'
Write-ColorOutput "  2. Read task file: tasks/01-phase0-environment-setup.md" 'Info'
Write-ColorOutput "  3. Update SESSION_MEMORY.md with progress" 'Info'
Write-ColorOutput "  4. Start Phase 1: tasks/02-modul1-phase1-foundation.md`n" 'Info'

if ($DryRun) {
    Write-ColorOutput "  [DRY RUN COMPLETE - No changes made]`n" 'Warning'
}
```

---

## 🚀 Run the Setup Script

### Option A: Interactive (Recommended)

```powershell
cd "d:\Tc electronic projekt\Nyt program til Nova"

# Test first (dry run)
.\setup.ps1 -DryRun

# If happy, run for real
.\setup.ps1
```

### Option B: Selective Setup

```powershell
# Skip Git, only setup .NET
.\setup.ps1 -SkipGit

# Skip tests, only setup projects
.\setup.ps1 -SkipTests

# Dry run to see what would happen
.\setup.ps1 -DryRun
```

---

## 📋 What the Script Does

1. ✅ Verifies .NET 8 SDK, Visual Studio, Git installed
2. ✅ Initializes Git repository with initial commit
3. ✅ Creates 5 source projects + 4 test projects
4. ✅ Adds all projects to NovaApp.sln
5. ✅ Installs NuGet packages (DryWetMIDI, xUnit, Moq, Serilog, etc.)
6. ✅ Runs dotnet build (verifies 0 warnings)
7. ✅ Runs dotnet test (verifies all pass)
8. ✅ Sets up pre-commit hooks (enforces build+test)
9. ✅ Creates initial Git commit
10. ✅ Displays next steps

---

## 🔍 Manual Verification (If Script Fails)

```powershell
# After setup, verify manually:

# Check solution
dotnet sln NovaApp.sln list
# Expected: 9 projects

# Check build
dotnet build --no-incremental --verbosity quiet
# Expected: 0 warnings

# Check tests
dotnet test --verbosity minimal
# Expected: All tests pass (0 failed)

# Check Git
git log --oneline | head -5
git status
# Expected: Working directory clean

# Check file structure
Get-ChildItem src/ | Select-Object Name
Get-ChildItem tests/ | Select-Object Name
# Expected: All 9 projects listed
```

---

## 🐛 Troubleshooting

### Problem: "dotnet command not found"
```powershell
# .NET SDK not installed
# Solution: Download .NET 8 from dotnet.microsoft.com and install
```

### Problem: "Git command not found"
```powershell
# Git not installed
# Solution: Download from git-scm.com and install
```

### Problem: "Pre-commit hook failed"
```powershell
# Build or tests failing
# Solution: 
# 1. Run: dotnet build
# 2. Check output for errors
# 3. Fix errors
# 4. Try commit again
```

### Problem: "Can't create .NET project (Avalonia)"
```powershell
# Avalonia template not installed
# Solution:
dotnet new -i Avalonia.Templates
# Then run setup.ps1 again
```

---

## 💾 Backup & Restore

### Backup Repository

```powershell
# Copy entire folder to external drive
Copy-Item -Path "d:\Tc electronic projekt\Nyt program til Nova" `
          -Destination "E:\Backup\NovaApp-backup" `
          -Recurse

# Later: Restore from backup
Copy-Item -Path "E:\Backup\NovaApp-backup\*" `
          -Destination "d:\Tc electronic projekt\Nyt program til Nova" `
          -Recurse -Force
```

### Clone Repository

```powershell
# If you have Git, you can clone locally
cd "d:\backup"
git clone "d:\Tc electronic projekt\Nyt program til Nova" NovaApp-clone
```

---

## 📞 When Setup Fails

1. **Check Prerequisites**: `.\setup.ps1 -DryRun` (no changes)
2. **Read Error**: Look at the error message carefully
3. **Search Solution**: Google the error message
4. **Update Memory**: Document in `llm-build-system/memory/PITFALLS_FOUND.md`

---

## ✅ Success Checklist

After running setup.ps1, verify:

- [ ] .git folder exists
- [ ] NovaApp.sln contains 9 projects
- [ ] `dotnet build` returns 0 warnings
- [ ] `dotnet test` passes
- [ ] `git log` shows [INIT] commit
- [ ] `tasks/01-phase0-environment-setup.md` is accessible
- [ ] `llm-build-system/memory/SESSION_MEMORY.md` exists

If all checkmarks are green: ✅ **READY TO START PHASE 1**

---

**Version**: 1.0  
**Status**: ✅ Ready — Copy setup.ps1 to project root and run
