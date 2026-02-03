#!/usr/bin/env pwsh
# Build script for Nova System Manager WiX Installer

param(
    [string]$Configuration = "Release",
    [string]$OutputPath = ".\installer\output",
    [string]$RuntimeIdentifier = "win-x64",
    [switch]$SkipPublish = $false,
    [switch]$Help = $false
)

function Show-Help {
    $help = @'
Build Nova System Manager WiX Installer (.msi)

Usage:
  .\build-installer.ps1 [-Configuration <config>] [-OutputPath <path>] [-RuntimeIdentifier <rid>] [-SkipPublish]

Parameters:
  -Configuration       Build configuration (default: Release)
  -OutputPath          Output directory for MSI file (default: .\installer\output)
  -RuntimeIdentifier   Publish RID (default: win-x64)
  -SkipPublish         Skip the dotnet publish step (use existing publish folder)
  -Help                Show this help message

Examples:
  .\build-installer.ps1
  .\build-installer.ps1 -Configuration Debug
  .\build-installer.ps1 -RuntimeIdentifier win-x64
  .\build-installer.ps1 -SkipPublish -OutputPath .\output
