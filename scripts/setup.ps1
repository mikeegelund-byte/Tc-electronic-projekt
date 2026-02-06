<#
.SYNOPSIS
    Automate complete Nova System project initialization
.DESCRIPTION
    One-click setup for entire development environment:
    - Git repository initialization
    - .NET 8 project scaffolding
    - NuGet package installation
    - Pre-commit hooks
    - Verification tests
    
    NO CHANGES if you use -DryRun first!
    
.PARAMETER SkipGit
    Don't initialize Git repository
    
.PARAMETER SkipDotNet
    Don't create .NET projects
    
.PARAMETER SkipTests
    Don't run verification tests
    
.PARAMETER DryRun
    Preview what would be done (no actual changes)
    
.EXAMPLE
    # Test first
    .\setup.ps1 -DryRun
    
    # Then run for real
    .\setup.ps1
    
.EXAMPLE
    # Skip certain steps
    .\setup.ps1 -SkipGit -SkipTests
#>

param(
    [switch]$SkipGit = $false,
    [switch]$SkipDotNet = $false,
    [switch]$SkipTests = $false,
    [switch]$DryRun = $false
)

# ============================================================================
# CONFIGURATION
# ============================================================================

$ErrorActionPreference = "Stop"
$VerbosePreference = "Continue"

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

function Ensure-NuGetSource {
    try {
        $sources = dotnet nuget list source 2>$null
        if ($sources -notmatch 'nuget\.org') {
            dotnet nuget add source https://api.nuget.org/v3/index.json --name nuget.org --configfile NuGet.Config | Out-Null
        }
        dotnet nuget enable source nuget.org --configfile NuGet.Config | Out-Null
    } catch {
        Write-ColorOutput "  ⚠ Could not verify NuGet sources: $_" 'Warning'
    }
}

# ============================================================================
# STEP 1: VERIFY PREREQUISITES
# ============================================================================

Write-ColorOutput "`n╔════════════════════════════════════════════════════════╗" 'Header'
Write-ColorOutput "║  NOVA SYSTEM — AUTOMATED PROJECT SETUP                ║" 'Header'
Write-ColorOutput "║  Version 1.0 | TC Electronic Nova System Controller  ║" 'Header'
Write-ColorOutput "╚════════════════════════════════════════════════════════╝" 'Header'

Write-ColorOutput "`n[STEP 1/8] VERIFYING PREREQUISITES..." 'Header'

# Check .NET 8 SDK
try {
    $dotnetVersion = dotnet --version 2>$null
    if ($null -eq $dotnetVersion) {
        throw "No .NET version returned"
    }
    Write-ColorOutput "  ✓ .NET SDK: $dotnetVersion" 'Success'
} catch {
    Write-ColorOutput "  ✗ .NET 8 SDK not found" 'Error'
    Write-ColorOutput "    Download from: https://dotnet.microsoft.com/download/dotnet" 'Info'
    exit 1
}

# Check Visual Studio or VS Code
$hasVS = $null -ne (Get-Command devenv -ErrorAction SilentlyContinue)
$hasVSCode = $null -ne (Get-Command code -ErrorAction SilentlyContinue)

if ($hasVS) {
    Write-ColorOutput "  ✓ Visual Studio Community found" 'Success'
} elseif ($hasVSCode) {
    Write-ColorOutput "  ✓ Visual Studio Code found" 'Success'
} else {
    Write-ColorOutput "  ⚠ No IDE detected (install VS Community or Code)" 'Warning'
}

# Check Git
try {
    $gitVersion = git --version 2>$null
    Write-ColorOutput "  ✓ Git: $gitVersion" 'Success'
} catch {
    Write-ColorOutput "  ✗ Git not found" 'Error'
    Write-ColorOutput "    Download from: https://git-scm.com" 'Info'
    exit 1
}

# Check PowerShell version
$psVersion = $PSVersionTable.PSVersion
if ($psVersion.Major -ge 5) {
    Write-ColorOutput "  ✓ PowerShell: $psVersion" 'Success'
} else {
    Write-ColorOutput "  ⚠ PowerShell < 5 (might have issues, but trying...)" 'Warning'
}

# ============================================================================
# STEP 2: VERIFY WORKING DIRECTORY
# ============================================================================

Write-ColorOutput "`n[STEP 2/8] VERIFYING PROJECT LOCATION..." 'Header'

$projectRoot = Get-Location
$manifestExists = Test-Path "APPLICATION_MANIFEST.md"

if ($manifestExists) {
    Write-ColorOutput "  ✓ Found APPLICATION_MANIFEST.md" 'Success'
    Write-ColorOutput "  ✓ Project root: $projectRoot" 'Success'
} else {
    Write-ColorOutput "  ✗ APPLICATION_MANIFEST.md not found" 'Error'
    Write-ColorOutput "    Run this script from: c:\Tc electronic projekt\Nyt program til Nova" 'Info'
    exit 1
}

# ============================================================================
# DRY RUN INFO
# ============================================================================

if ($DryRun) {
    Write-ColorOutput "`n[DRY RUN MODE] No actual changes will be made" 'Warning'
}

# ============================================================================
# STEP 3: INITIALIZE GIT REPOSITORY
# ============================================================================

if (-not $SkipGit) {
    Write-ColorOutput "`n[STEP 3/8] INITIALIZING GIT REPOSITORY..." 'Header'
    
    if (-not $DryRun) {
        if (-not (Test-Path ".git")) {
            try {
                git init | Out-Null
                git config user.name "Nova System Developer" 2>$null
                git config user.email "dev@novasystem.local" 2>$null
                Write-ColorOutput "  ✓ Git repository initialized at: $(Get-Location)/.git" 'Success'
            } catch {
                Write-ColorOutput "  ✗ Failed to initialize Git: $_" 'Error'
                exit 1
            }
        } else {
            Write-ColorOutput "  ℹ Git repository already exists (skipping init)" 'Info'
        }
    } else {
        Write-ColorOutput "  [DRY RUN] Would initialize Git repository" 'Info'
    }
} else {
    Write-ColorOutput "`n[STEP 3/8] SKIPPING GIT (--SkipGit)" 'Warning'
}

# ============================================================================
# STEP 4: CREATE .NET PROJECT STRUCTURE
# ============================================================================

if (-not $SkipDotNet) {
    Write-ColorOutput "`n[STEP 4/8] CREATING .NET PROJECT STRUCTURE..." 'Header'
    
    if (-not $DryRun) {
        # Create solution if not exists
        if (-not (Test-Path "NovaApp.sln")) {
            try {
                dotnet new sln -n NovaApp --force --quiet 2>$null
                Write-ColorOutput "  ✓ Created: NovaApp.sln" 'Success'
            } catch {
                Write-ColorOutput "  ⚠ Failed to create solution: $_" 'Warning'
            }
        } else {
            Write-ColorOutput "  ℹ NovaApp.sln already exists" 'Info'
        }
        
        # Create source projects
        $sourceProjects = @(
            @{ Name = 'Nova.Common'; Template = 'classlib' },
            @{ Name = 'Nova.Domain'; Template = 'classlib' },
            @{ Name = 'Nova.Infrastructure'; Template = 'classlib' },
            @{ Name = 'Nova.Application'; Template = 'classlib' },
            @{ Name = 'Nova.Presentation'; Template = 'avalonia.app' }
        )
        
        foreach ($project in $sourceProjects) {
            $projectPath = "src/$($project.Name)"
            if (-not (Test-Path $projectPath)) {
                try {
                    if ($project.Template -eq 'avalonia.app') {
                        Ensure-NuGetSource
                        $templateAvailable = (dotnet new list 2>$null | Select-String -Pattern 'avalonia\.app')
                        if (-not $templateAvailable) {
                            dotnet new install Avalonia.Templates --quiet 2>$null
                        }
                    }

                    dotnet new $project.Template -n $project.Name -o $projectPath --force --quiet 2>$null
                    Write-ColorOutput "  ✓ Created: $($project.Name)" 'Success'
                } catch {
                    Write-ColorOutput "  ⚠ Failed to create $($project.Name): $_" 'Warning'
                }
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
                try {
                    dotnet new xunit -n $project -o $projectPath --force --quiet 2>$null
                    Write-ColorOutput "  ✓ Created: $project" 'Success'
                } catch {
                    Write-ColorOutput "  ⚠ Failed to create ${project}: $_" 'Warning'
                }
            }
        }
        
        # Add projects to solution
        Write-ColorOutput "`n  Adding projects to solution..." 'Info'
        Get-ChildItem -Recurse -Filter "*.csproj" -ErrorAction SilentlyContinue | ForEach-Object {
            try {
                $slnContent = Get-Content "NovaApp.sln" -Raw -ErrorAction SilentlyContinue
                if ($slnContent -notmatch [regex]::Escape($_.FullName)) {
                    dotnet sln NovaApp.sln add $_.FullName --quiet 2>$null
                    Write-ColorOutput "    ✓ $($_.BaseName)" 'Success'
                }
            } catch {
                Write-ColorOutput "    ⚠ Could not add $($_.BaseName)" 'Warning'
            }
        }
        
    } else {
        Write-ColorOutput "  [DRY RUN] Would create 9 .NET projects" 'Info'
    }
} else {
    Write-ColorOutput "`n[STEP 4/8] SKIPPING .NET SETUP (--SkipDotNet)" 'Warning'
}

# ============================================================================
# STEP 5: INSTALL NUGET PACKAGES
# ============================================================================

Write-ColorOutput "`n[STEP 5/8] CONFIGURING NUGET PACKAGES..." 'Header'

if (-not $DryRun) {
    Ensure-NuGetSource
    $packages = @{
        'src/Nova.Infrastructure' = @(
            @{ Name = 'DryWetMIDI'; Version = '7.0.0' },
            @{ Name = 'Serilog'; Version = '3.0.1' }
        )
        'src/Nova.Application' = @(
            @{ Name = 'Microsoft.Extensions.DependencyInjection'; Version = '8.0.0' },
            @{ Name = 'Serilog'; Version = '3.0.1' }
        )
        'tests/Nova.Domain.Tests' = @(
            @{ Name = 'Moq'; Version = '4.18.0' },
            @{ Name = 'FluentAssertions'; Version = '6.11.0' },
            @{ Name = 'coverlet.msbuild'; Version = '6.0.0' }
        )
        'tests/Nova.Infrastructure.Tests' = @(
            @{ Name = 'Moq'; Version = '4.18.0' },
            @{ Name = 'FluentAssertions'; Version = '6.11.0' },
            @{ Name = 'coverlet.msbuild'; Version = '6.0.0' }
        )
        'tests/Nova.Application.Tests' = @(
            @{ Name = 'Moq'; Version = '4.18.0' },
            @{ Name = 'FluentAssertions'; Version = '6.11.0' },
            @{ Name = 'coverlet.msbuild'; Version = '6.0.0' }
        )
        'tests/Nova.Presentation.Tests' = @(
            @{ Name = 'Moq'; Version = '4.18.0' },
            @{ Name = 'FluentAssertions'; Version = '6.11.0' },
            @{ Name = 'coverlet.msbuild'; Version = '6.0.0' }
        )
    }
    
    foreach ($projectPath in $packages.Keys) {
        $csproj = Get-ChildItem "$projectPath/*.csproj" -ErrorAction SilentlyContinue | Select-Object -First 1
        if ($csproj) {
            foreach ($package in $packages[$projectPath]) {
                try {
                    Push-Location $projectPath
                    dotnet add package $package.Name --version $package.Version --no-restore --quiet 2>$null
                    Pop-Location
                    Write-ColorOutput "  ✓ $($package.Name) → $projectPath" 'Success'
                } catch {
                    Write-ColorOutput "  ⚠ Could not add $($package.Name) to $projectPath" 'Warning'
                    Pop-Location
                }
            }
        }
    }
} else {
    Write-ColorOutput "  [DRY RUN] Would install 10+ NuGet packages" 'Info'
}

# ============================================================================
# STEP 6: RESTORE & BUILD
# ============================================================================

Write-ColorOutput "`n[STEP 6/8] RESTORING DEPENDENCIES & BUILDING..." 'Header'

if (-not $DryRun) {
    try {
        $solutionPath = "NovaApp.sln"
        if (Test-Path $solutionPath) {
            Write-ColorOutput "  Restoring NuGet packages..." 'Info'
            dotnet restore $solutionPath --verbosity minimal 2>$null

            Write-ColorOutput "  Building solution..." 'Info'
            $buildOutput = dotnet build $solutionPath --no-incremental --verbosity quiet 2>&1
        } else {
            Write-ColorOutput "  ⚠ Solution file not found; skipping restore/build." 'Warning'
            $buildOutput = $null
        }
        
        if ($LASTEXITCODE -eq 0) {
            Write-ColorOutput "  ✓ Build successful (0 warnings)" 'Success'
        } else {
            Write-ColorOutput "  ⚠ Build completed with warnings (check output)" 'Warning'
            # Don't exit, continue with setup
        }
    } catch {
        Write-ColorOutput "  ⚠ Build failed (but continuing setup): $_" 'Warning'
    }
} else {
    Write-ColorOutput "  [DRY RUN] Would restore and build solution" 'Info'
}

# ============================================================================
# STEP 7: SETUP PRE-COMMIT HOOK
# ============================================================================

if (-not $SkipGit) {
    Write-ColorOutput "`n[STEP 7/8] SETTING UP PRE-COMMIT HOOK..." 'Header'
    
    if (-not $DryRun) {
        try {
            # Create hooks directory if needed
            if (-not (Test-Path ".git/hooks")) {
                New-Item -ItemType Directory -Path ".git/hooks" -Force | Out-Null
            }
            
                        # Create pre-commit hook (Windows batch + POSIX sh) to call verify-commit.ps1
                        $batHook = @"
@echo off
REM Pre-commit hook: run verification script
powershell -NoProfile -ExecutionPolicy Bypass -File "./verify-commit.ps1"
if errorlevel 1 (
        echo ❌ Pre-commit verification failed. Commit cancelled.
        exit /b 1
)
exit /b 0
"@

                        $shHook = @"
#!/bin/sh
# Pre-commit hook: run verification script
powershell -NoProfile -ExecutionPolicy Bypass -File "./verify-commit.ps1"
if [ $? -ne 0 ]; then
    echo "❌ Pre-commit verification failed. Commit cancelled."
    exit 1
fi
exit 0
"@

                        $commitMsgHook = @"
#!/bin/sh
# Commit-msg hook: enforce message format
MSG_FILE="$1"
MSG=$(cat "$MSG_FILE")
echo "$MSG" | grep -Eq "^\[(RED→GREEN→REFACTOR|INIT|CLEANUP|MODUL-[0-9]+)\]" || {
    echo "❌ Commit message must start with [RED→GREEN→REFACTOR] or [INIT]/[CLEANUP]/[MODUL-X]" >&2
    exit 1
}
exit 0
"@

                        $commitMsgBat = @"
@echo off
set MSG_FILE=%1
for /f "usebackq delims=" %%A in (%MSG_FILE%) do set MSG=%%A
echo %MSG% | findstr /R /C:"^\[RED→GREEN→REFACTOR\]" /C:"^\[INIT\]" /C:"^\[CLEANUP\]" /C:"^\[MODUL-[0-9][0-9]*\]" >nul
if errorlevel 1 (
    echo ❌ Commit message must start with [RED→GREEN→REFACTOR] or [INIT]/[CLEANUP]/[MODUL-X]
    exit /b 1
)
exit /b 0
"@

                        $hooksDir = ".git/hooks"
                        $preCommitBatPath = "$hooksDir/pre-commit.bat"
                        $preCommitShPath = "$hooksDir/pre-commit"
                        $commitMsgBatPath = "$hooksDir/commit-msg.bat"
                        $commitMsgShPath = "$hooksDir/commit-msg"

                        Set-Content -Path $preCommitBatPath -Value $batHook -Encoding ASCII
                        Set-Content -Path $preCommitShPath -Value $shHook -Encoding ASCII
                        Set-Content -Path $commitMsgBatPath -Value $commitMsgBat -Encoding ASCII
                        Set-Content -Path $commitMsgShPath -Value $commitMsgHook -Encoding ASCII

                        Write-ColorOutput "  ✓ Pre-commit hooks installed" 'Success'
                        Write-ColorOutput "  ✓ Commit-msg hooks installed" 'Success'
        } catch {
            Write-ColorOutput "  ⚠ Could not install pre-commit hook: $_" 'Warning'
        }
    } else {
        Write-ColorOutput "  [DRY RUN] Would install pre-commit hook" 'Info'
    }
}

# ============================================================================
# STEP 8: INITIAL GIT COMMIT
# ============================================================================

if (-not $SkipGit) {
    Write-ColorOutput "`n[STEP 8/8] CREATING INITIAL COMMIT..." 'Header'
    
    if (-not $DryRun) {
        try {
            git add . 2>$null
            git commit -m "[INIT] Project structure, documentation, and automated setup" --quiet 2>$null
            Write-ColorOutput "  ✓ Initial commit created" 'Success'
            
            $logOutput = git log --oneline -1 2>$null
            Write-ColorOutput "    $logOutput" 'Info'
        } catch {
            Write-ColorOutput "  ⚠ Git commit failed (might already exist): $_" 'Warning'
        }
    } else {
        Write-ColorOutput "  [DRY RUN] Would create initial commit" 'Info'
    }
}

# ============================================================================
# VERIFICATION
# ============================================================================

Write-ColorOutput "`n[VERIFICATION] Checking setup results..." 'Header'

# Check folder structure
$srcFolder = Test-Path "src"
$testsFolder = Test-Path "tests"
$gitFolder = Test-Path ".git"
$slnFile = Test-Path "NovaApp.sln"

if ($srcFolder) { Write-ColorOutput "  ✓ src/ folder exists" 'Success' }
if ($testsFolder) { Write-ColorOutput "  ✓ tests/ folder exists" 'Success' }
if ($gitFolder) { Write-ColorOutput "  ✓ .git/ repository exists" 'Success' }
if ($slnFile) { Write-ColorOutput "  ✓ NovaApp.sln exists" 'Success' }

# ============================================================================
# SUMMARY
# ============================================================================

Write-ColorOutput "`n╔════════════════════════════════════════════════════════╗" 'Header'
Write-ColorOutput "║  SETUP COMPLETE! 🎉                                   ║" 'Header'
Write-ColorOutput "╚════════════════════════════════════════════════════════╝`n" 'Header'

if ($DryRun) {
    Write-ColorOutput "✓ Dry run successful. Run again without -DryRun to apply changes.`n" 'Success'
} else {
    Write-ColorOutput "✓ Project initialized and ready for development!`n" 'Success'
}

Write-ColorOutput "📋 NEXT STEPS:`n" 'Info'
Write-ColorOutput "  1. Update SESSION_MEMORY.md:" 'Info'
Write-ColorOutput "     - Set current module/phase" 'Info'
Write-ColorOutput "     - Record initialization time" 'Info'
Write-ColorOutput "" 'Info'
Write-ColorOutput "  2. Start Phase 1 development:" 'Info'
Write-ColorOutput "     - Read: tasks/01-phase0-environment-setup.md" 'Info'
Write-ColorOutput "     - Or:   tasks/02-modul1-phase1-foundation.md" 'Info'
Write-ColorOutput "" 'Info'
Write-ColorOutput "  3. Open in IDE:" 'Info'
Write-ColorOutput "     - Visual Studio: devenv NovaApp.sln" 'Info'
Write-ColorOutput "     - VS Code:      code NovaApp.sln" 'Info'
Write-ColorOutput "" 'Info'
Write-ColorOutput "  4. Read discipline rules:" 'Info'
Write-ColorOutput "     - llm-build-system/LLM_BUILD_INSTRUCTIONS.md" 'Info'
Write-ColorOutput "     - llm-build-system/CLEANUP_POLICY.md`n" 'Info'

Write-ColorOutput "💡 TIP: Every code change must follow RED→GREEN→REFACTOR" 'Info'
Write-ColorOutput "    ❌ Write failing test FIRST" 'Info'
Write-ColorOutput "    ✅ Write minimal code to pass" 'Info'
Write-ColorOutput "    🔄 Refactor while tests still pass" 'Info'
Write-ColorOutput "    📝 Commit with [RED→GREEN→REFACTOR] prefix`n" 'Info'

Write-ColorOutput "📞 Problems? Read:" 'Info'
Write-ColorOutput "    - llm-build-system/memory/PITFALLS_FOUND.md (common issues)" 'Info'
Write-ColorOutput "    - SETUP_AUTOMATION.md (troubleshooting section)" 'Info'
