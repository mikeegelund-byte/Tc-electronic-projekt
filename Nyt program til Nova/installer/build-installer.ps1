#!/usr/bin/env pwsh
# Build script for Nova System Manager WiX Installer

param(
    [string]$Configuration = "Release",
    [string]$OutputPath = ".\installer\output",
    [switch]$SkipPublish = $false,
    [switch]$Help = $false
)

function Show-Help {
    Write-Host @"
Build Nova System Manager WiX Installer (.msi)

Usage:
  .\build-installer.ps1 [-Configuration <config>] [-OutputPath <path>] [-SkipPublish]

Parameters:
  -Configuration   Build configuration (default: Release)
  -OutputPath      Output directory for MSI file (default: .\installer\output)
  -SkipPublish     Skip the dotnet publish step (use existing publish folder)
  -Help            Show this help message

Examples:
  .\build-installer.ps1
  .\build-installer.ps1 -Configuration Debug
  .\build-installer.ps1 -SkipPublish -OutputPath .\output

Requirements:
  - .NET 8.0 SDK
  - WiX Toolset v4+ (install with: dotnet tool install --global wix)

"@
}

if ($Help) {
    Show-Help
    exit 0
}

# Script configuration
$ErrorActionPreference = "Stop"
$ProjectRoot = $PSScriptRoot
$PublishDir = Join-Path $ProjectRoot "publish"
$InstallerDir = Join-Path $ProjectRoot "installer"
$OutputDir = $OutputPath

# Ensure output directory exists
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Nova System Manager - Installer Build" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Publish the application (unless skipped)
if (-not $SkipPublish) {
    Write-Host "Step 1: Publishing application..." -ForegroundColor Yellow
    Write-Host "  Configuration: $Configuration" -ForegroundColor Gray
    Write-Host "  Output: $PublishDir" -ForegroundColor Gray
    Write-Host ""
    
    $publishArgs = @(
        "publish"
        "src\Nova.Presentation\Nova.Presentation.csproj"
        "-c", $Configuration
        "-o", $PublishDir
        "--self-contained", "false"
        "-p:PublishSingleFile=false"
    )
    
    & dotnet @publishArgs
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: dotnet publish failed with exit code $LASTEXITCODE" -ForegroundColor Red
        exit $LASTEXITCODE
    }
    
    Write-Host "  ✓ Application published successfully" -ForegroundColor Green
    Write-Host ""
} else {
    Write-Host "Step 1: Skipping publish (using existing publish folder)" -ForegroundColor Yellow
    Write-Host ""
}

# Step 2: Verify publish folder exists
if (-not (Test-Path $PublishDir)) {
    Write-Host "ERROR: Publish folder not found at: $PublishDir" -ForegroundColor Red
    Write-Host "Run without -SkipPublish to create it." -ForegroundColor Red
    exit 1
}

# Count files in publish folder
$publishedFiles = Get-ChildItem -Path $PublishDir -Recurse -File
Write-Host "  Found $($publishedFiles.Count) files in publish folder" -ForegroundColor Gray
Write-Host ""

# Step 3: Check WiX toolset installation
Write-Host "Step 2: Checking WiX toolset..." -ForegroundColor Yellow

$wixInstalled = $false
try {
    $wixVersion = & wix --version 2>&1
    if ($LASTEXITCODE -eq 0) {
        $wixInstalled = $true
        Write-Host "  ✓ WiX toolset found: $wixVersion" -ForegroundColor Green
    }
} catch {
    # WiX not found
}

if (-not $wixInstalled) {
    Write-Host "  ✗ WiX toolset not found" -ForegroundColor Red
    Write-Host ""
    Write-Host "Installing WiX toolset..." -ForegroundColor Yellow
    & dotnet tool install --global wix
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Failed to install WiX toolset" -ForegroundColor Red
        exit $LASTEXITCODE
    }
    
    Write-Host "  ✓ WiX toolset installed successfully" -ForegroundColor Green
}
Write-Host ""

# Step 4: Harvest published files using WiX heat
Write-Host "Step 3: Harvesting published files..." -ForegroundColor Yellow

$componentsWxs = Join-Path $InstallerDir "Components.wxs"

$heatArgs = @(
    "heat", "dir"
    $PublishDir
    "-cg", "PublishedFiles"
    "-gg"
    "-scom", "-sfrag", "-srd", "-sreg"
    "-dr", "INSTALLFOLDER"
    "-var", "var.PublishDir"
    "-out", $componentsWxs
)

& wix @heatArgs

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: WiX heat failed with exit code $LASTEXITCODE" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "  ✓ Components harvested successfully" -ForegroundColor Green
Write-Host ""

# Step 5: Build the MSI installer
Write-Host "Step 4: Building MSI installer..." -ForegroundColor Yellow

$productWxs = Join-Path $InstallerDir "Product.wxs"
$msiOutput = Join-Path $OutputDir "NovaSystemManager.msi"

Write-Host "  Source: $productWxs" -ForegroundColor Gray
Write-Host "  Components: $componentsWxs" -ForegroundColor Gray
Write-Host "  Output: $msiOutput" -ForegroundColor Gray
Write-Host ""

$wixArgs = @(
    "build"
    $productWxs
    $componentsWxs
    "-o", $msiOutput
    "-d", "PublishDir=$PublishDir"
)

& wix @wixArgs

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: WiX build failed with exit code $LASTEXITCODE" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "  ✓ MSI installer built successfully" -ForegroundColor Green
Write-Host ""

# Step 6: Display results
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Build Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Installer location:" -ForegroundColor Yellow
Write-Host "  $msiOutput" -ForegroundColor White
Write-Host ""

# Display file size
$msiFile = Get-Item $msiOutput
$sizeMB = [math]::Round($msiFile.Length / 1MB, 2)
Write-Host "File size: $sizeMB MB" -ForegroundColor Gray
Write-Host ""

Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Test the installer on a clean Windows machine" -ForegroundColor Gray
Write-Host "  2. Verify the application launches correctly" -ForegroundColor Gray
Write-Host "  3. Test the uninstall process" -ForegroundColor Gray
Write-Host ""

